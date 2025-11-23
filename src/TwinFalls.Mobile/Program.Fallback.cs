using System;

namespace TwinFalls.Mobile
{
    // This fallback Program is compiled when the DOTNET_MAUI_AVAILABLE
    // constant is NOT defined (i.e. MAUI workloads are not installed).
    // It allows the solution to build for development machines that
    // don't have mobile workloads. Replace or remove when MAUI is available.
#if !DOTNET_MAUI_AVAILABLE
    public static class Program
    {
        public static int Main(string[] args)
        {
            Console.WriteLine("DOTNET_MAUI_AVAILABLE is not set. Mobile project compiled in fallback mode.");
            return 0;
        }
    }
#endif
}
