using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace BalotoAppOnline
{
    public static class DatosBaloto
    {
        private static readonly string AppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BalotoAppOnline");
        private static readonly string HistorialFile = Path.Combine(AppDataPath, "historial.json");
        public static List<Sorteo> Sorteos { get; private set; } = new List<Sorteo>();

        static DatosBaloto() { Cargar(); }

        public static void Cargar()
        {
            if (File.Exists(HistorialFile))
            {
                var json = File.ReadAllText(HistorialFile);
                Sorteos = JsonConvert.DeserializeObject<List<Sorteo>>(json) ?? new List<Sorteo>();
            }
            else Sorteos = new List<Sorteo>();
        }

        public static void Guardar()
        {
            if (!Directory.Exists(AppDataPath)) Directory.CreateDirectory(AppDataPath);
            var json = JsonConvert.SerializeObject(Sorteos, Formatting.Indented);
            File.WriteAllText(HistorialFile, json);
        }

        public static bool AgregarSorteo(Sorteo nuevo, out string mensajeError)
        {
            var existente = Sorteos.FirstOrDefault(s => s.Fecha == nuevo.Fecha && s.Numeros.SequenceEqual(nuevo.Numeros));
            if (existente != null)
            {
                string numerosStr = string.Join(", ", nuevo.Numeros.Select(n => n.ToString("00")));
                mensajeError = $"El número ({numerosStr}) de la fecha {nuevo.Fecha:dd/MM/yyyy} ya se encuentra en la base de datos.";
                return false;
            }
            Sorteos.Add(nuevo);
            Guardar();
            mensajeError = null;
            return true;
        }

        public static int AgregarSorteosMultiples(List<Sorteo> nuevosSorteos, out List<string> errores)
        {
            errores = new List<string>();
            int agregados = 0;
            foreach (var nuevo in nuevosSorteos.OrderBy(s => s.Fecha))
            {
                if (AgregarSorteo(nuevo, out string error))
                    agregados++;
                else
                    errores.Add(error);
            }
            return agregados;
        }

        public static void ExportarTxt(string rutaArchivo)
        {
            using (var sw = new StreamWriter(rutaArchivo))
            {
                foreach (var s in Sorteos.OrderBy(s => s.Fecha))
                {
                    var linea = $"{s.Fecha:dd/MM/yyyy} ";
                    for (int i = 0; i < 6; i++) linea += $"{s.Numeros[i]:00} ";
                    sw.WriteLine(linea.TrimEnd());
                }
            }
        }

        public static void ImportarTxt(string rutaArchivo)
        {
            var nuevos = new List<Sorteo>();
            foreach (var linea in File.ReadAllLines(rutaArchivo))
            {
                if (string.IsNullOrWhiteSpace(linea)) continue;
                var partes = linea.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (partes.Length != 7) continue;
                if (!DateTime.TryParseExact(partes[0], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime fecha)) continue;
                var numeros = new int[6];
                bool error = false;
                for (int i = 0; i < 6; i++) if (!int.TryParse(partes[i + 1], out numeros[i])) { error = true; break; }
                if (error) continue;
                var sorteo = new Sorteo { Fecha = fecha, Numeros = numeros };
                if (sorteo.EsValido() && !Sorteos.Contains(sorteo)) nuevos.Add(sorteo);
            }
            Sorteos.AddRange(nuevos);
            Guardar();
        }

        public static Dictionary<int, Dictionary<int, int>> ObtenerFrecuenciasPorPosicion()
        {
            var frec = new Dictionary<int, Dictionary<int, int>>();
            for (int pos = 0; pos < 6; pos++) frec[pos] = new Dictionary<int, int>();
            foreach (var s in Sorteos)
                for (int pos = 0; pos < 6; pos++)
                {
                    int num = s.Numeros[pos];
                    if (!frec[pos].ContainsKey(num)) frec[pos][num] = 0;
                    frec[pos][num]++;
                }
            return frec;
        }

        public static Dictionary<int, Dictionary<int, int>> ObtenerFrecuenciasPorMes(string mesAnio)
        {
            if (mesAnio.Length != 6) return ObtenerFrecuenciasPorPosicion();
            int mes = int.Parse(mesAnio.Substring(0, 2));
            int anio = int.Parse(mesAnio.Substring(2, 4));
            var filtrados = Sorteos.Where(s => s.Fecha.Year == anio && s.Fecha.Month == mes).ToList();
            var frec = new Dictionary<int, Dictionary<int, int>>();
            for (int pos = 0; pos < 6; pos++) frec[pos] = new Dictionary<int, int>();
            foreach (var s in filtrados)
                for (int pos = 0; pos < 6; pos++)
                {
                    int num = s.Numeros[pos];
                    if (!frec[pos].ContainsKey(num)) frec[pos][num] = 0;
                    frec[pos][num]++;
                }
            return frec;
        }

        public static Dictionary<int, Dictionary<int, int>> ObtenerFrecuenciasPorAnio(int anio)
        {
            var filtrados = Sorteos.Where(s => s.Fecha.Year == anio).ToList();
            var frec = new Dictionary<int, Dictionary<int, int>>();
            for (int pos = 0; pos < 6; pos++) frec[pos] = new Dictionary<int, int>();
            foreach (var s in filtrados)
                for (int pos = 0; pos < 6; pos++)
                {
                    int num = s.Numeros[pos];
                    if (!frec[pos].ContainsKey(num)) frec[pos][num] = 0;
                    frec[pos][num]++;
                }
            return frec;
        }

        public static int[] SugerenciaFrecuentista()
        {
            var frec = ObtenerFrecuenciasPorPosicion();
            var seleccionados = new HashSet<int>();
            var resultado = new int[6];
            for (int pos = 0; pos < 5; pos++)
            {
                var candidatos = frec[pos].OrderByDescending(kv => kv.Value).Select(kv => kv.Key).ToList();
                int elegido = 0;
                foreach (var num in candidatos)
                    if (!seleccionados.Contains(num)) { elegido = num; break; }
                if (elegido == 0)
                    for (int n = 1; n <= 43; n++)
                        if (!seleccionados.Contains(n)) { elegido = n; break; }
                resultado[pos] = elegido;
                seleccionados.Add(elegido);
            }
            var superCandidatos = frec[5].OrderByDescending(kv => kv.Value).Select(kv => kv.Key).ToList();
            resultado[5] = superCandidatos.FirstOrDefault();
            if (resultado[5] == 0) resultado[5] = 1;
            return resultado;
        }

        private static int ElegirConPesos(List<int> elementos, List<int> pesos, Random rand)
        {
            int total = pesos.Sum();
            int acum = rand.Next(total);
            int suma = 0;
            for (int i = 0; i < elementos.Count; i++)
            {
                suma += pesos[i];
                if (acum < suma) return i;
            }
            return 0;
        }

        public static int[] SugerenciaGaussiana(Random rand)
        {
            var frec = ObtenerFrecuenciasPorPosicion();
            var seleccionados = new HashSet<int>();
            var resultado = new int[6];
            for (int pos = 0; pos < 5; pos++)
            {
                var numeros = new List<int>();
                var pesos = new List<int>();
                foreach (var kv in frec[pos]) { numeros.Add(kv.Key); pesos.Add(kv.Value); }
                if (numeros.Count == 0)
                    for (int i = 1; i <= 43; i++) { numeros.Add(i); pesos.Add(1); }
                int intentos = 0;
                int elegido = 0;
                while (intentos < 100)
                {
                    int idx = ElegirConPesos(numeros, pesos, rand);
                    int candidato = numeros[idx];
                    if (!seleccionados.Contains(candidato)) { elegido = candidato; break; }
                    intentos++;
                }
                if (elegido == 0)
                    for (int n = 1; n <= 43; n++)
                        if (!seleccionados.Contains(n)) { elegido = n; break; }
                resultado[pos] = elegido;
                seleccionados.Add(elegido);
            }
            var superNumeros = new List<int>();
            var superPesos = new List<int>();
            for (int i = 1; i <= 16; i++)
            {
                superNumeros.Add(i);
                int freq = frec[5].ContainsKey(i) ? frec[5][i] : 1;
                superPesos.Add(freq);
            }
            int idxSuper = ElegirConPesos(superNumeros, superPesos, rand);
            resultado[5] = superNumeros[idxSuper];
            return resultado;
        }

        // *** MÉTODO AUTOMÁTICO MEJORADO (diferente del frecuentista) ***
        public static int[] SugerenciaAutomatica(Random rand)
        {
            if (Sorteos.Count < 5) return SugerenciaFrecuentista();

            var frec = ObtenerFrecuenciasPorPosicion();

            // Números calientes: más repetidos en los últimos 10 sorteos
            var ultimosSorteos = Sorteos.OrderByDescending(s => s.Fecha).Take(10).ToList();
            var calientes = new Dictionary<int, int>();
            for (int i = 0; i < 5; i++)
            {
                foreach (var s in ultimosSorteos)
                {
                    int num = s.Numeros[i];
                    if (!calientes.ContainsKey(num)) calientes[num] = 0;
                    calientes[num]++;
                }
            }
            var numerosCalientes = calientes.OrderByDescending(kv => kv.Value).Select(kv => kv.Key).ToList();

            // Números fríos (no han salido en últimos 20 sorteos)
            var ultimos20 = Sorteos.OrderByDescending(s => s.Fecha).Take(20).SelectMany(s => s.Numeros.Take(5)).Distinct().ToHashSet();
            var todosNumeros = Enumerable.Range(1, 43).ToHashSet();
            var frios = todosNumeros.Except(ultimos20).ToList();

            var seleccionados = new HashSet<int>();
            var resultado = new int[5];

            for (int pos = 0; pos < 5; pos++)
            {
                int elegido = 0;
                // 1. Intentar usar un número caliente
                if (numerosCalientes.Count > 0)
                {
                    var disponibles = numerosCalientes.Where(n => !seleccionados.Contains(n) && n >= 1 && n <= 43).ToList();
                    if (disponibles.Count > 0)
                    {
                        elegido = disponibles[rand.Next(disponibles.Count)];
                        numerosCalientes.Remove(elegido);
                    }
                }
                // 2. Si no hay caliente disponible, usar el más frecuente de la posición
                if (elegido == 0)
                {
                    var candidatosPos = frec[pos].OrderByDescending(kv => kv.Value).Select(kv => kv.Key).ToList();
                    foreach (var num in candidatosPos)
                        if (!seleccionados.Contains(num))
                        {
                            elegido = num;
                            break;
                        }
                }
                // 3. Último recurso: número aleatorio evitando fríos
                if (elegido == 0)
                {
                    var candidatos = Enumerable.Range(1, 43).Where(n => !seleccionados.Contains(n) && !frios.Contains(n)).ToList();
                    if (candidatos.Count == 0) candidatos = Enumerable.Range(1, 43).Where(n => !seleccionados.Contains(n)).ToList();
                    if (candidatos.Count > 0)
                        elegido = candidatos[rand.Next(candidatos.Count)];
                }
                resultado[pos] = elegido;
                seleccionados.Add(elegido);
            }

            Array.Sort(resultado);

            // Súper balota: priorizar la más frecuente en últimos sorteos, luego la global
            var superUltimos = new Dictionary<int, int>();
            foreach (var s in ultimosSorteos)
            {
                int sup = s.Numeros[5];
                if (!superUltimos.ContainsKey(sup)) superUltimos[sup] = 0;
                superUltimos[sup]++;
            }
            int super = 0;
            if (superUltimos.Count > 0)
                super = superUltimos.OrderByDescending(kv => kv.Value).Select(kv => kv.Key).FirstOrDefault();
            if (super == 0 && frec[5].Count > 0)
                super = frec[5].OrderByDescending(kv => kv.Value).Select(kv => kv.Key).FirstOrDefault();
            if (super == 0) super = rand.Next(1, 17);

            return new int[] { resultado[0], resultado[1], resultado[2], resultado[3], resultado[4], super };
        }

        public static List<(string combinacion, int frecuencia)> CombinacionesMasFrecuentesPorMes(string mesAnio, int top = 10)
        {
            if (mesAnio.Length != 6) return new List<(string, int)>();
            int mes = int.Parse(mesAnio.Substring(0, 2));
            int anio = int.Parse(mesAnio.Substring(2, 4));
            var filtrados = Sorteos.Where(s => s.Fecha.Year == anio && s.Fecha.Month == mes).ToList();
            var grupos = new Dictionary<string, int>();
            foreach (var s in filtrados)
            {
                var primeros5 = s.Numeros.Take(5).OrderBy(x => x).ToArray();
                string clave = string.Join(",", primeros5);
                if (!grupos.ContainsKey(clave)) grupos[clave] = 0;
                grupos[clave]++;
            }
            return grupos.OrderByDescending(kv => kv.Value).Take(top).Select(kv => (kv.Key, kv.Value)).ToList();
        }

        public static List<(string combinacion, int frecuencia)> CombinacionesMasFrecuentesGeneral(int top = 10)
        {
            var grupos = new Dictionary<string, int>();
            foreach (var s in Sorteos)
            {
                var primeros5 = s.Numeros.Take(5).OrderBy(x => x).ToArray();
                string clave = string.Join(",", primeros5);
                if (!grupos.ContainsKey(clave)) grupos[clave] = 0;
                grupos[clave]++;
            }
            return grupos.OrderByDescending(kv => kv.Value).Take(top).Select(kv => (kv.Key, kv.Value)).ToList();
        }
    }
}