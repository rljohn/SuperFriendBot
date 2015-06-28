using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SuperFriendBot.Slack
{
    public class rtm_start : SlackBase
    {
        public override String Method
        {
            get
            {
                return "rtm.start";
            }
        }

        public override void AddHeaders(HttpClient client)
        {
        }
    }
}
