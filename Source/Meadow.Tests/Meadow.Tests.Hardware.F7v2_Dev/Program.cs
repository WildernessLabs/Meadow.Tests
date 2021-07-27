using System.Threading;
using Meadow;

namespace Meadow.Tests.Hardware.F7v2_Dev.Tests
{
    class Program
    {
        static IApp app;
        public static void Main(string[] args)
        {
            // instantiate and run new meadow app
            app = new MeadowApp();

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
