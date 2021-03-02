using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InputCollector
{
    class AlphaNumericCollector
    {
        private readonly List<string> _strings = new List<string>();
        public void Process(string text)
        {
            if (text.Any(c => char.IsDigit(c)))
            {
                _strings.Add(text);
            }
        }
    }
}
