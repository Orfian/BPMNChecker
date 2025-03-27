using BPMNModel.Camunda;
using BPMNModel.Model;
using System.Collections;
using System.Reflection;
using Utility;

namespace BPMNModel
{
    public class ModelRoot
    {
        private SortedDictionary<string, IElementWithId> allObjectsWithIds = new();

        public ModelRoot(Definitions definitions, IEnumerable<IElementWithId> elements)
        {
            foreach (var (element, index) in elements.Select((value, index) => (value, index)))
            {
                if (element.Id is null)
                {
                    throw new BPMNCheckerExceptions($"Root level of the model contains only elements with ID. Not true for: {element}[{index}].");
                }
                allObjectsWithIds.Add(element.Id, element);
            }

            Definition = definitions;
        }

        public SortedDictionary<string, IElementWithId> AllObjectsWithIds => allObjectsWithIds;
        public Definitions Definition { get; private set; }

        public void DumpModel(string fileName)
        {
            using var writer = new StreamWriter(fileName);

            foreach (var (_, item) in allObjectsWithIds)
            {
                DumpItem(writer, item, "");
            }
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

                if (valueId != null && value is not ICamundaBaseElement)
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

                if (propertyRealType == typeof(string) || propertyRealType == typeof(long) || propertyRealType == typeof(bool) || propertyRealType == typeof(double))
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
                            foreach (var value in enumerableList)
                            {
                                PrintOneObject(value, newIndent + "    ", $"{property.Name}[{count}]");
                                count++;
                            }
                            writer.WriteLine($"{newIndent}  ]");
                        }
                        else
                        {
                            emptyProperties.Add($"{niceTypeName} {property.Name}");
                        }
                    }
                    else
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
            if (id != null)
            {
                writer.WriteLine($"{indent}{itemType.Name} ({id})");
            }
            else
            {
                writer.WriteLine($"{indent}{itemType.Name}");
            }
            writer.WriteLine(indent + "{");

            foreach (var property in itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy).OrderBy(x => x.Name))
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
    }
}