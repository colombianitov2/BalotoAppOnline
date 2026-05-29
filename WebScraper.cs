using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using PuppeteerSharp;
using HtmlAgilityPack;

namespace BalotoAppOnline
{
    public static class WebScraper
    {
        public static event Action<string> OnProgreso;
        public static event Action<int, int> OnPaginaProcesada;

        public static async Task<List<Sorteo>> ObtenerResultadosHistoricosAsync()
        {
            var todos = new List<Sorteo>();
            string urlBase = "https://www.baloto.com/resultados";
            int paginaActual = 1;
            bool haySiguiente = true;

            string chromeInstalado = BuscarGoogleChromeInstalado();
            if (string.IsNullOrWhiteSpace(chromeInstalado))
                chromeInstalado = BuscarNavegadorIncluido();

            if (string.IsNullOrWhiteSpace(chromeInstalado))
            {
                OnProgreso?.Invoke("Descargando Chromium (primera vez)...");
                await new BrowserFetcher().DownloadAsync();
            }
            else
            {
                OnProgreso?.Invoke("Usando Google Chrome instalado.");
            }

            OnProgreso?.Invoke("Iniciando navegador...");
            using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                ExecutablePath = chromeInstalado
            });
            using var page = await browser.NewPageAsync();

            while (haySiguiente)
            {
                string url = paginaActual == 1 ? urlBase : $"{urlBase}?page={paginaActual}";
                OnProgreso?.Invoke($"Cargando página {paginaActual}: {url}");
                await page.GoToAsync(url, WaitUntilNavigation.DOMContentLoaded);
                await Task.Delay(700);

                // Cerrar posibles modales en la primera página
                if (paginaActual == 1)
                {
                    try
                    {
                        OnProgreso?.Invoke("Cerrando publicidad...");
                        await page.ClickAsync("button[data-dismiss='modal'], .close, .modal .btn-cerrar");
                        await Task.Delay(500);
                    }
                    catch { }
                }

                // Obtener el HTML completo
                string html = await page.GetContentAsync();
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                // Buscar la tabla de resultados
                var table = doc.DocumentNode.SelectSingleNode("//table[@id='results-table']");
                if (table == null)
                {
                    OnProgreso?.Invoke("No se encontró la tabla de resultados.");
                    break;
                }

                var rows = table.SelectNodes(".//tbody/tr");
                if (rows != null)
                {
                    OnProgreso?.Invoke($"Procesando {rows.Count} filas...");
                    foreach (var row in rows)
                    {
                        var cells = row.SelectNodes(".//td");
                        if (cells == null || cells.Count < 3) continue;

                        // Verificar si es Baloto (primera celda contiene imagen con "baloto-kind.png")
                        var img = cells[0].SelectSingleNode(".//img");
                        if (img == null) continue;
                        string src = img.GetAttributeValue("src", "");
                        if (!src.Contains("baloto-kind.png")) continue; // Ignorar Revancha

                        string fechaTxt = cells[1].InnerText.Trim();
                        string numerosTxt = cells[2].InnerText.Trim();

                        DateTime fecha = ParsearFecha(fechaTxt);
                        if (fecha == DateTime.MinValue) continue;

                        // Limpiar números: separar por guiones y espacios
                        var listaNumeros = new List<int>();
                        var partes = numerosTxt.Split(new char[] { '-', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var parte in partes)
                            if (int.TryParse(parte, out int num))
                                listaNumeros.Add(num);

                        if (listaNumeros.Count < 6) continue;
                        int[] numeros = listaNumeros.Take(6).ToArray();

                        bool valido = numeros.Take(5).All(n => n >= 1 && n <= 43) &&
                                      numeros.Take(5).Distinct().Count() == 5 &&
                                      numeros[5] >= 1 && numeros[5] <= 16;

                        if (valido)
                        {
                            var sorteo = new Sorteo { Fecha = fecha, Numeros = numeros };
                            if (!todos.Contains(sorteo))
                            {
                                todos.Add(sorteo);
                                OnPaginaProcesada?.Invoke(paginaActual, todos.Count);
                            }
                        }
                    }
                }

                // Buscar enlace "Siguiente" en el HTML
                var nextLink = doc.DocumentNode.SelectSingleNode("//a[contains(text(), 'Siguiente')]");
                if (nextLink != null)
                {
                    string href = nextLink.GetAttributeValue("href", "");
                    if (!string.IsNullOrEmpty(href))
                    {
                        paginaActual++;
                        OnProgreso?.Invoke($"Hay página {paginaActual}, continuando...");
                        await Task.Delay(1000); // pausa cortés
                    }
                    else
                    {
                        haySiguiente = false;
                    }
                }
                else
                {
                    haySiguiente = false;
                }
            }

            OnProgreso?.Invoke($"Extracción completada. Total sorteos únicos: {todos.Count}");
            return todos;
        }

        private static DateTime ParsearFecha(string txt)
        {
            string[] formatos = {
                "dd 'de' MMMM 'de' yyyy",
                "d 'de' MMMM 'de' yyyy",
                "dd/MM/yyyy",
                "dd-MM-yyyy"
            };
            if (DateTime.TryParseExact(txt, formatos,
                System.Globalization.CultureInfo.GetCultureInfo("es-CO"),
                System.Globalization.DateTimeStyles.None, out DateTime f))
                return f;
            return DateTime.MinValue;
        }

        private static string BuscarGoogleChromeInstalado()
        {
            string[] rutasChromeSistema =
            {
                Path.Combine(Environment.GetEnvironmentVariable("ProgramW6432") ?? string.Empty, "Google", "Chrome", "Application", "chrome.exe"),
                Path.Combine(Environment.GetEnvironmentVariable("ProgramFiles") ?? string.Empty, "Google", "Chrome", "Application", "chrome.exe"),
                Path.Combine(Environment.GetEnvironmentVariable("ProgramFiles(x86)") ?? string.Empty, "Google", "Chrome", "Application", "chrome.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Google", "Chrome", "Application", "chrome.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Google", "Chrome", "Application", "chrome.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Google", "Chrome", "Application", "chrome.exe")
            };

            foreach (string ruta in rutasChromeSistema)
            {
                if (!File.Exists(ruta))
                    continue;

                return ruta;
            }

            return null;
        }

        private static string BuscarNavegadorIncluido()
        {
            string baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string[] nombres =
            {
                "chrome-headless-shell.exe",
                "chrome.exe"
            };

            foreach (string nombre in nombres)
            {
                string encontrado = Directory
                    .GetFiles(baseDir, nombre, SearchOption.AllDirectories)
                    .FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(encontrado))
                    return encontrado;
            }

            return null;
        }
    }
}
