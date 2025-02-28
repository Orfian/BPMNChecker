using System.Text.Json;
using System.Xml;
using System.Xml.Linq;

namespace Utility
{
    public class ResourcesUtility
    {
        public static Stream LoadResource(string path)
        {
            Stream? result = null;
            result = typeof(ResourcesUtility).Assembly.GetManifestResourceStream("Utility." + path.Replace("/", "."));
            if (result == null)
            {
                throw new Exception("No such file in the repository.");
            }
            return result;
        }

        public static JsonDocument LoadResourceAsJsonDocument(string path)
        {
            Stream src = LoadResource(path);
            var result = JsonDocument.Parse(src);
            return result;
        }

        public static XDocument LoadResourceAsXDocument(string path)
        {
            Stream src = LoadResource(path);
            var ret = XDocument.Load(src, LoadOptions.SetLineInfo);
            return ret;
        }

        public static XmlDocument LoadResourceAsXmlDocument(string path)
        {
            Stream src = LoadResource(path);
            var ret = new XmlDocument();
            ret.Load(src);
            return ret;
        }
    }
}