using System;
using Meadow.Devices;

namespace Meadow.Tests
{
    public class Globals
    {
        public static Globals? Instance { get; private set; }

        public IF7MeadowDevice Device { get; }

        public Globals(IF7MeadowDevice device)
        {
            if(Instance != null) { return; }
            Instance = this;
            this.Device = device;
        }
    }
}
