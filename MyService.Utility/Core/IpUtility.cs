using System.Net;
using System.Net.Sockets;

namespace MyService.Utility.Core
{
    public class IpUtility
    {
        public string GetIPv6Address()
        {
            IPAddress[] addresses = Dns.GetHostAddresses(Dns.GetHostName());
            var ipAddress = addresses.Where(s => s.AddressFamily == AddressFamily.InterNetworkV6 && !s.IsIPv6LinkLocal);
            if (ipAddress.Any())
            {
                return ipAddress.LastOrDefault().ToString();
            }
            else
            {
                return "";
            }
        }

        public string GetIPv4Address()
        {
            IPAddress[] addresses = Dns.GetHostAddresses(Dns.GetHostName());
            var ipAddress = addresses.Where(s => s.AddressFamily == AddressFamily.InterNetwork);
            if (ipAddress.Any())
            {
                return ipAddress.LastOrDefault().ToString();
            }
            else
            {
                return "";
            }
        }
    }
}
