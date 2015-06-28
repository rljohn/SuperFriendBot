using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SuperFriendBot
{
    static class Program
    {
        [DllImport("Kernel32.dll")]
        private static extern bool AllocConsole();

        static bool GetBoolSetting(String appSetting, bool defaultValue)
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains(appSetting))
            {
                bool result;
                if (bool.TryParse(ConfigurationManager.AppSettings[appSetting], out result))
                    return result;
                else
                    return defaultValue;
            }

            return defaultValue;
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            bool RunAsService = GetBoolSetting("RunAsService", true);
            if (RunAsService)
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new Service()
                };
                ServiceBase.Run(ServicesToRun);
            }
            else
            {
                SuperFriendBot bot = new SuperFriendBot();
                bot.Start();

                AllocConsole();
                string result = Console.ReadLine();
                while(result != "q")
                {
                    result = Console.ReadLine();
                }
            }
        }
    }
}
