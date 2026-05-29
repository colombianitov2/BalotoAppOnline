using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BalotoAppOnline
{
    public static class ComentariosService
    {
        public static async Task<bool> EnviarAsync(string comentario, string contacto)
        {
            string endpoint = ConfigurationManager.AppSettings["FeedbackEndpoint"];
            if (string.IsNullOrWhiteSpace(endpoint))
            {
                AbrirClienteCorreo(comentario, contacto);
                return false;
            }

            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            using (var cliente = new HttpClient())
            {
                var payload = new JObject
                {
                    ["aplicacion"] = "BalotoAppOnline",
                    ["version"] = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0.0",
                    ["comentario"] = comentario,
                    ["contacto"] = contacto ?? string.Empty,
                    ["fecha"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };

                var contenido = new StringContent(payload.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage respuesta = await cliente.PostAsync(endpoint, contenido);
                respuesta.EnsureSuccessStatusCode();
                return true;
            }
        }

        private static void AbrirClienteCorreo(string comentario, string contacto)
        {
            string asunto = Uri.EscapeDataString("Comentario BalotoAppOnline");
            string cuerpo = Uri.EscapeDataString(
                "Comentario:\r\n" + comentario +
                "\r\n\r\nContacto:\r\n" + (contacto ?? string.Empty));
            string uri = "mailto:" + ObtenerCorreoDestino() + "?subject=" + asunto + "&body=" + cuerpo;
            Process.Start(new ProcessStartInfo(uri) { UseShellExecute = true });
        }

        private static string ObtenerCorreoDestino()
        {
            char[] chars =
            {
                'e','p','e','r','n','e','t','t','1','0','2','0',
                '@','h','o','t','m','a','i','l','.','c','o','m'
            };
            return new string(chars);
        }
    }
}
