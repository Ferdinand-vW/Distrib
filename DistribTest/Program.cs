using System;
using System.Collections.Generic;
using System.Threading;
using Distrib;

namespace DistribTest
{
    class Program
    {
        static void Main(string[] args)
        {
          Node n = new Node("127.0.0.1", 44444);
          n.Init(Computation);
          n.ShutDown();
          Console.ReadLine();
        }

      static void Computation()
      {
        int sum = 0;
        for (int i = 0; i < 1000; i++)
        {
          sum += i;
          Thread.Sleep(1);
        }

        Console.WriteLine(sum);
        Console.WriteLine("done");
      }

    }
}
