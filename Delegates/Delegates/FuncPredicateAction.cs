using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates
{
    class FuncPredicateAction
    {
        static void Main(string[] args)
        {
            MathInvoke(Add, 3, 5);
            MathInvoke(Subtract, 10, 3);
            EvaluateInvoke(IsGreatEnough, 500);
            EvaluateInvoke(IsGreatEnough, 2000);
            Console.ReadLine();
        }

        private static string Add(int x, int y)
        {
            return (x + y).ToString();
        }

        private static string Subtract(int x, int y)
        {
            return (x - y).ToString();
        }

        private static string Multiply(int x, int y)
        {
            return (x * y).ToString();
        }

        private static bool IsGreatEnough(int x)
        {
            return x > 1000;
        }

        //private static void MathInvoke(Func<int, int, string> mathFunc, int x, int y, Action<string> writer)
        //{
        //    var result = mathFunc(x, y);
        //    writer(result);
        //}
        private static void MathInvoke(Func<int, int, string> mathFunc, int x, int y)
        {
            var result = mathFunc(x, y);
            Console.WriteLine($"Math Result: {result}");
        }

        private static void EvaluateInvoke(Predicate<int> predicateFunc, int x)
        {
            var result = predicateFunc(x);
            Console.WriteLine($"is great enough: {result}");
        }

        //private static void WriteFancy(string text)
        //{
        //    Console.WriteLine($"Fancy output: {text}");
        //}
    }
}
