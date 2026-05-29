using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace BalotoAppOnline
{
    public class ResultadoActualizacion
    {
        public bool HayNuevaVersion { get; set; }
        public string VersionActual { get; set; }
        public string VersionDisponible { get; set; }
        public string Mensaje { get; set; }
        public string UrlRelease { get; set; }
        public string RutaDescarga { get; set; }
    }

    public static class ActualizadorGithub
    {
        private const string Repositorio = "BalotoAppOnline";

        public static string UrlRepositorio => "https://github.com/" + ObtenerCuentaGithub() + "/" + Repositorio;

        public static async Task<ResultadoActualizacion> BuscarYDescargarAsync()
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            Version actual = Assembly.GetExecutingAssembly().GetName().Version ?? new Version(1, 0, 0, 0);
            JObject release = await ObtenerUltimoReleaseAsync();
            string tag = ((string)release["tag_name"] ?? string.Empty).Trim();
            string versionLimpia = LimpiarVersion(tag);
            Version disponible = ParsearVersion(versionLimpia);
            string releaseUrl = (string)release["html_url"] ?? UrlRepositorio + "/releases";

            if (disponible <= actual)
            {
                return new ResultadoActualizacion
                {
                    HayNuevaVersion = false,
                    VersionActual = actual.ToString(),
                    VersionDisponible = disponible.ToString(),
                    UrlRelease = releaseUrl,
                    Mensaje = "El paquete más reciente ya está instalado."
                };
            }

            JToken asset = release["assets"]?
                .Children()
                .FirstOrDefault(a => EsPaqueteDescargable((string)a["name"]));

            if (asset == null)
            {
                return new ResultadoActualizacion
                {
                    HayNuevaVersion = true,
                    VersionActual = actual.ToString(),
                    VersionDisponible = disponible.ToString(),
                    UrlRelease = releaseUrl,
                    Mensaje = "Hay una versión nueva, pero el release no tiene un instalador descargable."
                };
            }

            string assetName = (string)asset["name"];
            string downloadUrl = (string)asset["browser_download_url"];
            string destino = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Downloads",
                assetName);

            using (var cliente = CrearCliente())
                await cliente.DownloadFileTaskAsync(new Uri(downloadUrl), destino);

            return new ResultadoActualizacion
            {
                HayNuevaVersion = true,
                VersionActual = actual.ToString(),
                VersionDisponible = disponible.ToString(),
                UrlRelease = releaseUrl,
                RutaDescarga = destino,
                Mensaje = "Actualización descargada correctamente."
            };
        }

        public static void AbrirRepositorio()
        {
            Process.Start(new ProcessStartInfo(UrlRepositorio) { UseShellExecute = true });
        }

        public static void AbrirRelease(string url)
        {
            Process.Start(new ProcessStartInfo(string.IsNullOrWhiteSpace(url) ? UrlRepositorio + "/releases" : url) { UseShellExecute = true });
        }

        private static async Task<JObject> ObtenerUltimoReleaseAsync()
        {
            string api = "https://api.github.com/repos/" + ObtenerCuentaGithub() + "/" + Repositorio + "/releases/latest";
            using (var cliente = CrearCliente())
            {
                string json = await cliente.DownloadStringTaskAsync(api);
                return JObject.Parse(json);
            }
        }

        private static WebClient CrearCliente()
        {
            var cliente = new WebClient();
            cliente.Headers.Add("User-Agent", "BalotoAppOnline");
            cliente.Headers.Add("Accept", "application/vnd.github+json");
            return cliente;
        }

        private static bool EsPaqueteDescargable(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return false;

            string ext = Path.GetExtension(nombre).ToLowerInvariant();
            return ext == ".exe" || ext == ".msi" || ext == ".zip";
        }

        private static string LimpiarVersion(string tag)
        {
            return tag.Trim().TrimStart('v', 'V');
        }

        private static Version ParsearVersion(string valor)
        {
            if (Version.TryParse(valor, out Version version))
                return version;

            return new Version(0, 0, 0, 0);
        }

        private static string ObtenerCuentaGithub()
        {
            string[] partes = { "colombian", "itov2" };
            return string.Concat(partes);
        }
    }
}
