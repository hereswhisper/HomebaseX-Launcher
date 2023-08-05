using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.NetworkInformation;

namespace HomebaseX
{
    internal class API
    {
        public static async Task<bool> pingAPI()
        {
            string ipAddress = "147.185.221.16";
            int port = 15316;

            bool isServerOnline = PingServer(ipAddress, port);

            if (isServerOnline)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool PingServer(string ipAddress, int port)
        {
            try
            {
                using (Ping ping = new Ping())
                {
                    PingReply reply = ping.Send(ipAddress, 1000); // Timeout set to 1000ms (1 second)
                    return reply.Status == IPStatus.Success;
                }
            }
            catch (PingException)
            {
                return false;
            }
        }
    }
}
