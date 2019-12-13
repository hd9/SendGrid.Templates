using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;

namespace SendGrid.Templates
{

    public enum Op
    {
        Transfer,
    }

    public class Options
    {
        public Op Op { get; }

        [Option(HelpText = "Specify source access token", Required = true)]
        public string SrcToken { get; set; }

        [Option(HelpText = "Specify destination access token", Required = true)]
        public string DstToken { get; set; }
    }

}
