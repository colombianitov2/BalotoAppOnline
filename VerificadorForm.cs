using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BalotoAppOnline
{
    public class VerificadorForm : Form
    {
        private TextBox[] txtBalotas = new TextBox[5];
        private TextBox txtSuper;
        private Button btnVerificar;
        private RichTextBox rtbResultado;

        public VerificadorForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Verificador de tiquetes - Baloto";
            Size = new Size(450, 550);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.White;

            Label lblTitulo = new Label
            {
                Text = "VERIFICADOR DE TIQUETES",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 69, 166),
                Dock = DockStyle.Top,
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter
            };

            Panel panelBalotas = new Panel { Dock = DockStyle.Top, Height = 200, Padding = new Padding(20) };
            TableLayoutPanel tlpBalotas = new TableLayoutPanel { ColumnCount = 2, RowCount = 5, Dock = DockStyle.Fill, Padding = new Padding(10) };
            tlpBalotas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
            tlpBalotas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));

            for (int i = 0; i < 5; i++)
            {
                Label lbl = new Label { Text = $"Balota {i + 1} (1-43):", AutoSize = true };
                txtBalotas[i] = new TextBox { MaxLength = 2, TextAlign = HorizontalAlignment.Center };
                txtBalotas[i].KeyPress += SoloDigitos;
                tlpBalotas.Controls.Add(lbl, 0, i);
                tlpBalotas.Controls.Add(txtBalotas[i], 1, i);
            }
            panelBalotas.Controls.Add(tlpBalotas);

            Panel panelSuper = new Panel { Dock = DockStyle.Top, Height = 60, Padding = new Padding(20) };
            TableLayoutPanel tlpSuper = new TableLayoutPanel { ColumnCount = 2, RowCount = 1, Dock = DockStyle.Fill };
            tlpSuper.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
            tlpSuper.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
            Label lblSuper = new Label { Text = "Súper Balota (1-16):", AutoSize = true };
            txtSuper = new TextBox { MaxLength = 2, TextAlign = HorizontalAlignment.Center };
            txtSuper.KeyPress += SoloDigitos;
            tlpSuper.Controls.Add(lblSuper, 0, 0);
            tlpSuper.Controls.Add(txtSuper, 1, 0);
            panelSuper.Controls.Add(tlpSuper);

            btnVerificar = new Button
            {
                Text = "Verificar combinación",
                Dock = DockStyle.Top,
                Height = 45,
                BackColor = Color.FromArgb(0, 69, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnVerificar.Click += BtnVerificar_Click;

            rtbResultado = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Font = new Font("Consolas", 10),
                BackColor = Color.FromArgb(245, 246, 248),
                BorderStyle = BorderStyle.None,
                Padding = new Padding(10)
            };

            Controls.Add(rtbResultado);
            Controls.Add(btnVerificar);
            Controls.Add(panelSuper);
            Controls.Add(panelBalotas);
            Controls.Add(lblTitulo);
        }

        private void SoloDigitos(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void BtnVerificar_Click(object sender, EventArgs e)
        {
            // Limpiar resultado anterior
            rtbResultado.Clear();

            // Validar entrada
            int[] balotas = new int[5];
            for (int i = 0; i < 5; i++)
            {
                if (!int.TryParse(txtBalotas[i].Text, out balotas[i]) || balotas[i] < 1 || balotas[i] > 43)
                {
                    MessageBox.Show($"Balota {i + 1} debe ser un número entre 1 y 43.", "Error de entrada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            if (balotas.Distinct().Count() != 5)
            {
                MessageBox.Show("Las cinco balotas principales deben ser diferentes.", "Error de entrada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int super;
            if (!int.TryParse(txtSuper.Text, out super) || super < 1 || super > 16)
            {
                MessageBox.Show("La súper balota debe ser un número entre 1 y 16.", "Error de entrada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Buscar en el historial
            var coincidencias = DatosBaloto.Sorteos.Where(s =>
                s.Numeros[0] == balotas[0] &&
                s.Numeros[1] == balotas[1] &&
                s.Numeros[2] == balotas[2] &&
                s.Numeros[3] == balotas[3] &&
                s.Numeros[4] == balotas[4] &&
                s.Numeros[5] == super
            ).OrderBy(s => s.Fecha).ToList();

            if (coincidencias.Count == 0)
            {
                rtbResultado.SelectionColor = Color.Red;
                rtbResultado.AppendText("❌ NO SE HA ENCONTRADO\n");
                rtbResultado.SelectionColor = Color.Black;
                rtbResultado.AppendText($"La combinación {string.Join(", ", balotas)} - {super} no ha aparecido en ningún sorteo.\n");
                rtbResultado.AppendText("Si acabas de agregar sorteos recientes, asegúrate de haber actualizado desde la web.");
            }
            else
            {
                rtbResultado.SelectionColor = Color.Green;
                rtbResultado.AppendText($"✅ ¡ENCONTRADO! La combinación ha aparecido {coincidencias.Count} vez/veces.\n\n");
                rtbResultado.SelectionColor = Color.Black;
                foreach (var s in coincidencias)
                {
                    rtbResultado.AppendText($"📅 {s.Fecha:dddd, dd/MM/yyyy}\n");
                    rtbResultado.AppendText($"   Números: {string.Join(" - ", s.Numeros.Select(n => n.ToString("00")))}\n\n");
                }
            }
        }
    }
}