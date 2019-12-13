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
                Bar();
                Log("SendGrid Template Migration Tool");
                Bar();

                new Parser(p => { p.EnableDashDash = false; p.HelpWriter = Console.Out; p.CaseSensitive = true; })
                    .ParseArguments<Options>(args)
                    .MapResult(
                      (Options opts) => Run(opts),
                      errs => 1);
            }
            catch (Exception e)
            {
                Log(e);
            }
        }

        private static int Run(Options o)
        {
            var sg = new SendGridOperation(o.SrcToken, o.DstToken);

            switch (o.Op)
            {
                case Op.Transfer:
                    sg.TransferTemplates();
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
        
    }
}
