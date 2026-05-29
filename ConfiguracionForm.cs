using System;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BalotoAppOnline
{
    public class ConfiguracionForm : Form
    {
        private readonly Button btnActualizar;
        private readonly Label lblEstado;

        public ConfiguracionForm()
        {
            Text = "Configuración";
            ClientSize = new Size(500, 390);
            MinimumSize = Size;
            MaximumSize = Size;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.White;
            Font = new Font("Segoe UI", 10);

            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 6,
                Padding = new Padding(22),
                BackColor = Color.White
            };
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 88));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));

            panel.Controls.Add(new Label
            {
                Text = "Configuración de la aplicación",
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(0, 69, 166),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            }, 0, 0);

            btnActualizar = CrearBoton("Actualizar", Color.FromArgb(0, 150, 100), Color.White);
            btnActualizar.Click += async (s, e) => await BuscarActualizacion();
            panel.Controls.Add(btnActualizar, 0, 1);

            Button btnComentarios = CrearBoton("Ayuda / comentarios", Color.FromArgb(0, 69, 166), Color.White);
            btnComentarios.Click += (s, e) => new ComentariosForm().ShowDialog(this);
            panel.Controls.Add(btnComentarios, 0, 2);

            Button btnGithub = CrearBoton("Abrir GitHub", Color.FromArgb(245, 246, 248), Color.FromArgb(35, 35, 35));
            btnGithub.Click += (s, e) => ActualizadorGithub.AbrirPerfil();
            panel.Controls.Add(btnGithub, 0, 3);

            lblEstado = new Label
            {
                Text = "Listo",
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(90, 90, 90),
                TextAlign = ContentAlignment.TopLeft,
                Padding = new Padding(0, 12, 0, 0)
            };
            panel.Controls.Add(lblEstado, 0, 4);

            Button btnCerrar = CrearBoton("Cerrar", Color.FromArgb(230, 232, 235), Color.FromArgb(45, 45, 45));
            btnCerrar.Anchor = AnchorStyles.Right;
            btnCerrar.Click += (s, e) => Close();
            panel.Controls.Add(btnCerrar, 0, 5);

            Controls.Add(panel);
        }

        private Button CrearBoton(string texto, Color fondo, Color frente)
        {
            var boton = new Button
            {
                Text = texto,
                Dock = DockStyle.Fill,
                BackColor = fondo,
                ForeColor = frente,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(0, 5, 0, 5)
            };
            boton.FlatAppearance.BorderSize = 0;
            return boton;
        }

        private async Task BuscarActualizacion()
        {
            btnActualizar.Enabled = false;
            lblEstado.Text = "Buscando la versión más reciente...";

            try
            {
                ResultadoActualizacion resultado = await ActualizadorGithub.BuscarYDescargarAsync();
                lblEstado.Text = resultado.Mensaje;

                if (!resultado.HayNuevaVersion)
                {
                    MessageBox.Show(resultado.Mensaje, "Actualización", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (string.IsNullOrWhiteSpace(resultado.RutaDescarga))
                {
                    DialogResult abrir = MessageBox.Show(
                        resultado.Mensaje + "\n\n¿Quieres abrir la página del release?",
                        "Actualización disponible",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information);
                    if (abrir == DialogResult.Yes)
                        ActualizadorGithub.AbrirRelease(resultado.UrlRelease);
                    return;
                }

                DialogResult ejecutar = MessageBox.Show(
                    "Se descargó la versión " + resultado.VersionDisponible + " en:\n" +
                    resultado.RutaDescarga + "\n\n¿Quieres abrir el instalador ahora?",
                    "Actualización descargada",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information);

                if (ejecutar == DialogResult.Yes)
                    Process.Start(new ProcessStartInfo(resultado.RutaDescarga) { UseShellExecute = true });
            }
            catch (WebException)
            {
                lblEstado.Text = "No se pudo consultar GitHub.";
                MessageBox.Show(
                    "No se pudo consultar GitHub. Verifica que el repositorio tenga un release publicado.",
                    "Actualización",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                lblEstado.Text = "No se pudo completar la actualización.";
                MessageBox.Show("Error: " + ex.Message, "Actualización", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnActualizar.Enabled = true;
            }
        }
    }
}
