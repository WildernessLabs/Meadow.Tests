using System;
namespace Meadow.Tests
{
    public abstract class TestsBase
    {
        public IF7MeadowDevice Device;

        public TestsBase()
        {
            if (Globals.Instance is { } glbls) {
                Device = glbls.Device;
            } else {
                throw new Exception("Globals must be initialized before instantiating tests.");
            }
        }
    }
}
