using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace TiledDataService
{
    class Program
    {
        static void Main(string[] args)
        {
            // get the hostname.
            string hostname = ConfigurationManager.AppSettings["hostname"];

            // start the self-hosting.
            var host = new AppHost();
            host.Init();
            host.Start(hostname);

            Console.ReadLine();
        }
    }
}
