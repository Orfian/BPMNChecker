using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMNModel.Camunda
{
    public class CamundaJSonType
    {
        public string Name { get; init; }

        public bool IsAbstract { get; init; }

        public List<string> Extends { get; } = new();

        public string? SuperClass { get; init; }

        public Dictionary<string, (string Type, string? Default)> Attributes { get; } = new();

        public Dictionary<string, (string Type, bool IsBody, bool IsMany)> ExtensionElements { get; } = new();

        public List<(string Name, string Type, bool IsBody, bool IsMany)> AllExtensionElements { get; } = new();

        public List<string> AllowedIn { get; } = new();

        public CamundaJSonType(string name, bool isAbstract, string? superClass)
        {
            Name = name;
            IsAbstract = isAbstract;
            SuperClass = superClass;
        }

        public List<string> GetAllInInheritance(Dictionary<string, CamundaJSonType> camuntaTypes)
        {
            if (SuperClass==null) return new List<string> { this.Name };
            else
            {
                if (SuperClass.StartsWith("camunda"))
                {
                    var parentClasses = camuntaTypes[this.SuperClass].GetAllInInheritance(camuntaTypes);
                    parentClasses.Add(this.Name);
                    return parentClasses;
                }else
                {
                    return new List<string> { SuperClass, Name };
                }
            }
        }

        public void Dump(StreamWriter writer)
        {
            writer.WriteLine($"{Name} {(IsAbstract ? "(Abstract)" : "")} : {SuperClass}");
            writer.WriteLine($"Extends: {string.Join(", ", Extends)}");
            writer.WriteLine($"Allowed: {string.Join(", ", AllowedIn)}");
            writer.WriteLine("{");
            if (Attributes.Any())
            {
                writer.WriteLine("  Attributes:");
                foreach (var (attName, (attType, attDefault)) in Attributes)
                {
                    writer.WriteLine($"    {attType} {attName}{(attDefault != null ? " = " + attDefault : "")}");
                }
            }

            if (ExtensionElements.Any())
            {
                if (Attributes.Any()) writer.WriteLine();
                writer.WriteLine("  Extension Elements:");
                foreach (var (elName, (elType, elIsBody, elIsMany)) in ExtensionElements)
                {
                    writer.WriteLine($"    {elType} {elName}{(elIsBody ? " (Body)" : "")}{(elIsMany ? " (Many)" : "")}");
                }
            }
            writer.WriteLine("}\n");
        }
    }
}
