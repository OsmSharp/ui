using OsmSharp.IO.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.Test.Unittests.IO.Json
{
    class JsonConvertTest
    {
        public void Test1()
        {
            var product = new Product();
            product.Name = "Apple";
            product.Expiry = new DateTime(2008, 12, 28);
            product.Sizes = new string[] { "Small" };

            var json = JsonConvert.SerializeObject(product);

            var deserialized = JsonConvert.DeserializeObject<Product>(json);
            
        }

        private class Product
        {
            public string Name { get; set; }

            public DateTime Expiry { get; set; }

            public string[] Sizes { get; set; }
        }
    }
}
