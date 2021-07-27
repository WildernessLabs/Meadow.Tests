using System;
using System.Collections.Generic;
using Meadow.Devices;
using Meadow.Gateway.WiFi;

namespace Meadow.Tests.Hardware.F7v2_Dev.Tests
{
    /// <summary>
    /// Run this test without an antenna attached. It'll validate whether or not
    /// the antenna can be switched, and if it can, the network quality is significantly
    /// worse with the external antenna.
    /// </summary>
    public class AntennaSwitchTests
    {
        //==== convenience
        // TODO: need to consider adding the wifi stuff to `IF7MeadowDevice` in Meadow.Core
        protected F7Micro Device => MeadowApp.Device;

        //==== singleton stuff
        private static readonly Lazy<AntennaSwitchTests> instance =
            new Lazy<AntennaSwitchTests>(() => new AntennaSwitchTests());
        public static AntennaSwitchTests Instance {
            get { return instance.Value; }
        }

        protected AntennaSwitchTests()
        {
            Initialize();
        }


        void Initialize()
        {
            Console.WriteLine("Initialize hardware...");
        }

        public void RunTests()
        {
            try {
                Console.WriteLine("Running antenna switch test.");

                // set the antenna to onboard, don't persist the change on reboot
                Console.WriteLine("Setting to onboard antenna.");
                Device.SetAntenna(Gateways.AntennaType.OnBoard, persist: false);

                // get the networks
                Console.WriteLine("Scanning for networks.");
                var onboardAntennaNetworks = Device.WiFiAdapter.Scan();
                Console.WriteLine($"Found {onboardAntennaNetworks.Count} networks.");

                // switch to the external antenna
                Console.WriteLine("Setting to external antenna.");
                Device.SetAntenna(Gateways.AntennaType.External, persist: false);

                // get the networks again
                Console.WriteLine("Scanning for networks.");
                var externalAntennaNetworks = Device.WiFiAdapter.Scan();
                Console.WriteLine($"Found {onboardAntennaNetworks.Count} networks.");

                // validate them
                bool success = ValidateNetworkChange(onboardAntennaNetworks, externalAntennaNetworks);

                // switch back to the onboard
                Console.WriteLine("Setting to onboard antenna.");
                Device.SetAntenna(Gateways.AntennaType.OnBoard, persist: false);

            } catch (Exception e) {
                Console.WriteLine($"Antenna switch failed: {e.Message}");
            }
        }

        /// <summary>
        /// Validates the two network set to determine if either of these are true:
        ///  * There are fewer networks with external antenna
        ///  * SSIDs found with external antenna are significantly weaker (10db or more)
        ///
        /// If 
        /// </summary>
        /// <param name="onboardAntennaNetworks"></param>
        /// <param name="externalAntennaNetworks"></param>
        /// <returns></returns>
        protected bool ValidateNetworkChange(
            IList<WifiNetwork> onboardAntennaNetworks,
            IList<WifiNetwork> externalAntennaNetworks)
        {
            // if there are fewer networks, that's good.
            if (onboardAntennaNetworks.Count > externalAntennaNetworks.Count) {
                Console.WriteLine("Test passed; found fewer networks with external antenna");
                return true;
            }

            // if the same number of networks are present, see if they're at least
            // 10db worse in strength
            foreach (WifiNetwork network in onboardAntennaNetworks) {
                WifiNetwork? externalNetwork = FindNetwork(network.Bssid, externalAntennaNetworks);
                if (externalNetwork is { } extNet) {
                    if (extNet.SignalDbStrength - network.SignalDbStrength >= 10) {
                        Console.WriteLine("Test passed; network strength is significantly worse.");
                        return true;
                    }
                }
            }
            // if we got here, we didn't pass
            Console.WriteLine("Test failed; same number of networks, and similar strength.");
            return false;
        }

        protected WifiNetwork? FindNetwork(string bssid, IList<WifiNetwork> networks)
        {
            foreach (var network in networks) {
                if (network.Bssid == bssid) {
                    return network;
                }
            }
            return null;
        }
    }
}
