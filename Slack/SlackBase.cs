using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace SuperFriendBot.Slack
{
    public abstract class SlackBase
    {
        protected WebClient _webClient;
        protected NameValueCollection _queryString;
        protected readonly Encoding _encoding = new UTF8Encoding();

        public abstract String Method { get; }

        public virtual async Task<bool> Call()
        {
            using (_webClient = new WebClient())
            {
                string botKey = System.Configuration.ConfigurationManager.AppSettings["BotKey"];
                _queryString = new NameValueCollection();
                _queryString["token"] = botKey;
                AddQueryStringTokens();

                try
                {
                    Uri uri = new Uri("https://slack.com/api/" + Method);
                    Task<byte[]> downloadTask = _webClient.UploadValuesTaskAsync(uri, "POST", _queryString);
                    var response = await downloadTask;

                    //The response text is usually "ok"
                    string responseText = _encoding.GetString(response);
                    JObject result = JObject.Parse(responseText);
                    
                    bool success = false;
                    if ((bool)result["ok"] == true)
                    {
                        ProcessSuccess(result);
                        success = true;
                    }
                    else
                    {
                        ProcessFailure(result);
                    }

                    return success;
                }
                catch (HttpRequestException /* e */)
                {
                    ProcessError();
                    return false;
                }
            }
        }

        protected virtual void AddQueryStringTokens() {}
        protected virtual void ProcessSuccess(JObject obj) { }
        protected virtual void ProcessFailure(JObject obj) { }
        protected virtual void ProcessError() { }
    }
}
