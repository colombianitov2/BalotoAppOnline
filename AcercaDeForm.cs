using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BalotoAppOnline
{
    public class AcercaDeForm : Form
    {
        public AcercaDeForm()
        {
            Text = "Acerca de Generador de Baloto Online";
            Size = new Size(580, 720);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.White;

            TabControl tabControl = new TabControl { Dock = DockStyle.Fill };

            // ==================== PESTAÑA CRÉDITOS ====================
            TabPage tabCreditos = new TabPage("Créditos");
            tabCreditos.BackColor = Color.White;

            Label lblTitulo = new Label
            {
                Text = "Generador de Baloto Online",
                Font = new Font("Segoe UI", 17, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 69, 166),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 58
            };

            Label lblVersion = new Label
            {
                Text = "Versión 1.0 · © 2026",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(150, 150, 150),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 22
            };

            RichTextBox rtbDesc = new RichTextBox
            {
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                BackColor = Color.White,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(50, 50, 50),
                Dock = DockStyle.Top,
                Height = 230,
                ScrollBars = RichTextBoxScrollBars.None,
                Padding = new Padding(16, 8, 16, 0),
                Text = "Esta aplicación ayuda a analizar y predecir sorteos del Baloto " +
                       "mediante estadísticas históricas.\n\n" +
                       "Funcionalidades:\n" +
                       " • Resultado por frecuencia — sugerencia frecuentista por posición\n" +
                       " • Sugerencia Gaussiana — selección ponderada por frecuencia\n" +
                       " • Ingresar, exportar e importar sorteos (.txt)\n" +
                       " • Tabla de análisis: frecuencias por posición, mes y año\n" +
                       " • Historial con posibilidad de eliminar registros\n" +
                       " • Actualización automática desde la web oficial\n" +
                       " • Sugerencia Automática (inteligente) basada en múltiples factores\n\n" +
                       "Desarrollada para Windows 10/11 32/64 bits · .NET Framework 4.7.2"
            };

            Panel separador = new Panel { Dock = DockStyle.Top, Height = 1, BackColor = Color.FromArgb(218, 220, 224) };

            Panel panelCreditos = new Panel { Dock = DockStyle.Top, Height = 280, BackColor = Color.FromArgb(245, 246, 248) };
            TableLayoutPanel tblCreditos = new TableLayoutPanel
            {
                Location = new Point(0, 0),
                Size = new Size(564, 280),
                ColumnCount = 1,
                RowCount = 5,
                BackColor = Color.Transparent
            };
            tblCreditos.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            tblCreditos.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
            tblCreditos.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
            tblCreditos.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
            tblCreditos.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
            tblCreditos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            Label lblCredTitulo = new Label
            {
                Text = "COLABORADORES",
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = Color.FromArgb(140, 140, 140),
                TextAlign = ContentAlignment.BottomLeft,
                Dock = DockStyle.Fill,
                Padding = new Padding(16, 0, 0, 4),
                BackColor = Color.Transparent
            };

            Panel rowErnesto = CrearFila("Ernesto Pernett Cuesta", "Idea, dirección y diseño · Ingeniero Mecánico", Color.FromArgb(0, 69, 166), "EP");
            Panel rowClaude = CrearFila("Claude · Anthropic", "Desarrollo de software e interfaz", Color.FromArgb(205, 92, 0), "AI");
            Panel rowDeepSeek = CrearFila("DeepSeek (IA)", "Asistencia y revisión de algoritmos", Color.FromArgb(22, 119, 79), "DS");
            Panel rowGemini = CrearFila("Gemini · Google", "Asesor de imagen y consultas variadas", Color.FromArgb(66, 133, 244), "GM");

            tblCreditos.Controls.Add(lblCredTitulo, 0, 0);
            tblCreditos.Controls.Add(rowErnesto, 0, 1);
            tblCreditos.Controls.Add(rowClaude, 0, 2);
            tblCreditos.Controls.Add(rowDeepSeek, 0, 3);
            tblCreditos.Controls.Add(rowGemini, 0, 4);
            panelCreditos.Controls.Add(tblCreditos);

            Panel panelBoton = new Panel { Dock = DockStyle.Bottom, Height = 60, BackColor = Color.White };
            Button btnCerrar = new Button
            {
                Text = "Cerrar",
                Size = new Size(110, 36),
                BackColor = Color.FromArgb(0, 69, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK,
                Font = new Font("Segoe UI", 10)
            };
            btnCerrar.FlatAppearance.BorderSize = 0;
            btnCerrar.Location = new Point((564 - 110) / 2, 12);
            panelBoton.Controls.Add(btnCerrar);

            tabCreditos.Controls.Add(panelBoton);
            tabCreditos.Controls.Add(panelCreditos);
            tabCreditos.Controls.Add(separador);
            tabCreditos.Controls.Add(rtbDesc);
            tabCreditos.Controls.Add(lblVersion);
            tabCreditos.Controls.Add(lblTitulo);

            // ==================== PESTAÑA AYUDA ====================
            TabPage tabAyuda = new TabPage("Ayuda");
            tabAyuda.BackColor = Color.White;

            RichTextBox rtbAyuda = new RichTextBox
            {
                ReadOnly = true,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(50, 50, 50),
                ScrollBars = RichTextBoxScrollBars.Vertical,
                Padding = new Padding(16),
                Text =
"EXPLICACIÓN DE FUNCIONES\n" +
"====================================\n\n" +
"1. AUTOMÁTICO (Inteligente)\n" +
"   - Combina varios algoritmos: frecuencias por posición, números calientes, combinaciones repetidas.\n" +
"   - Elige los números con mayor probabilidad estadística usando análisis avanzado.\n" +
"   - Ideal si quieres una apuesta con el máximo respaldo.\n\n" +
"2. RESULTADO POR FRECUENCIA (Frecuentista)\n" +
"   - Sugiere números basándose en la frecuencia histórica de cada posición.\n" +
"   - Para cada posición (balota 1 a 5), elige el número más repetido.\n" +
"   - La súper balota también se elige por la más frecuente.\n\n" +
"3. SUGERENCIA GAUSSIANA\n" +
"   - Similar a la frecuentista, pero añade aleatoriedad ponderada.\n" +
"   - Los números con mayor frecuencia tienen más probabilidad, pero no son fijos.\n" +
"   - Útil para evitar siempre los mismos números.\n\n" +
"4. INGRESAR DATOS\n" +
"   - Permite añadir manualmente un sorteo ganador.\n" +
"   - Debes ingresar 5 balotas (1-43, distintas) y una súper balota (1-16).\n" +
"   - La fecha se puede cambiar; por defecto es la actual.\n\n" +
"5. EXPORTAR / IMPORTAR (.txt)\n" +
"   - Exporta todos los sorteos a un archivo de texto.\n" +
"   - Importa desde un archivo con formato: dd/MM/yyyy N1 N2 N3 N4 N5 N6\n" +
"   - Útil para respaldos o para compartir datos.\n\n" +
"6. HISTORIAL\n" +
"   - Muestra la lista completa de sorteos guardados.\n" +
"   - Permite eliminar un sorteo seleccionado.\n\n" +
"7. VERIFICADOR DE TIQUETES\n" +
"   - Ingresa 5 balotas y 1 súper balota.\n" +
"   - Te indica si esa combinación exacta ha salido antes, y en qué fechas.\n\n" +
"8. TABLA DE ANÁLISIS\n" +
"   - Frecuencia por posición: cuántas veces ha salido cada número en cada columna.\n" +
"   - Frecuencia mensual/anual: permite filtrar por mes o año.\n" +
"   - Combinaciones más repetidas: muestra qué grupos de 5 balotas han aparecido juntos.\n" +
"   - Puedes exportar cualquier tabla a CSV (Excel).\n\n" +
"9. ACTUALIZAR DESDE WEB\n" +
"   - Conecta a la página oficial de Baloto y descarga automáticamente todos los sorteos históricos.\n" +
"   - Solo añade los que no existan en tu base de datos.\n" +
"   - La primera vez descarga Chromium (unos 150 MB) – puede tardar.\n\n" +
"CONSEJO: Cuantos más sorteos tengas en el historial, más precisas serán las sugerencias.\n" +
"Actualiza desde web periódicamente para mantener los datos al día."
            };
            tabAyuda.Controls.Add(rtbAyuda);

            tabControl.TabPages.Add(tabCreditos);
            tabControl.TabPages.Add(tabAyuda);
            Controls.Add(tabControl);
        }

        private Panel CrearFila(string nombre, string rol, Color color, string iniciales)
        {
            Panel row = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
            Color colorCaptura = color;
            string inicialesCaptura = iniciales;
            Panel avatar = new Panel { Size = new Size(36, 36), Location = new Point(16, 8), BackColor = Color.Transparent };
            avatar.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (SolidBrush br = new SolidBrush(colorCaptura))
                    e.Graphics.FillEllipse(br, 0, 0, 35, 35);
                StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                using (SolidBrush tb = new SolidBrush(Color.White))
                    e.Graphics.DrawString(inicialesCaptura, new Font("Segoe UI", 9, FontStyle.Bold), tb, new RectangleF(0, 0, 35, 35), sf);
            };
            Label lblNombre = new Label
            {
                Text = nombre,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = color,
                AutoSize = false,
                Size = new Size(400, 20),
                Location = new Point(60, 8),
                BackColor = Color.Transparent
            };
            Label lblRol = new Label
            {
                Text = rol,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(110, 110, 110),
                AutoSize = false,
                Size = new Size(400, 18),
                Location = new Point(60, 28),
                BackColor = Color.Transparent
            };
            row.Controls.AddRange(new Control[] { avatar, lblNombre, lblRol });
            return row;
        }
    }
}