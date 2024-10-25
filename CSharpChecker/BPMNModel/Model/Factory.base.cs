using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Utility;

namespace BPMNModel.Model
{
    public partial class Factory
    {
        private Dictionary<string, object> cacheWithObjects = new Dictionary<string, object>();
        private string indent = "";
        private HashSet<string> loadedIds = new();
        private ILogger logger;
        private XmlParserComplexNode root;

        private Factory(ILogger logger, XmlParser parser)
        {
            this.logger = logger;

            if (parser.Root is null)
            {
                throw new BPMNCheckerExceptions($"Error in parser, unable to continue with empty instance.");
            }

            root = parser.Root;

            foreach (var (id, complexNode) in parser.Cache)
            {
                if (complexNode.Type is null)
                {
                    throw new BPMNCheckerExceptions($"Error in parser, {complexNode.ID} has no type.");
                }
                string className = complexNode.Type.Name.StartsWith('t') ? complexNode.Type.Name.Substring(1) : complexNode.Type.Name;
                string fullClassName = "BPMNModel.Model." + className;
                Type? type = Type.GetType(fullClassName);

                if (type != null)
                {
                    var instance = Activator.CreateInstance(type);
                    if (instance != null)
                    {
                        logger.Debug($"Create in cache: {id} - {instance.GetType().Name}");
                        cacheWithObjects[id] = instance;
                    }
                    else
                    {
                        throw new BPMNCheckerExceptions($"Error in parser, unable to create instance of class: {type.FullName}.");
                    }
                }
                else
                {
                    throw new BPMNCheckerExceptions($"Error in parser, there is no class: {fullClassName}.");
                }
            }
        }

        public Definitions? Definition { get; private set; }

        #region Missing types if CMOF
        private string LoadText(XmlParserComplexNode node)
        {
            //TODO: Omitting inner nodes
            return node.MixedContent ?? "";
        }

        #endregion

        #region Known casts

        //TODO: Do real data conversion.
        private object SolveKnownCasts(XmlParserCastNode castNode)
        {

            if (castNode.Type.Name == "tFormalExpression")
            {
                FormalExpression result = new FormalExpression();
                result.Body = new Element(castNode.Value);
                return result;   
            }
            throw new BPMNCheckerExceptions($"Unknown cast to type: {castNode.Type.Name}.");
        }

        #endregion

        #region Solving missing attributes

        void ManualySolve_body_in_FormalExpression(FormalExpression result, XmlParserComplexNode node)
        {
            if (node.MixedContent != null)
            {
                result.Body = new Element(node.MixedContent);
            }
        }

        void ManualySolve_protocol_in_Transaction(Transaction result, XmlParserComplexNode node)
        {

        }

        void ManualySolve_text_in_Documentation(Documentation result, XmlParserComplexNode node)
        {
            if (node.MixedContent != null)
            {
                result.Text = node.MixedContent;
            }
        }

        #endregion

        public static Factory ProcessModel(ILogger logger, XmlParser parser)
        {
            Factory factory = new Factory(Log.Logger, parser);
            factory.Definition = factory.LoadModel();

            foreach (var (_, item) in factory.cacheWithObjects)
            {
                PostProcessing.Process(item);
            }

            return factory;
        }

        public T? FillElement<T>(List<XmlParserNode> data)
        {

            if (data.Count > 1) throw new BPMNCheckerExceptions($"There should be at most one element.");
            if (data.Count == 0) return default;

            var result = new List<T>();

            FillElements(data, result);

            return result[0];
        }

        private T CreateEnum<T>(string value) where T : struct
        {
            if (Enum.TryParse(value, out T result))
            {
                return result;
            }
            throw new BPMNCheckerExceptions($"Can not convert {value} to Enum type {typeof(T).Name}.");
        }

        private void FillElements<T>(List<XmlParserNode> data, List<T> target)
        {
            foreach (XmlParserNode node in data)
            {
                if (node is XmlParserComplexNode complexNode)
                {
                    var item = Load<T>(complexNode);
                    target.Add(item);
                }
                else if (node is XmlParserCastNode castNode)
                {
                    target.Add((T)SolveKnownCasts(castNode));
                }
                else
                {
                    //TODO: other node types
                    throw new BPMNCheckerExceptions($"Not processing this node type now.");
                }
            }
        }

        private T GetOrCreate<T>(XmlParserComplexNode complexNode) where T : class, new()
        {
            var id = complexNode.Attributes.ContainsKey("id") ? complexNode.Attributes["id"].Value : null;
            if (id is not null)
            {
                if (cacheWithObjects.ContainsKey(id))
                {
                    logger.Debug($"{indent}GetOrCreate: {id} - {cacheWithObjects[id].GetType().Name}");
                    return (T)cacheWithObjects[id];
                }
                else
                {
                    throw new BPMNCheckerExceptions($"Element with ID: {id} should be in cache.");
                }
            }
            else
            {
                logger.Debug($"{indent}GetOrCreate: without id {typeof(T).Name}");
                return new T();
            }
        }

        private T Load<T>(XmlParserComplexNode complexNode)
        {
            logger.Debug($"{indent}{complexNode.ToString()} - Start to load.");

            indent += "  ";

            var idToCheck = complexNode.Attributes.ContainsKey("id") ? complexNode.Attributes["id"].Value : null;
            if (idToCheck is not null)
            {
                if (loadedIds.Contains(idToCheck))
                {
                    indent = indent.Substring(2);
                    logger.Debug($"{indent}{complexNode.ToString()} - Already loading, using cache.");

                    return (T)cacheWithObjects[idToCheck];
                }
                else
                {
                    loadedIds.Add(idToCheck);
                }
            }
            Type type = this.GetType();
            if (complexNode.Type is null)
            {
                throw new BPMNCheckerExceptions($"Error in parser, {complexNode.ID} has no type.");
            }

            var realName = complexNode.Type.Name.StartsWith('t') ? complexNode.Type.Name.Substring(1) : complexNode.Type.Name;
            var fullName = "Load" + realName;
            var method = type.GetMethod(fullName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (method == null)
            {
                throw new BPMNCheckerExceptions($"Error in generator, Factory does not contain method {fullName}.");
            }

            var result = method.Invoke(this, [complexNode]);

            if (result == null)
            {
                throw new BPMNCheckerExceptions($"Error while invoking method {fullName}.");
            }

            if (result is T realResult)
            {
                if (result is CamundaExtensionBaseElement camunda)
                {
                    LoadCamunda(complexNode, camunda);
                }
                indent = indent.Substring(2);

                logger.Debug($"{indent}{complexNode.ToString()} - Finished loading");
                return realResult;
            }
            else
            {
                throw new BPMNCheckerExceptions($"Error in data, data element of type {realName} can not be cast to {typeof(T).Name}.");
            }
        }

        private void LoadCamunda(XmlParserComplexNode complexNode, CamundaExtensionBaseElement target) 
        {
            if (complexNode.ChildNodes.ContainsKey("extensionElements")) 
            {
                if (complexNode.ChildNodes["extensionElements"].Any())
                {
                    if (complexNode.ChildNodes["extensionElements"].Count != 1) throw new BPMNCheckerExceptions("There is at most one extension section.");

                    var extensionBlock = complexNode.ChildNodes["extensionElements"][0];

                    if (extensionBlock is XmlParserComplexNode complexBlock && complexBlock.Type is not null && complexBlock.Type.Name == "tExtensionElements")
                    {
                        foreach(var item in complexBlock.ChildNodes["any"])
                        {
                            if (item is XmlParserAnyNode anyNode)
                            {


                            }else
                            {
                                throw new BPMNCheckerExceptions("Processing error, there are only anyNodes inside extension elements.");
                            }
                        }
                    }else
                    {
                        throw new BPMNCheckerExceptions("In extensionElements, there should be complex type: tExtensionElements");
                    }
                }
            }
        }

        private Definitions LoadModel()
        {
            var result = Load<Definitions>(root);
            return result;
        }

        private void DumpItem(StreamWriter writer, object item, string indent)
        {
            var newIndent = indent + "  ";
            var emptyProperties = new List<string>();

            string? getId(object value)
            {
                Type itemType = value.GetType();
                PropertyInfo? idProperty = itemType.GetProperty("Id", BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                if (idProperty != null && idProperty.PropertyType == typeof(string))
                {
                    return (string?)idProperty.GetValue(value);
                }
                return null;
            }

            void PrintOneObject(object value, string indent, string leftPart)
            {
                var valueId = getId(value);
                Type valueType = value.GetType();

                if (valueId != null)
                {
                    writer.WriteLine($"{indent}{leftPart} = REF({valueType.Name}, {valueId})");
                }
                else
                {
                    writer.WriteLine($"{indent}{leftPart} = ");
                    DumpItem(writer, value, indent + "  ");
                }
            }

            void ProcessType(PropertyInfo property, Type propertyRealType, bool isNulable)
            {
                var nullableString = isNulable ? "?" : "";

                if (propertyRealType == typeof(string) || propertyRealType == typeof(long) || propertyRealType == typeof(bool))
                {
                    var value = property.GetValue(item);
                    if (value != null)
                    {
                        writer.WriteLine($"{newIndent}{propertyRealType.Name}{nullableString} {property.Name} = {property.GetValue(item)}");
                    }
                    else
                    {
                        emptyProperties.Add($"{propertyRealType.Name}{nullableString} {property.Name}");
                    }
                }
                else if (propertyRealType.IsEnum)
                {

                    var value = property.GetValue(item);
                    if (value != null)
                    {
                        writer.WriteLine($"{newIndent}{propertyRealType.Name}{nullableString} {property.Name} = {property.GetValue(item)}");
                    }
                    else
                    {
                        emptyProperties.Add($"{propertyRealType.Name}{nullableString} {property.Name}");
                    }
                }
                else if (propertyRealType == typeof(List<>))
                {

                    var list = property.GetValue(item);

                    if (list == null) throw new BPMNCheckerExceptions("Something wet wrong...");
                    if (list is IEnumerable enumerableList)
                    {
                        string niceTypeName = $"List<{property.PropertyType.GetGenericArguments()[0].Name}>";

                        if (enumerableList.Cast<object>().Any())
                        {
                            writer.WriteLine($"{newIndent}{niceTypeName} {property.Name} = ");
                            writer.WriteLine($"{newIndent}  [");
                            int count = 0;
                            foreach(var value in  enumerableList)
                            {
                                PrintOneObject(value, newIndent+"    ", $"{property.Name}[{count}]");
                                count++;
                            }
                            writer.WriteLine($"{newIndent}  ]");

                        }
                        else
                        {
                            emptyProperties.Add($"{niceTypeName} {property.Name}");
                        }
                    }else
                    {
                        throw new BPMNCheckerExceptions("Something wet wrong...");
                    }

                }
                else
                {
                    var value = property.GetValue(item);
                    if (value == null)
                    {
                        emptyProperties.Add($"{property.PropertyType.Name} {property.Name}");
                    }
                    else
                    {
                        PrintOneObject(value, newIndent, $"{property.PropertyType.Name} {property.Name}");
                    }
                }
            }

            Type itemType = item.GetType();

            var id = getId(item);
            if (id != null )
            {
                writer.WriteLine($"{indent}{itemType.Name} ({id})");
            }else
            {
                writer.WriteLine($"{indent}{itemType.Name}");
            }
            writer.WriteLine(indent + "{");

            foreach (var property in itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy).OrderBy(x=>x.Name))
            {
                if (property.Name != "Id")
                {
                    if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        var nullableType = Nullable.GetUnderlyingType(property.PropertyType);
                        if (nullableType != null) ProcessType(property, nullableType, true);
                        else throw new BPMNCheckerExceptions("Something wet wrong...");
                    }
                    else if (property.PropertyType.IsGenericType) ProcessType(property, property.PropertyType.GetGenericTypeDefinition(), false);
                    else ProcessType(property, property.PropertyType, false);
                }
            }

            if (emptyProperties.Any())
            {
                writer.WriteLine();
                writer.WriteLine($"{newIndent}Empty:");
                foreach (var empty in emptyProperties)
                {
                    writer.WriteLine($"{newIndent}  {empty}");
                }
            }
            writer.WriteLine(indent + "}\n");
        }

        public void DumpModel(string fileName)
        {
            using var writer = new StreamWriter(fileName);

            foreach (var (_, item) in cacheWithObjects)
            {
                DumpItem(writer, item,"");
            }
        }
    }
}