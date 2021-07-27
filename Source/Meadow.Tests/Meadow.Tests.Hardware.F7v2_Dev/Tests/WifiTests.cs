using System;
using System.Net.Http;
using System.Threading.Tasks;
using Meadow.Devices;
using Meadow.Gateway.WiFi;

namespace Meadow.Tests.Hardware.F7v2_Dev.Tests
{
    public class WifiTests
    {
        //==== convenience
        // TODO: need to consider adding the wifi stuff to `IF7MeadowDevice` in Meadow.Core
        protected F7Micro Device => MeadowApp.Device;

        //==== singleton stuff
        private static readonly Lazy<WifiTests> instance =
            new Lazy<WifiTests>(() => new WifiTests());
        public static WifiTests Instance {
            get { return instance.Value; }
        }

        protected WifiTests()
        {
            Initialize();
        }

        protected void Initialize()
        {
            Console.WriteLine("Initialize hardware...");

        }

        public async Task RunTests()
        {
            bool success = true;

            //---- Network Scan
            try {
                ScanForAccessPoints();
            } catch (Exception e) {
                success = false;
                Console.WriteLine($"Test failed: Err while scanning wifi networks: {e.Message}");
            }

            //---- network connnection
            try {
                Device.WiFiAdapter.WiFiConnected += (s, e) => {
                    Console.WriteLine("Connection request completed.");
                };

                Console.WriteLine($"Connecting to WiFi Network {Secrets.WIFI_NAME}");
                var connectionResult = await Device.WiFiAdapter.Connect(Secrets.WIFI_NAME, Secrets.WIFI_PASSWORD);
                if (connectionResult.ConnectionStatus != ConnectionStatus.Success) {
                    throw new Exception($"Cannot connect to network: {connectionResult.ConnectionStatus}");
                }
            } catch (Exception e) {
                success = false;
                Console.WriteLine($"Test failed: Err while connecting to network: {e.Message}");
            }

            //---- SSL request
            try {
                GetWebPageViaHttpClient("https://postman-echo.com/get?foo1=bar1&foo2=bar2").Wait();
                Console.WriteLine("SSL Wifi request successful.");
            } catch (Exception e) {
                success = false;
                Console.WriteLine($"Test failed: Err while retreiving webpage: {e.Message}");
            }
        }

        protected void ScanForAccessPoints()
        {
            Console.WriteLine("Getting list of access points.");
            var networks = Device.WiFiAdapter.Scan();
            if (networks.Count > 0) {
                Console.WriteLine("|-------------------------------------------------------------|---------|");
                Console.WriteLine("|         Network Name             | RSSI |       BSSID       | Channel |");
                Console.WriteLine("|-------------------------------------------------------------|---------|");
                foreach (WifiNetwork accessPoint in networks) {
                    Console.WriteLine($"| {accessPoint.Ssid,-32} | {accessPoint.SignalDbStrength,4} | {accessPoint.Bssid,17} |   {accessPoint.ChannelCenterFrequency,3}   |");
                }
            } else {
                Console.WriteLine($"No access points detected.");
            }
        }

        public async Task GetWebPageViaHttpClient(string uri)
        {
            Console.WriteLine($"Requesting {uri}");

            using (HttpClient client = new HttpClient()) {
                client.Timeout = new TimeSpan(0, 5, 0);

                HttpResponseMessage response = await client.GetAsync(uri);

                try {
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseBody);
                } catch (TaskCanceledException) {
                    Console.WriteLine("Request time out.");
                } catch (Exception e) {
                    Console.WriteLine($"Request went sideways: {e.Message}");
                }
            }
        }
    }
}
