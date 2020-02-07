using CommandLine;
using System;

namespace SendGrid.Templates
{
    class Program
    {

        static readonly DateTime startedAt = DateTime.Now;


        static void Main(string[] args)
        {
            try
            {
                Header("SendGrid Template Migration Tool");

                new Parser(p => { p.EnableDashDash = false; p.HelpWriter = Console.Out; p.CaseSensitive = true; })
                    .ParseArguments<Options>(args)
                    .WithParsed(o => o.Op = o.Save ? Op.Save : Op.Transfer)
                    .MapResult((Options opts) => Run(opts), errs => 1);
            }
            catch (Exception e)
            {
                Log(e);
            }
        }

        private static int Run(Options o)
        {
            var sg = new SendGridOperation(o.FromKey, o.ToKey);

            switch (o.Op)
            {
                case Op.Transfer:
                    sg.TransferTemplates();
                    break;
                case Op.Save:
                    sg.SaveTemplates(o.Regex);
                    break;
            }

            Bar();
            Log("All operations succeeded!");
            Log($"Total Time: {(DateTime.Now - startedAt).TotalSeconds} seconds");
            Bar();

            return 0;
        }

        private static void Log(string log)
        {
            Console.WriteLine(log);
        }

        private static void Log(Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Log("\n");
            Console.WriteLine(e);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private static void Logg(string log)
        {
            Console.Write(log);
        }

        private static void Ok()
        {
            Console.WriteLine("OK");
        }

        private static void Bar()
        {
            Console.WriteLine("----------------------------------");
        }

        private static void Header(string name)
        {
            Bar();
            Log(name);
            Bar();
        }
    }
}
