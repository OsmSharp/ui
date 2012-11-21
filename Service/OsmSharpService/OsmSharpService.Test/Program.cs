using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.ServiceClient.Web;
using System.IO;
using OsmSharpService.Core.Routing;

namespace OsmSharpService.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            JsonServiceClient client = new JsonServiceClient("http://localhost:666/");

            RoutingHook hook1 = new RoutingHook()
            {
                Id = 1,
                Latitude = 51.18221f,
                Longitude = 3.573142f
            };
            RoutingHook hook2 = new RoutingHook()
            {
                Id = 1,
                Latitude = 51.18479f,
                Longitude = 3.56409f
            };
            for (int idx = 0; idx < 1000000; idx++)
            {
                var response = client.Send<RoutingResponse>(
                    new RoutingOperation()
                    {
                        Hooks = new RoutingHook[] { hook1, hook2 },
                        Type = RoutingOperationType.Regular
                    });

                if (response.Route != null)
                {
                    //response.Route.SaveAsGpx(new FileInfo(string.Format(@"c:\temp\route_{0}.gpx",
                    //    idx)));
                }
                Console.WriteLine(response); // => Hello, World
            }
            Console.ReadLine();
        }
    }
}
