using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


namespace TestApp
{
    class Program
    {
        static void HeavyMethod()
        {
            Console.WriteLine("---Enter HeavyMethod()---");

            Console.WriteLine("---Task id={0},Thread id={1},Context={2}---",
                            Task.CurrentId, Thread.CurrentThread.ManagedThreadId,
                            SynchronizationContext.Current);

            Thread.Sleep(5000);

            Console.WriteLine("---Task id={0},Thread id={1},Context={2}---",
                            Task.CurrentId, Thread.CurrentThread.ManagedThreadId,
                            SynchronizationContext.Current);
            Console.WriteLine("---Exit HeavyMethod()---");
        }

        static void ExecContinue()
        {
            Console.WriteLine("--Enter ExecContinue()--");

            Console.WriteLine("--Task id={0},Thread id={1},Context={2}--",
                Task.CurrentId, Thread.CurrentThread.ManagedThreadId,
                SynchronizationContext.Current);

            Console.WriteLine("--Exit ExecContinue()--");
        }

        static async Task HeavyWork()
        {
            Console.WriteLine("--Enter HeavyWork()--");

            Console.WriteLine("--Task id={0},Thread id={1},Context={2}--",
                Task.CurrentId, Thread.CurrentThread.ManagedThreadId,
                SynchronizationContext.Current);

            await Task.Run(() => { HeavyMethod(); });

            ExecContinue();

            Console.WriteLine("--Exit HeavyWork()--");

        }

        static void HeavyWork2()
        {
            Console.WriteLine("--Enter HeavyWork2()--");

            Console.WriteLine("--Task id={0},Thread id={1},Context={2}--",
                Task.CurrentId, Thread.CurrentThread.ManagedThreadId,
                SynchronizationContext.Current);

            Task t = Task.Run(() => { HeavyMethod(); }).
                ContinueWith((_) =>
                {
                    ExecContinue();
                },
                TaskScheduler.FromCurrentSynchronizationContext());

            Console.WriteLine("--Exit HeavyWork2()--");

        }

        static void Main(string[] args)
        {
            Console.WriteLine("Enter Main()");

            Console.WriteLine("Task id={0},Thread id={1},Context={2}",
                Task.CurrentId, Thread.CurrentThread.ManagedThreadId,
                SynchronizationContext.Current);

            Console.WriteLine("Before call HeavyWork()");
            Task t = HeavyWork();
            Console.WriteLine("Return from HeavyWork()[tid={0}]", t.Id);
            Console.WriteLine("Task id={0},Thread id={1},Context={2}",
                Task.CurrentId, Thread.CurrentThread.ManagedThreadId,
                SynchronizationContext.Current);
            Console.WriteLine("Enter something");
            Console.ReadLine();

            Console.WriteLine("Before call HeavyWork2()");
            HeavyWork2();
            Console.WriteLine("Return from HeavyWork2()");
            Console.WriteLine("Task id={0},Thread id={1},Context={2}",
                Task.CurrentId, Thread.CurrentThread.ManagedThreadId,
                SynchronizationContext.Current);

            Console.WriteLine("Enter something");
            Console.ReadLine();

            Console.WriteLine("Exit Main()");

        }

    }
}
