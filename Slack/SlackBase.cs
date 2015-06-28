using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;

namespace SuperFriendBot.Slack
{
    public abstract class SlackBase
    {
        public abstract String Method { get; }

        public virtual async Task Call()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://slack.com/api/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                AddHeaders(client);

                try
                {
                    HttpResponseMessage response = await client.GetAsync(Method);
                    response.EnsureSuccessStatusCode();    // Throw if not a success code.
                    
                    string content = await response.Content.ReadAsStringAsync();
                    var jss = JsonConvert.DeserializeObject(content);

                    // ...
                }
                catch (HttpRequestException /* e */)
                {
                    // Handle exception.
                }
            }
        }

        public virtual void AddHeaders(HttpClient client) { }
    }
}
