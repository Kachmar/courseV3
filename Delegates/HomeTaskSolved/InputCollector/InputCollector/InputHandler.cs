using System;
using System.Collections.Generic;
using System.Text;

namespace InputCollector
{
    //Зверніть увагу, що це клас є абсолютно незалежним від класів Collector
    class InputHandler
    {
        public Action<string> Input { get; set; }

        public void Run()
        {
            do
            {
                var text = Console.ReadLine();
                Input(text);

            } while (true);
        }
    }
}
