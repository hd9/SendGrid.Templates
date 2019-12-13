using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SendGrid.Templates
{

    public class SendGridOperation
    {

        #region Attributes
        private readonly SendGridClient sgSrc;
        private readonly SendGridClient sgDst;
        const int pad = 70;
        #endregion

        #region ctor
        public SendGridOperation(string srcToken, string dstToken)
        {
            Logg($"  - Connecting to the SendGrid source account: ");
            sgSrc = new SendGridClient(srcToken);
            Ok();

            Logg($"  - Connecting to the SendGrid destination account: ");
            sgDst = new SendGridClient(dstToken);
            Ok();
        }
        #endregion

        #region Public Members
        public void TransferTemplates()
        {
            try
            {
                // get all templates
                var templates = GetTemplates();
                CreateTemplates(templates.Templates);

                Log($"\nFinished! {templates.Templates.Sum(x => x.Versions.Count())} transactional templates created");
            }
            catch (Exception e)
            {
                Log(e);
            }
        }
        #endregion

        #region Private Members

        private void CreateTemplates(List<Template> templates)
        {
            Log("  - Creating templates: ");
            templates.ForEach(t => CreateTemplate(t));

        }

        // https://github.com/sendgrid/sendgrid-csharp/blob/master/USAGE.md#retrieve-all-transactional-templates
        private TemplateData GetTemplates(bool loadVersions = true)
        {
            Logg("  - Getting all templates: ");
            var response = sgSrc.RequestAsync(method: SendGridClient.Method.GET, urlPath: "templates?generations=dynamic").GetAwaiter().GetResult();

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception($"Error connecting to remote server: {response.StatusCode}");

            var r = response.Body.ReadAsStringAsync().Result;
            var templates = JsonConvert.DeserializeObject<TemplateData>(r);
            Ok();

            if (!loadVersions)
                return templates;

            // get template markup
            Log("  - Loading Versions:");
            templates.Templates.ForEach(t => LoadContent(t.Versions.FirstOrDefault()));

            return templates;
        }

        // https://github.com/sendgrid/sendgrid-csharp/blob/master/USAGE.md#create-a-transactional-template
        private void CreateTemplate(Template t)
        {
            if (t == null)
                return;

            try
            {
                Logg($"     - Creating template {t.Name.PadRight(pad)} ");
                var data = JsonConvert.DeserializeObject<Object>($"{{ 'name': '{t.Name }', 'generation': 'dynamic' }}").ToString();
                var resp = sgDst.RequestAsync(method: SendGridClient.Method.POST, urlPath: "templates", requestBody: data).GetAwaiter().GetResult();
                var r = resp.Body.ReadAsStringAsync().Result;
                var tResp = JsonConvert.DeserializeObject<Template>(r);
                Ok();

                var vSource = t.Versions.FirstOrDefault();
                if (vSource == null)
                    return;

                Logg($"       - Creating version ");
                vSource.Template_id = tResp.Id;
                data = JsonConvert.SerializeObject(vSource);
                resp = sgDst.RequestAsync(method: SendGridClient.Method.POST, urlPath: $"templates/{tResp.Id}/versions", requestBody: data).GetAwaiter().GetResult();
                r = resp.Body.ReadAsStringAsync().Result;
                var vResp = JsonConvert.DeserializeObject<Version>(r);
                Ok();
            }
            catch (Exception e)
            {
                Log(e);
            }
        }

        private void LoadContent(Version v)
        {
            if (v == null)
                return;

            try
            {
                Logg($"     - Downloading {v.Name.PadRight(pad)} ");
                var response = sgSrc.RequestAsync(method: SendGridClient.Method.GET, urlPath: $"templates/{v.Template_id}").GetAwaiter().GetResult();
                Ok();

                var r = response.Body.ReadAsStringAsync().Result;
                var t = JsonConvert.DeserializeObject<Template>(r);
                var v2 = t.Versions.FirstOrDefault();

                if (v2 != null)
                {
                    v.Html_content = v2.Html_content;
                    v.Plain_content = v2.Plain_content;
                }
            }
            catch (Exception e)
            {
                Log(e);
            }
        }

        private void Log(string log)
        {
            Console.WriteLine(log);
        }

        private void Logg(string log)
        {
            Console.Write(log);
        }

        private void Ok()
        {
            Console.WriteLine("[ OK ]");
        }

        private void Log(Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Log("\n");
            Console.WriteLine(e);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        #endregion 
    }

}
