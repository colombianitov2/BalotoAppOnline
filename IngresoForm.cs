using System;
using System.Linq;
using System.Windows.Forms;

namespace BalotoAppOnline
{
    public class IngresoForm : Form
    {
        private TextBox[] txtBalotas = new TextBox[5];
        private TextBox txtSuper;
        private DateTimePicker dtpFecha;
        private Button btnGuardar;

        public IngresoForm() => InitializeComponent();

        private void InitializeComponent()
        {
            Text = "Ingresar sorteo ganador";
            Size = new System.Drawing.Size(320, 280);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            TableLayoutPanel panel = new TableLayoutPanel { ColumnCount = 2, RowCount = 7, Dock = DockStyle.Fill, Padding = new Padding(10) };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));

            for (int i = 0; i < 5; i++)
            {
                panel.Controls.Add(new Label { Text = $"Balota {i + 1} (1-43):", AutoSize = true }, 0, i);
                txtBalotas[i] = new TextBox { MaxLength = 2, TextAlign = HorizontalAlignment.Center };
                txtBalotas[i].KeyPress += SoloDigitos;
                panel.Controls.Add(txtBalotas[i], 1, i);
            }

            panel.Controls.Add(new Label { Text = "Súper Balota (1-16):", AutoSize = true }, 0, 5);
            txtSuper = new TextBox { MaxLength = 2, TextAlign = HorizontalAlignment.Center };
            txtSuper.KeyPress += SoloDigitos;
            panel.Controls.Add(txtSuper, 1, 5);

            panel.Controls.Add(new Label { Text = "Fecha:", AutoSize = true }, 0, 6);
            dtpFecha = new DateTimePicker { Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy", Value = DateTime.Today };
            panel.Controls.Add(dtpFecha, 1, 6);

            btnGuardar = new Button { Text = "Guardar sorteo", Dock = DockStyle.Bottom, Height = 35 };
            btnGuardar.Click += BtnGuardar_Click;

            Controls.Add(panel);
            Controls.Add(btnGuardar);
        }

        private void SoloDigitos(object sender, KeyPressEventArgs e) =>
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                int[] nums = new int[6];
                for (int i = 0; i < 5; i++)
                    if (!int.TryParse(txtBalotas[i].Text, out nums[i]) || nums[i] < 1 || nums[i] > 43)
                        throw new Exception($"Balota {i + 1} debe ser entre 1 y 43.");
                if (!int.TryParse(txtSuper.Text, out nums[5]) || nums[5] < 1 || nums[5] > 16)
                    throw new Exception("Súper balota debe ser entre 1 y 16.");
                if (nums.Take(5).Distinct().Count() != 5) throw new Exception("Las cinco balotas deben ser distintas.");

                Sorteo sorteo = new Sorteo { Fecha = dtpFecha.Value.Date, Numeros = nums };
                if (!sorteo.EsValido()) throw new Exception("Datos inválidos.");

                if (DatosBaloto.AgregarSorteo(sorteo, out string error))
                {
                    MessageBox.Show("Sorteo guardado correctamente.", "Éxito");
                    Close();
                }
                else MessageBox.Show(error, "Sorteo duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }
    }
}