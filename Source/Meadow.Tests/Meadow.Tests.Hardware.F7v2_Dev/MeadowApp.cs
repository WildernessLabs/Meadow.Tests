using System;
using System.Threading;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Leds;

namespace Meadow.Tests.Hardware.F7v2_Dev.Tests
{
    // To be run on the AISU board

    public class MeadowApp : App<F7MicroV2, MeadowApp>
    {
        RgbPwmLed onboardLed;

        public MeadowApp()
        {
            Initialize();

            Globals globals = new Globals(Device);

            //---- wifi
            WifiTests.Instance.RunBasicHardwareTests().Wait();

            //---- antenna switch tests
            AntennaSwitchTests.Instance.RunTests();
        }

        void Initialize()
        {
            Console.WriteLine("Initialize hardware...");

            //---- onboard LED
            onboardLed = new RgbPwmLed(device: Device,
                redPwmPin: Device.Pins.OnboardLedRed,
                greenPwmPin: Device.Pins.OnboardLedGreen,
                bluePwmPin: Device.Pins.OnboardLedBlue,
                3.3f, 3.3f, 3.3f,
                Meadow.Peripherals.Leds.IRgbLed.CommonType.CommonAnode);

            //---- wifi adapter
            if (!Device.InitCoprocessor().Result) {
                throw new Exception("Could not initialize the coprocessor.");
            }
        }

    }
}
