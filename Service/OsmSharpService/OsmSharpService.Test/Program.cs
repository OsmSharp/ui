using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.ServiceClient.Web;
using OsmSharpService.Core.Routing;

namespace OsmSharpService.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            JsonServiceClient client = new JsonServiceClient("http://localhost:666/");
            //2;1551494;51.18221;3.573142
            //3;1551503;51.18324;3.57229
            RoutingHook hook1 = new RoutingHook()
            {
                Id = 1,
                Latitude = 51.18221f,
                Longitude = 3.573142f
            };
            RoutingHook hook2 = new RoutingHook()
            {
                Id = 1,
                Latitude = 51.18324f,
                Longitude = 3.57229f
            };

            for (int idx = 0; idx < 100; idx++)
            {
                var response = client.Send<RoutingResponse>(
                    new RoutingOperation()
                    {
                        Hooks = new RoutingHook[] { hook1, hook2 },
                        Type = RoutingOperationType.ManyToMany
                    });
                Console.WriteLine(response); // => Hello, World
            }
            Console.ReadLine();
        }
    }
}
