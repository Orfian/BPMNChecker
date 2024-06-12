using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMNModel.Model
{
    public class Element
    {
        public string Value { get; init; }

        public Element(string value)
        {
            Value = value;  
        }
    }
}