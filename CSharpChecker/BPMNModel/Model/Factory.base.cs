using Serilog;
using System;
using System.Collections.Generic;
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
        private HashSet<string> loadedIds = new();

        private ILogger logger;

        public Factory(ILogger logger, XmlParser parser)
        {
            this.logger = logger;

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
                        logger.Debug($"Create cache: {id} - {instance.GetType().Name}");
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

        public T GetOrCreate<T>(XmlParserComplexNode complexNode) where T : class, new()
        {
            var id = complexNode.Attributes.ContainsKey("id") ? complexNode.Attributes["id"].Value : null;
            if (id is not null)
            {
                if (cacheWithObjects.ContainsKey(id))
                {
                    logger.Debug($"Get from cache: {id} - {cacheWithObjects[id].GetType().Name}");
                    return (T)cacheWithObjects[id];
                }else
                {
                    throw new BPMNCheckerExceptions($"Element with ID: {id} should be in cache.");
                }
            }else
            {
                logger.Debug($"Creating without id: {typeof(T).Name}");
                return new T();
            }
        }

        public T CreateEnum<T>(string value) where T : struct
        {
            if (Enum.TryParse(value, out T result))
            {
                return result;
            }
            throw new BPMNCheckerExceptions($"Can not convert {value} to Enum type {typeof(T).Name}.");
        }

        public T Load<T>(XmlParserComplexNode complexNode)
        {
            var idToCheck = complexNode.Attributes.ContainsKey("id")? complexNode.Attributes["id"].Value : null;
            if (idToCheck is not null)
            {
                if (loadedIds.Contains(idToCheck))
                {
                    logger.Debug($"{complexNode.ToString()} - Loaded, using from cache.");
                    return (T)cacheWithObjects[idToCheck];
                }else
                {
                   loadedIds.Add(idToCheck);
                }
            }
            Type type = typeof(Factory);
            if (complexNode.Type is null) 
            {
                throw new BPMNCheckerExceptions($"Error in parser, {complexNode.ID} has no type.");
            }

            var realName = complexNode.Type.Name.StartsWith('t') ? complexNode.Type.Name.Substring(1) : complexNode.Type.Name;
            var fullName = "Load" + realName;
            var method = type.GetMethod(fullName);
            if (method == null)
            {
                throw new BPMNCheckerExceptions($"Error in generator, Factory does not contain method {fullName}.");
            }

            var result =  method.Invoke(this, [ complexNode ]);

            if (result == null)
            {
                throw new BPMNCheckerExceptions($"Error while invoking method {fullName}.");
            }

            if (result is T realResult)
            {
                logger.Debug(complexNode.ToString() + " - Finished loading");
                return realResult;
            }else
            {
                throw new BPMNCheckerExceptions($"Error in data, data element of type {realName} can not be cast to {typeof(T).Name}.");
            }
        }
        public void FillElements<T>(List<XmlParserNode> data, List<T> target)
        {
            foreach (XmlParserNode node in data)
            {
                if (node is XmlParserComplexNode complexNode)
                {
                    var item = Load<T>(complexNode);
                    target.Add(item);
                }else
                {
                    //TODO: other node types
                    throw new BPMNCheckerExceptions($"Not processing this node type now.");
                }
            }
        }
        public T? FillElement<T>(List<XmlParserNode> data)
        {
            if (data.Count > 1) throw new BPMNCheckerExceptions($"There should be at most one element.");
            if (data.Count == 0) return default;
            var node = data[0];
            if (node is XmlParserComplexNode complexNode)
            {
                var item = Load<T>(complexNode);
                return item;
            }
            else
            {
                //TODO: other node types
                throw new BPMNCheckerExceptions($"Not processing this node type now.");
            }

        }
    }
}
