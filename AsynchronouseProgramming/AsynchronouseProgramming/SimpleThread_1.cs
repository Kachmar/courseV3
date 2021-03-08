using System;
using System.Threading;

class ThreadTest
{
    static void Main()
    {
        Thread t = new Thread(WriteY);
        t.Name = "TestName";
        t.Start();
        // Kick off a new thread
        // running WriteY()
        // Simultaneously, do something on the main thread.
        //try uncomment next line and see what happens
        //t.Join();
        for (int i = 0; i < 1000; i++)
        {
            Console.Write("x");
        }
    }
    static void WriteY()
    {
        for (int i = 0; i < 1000; i++)
        {
            Console.Write("y");
        }
    }  
}