using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using NLog;


namespace TestApp
{
    class AwaitStructure
    {
        //private static Logger logger = LogManager.GetCurrentClassLogger();

        public static async Task HeavyWork()
        {
            Console.WriteLine("--Enter HeavyWork()--");

            Console.WriteLine("--Task id={0},Thread id={1},Context={2}--",
                Task.CurrentId, Thread.CurrentThread.ManagedThreadId,
                SynchronizationContext.Current);

            await Task.Run(() => { Program.HeavyMethod(); });

            Console.WriteLine("--Task id={0},Thread id={1},Context={2}--",
                Task.CurrentId, Thread.CurrentThread.ManagedThreadId,
                SynchronizationContext.Current);

            Console.WriteLine("--Exit HeavyWork()--");

        }

        public static async Task<string> HeavyWorkWithReturn()
        {
            Console.WriteLine("--Enter HeavyWork()--");

            Console.WriteLine("--Task id={0},Thread id={1},Context={2}--",
                Task.CurrentId, Thread.CurrentThread.ManagedThreadId,
                SynchronizationContext.Current);

            await Task.Run(() => { Program.HeavyMethod(); });

            Console.WriteLine("--Task id={0},Thread id={1},Context={2}--",
                Task.CurrentId, Thread.CurrentThread.ManagedThreadId,
                SynchronizationContext.Current);

            Console.WriteLine("--Exit HeavyWork()--");

            return "@@@@ AwaitStructure.HeavyWorkWithReturn() Finished @@@@";

        }
    }

    class AwaitCodeSim
    {
        //private static Logger logger = LogManager.GetCurrentClassLogger();

        public static Task HeavyWork()
        {
            Console.WriteLine("--Enter HeavyWork()--");

            Console.WriteLine("--Task id={0},Thread id={1},Context={2}--",
                Task.CurrentId, Thread.CurrentThread.ManagedThreadId,
                SynchronizationContext.Current);
            var state = 0;
            Task tsk = null;
            Task lastTsk = null;

            void a()
            {
                switch (state)
                {
                    case 1: goto Case_1;
                }

                tsk = Task.Run(() => { Program.HeavyMethod(); });
                lastTsk = tsk.ContinueWith((_) => { a(); });
                state = 1;
                return;

            Case_1:
                tsk.Wait();

                Console.WriteLine("--Task id={0},Thread id={1},Context={2}--",
                    Task.CurrentId, Thread.CurrentThread.ManagedThreadId,
                    SynchronizationContext.Current);

                Console.WriteLine("--Exit HeavyWork()--");

                return;

            }

            a();
            return lastTsk;

        }

        public static Task<string> HeavyWorkWithReturn()
        {
            Console.WriteLine("--Enter HeavyWork()--");

            Console.WriteLine("--Task id={0},Thread id={1},Context={2}--",
                Task.CurrentId, Thread.CurrentThread.ManagedThreadId,
                SynchronizationContext.Current);
            var state = 0;
            var tcs = new TaskCompletionSource<string>();
            Task tsk = null;

            void a()
            {
                switch (state)
                {
                    case 1: goto Case_1;
                }

                tsk = Task.Run(() => { Program.HeavyMethod(); });
                tsk.ContinueWith((_) => { a(); });
                state = 1;
                return;

            Case_1:
                tsk.Wait();

                Console.WriteLine("--Task id={0},Thread id={1},Context={2}--",
                    Task.CurrentId, Thread.CurrentThread.ManagedThreadId,
                    SynchronizationContext.Current);

                Console.WriteLine("--Exit HeavyWork()--");

                tcs.SetResult("@@@@ AwaitCodeSim.HeavyWorkWithReturn() Finished @@@@");

            }

            a();
            return tcs.Task;

        }

    }

    class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        //private static Logger logger = null;

        public static void HeavyMethod()
        {
            Console.WriteLine("---Enter HeavyMethod()---");

            Console.WriteLine("---Task id={0},Thread id={1},Context={2}---",
                            Task.CurrentId, Thread.CurrentThread.ManagedThreadId,
                            SynchronizationContext.Current);

            Thread.Sleep(2000);

            Console.WriteLine("---Task id={0},Thread id={1},Context={2}---",
                            Task.CurrentId, Thread.CurrentThread.ManagedThreadId,
                            SynchronizationContext.Current);
            Console.WriteLine("---Exit HeavyMethod()---");
        }


        static void Main(string[] args)
        {
#if false
            if (logger == null)
            {
                LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration(
                    @"C:\Users\chara\source\repos\TestApp2\NLog.config");
                logger = LogManager.GetCurrentClassLogger();
            }
#endif // false
            while (true)
            {
                logger.Info("Enter Main() {Name}", "Sakamoto");

                Console.WriteLine("Enter Main()");

                Console.WriteLine("Task id={0},Thread id={1},Context={2}",
                    Task.CurrentId, Thread.CurrentThread.ManagedThreadId,
                    SynchronizationContext.Current);

                Console.WriteLine("Before call AwaitStructure.HeavyWork()");
                Task t = AwaitStructure.HeavyWork();
                Console.WriteLine("Return from AwaitStructure.HeavyWork() [tid={0}]", t.Id);
                Console.WriteLine("Task id={0},Thread id={1},Context={2}",
                    Task.CurrentId, Thread.CurrentThread.ManagedThreadId,
                    SynchronizationContext.Current);
                Console.WriteLine("...Waiting task completion...");
                t.Wait();
                Console.WriteLine("...Task Complete...");
                Console.WriteLine("===============================================");

                Console.WriteLine("Before call AwaitCodeSim.HeavyWork()");
                t = AwaitCodeSim.HeavyWork();
                Console.WriteLine("Return from AwaitCodeSim.HeavyWork() [tid={0}]", t.Id);
                Console.WriteLine("Task id={0},Thread id={1},Context={2}",
                    Task.CurrentId, Thread.CurrentThread.ManagedThreadId,
                    SynchronizationContext.Current);
                Console.WriteLine("...Waiting task completion...");
                t.Wait();
                Console.WriteLine("...Task Complete...");

                Console.WriteLine("===============================================");
                string ret = null;
                Console.WriteLine("Before call AwaitStructure.HeavyWorkWithReturn()");
                Task<string> t2 = AwaitStructure.HeavyWorkWithReturn();
                Console.WriteLine("Return from AwaitStructure.HeavyWorkWithReturn() [tid={0}]", t.Id);
                Console.WriteLine("Task id={0},Thread id={1},Context={2}",
                    Task.CurrentId, Thread.CurrentThread.ManagedThreadId,
                    SynchronizationContext.Current);
                Console.WriteLine("...Waiting task completion...");
                ret = t2.Result;
                Console.WriteLine("...Task Complete:{0}...", ret);
                Console.WriteLine("===============================================");

                ret = null;
                Console.WriteLine("Before call AwaitCodeSim.HeavyWorkWithReturn()");
                t2 = AwaitCodeSim.HeavyWorkWithReturn();
                Console.WriteLine("Return from AwaitCodeSim.HeavyWorkWithReturn() [tid={0}]", t.Id);
                Console.WriteLine("Task id={0},Thread id={1},Context={2}",
                    Task.CurrentId, Thread.CurrentThread.ManagedThreadId,
                    SynchronizationContext.Current);
                Console.WriteLine("...Waiting task completion...");
                ret = t2.Result;
                Console.WriteLine("...Task Complete:{0}...", ret);

                Console.WriteLine("#### Input exit to finish or just enter to continue. ####");

            
                string str = Console.ReadLine();
                if (str == "exit")
                    break;

                /* Reload Nlog.config
                 * loggerをcreateした後でもconfigは変え放題であることが分かった。
                 * なので、Terminal毎に変更IFを準備したら、ランタイムでConfig.XXX
                 * を変更すれば幾らでも再構成可能。
                 * 下記例は、コンフィグファイルを再読み込みしているが、APIを使って
                 * プログラムで処理しても全く同じことが可能。
                 */
                LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration(
                    @"C:\Users\chara\source\repos\TestApp2\TestApp\NLog.config");

            }

        }

    }
}
