using System;
using System.Linq;
using System.Windows.Forms;

namespace BalotoAppOnline
{
    public class HistorialForm : Form
    {
        private DataGridView dgvHistorial;
        private Button btnEliminar;

        public HistorialForm()
        {
            InitializeComponent();
            CargarHistorial();
        }

        private void InitializeComponent()
        {
            Text = "Historial de sorteos";
            Size = new System.Drawing.Size(800, 500);
            StartPosition = FormStartPosition.CenterParent;

            dgvHistorial = new DataGridView { Dock = DockStyle.Fill, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false };
            btnEliminar = new Button { Text = "Eliminar sorteo seleccionado", Dock = DockStyle.Bottom, Height = 40, BackColor = System.Drawing.Color.FromArgb(231, 76, 60), ForeColor = System.Drawing.Color.White, FlatStyle = FlatStyle.Flat };
            btnEliminar.Click += BtnEliminar_Click;

            Controls.Add(dgvHistorial);
            Controls.Add(btnEliminar);
        }

        private void CargarHistorial()
        {
            dgvHistorial.Columns.Clear();
            dgvHistorial.Columns.Add("Fecha", "Fecha (dd/MM/yyyy)");
            for (int i = 0; i < 5; i++) dgvHistorial.Columns.Add($"Balota{i + 1}", $"Balota {i + 1}");
            dgvHistorial.Columns.Add("Super", "Súper Balota");

            var ordenados = DatosBaloto.Sorteos.OrderByDescending(s => s.Fecha).ToList();
            foreach (var s in ordenados)
                dgvHistorial.Rows.Add(s.Fecha.ToString("dd/MM/yyyy"), s.Numeros[0].ToString("00"), s.Numeros[1].ToString("00"), s.Numeros[2].ToString("00"), s.Numeros[3].ToString("00"), s.Numeros[4].ToString("00"), s.Numeros[5].ToString("00"));
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvHistorial.SelectedRows.Count == 0) { MessageBox.Show("Seleccione un sorteo."); return; }
            var row = dgvHistorial.SelectedRows[0];
            DateTime fecha = DateTime.ParseExact(row.Cells[0].Value.ToString(), "dd/MM/yyyy", null);
            int[] nums = new int[6];
            for (int i = 0; i < 5; i++) nums[i] = int.Parse(row.Cells[i + 1].Value.ToString());
            nums[5] = int.Parse(row.Cells[6].Value.ToString());
            var sorteo = DatosBaloto.Sorteos.FirstOrDefault(s => s.Fecha == fecha && s.Numeros.SequenceEqual(nums));
            if (sorteo != null && MessageBox.Show($"¿Eliminar sorteo del {fecha:dd/MM/yyyy} con números {string.Join(",", nums)}?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                DatosBaloto.Sorteos.Remove(sorteo);
                DatosBaloto.Guardar();
                CargarHistorial();
                MessageBox.Show("Eliminado.");
            }
        }
    }
}