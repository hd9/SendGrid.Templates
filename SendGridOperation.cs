using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace SendGrid.Templates
{

    public class SendGridOperation
    {

        #region Attributes
        private readonly SendGridClient sgSrc;
        private readonly SendGridClient sgDst;
        const int pad = 80;
        const string outDir = "out";
        #endregion

        #region ctor
        public SendGridOperation(string srcToken, string dstToken)
        {
            Logg($"- Connecting to the SendGrid source account: ", postpend: pad - 27);
            sgSrc = new SendGridClient(srcToken);
            Ok();

            if (!string.IsNullOrEmpty(dstToken))
            {
                Logg($"- Connecting to the SendGrid destination account: ");
                sgDst = new SendGridClient(dstToken);
                Ok();
            }
        }
        #endregion

        #region Public Members
        public void TransferTemplates()
        {
            try
            {
                var templates = GetTemplates();
                if (templates == null || templates.Templates == null || !templates.Templates.Any())
                {
                    Log("No templates found to process.");
                    return;
                }

                Log("- Creating templates: ", 2);
                templates.Templates.ForEach(t => CreateTemplate(t));

                Log($"\nFinished! {templates.Templates.Sum(x => x.Versions.Count())} transactional templates created");
            }
            catch (Exception e)
            {
                Log(e);
            }
        }

        /// <summary>
        /// Saves transaction email templates on disk.
        /// </summary>
        public void SaveTemplates(string regex)
        {
            try
            {
                var templates = GetTemplates();
                if (templates == null || templates.Templates == null || !templates.Templates.Any())
                {
                    Log("No templates found to process.");
                    return;
                }

                Log("- Saving templates: ", 2);
                if (Directory.Exists(outDir)) Directory.Delete(outDir, true);
                Directory.CreateDirectory(outDir);

                templates.Templates.ForEach(t =>
                {
                    try
                    {
                        var v = t.Versions.FirstOrDefault();
                        if (v == null)
                            return;

                        var fname = ParseName(v.Name, regex);
                        File.WriteAllText($"{outDir}/{fname}.html", v.Html_content);
                    }
                    catch (Exception e)
                    {
                        Log(e);
                    }
                });

                Log($"- {templates.Templates.Count} templates successfully on the '{outDir}' directory.", 5);
            }
            catch (Exception e)
            {
                Log(e);
            }
        }

        #endregion

        #region Private Members
        
        /// <summary>
        /// Downloads all transaction SendGrid Email templates
        ///  Docs: https://github.com/sendgrid/sendgrid-csharp/blob/master/USAGE.md#retrieve-all-transactional-templates
        /// </summary>
        /// <param name="loadVersions"></param>
        /// <returns></returns>
        private TemplateData GetTemplates(bool loadVersions = true)
        {
            Logg("- Getting all templates: ", postpend: pad - 7);
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

        /// <summary>
        /// Creates a new transaction email template on SendGrid
        /// Docs: https://github.com/sendgrid/sendgrid-csharp/blob/master/USAGE.md#create-a-transactional-template
        /// </summary>
        /// <param name="t"></param>
        private void CreateTemplate(Template t)
        {
            if (t == null)
                return;

            if (sgDst == null)
            {
                Log("ERROR: Couldn't establish a connection to the target SendGrid account. Please provide a valid api key and retry.");
                return;
            }

            try
            {
                Logg($"- Creating template {t.Name}", 5, pad);
                var data = JsonConvert.DeserializeObject($"{{ 'name': '{t.Name }', 'generation': 'dynamic' }}").ToString();
                var resp = sgDst.RequestAsync(method: SendGridClient.Method.POST, urlPath: "templates", requestBody: data).GetAwaiter().GetResult();
                var r = resp.Body.ReadAsStringAsync().Result;
                var tResp = JsonConvert.DeserializeObject<Template>(r);
                Ok();

                var v = t.Versions.FirstOrDefault();
                if (v == null)
                    return;

                Logg($"- Creating version ", 7);
                v.Template_id = tResp.Id;
                data = JsonConvert.SerializeObject(v);
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
                Logg($"- Downloading {v.Name.PadRight(pad)} ", 5);
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
        private object ParseName(string name, string regex)
        {
            return string.IsNullOrEmpty(regex) ? name : Regex.Match(name, regex).Value;
        }

        private void Log(string log, int prepend = 0)
        {
            Console.WriteLine($"{"".PadLeft(prepend)}{log}");
        }

        private void Logg(string log, int prepend = 2, int postpend = 0)
        {
            Console.Write($"{"".PadLeft(prepend)}{log}{"".PadRight(postpend)}");
        }

        private void Ok()
        {
            Console.WriteLine("[ OK ]");
        }

        private void Log(Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Log("\n");
            Console.Error.WriteLine(e);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        #endregion 
    }

}
