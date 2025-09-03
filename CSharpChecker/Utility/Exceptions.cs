namespace Utility
{
    public class BPMNCheckerExceptions : Exception
    {
        public BPMNCheckerExceptions(string message, Exception innerException) : base(message,innerException)
        {
        }
        public BPMNCheckerExceptions(string message) : base(message)
        {
        }
    }


}