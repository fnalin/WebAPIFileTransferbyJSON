using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace DemoWebAPIServerClient.API.Controllers
{
    public class TestController:ApiController
    {
        public string Get() {
            return "Funcionou";
        }
    }
}