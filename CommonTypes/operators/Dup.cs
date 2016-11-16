namespace CommonTypes.operators
{
    public class Dup : RemoteOperator
    {
        public Dup(string[] inputSources, string[] outputSources, string routing, bool logLevel)
            : base("DUP", inputSources, outputSources, routing, logLevel)
        {
        }

        public override void doOperation(Tuple input)
        {
            result = input;
        }
    }
}
