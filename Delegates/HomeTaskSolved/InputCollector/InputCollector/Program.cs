using System;

namespace InputCollector
{
    class Program
    {
        static void Main(string[] args)
        {
            //Це є центральне місце, для створення об'єктів і підписування на події
            InputHandler handler = new InputHandler();

            StringCollector stringCollector = new StringCollector();
            AlphaNumericCollector alphaNumericCollector = new AlphaNumericCollector();
            handler.Input += stringCollector.Process;
            handler.Input += alphaNumericCollector.Process;
            handler.Run();
        }
    }
}
