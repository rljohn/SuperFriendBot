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
        public Uri WebSocketUrl { get; set; }
        public List<Channel> ChatChannels { get; set; }

        public rtm_start()
        {
            ChatChannels = new List<Channel>();
        }

        public override String Method
        {
            get
            {
                return "rtm.start";
            }
        }

        protected override void ProcessSuccess(JObject obj) 
        {
            WebSocketUrl = new Uri((string)obj["url"]);

            JArray channels = (JArray)obj["channels"];
            foreach (JObject channel in channels)
            {
                Channel c = new Channel();
                c.Id = (string)channel["id"];
                c.Name = (string)channel["name"];
                ChatChannels.Add(c);
            }

            JArray users = (JArray)obj["users"];
            foreach(JObject user in users)
            {
                User u = new User();
                u.Id = (string)user["id"];
                u.Name = (string)user["name"];
                u.Color = (string)user["color"];
                u.Presence = (string)user["presence"];
            }

            Logger.Log(obj.ToString());
        }

        protected override void ProcessFailure(JObject obj) 
        {
 
        }
    }
}
