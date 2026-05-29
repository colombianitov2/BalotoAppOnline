using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

namespace BalotoAppOnline
{
    public class EstadisticasForm : Form
    {
        private TabControl tabControl;
        private DataGridView dgvFrecuenciasGlobal, dgvFrecuenciasMensual, dgvFrecuenciasAnual, dgvCombinacionesFrec;
        private ComboBox cmbMes, cmbAnio;
        private Label lblSinCombinaciones;
        private Button btnExportar;

        public EstadisticasForm()
        {
            InitializeComponent();
            CargarFrecuenciasGlobal();
            CargarMesesDisponibles();
            CargarAniosDisponibles();
            CargarCombinacionesFrecuentes();
        }

        private void InitializeComponent()
        {
            Text = "Tabla de análisis - Estadísticas del Baloto";
            Size = new Size(850, 650);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.White;

            Panel topPanel = new Panel { Dock = DockStyle.Top, Height = 45, BackColor = Color.FromArgb(240, 242, 245) };
            btnExportar = new Button
            {
                Text = "Exportar a CSV",
                Size = new Size(150, 30),
                Location = new Point(10, 8),
                BackColor = Color.FromArgb(0, 69, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnExportar.Click += BtnExportar_Click;
            topPanel.Controls.Add(btnExportar);

            tabControl = new TabControl { Dock = DockStyle.Fill };

            // Tabla de frecuencias por posición (global)
            TabPage tabGlobal = new TabPage("Frecuencia por posición");
            dgvFrecuenciasGlobal = new DataGridView { Dock = DockStyle.Fill, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells, ReadOnly = true };
            tabGlobal.Controls.Add(dgvFrecuenciasGlobal);

            // Tabla de frecuencias mensual
            TabPage tabMensual = new TabPage("Frecuencia mensual");
            cmbMes = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 120 };
            dgvFrecuenciasMensual = new DataGridView { Dock = DockStyle.Fill, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells, ReadOnly = true };
            Panel panelMensual = new Panel { Dock = DockStyle.Top, Height = 50 };
            panelMensual.Controls.Add(cmbMes);
            cmbMes.Location = new Point(10, 10);
            cmbMes.SelectedIndexChanged += (s, e) => CargarFrecuenciasMensual(cmbMes.SelectedItem?.ToString());
            tabMensual.Controls.Add(panelMensual);
            tabMensual.Controls.Add(dgvFrecuenciasMensual);

            // Tabla de frecuencias anual
            TabPage tabAnual = new TabPage("Frecuencia por año");
            cmbAnio = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 120 };
            dgvFrecuenciasAnual = new DataGridView { Dock = DockStyle.Fill, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells, ReadOnly = true };
            Panel panelAnual = new Panel { Dock = DockStyle.Top, Height = 50 };
            panelAnual.Controls.Add(cmbAnio);
            cmbAnio.Location = new Point(10, 10);
            cmbAnio.SelectedIndexChanged += (s, e) => CargarFrecuenciasAnual(cmbAnio.SelectedItem?.ToString());
            tabAnual.Controls.Add(panelAnual);
            tabAnual.Controls.Add(dgvFrecuenciasAnual);

            // Tabla de combinaciones más repetidas
            TabPage tabCombos = new TabPage("Combinaciones más repetidas");
            dgvCombinacionesFrec = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false
            };
            lblSinCombinaciones = new Label
            {
                Text = "No hay combinaciones repetidas en el historial.",
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 12, FontStyle.Italic),
                ForeColor = Color.Gray,
                Visible = false
            };
            tabCombos.Controls.Add(dgvCombinacionesFrec);
            tabCombos.Controls.Add(lblSinCombinaciones);

            tabControl.TabPages.AddRange(new TabPage[] { tabGlobal, tabMensual, tabAnual, tabCombos });
            Controls.Add(tabControl);
            Controls.Add(topPanel);
        }

        private void MostrarTablaConMaximo(DataGridView dgv, Dictionary<int, Dictionary<int, int>> frec)
        {
            dgv.Columns.Clear();
            dgv.Columns.Add("Numero", "Número");
            for (int pos = 0; pos < 6; pos++)
                dgv.Columns.Add($"pos{pos}", pos < 5 ? $"Balota {pos + 1}" : "Súper Balota");

            for (int num = 1; num <= 43; num++)
            {
                var row = new DataGridViewRow();
                row.CreateCells(dgv);
                row.Cells[0].Value = num;
                for (int pos = 0; pos < 6; pos++)
                {
                    int conteo = frec[pos].ContainsKey(num) ? frec[pos][num] : 0;
                    row.Cells[pos + 1].Value = conteo;
                }
                dgv.Rows.Add(row);
            }

            var filaMax = new DataGridViewRow();
            filaMax.CreateCells(dgv);
            filaMax.Cells[0].Value = "Más repetido";
            for (int pos = 0; pos < 6; pos++)
            {
                if (frec[pos].Count == 0) filaMax.Cells[pos + 1].Value = "---";
                else
                {
                    int maxFreq = frec[pos].Values.Max();
                    var numsMax = frec[pos].Where(kv => kv.Value == maxFreq).Select(kv => kv.Key).OrderBy(n => n);
                    filaMax.Cells[pos + 1].Value = string.Join(",", numsMax);
                }
            }
            dgv.Rows.Add(filaMax);
        }

        private void CargarFrecuenciasGlobal() => MostrarTablaConMaximo(dgvFrecuenciasGlobal, DatosBaloto.ObtenerFrecuenciasPorPosicion());

        private void CargarMesesDisponibles()
        {
            var meses = DatosBaloto.Sorteos.Select(s => s.Fecha.ToString("MMyyyy")).Distinct().OrderBy(m => m).ToList();
            if (meses.Count == 0) meses.Add(DateTime.Now.ToString("MMyyyy"));
            cmbMes.Items.Clear();
            foreach (var m in meses) cmbMes.Items.Add(m);
            if (cmbMes.Items.Count > 0) cmbMes.SelectedIndex = 0;
        }

        private void CargarFrecuenciasMensual(string mesAnio)
        {
            if (string.IsNullOrEmpty(mesAnio)) return;
            MostrarTablaConMaximo(dgvFrecuenciasMensual, DatosBaloto.ObtenerFrecuenciasPorMes(mesAnio));
        }

        private void CargarAniosDisponibles()
        {
            var anios = DatosBaloto.Sorteos.Select(s => s.Fecha.Year).Distinct().OrderBy(a => a).ToList();
            if (anios.Count == 0) anios.Add(DateTime.Now.Year);
            cmbAnio.Items.Clear();
            foreach (var a in anios) cmbAnio.Items.Add(a.ToString());
            if (cmbAnio.Items.Count > 0) cmbAnio.SelectedIndex = 0;
        }

        private void CargarFrecuenciasAnual(string anioStr)
        {
            if (string.IsNullOrEmpty(anioStr)) return;
            MostrarTablaConMaximo(dgvFrecuenciasAnual, DatosBaloto.ObtenerFrecuenciasPorAnio(int.Parse(anioStr)));
        }

        private void CargarCombinacionesFrecuentes()
        {
            dgvCombinacionesFrec.Columns.Clear();
            dgvCombinacionesFrec.Rows.Clear();

            var combos = DatosBaloto.CombinacionesMasFrecuentesGeneral();

            var combosRepetidas = combos.Where(c => c.frecuencia > 1).ToList();

            if (combosRepetidas.Count == 0)
            {
                dgvCombinacionesFrec.Visible = false;
                lblSinCombinaciones.Visible = true;
                return;
            }

            dgvCombinacionesFrec.Visible = true;
            lblSinCombinaciones.Visible = false;

            dgvCombinacionesFrec.Columns.Add("Combinacion", "Combinación (5 balotas ordenadas)");
            dgvCombinacionesFrec.Columns.Add("Frecuencia", "Veces repetida");

            foreach (var c in combosRepetidas)
            {
                dgvCombinacionesFrec.Rows.Add(c.combinacion, c.frecuencia);
            }

            dgvCombinacionesFrec.Sort(dgvCombinacionesFrec.Columns["Frecuencia"], System.ComponentModel.ListSortDirection.Descending);
        }

        private void BtnExportar_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Archivos CSV|*.csv";
                sfd.Title = "Exportar estadísticas a CSV";
                sfd.FileName = $"Estadisticas_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        DataGridView tablaActual = ObtenerTablaActual();
                        if (tablaActual == null || tablaActual.Rows.Count == 0)
                        {
                            MessageBox.Show("No hay datos para exportar.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }

                        using (StreamWriter sw = new StreamWriter(sfd.FileName, false, System.Text.Encoding.UTF8))
                        {
                            // Escribir encabezados
                            for (int i = 0; i < tablaActual.Columns.Count; i++)
                            {
                                sw.Write(tablaActual.Columns[i].HeaderText);
                                if (i < tablaActual.Columns.Count - 1) sw.Write(",");
                            }
                            sw.WriteLine();

                            // Escribir filas
                            foreach (DataGridViewRow row in tablaActual.Rows)
                            {
                                if (row.IsNewRow) continue;
                                for (int i = 0; i < tablaActual.Columns.Count; i++)
                                {
                                    string valor = row.Cells[i].Value?.ToString() ?? "";
                                    // Si el valor contiene comas o comillas, lo envolvemos entre comillas dobles
                                    if (valor.Contains(",") || valor.Contains("\""))
                                        valor = "\"" + valor.Replace("\"", "\"\"") + "\"";
                                    sw.Write(valor);
                                    if (i < tablaActual.Columns.Count - 1) sw.Write(",");
                                }
                                sw.WriteLine();
                            }
                        }

                        MessageBox.Show($"Tabla exportada correctamente a:\n{sfd.FileName}", "Exportación completada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al exportar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private DataGridView ObtenerTablaActual()
        {
            // Devuelve el DataGridView correspondiente a la pestaña activa
            switch (tabControl.SelectedIndex)
            {
                case 0: return dgvFrecuenciasGlobal;
                case 1: return dgvFrecuenciasMensual;
                case 2: return dgvFrecuenciasAnual;
                case 3: return dgvCombinacionesFrec;
                default: return null;
            }
        }
    }
}