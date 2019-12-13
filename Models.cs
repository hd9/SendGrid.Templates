using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendGrid.Templates
{
    public class Version
    {
        public string Id { get; set; }
        public int User_id { get; set; }
        public string Template_id { get; set; }
        public int Active { get; set; }
        public string Name { get; set; }
        public string Html_content { get; set; }
        public string Plain_content { get; set; }
        public bool Generate_plain_content { get; set; }
        public string Subject { get; set; }
        public string Updated_at { get; set; }
        public string Editor { get; set; }
        public string Thumbnail_url { get; set; }
    }

    public class Template
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Generation { get; set; }
        public string Updated_at { get; set; }
        public IList<Version> Versions { get; set; }
    }

    public class TemplateData
    {
        public List<Template> Templates { get; set; }
    }
}
