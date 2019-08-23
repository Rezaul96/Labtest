using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LabTest.Web.Configs
{
    public class WebConfiguration
    {
        private static WebConfiguration _instance;
        public static WebConfiguration Instance => _instance ?? (_instance = new WebConfiguration());

        private WebConfiguration()
        {

        }
       
        public string WebApiConfig { get; private set; }
        public string Key { get; private set; }
    }
}
