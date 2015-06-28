using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;

namespace SuperFriendBot
{
    public partial class Service : ServiceBase
    {
        private SuperFriendBot _bot;

        public Service()
        {
            InitializeComponent();
            _bot = new SuperFriendBot();
        }

        protected override void OnStart(string[] args)
        {
            _bot.Start();
        }

        protected override void OnStop()
        {
            _bot.Stop();
        }
    }
}
