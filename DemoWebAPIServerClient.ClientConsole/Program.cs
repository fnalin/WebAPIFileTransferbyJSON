using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace DemoWebAPIServerClient.ClientConsole
{
    class Program
    {
        public static string FolderPath
        {
            get
            {
                return Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())) + @"/Files";
            }
        }

        public static string Token { get; set; }


        static void Main()
        {
            Token = GetToken();

            if (!string.IsNullOrEmpty(Token))
            {
                var continuar = true;
                while (continuar)
                {
                    BuscarArquivos();
                    Console.WriteLine("\n\n*********************---*****************\n");
                    Console.WriteLine("\nDeseja refazer os uploads na pasta {0}?\n('s'==>SIM | 'n' ==> NÂO)", FolderPath);
                    continuar = Console.ReadLine() == "s" ? true : false;
                }
            }
            else
                Console.WriteLine("\n\nNão é possível enviar os arquivos. Sessão não autorizada");

        }

        private static string GetToken()
        {

            Console.WriteLine("Buscando Token p/ autenticação...");
            
            var body = "userName=email@test.com&password=123@qwe&grant_type=password";
            var token = string.Empty;

            using (var client = new HttpClient())
            {
                var response =
                    client.PostAsync("http://localhost:41031/api/security/token",
                        new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded"))
                    .Result;

                if (response.IsSuccessStatusCode)
                {
                    dynamic content = JsonConvert.DeserializeObject(
                        response.Content.ReadAsStringAsync()
                        .Result);

                    Console.WriteLine("Token obtido!");
                    token = (string)content.access_token;
                }
            }
            return token;
        }

        private static void Post(byte[] arquivo, string fileName)
        {
            using (var client = new HttpClient())
            {
                var dados =
               JsonConvert.SerializeObject(new
               {
                   id = 1,
                   nome = "Fabiano Nalin",
                   file = new { nome = fileName, base64 = Convert.ToBase64String(arquivo) }
               });

                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);

                var response =
                    client.PostAsync("http://localhost:41031/api/fileuploads",
                        new StringContent(dados, Encoding.UTF8, "application/json"))
                    .Result;

                if (response.IsSuccessStatusCode)
                {
                    dynamic content = JsonConvert.DeserializeObject(
                        response.Content.ReadAsStringAsync()
                        .Result);

                    Console.WriteLine("Arquivo {0} enviado c/ sucesso!", fileName);

                    Console.WriteLine("ID: {0} - Nome: {1}\nNome do arquivo:{2} - Tamanho do arquivo convertido no Server em kb: {3}",
                        (string)content.id, (string)content.nome, (string)content.file.nome, (int)content.file.len);
                }
                else
                    Console.WriteLine("Erro ao tentar enviar arquivo. StatusCode: " + response.StatusCode.ToString());
            }
        }

        private static void BuscarArquivos()
        {
            Console.WriteLine("Buscando arquivos...\n");
            var filePaths = Directory.GetFiles(FolderPath, "*.*", SearchOption.TopDirectoryOnly);


            Console.WriteLine("Preparando arquivos...\n");


            filePaths.ToList().ForEach(file =>
            {
                //CompararTamanhos(file);
                var nomeArq = Path.GetFileName(file);
                Console.WriteLine("\nEnviando arquivo {0}...", nomeArq);
                Post(File.ReadAllBytes(file), nomeArq);
            });
        }

        private static void CompararTamanhos(string path)
        {
            var _file = new FileInfo(path);
            var originalSizeInBytes = _file.Length;
            Console.WriteLine("Tamanho Original em kb: " + originalSizeInBytes / 1024);

            var bytes = File.ReadAllBytes(path);
            var file = Convert.ToBase64String(bytes);

            Console.WriteLine("Tamanho Base64 em kb: " + file.Length / 1024);
        }

        
    }

}
