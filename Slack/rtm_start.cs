using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

using Newtonsoft;
using Newtonsoft.Json.Linq;

namespace SuperFriendBot.Slack
{
    public class rtm_start : SlackBase
    {
        public string WebSocketUrl { get; set; }
        public string GeneralChat { get; set; }

        public override String Method
        {
            get
            {
                return "rtm.start";
            }
        }

        protected override void ProcessSuccess(JObject obj) 
        {
            WebSocketUrl = (string)obj["url"];

            JArray channels = (JArray)obj["channels"];
            GeneralChat = (string) channels[0]["id"];
        }

        protected override void ProcessFailure(JObject obj) 
        {
 
        }
    }
}
