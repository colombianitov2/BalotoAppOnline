using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BalotoAppOnline
{
    public class ComentariosForm : Form
    {
        private readonly TextBox txtComentario;
        private readonly TextBox txtContacto;
        private readonly Button btnEnviar;
        private readonly Label lblEstado;

        public ComentariosForm()
        {
            Text = "Ayuda / comentarios";
            Size = new Size(520, 420);
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
                Padding = new Padding(18),
                BackColor = Color.White
            };
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));

            panel.Controls.Add(new Label
            {
                Text = "Escribe tu comentario o solicitud de ayuda",
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(0, 69, 166),
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            }, 0, 0);

            txtComentario = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                BorderStyle = BorderStyle.FixedSingle
            };
            panel.Controls.Add(txtComentario, 0, 1);

            panel.Controls.Add(new Label
            {
                Text = "Tu contacto (opcional)",
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(80, 80, 80),
                TextAlign = ContentAlignment.BottomLeft
            }, 0, 2);

            txtContacto = new TextBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle
            };
            panel.Controls.Add(txtContacto, 0, 3);

            lblEstado = new Label
            {
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(110, 110, 110),
                TextAlign = ContentAlignment.MiddleLeft
            };
            panel.Controls.Add(lblEstado, 0, 4);

            var botones = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft
            };

            btnEnviar = CrearBoton("Enviar", Color.FromArgb(0, 69, 166), Color.White);
            btnEnviar.Click += async (s, e) => await EnviarComentario();

            Button btnCancelar = CrearBoton("Cerrar", Color.FromArgb(230, 232, 235), Color.FromArgb(45, 45, 45));
            btnCancelar.Click += (s, e) => Close();

            botones.Controls.Add(btnEnviar);
            botones.Controls.Add(btnCancelar);
            panel.Controls.Add(botones, 0, 5);

            Controls.Add(panel);
        }

        private Button CrearBoton(string texto, Color fondo, Color frente)
        {
            var boton = new Button
            {
                Text = texto,
                Size = new Size(110, 34),
                BackColor = fondo,
                ForeColor = frente,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10)
            };
            boton.FlatAppearance.BorderSize = 0;
            return boton;
        }

        private async Task EnviarComentario()
        {
            string comentario = txtComentario.Text.Trim();
            if (comentario.Length < 5)
            {
                MessageBox.Show("Escribe un comentario un poco más completo.", "Comentario", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            btnEnviar.Enabled = false;
            lblEstado.Text = "Enviando...";

            try
            {
                bool enviadoDirecto = await ComentariosService.EnviarAsync(comentario, txtContacto.Text.Trim());
                lblEstado.Text = enviadoDirecto
                    ? "Comentario enviado correctamente."
                    : "Se abrió tu correo para terminar el envío.";

                if (enviadoDirecto)
                    MessageBox.Show("Gracias. Tu comentario fue enviado.", "Comentarios", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblEstado.Text = "No se pudo enviar el comentario.";
                MessageBox.Show("No se pudo enviar el comentario: " + ex.Message, "Comentarios", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnEnviar.Enabled = true;
            }
        }
    }
}
