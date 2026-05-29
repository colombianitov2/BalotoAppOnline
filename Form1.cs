using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BalotoAppOnline
{
    public partial class Form1 : Form
    {
        private Timer timerReloj;
        private Label lblReloj;
        private Label lblProgreso;
        private RoundedButton btnActualizarWeb;

        public Form1()
        {
            ConfigurarVentana();
            ConfigurarReloj();
            ConfigurarProgreso();
            ConfigurarControles();
        }

        private void ConfigurarVentana()
        {
            Text = "Generador de Baloto Online";
            ClientSize = new Size(600, 760);
            MinimumSize = Size;
            MaximumSize = Size;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(240, 242, 245);
            Font = new Font("Segoe UI", 10, FontStyle.Regular);
        }

        private void ConfigurarReloj()
        {
            lblReloj = new Label
            {
                Dock = DockStyle.Top,
                Height = 50,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                BackColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter
            };
            Controls.Add(lblReloj);

            timerReloj = new Timer { Interval = 60000 };
            timerReloj.Tick += (s, e) => lblReloj.Text = DateTime.Now.ToString("dddd, dd/MM/yyyy HH:mm");
            timerReloj.Start();
            lblReloj.Text = DateTime.Now.ToString("dddd, dd/MM/yyyy HH:mm");
        }

        private void ConfigurarProgreso()
        {
            lblProgreso = new Label
            {
                Dock = DockStyle.Top,
                Height = 30,
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 100, 100),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "Listo"
            };
            Controls.Add(lblProgreso);
        }

        private void ConfigurarControles()
        {
            TableLayoutPanel mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 1
            };
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            TableLayoutPanel centralPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1
            };
            centralPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 8));
            centralPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 84));
            centralPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 8));

            FlowLayoutPanel flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = false,
                AutoScroll = true,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 8, 0, 8)
            };

            // ========== BOTONES AMARILLOS (generadores) en el nuevo orden ==========
            RoundedButton btnAutomatico = CrearBoton("Automático", Color.FromArgb(253, 200, 47), Color.FromArgb(30, 30, 30), Color.FromArgb(240, 185, 30));
            RoundedButton btnFrecuencia = CrearBoton("Resultado por frecuencia", Color.FromArgb(253, 200, 47), Color.FromArgb(30, 30, 30), Color.FromArgb(240, 185, 30));
            RoundedButton btnGauss = CrearBoton("Sugerencia Gaussiana", Color.FromArgb(253, 200, 47), Color.FromArgb(30, 30, 30), Color.FromArgb(240, 185, 30));

            // ========== BOTONES AZULES (gestión de datos y utilidades) ==========
            RoundedButton btnIngresar = CrearBoton("Ingresar datos", Color.FromArgb(0, 69, 166), Color.White, Color.FromArgb(0, 90, 210));
            RoundedButton btnExportar = CrearBoton("Exportar datos (.txt)", Color.FromArgb(0, 69, 166), Color.White, Color.FromArgb(0, 90, 210));
            RoundedButton btnImportar = CrearBoton("Importar datos (.txt)", Color.FromArgb(0, 69, 166), Color.White, Color.FromArgb(0, 90, 210));
            RoundedButton btnHistorial = CrearBoton("Historial", Color.FromArgb(0, 69, 166), Color.White, Color.FromArgb(0, 90, 210));
            RoundedButton btnVerificador = CrearBoton("Verificador de tiquetes", Color.FromArgb(0, 69, 166), Color.White, Color.FromArgb(0, 90, 210));
            RoundedButton btnTabla = CrearBoton("Tabla de análisis", Color.FromArgb(0, 69, 166), Color.White, Color.FromArgb(0, 90, 210));
            btnActualizarWeb = CrearBoton("Actualizar desde web", Color.FromArgb(0, 150, 100), Color.White, Color.FromArgb(0, 170, 120));
            RoundedButton btnConfiguracion = CrearBoton("Configuración", Color.FromArgb(0, 69, 166), Color.White, Color.FromArgb(0, 90, 210));
            RoundedButton btnAcerca = CrearBoton("Acerca de / créditos", Color.FromArgb(0, 69, 166), Color.White, Color.FromArgb(0, 90, 210));

            // ========== EVENTOS ==========
            btnAutomatico.Click += (s, e) => MostrarSugerenciaAutomatica();
            btnFrecuencia.Click += (s, e) => MostrarSugerenciaFrecuentista();
            btnGauss.Click += (s, e) => MostrarSugerenciaGaussiana();
            btnIngresar.Click += (s, e) => new IngresoForm().ShowDialog();
            btnExportar.Click += (s, e) => Exportar();
            btnImportar.Click += (s, e) => Importar();
            btnHistorial.Click += (s, e) => new HistorialForm().ShowDialog();
            btnVerificador.Click += (s, e) => new VerificadorForm().ShowDialog();
            btnTabla.Click += (s, e) => new EstadisticasForm().ShowDialog();
            btnActualizarWeb.Click += async (s, e) => await ActualizarDesdeWeb();
            btnConfiguracion.Click += (s, e) => new ConfiguracionForm().ShowDialog(this);
            btnAcerca.Click += (s, e) => new AcercaDeForm().ShowDialog();

            // ========== ORDEN DE LOS BOTONES EN EL FLOW ==========
            flow.Controls.AddRange(new Control[]
            {
                btnAutomatico,      // 1º amarillo
                btnFrecuencia,      // 2º amarillo
                btnGauss,           // 3º amarillo
                btnIngresar,
                btnExportar,
                btnImportar,
                btnHistorial,
                btnVerificador,
                btnTabla,
                btnActualizarWeb,
                btnConfiguracion,
                btnAcerca
            });

            centralPanel.Controls.Add(flow, 1, 0);
            mainPanel.Controls.Add(centralPanel, 0, 0);
            Controls.Add(mainPanel);
        }

        private RoundedButton CrearBoton(string texto, Color fondoNormal, Color textoNormal, Color fondoHover)
        {
            return new RoundedButton
            {
                Text = texto,
                Size = new Size(480, 38),
                NormalBackColor = fondoNormal,
                NormalForeColor = textoNormal,
                HoverBackColor = fondoHover,
                HoverForeColor = textoNormal,
                BorderRadius = 14,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                Margin = new Padding(0, 2, 0, 2)
            };
        }

        private void Exportar()
        {
            using (var sfd = new SaveFileDialog { Filter = "Archivos de texto|*.txt", Title = "Exportar historial" })
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    DatosBaloto.ExportarTxt(sfd.FileName);
                    MessageBox.Show("Exportado correctamente.", "Éxito");
                }
        }

        private void Importar()
        {
            using (var ofd = new OpenFileDialog { Filter = "Archivos de texto|*.txt", Title = "Importar historial" })
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    DatosBaloto.ImportarTxt(ofd.FileName);
                    MessageBox.Show($"Importación completada. Total sorteos: {DatosBaloto.Sorteos.Count}", "Éxito");
                }
        }

        private void MostrarSugerenciaFrecuentista()
        {
            if (DatosBaloto.Sorteos.Count < 5)
            {
                MessageBox.Show("Mínimo 5 sorteos para generar sugerencia.", "Datos insuficientes");
                return;
            }
            int[] nums = DatosBaloto.SugerenciaFrecuentista();
            MessageBox.Show($"Balotas: {nums[0]:00} {nums[1]:00} {nums[2]:00} {nums[3]:00} {nums[4]:00}\nSúper Balota: {nums[5]:00}", "Resultado por frecuencia");
        }

        private void MostrarSugerenciaGaussiana()
        {
            if (DatosBaloto.Sorteos.Count < 5)
            {
                MessageBox.Show("Mínimo 5 sorteos para generar sugerencia.", "Datos insuficientes");
                return;
            }
            int[] nums = DatosBaloto.SugerenciaGaussiana(new Random());
            MessageBox.Show($"Balotas: {nums[0]:00} {nums[1]:00} {nums[2]:00} {nums[3]:00} {nums[4]:00}\nSúper Balota: {nums[5]:00}", "Sugerencia Gaussiana");
        }

        private void MostrarSugerenciaAutomatica()
        {
            if (DatosBaloto.Sorteos.Count < 5)
            {
                MessageBox.Show("Mínimo 5 sorteos para generar sugerencia.", "Datos insuficientes");
                return;
            }
            int[] nums = DatosBaloto.SugerenciaAutomatica(new Random());
            MessageBox.Show($"Balotas: {nums[0]:00} {nums[1]:00} {nums[2]:00} {nums[3]:00} {nums[4]:00}\nSúper Balota: {nums[5]:00}", "Sugerencia Automática (Inteligente)");
        }

        private async Task ActualizarDesdeWeb()
        {
            btnActualizarWeb.Enabled = false;
            btnActualizarWeb.Text = "Actualizando...";
            lblProgreso.Text = "Iniciando...";

            WebScraper.OnProgreso += (msg) =>
            {
                if (lblProgreso.InvokeRequired)
                    lblProgreso.Invoke(new Action(() => lblProgreso.Text = msg));
                else
                    lblProgreso.Text = msg;
            };

            WebScraper.OnPaginaProcesada += (pag, total) =>
            {
                string txt = $"Página {pag} - Sorteos acumulados: {total}";
                if (lblProgreso.InvokeRequired)
                    lblProgreso.Invoke(new Action(() => lblProgreso.Text = txt));
                else
                    lblProgreso.Text = txt;
            };

            try
            {
                var resultados = await WebScraper.ObtenerResultadosHistoricosAsync();
                if (resultados.Count == 0)
                {
                    lblProgreso.Text = "No se encontraron sorteos nuevos.";
                    MessageBox.Show("No se encontraron sorteos en la web o todos ya existían.", "Actualización", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                lblProgreso.Text = "Guardando en la base de datos...";
                int agregados = DatosBaloto.AgregarSorteosMultiples(resultados, out var errores);
                string mensaje = $"Se encontraron {resultados.Count} sorteos.\nSe agregaron {agregados} nuevos.";
                if (errores?.Count > 0)
                    mensaje += $"\n\nErrores ({errores.Count}):\n" + string.Join("\n", errores.Take(5));

                lblProgreso.Text = $"Finalizado. Agregados: {agregados}";
                MessageBox.Show(mensaje, "Actualización automática", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblProgreso.Text = "Error en la actualización.";
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnActualizarWeb.Enabled = true;
                btnActualizarWeb.Text = "Actualizar desde web";
            }
        }
    }
}
