using BPMNModel;
using System.Reflection.Metadata;
using System.Xml;

namespace BPMNChecker
{
    public class StaticAnalysis
    {
        public StaticAnalysis()
        {

        }

        public void ProcessAnalysis(ModelRoot modelRoot)
        {
            var step1 = new StepOne();

            step1.VisitOnce(modelRoot.Definition);
        }
    }
}
