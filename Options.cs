using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;

namespace SendGrid.Templates
{

    public enum Op
    {
        Transfer,
        Save,
    }

    public class Options
    {
        public Op Op { get; set; }

        [Option('f', "from", HelpText = "Specify source access token", Required = true)]
        public string FromKey { get; set; }

        [Option('t', "to", HelpText = "Specify target access token. Required if 'save' unset.", Required = false)]
        public string ToKey { get; set; }

        [Option('s', "save", HelpText = "Save templates on disk", Required = false)]
        public bool Save { get; set; }

        [Option('r', "regex", HelpText = "Regex to apply to template name. Used to generate fine name. If empty, filename will bet set to template name.", Required = false)]
        public string Regex { get; set; }
    }

}
