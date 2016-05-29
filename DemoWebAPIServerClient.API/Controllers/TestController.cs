using System.Web.Http;

namespace DemoWebAPIServerClient.API.Controllers
{
    public class TestController : ApiController
    {
        public string Get()
        {
            return "Funcionou";
        }
    }
}