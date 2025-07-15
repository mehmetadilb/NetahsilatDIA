using System;
using System.Diagnostics;
using System.ServiceProcess;

namespace NetahsilatDIA
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            var serviceToRun = new Service1();

            if (Debugger.IsAttached)
            {
                serviceToRun.OnDebug();
                return;
            }
            ServiceBase.Run(serviceToRun);
        }
    }
}
