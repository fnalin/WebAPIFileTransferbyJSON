using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace DemoWebAPIServerClient.API.Controllers
{
    [Authorize]
    public class FileUploadsController : ApiController
    {
        public Task<HttpResponseMessage> Post([FromBody]dynamic body)
        {
            var id = (int)body.id;
            var nome = (string)body.nome;
            var nomeArq = (string)body.file.nome;
            var bytes = Convert.FromBase64String((string)body.file.base64);

            var uploadPath = HttpContext.Current.Server.MapPath("~/Uploads");
            var fullPath = uploadPath + @"\" + nomeArq;

            if (!File.Exists(fullPath))
                File.WriteAllBytes(uploadPath + @"\" + nomeArq, bytes);

            var response =
               Request.CreateResponse(HttpStatusCode.OK, new { id = id, nome = nome, file = new { nome = nomeArq, len = bytes.Length / 1024 } });

            return Task.FromResult<HttpResponseMessage>(response);
        }
    }
}