using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Threading;
using Distrib;

namespace DistribTest
{
    class Program
    {
        static void Main(string[] args)
        {
          Node n = new Node("127.0.0.1", 44444);
          Node n2 = new Node("127.0.0.1", 44445);
          n2.Init(Communicate);
          n.Init(Computation);
          Thread.Sleep(1000);
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

      static void Communicate()
      {
        Process myProcess = Process.Current;
        myProcess.Send(pid, message);
      }


    }
}
