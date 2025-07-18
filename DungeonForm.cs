using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ProceduralDungeonGenerator.Configuration;
using ProceduralDungeonGenerator.Model.Structure;
using ProceduralDungeonGenerator.Model.Styles;


namespace ProceduralDungeonGenerator
{
    public partial class DungeonForm : Form
    {

        private ComboBox styleComboBox;
        private Button generateButton;
        private Button saveButton;
        private Dungeon? _dungeon;
        private float _scale = 1.0f;
        private Point _panOffset = new Point(0, 0);
        private Point _lastMousePos;
        private bool _isPanning = false;

        public DungeonForm()
        {
            ConfigManager.LoadGeneralConfig();
            InitializeComponent();
            this.Text = "Procedural Dungeon Generator";
            this.ClientSize = new Size(ConfigManager.dungeonWidth, ConfigManager.dungeonHeight);
            
            this.MouseWheel += DungeonForm_MouseWheel;
            this.DoubleBuffered = true;

            this.MouseWheel += DungeonForm_MouseWheel;
            this.MouseDown += DungeonForm_MouseDown;
            this.MouseMove += DungeonForm_MouseMove;
            this.MouseUp += DungeonForm_MouseUp;
            this.DoubleBuffered = true;


            styleComboBox = new ComboBox()
            {
                Location = new Point(10, 10),
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            styleComboBox.Items.AddRange(new[] { "Dungeon", "Spaceship", "Cave" });
            styleComboBox.SelectedIndex = 0;
            Controls.Add(styleComboBox);

            generateButton = new Button()
            {
                Text = "Generate",
                Location = new Point(170, 10),
                Width = 100
            };
            generateButton.Click += GenerateButton_Click;
            Controls.Add(generateButton);

            saveButton = new Button()
            {
                Text = "Save",
                Location = new Point(280, 10),
                Width = 100
            };
            saveButton.Click += SaveButton_Click;
            Controls.Add(saveButton);

            DoubleBuffered = true;
        }

        private void GenerateButton_Click(object? sender, EventArgs e)
        {
            string selectedStyle = styleComboBox.SelectedItem?.ToString() ?? "Dungeon";
            ConfigManager.LoadGeneralConfig();
            ConfigManager.LoadAllConfigs(selectedStyle);

            IDungeonStyle style = selectedStyle switch
            {
                "Dungeon" => new DungeonStyleDungeon(),
                "Spaceship" => new DungeonStyleSpaceship(),
                "Cave" => new DungeonStyleCave(),
                _ => throw new NotImplementedException()
            };

            _dungeon = new Dungeon(style);
            _dungeon.GenerateDungeon();

            this.ClientSize = new Size(ConfigManager.dungeonWidth, ConfigManager.dungeonHeight);

            Invalidate();
        }

        private void SaveButton_Click(object? sender, EventArgs e)
        {
            if (_dungeon == null)
            {
                MessageBox.Show("Generate map first.", "No map", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "File PNG|*.png|Plik JPG|*.jpg",
                Title = "Save as picture",
                FileName = "dungeon"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Utw�rz bitmap� i narysuj dungeon na niej
                using Bitmap bmp = new Bitmap(ConfigManager.dungeonWidth, ConfigManager.dungeonHeight);
                using Graphics g = Graphics.FromImage(bmp);

                g.Clear(Color.White); // lub inny kolor t�a
                g.ScaleTransform(_scale, _scale);
                g.TranslateTransform(_panOffset.X, _panOffset.Y);
                DrawGrid(g);
                _dungeon.Draw(g);

                var ext = System.IO.Path.GetExtension(saveFileDialog.FileName).ToLower();
                var format = ext switch
                {
                    ".jpg" => System.Drawing.Imaging.ImageFormat.Jpeg,
                    _ => System.Drawing.Imaging.ImageFormat.Png
                };

                bmp.Save(saveFileDialog.FileName, format);
                MessageBox.Show("Map has been saved.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void DungeonForm_MouseWheel(object sender, MouseEventArgs e)
        {
            float oldScale = _scale;
            if (e.Delta > 0)
                _scale *= 1.1f;
            else
                _scale /= 1.1f;

            _scale = Math.Clamp(_scale, 0.1f, 5f);

            _panOffset.X = (int)(e.X - ((e.X - _panOffset.X) / oldScale * _scale));
            _panOffset.Y = (int)(e.Y - ((e.Y - _panOffset.Y) / oldScale * _scale));

            Invalidate();
        }

        private void DungeonForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isPanning = true;
                _lastMousePos = e.Location;
                this.Cursor = Cursors.Hand;
            }
        }

        private void DungeonForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isPanning)
            {
                _panOffset.X += e.X - _lastMousePos.X;
                _panOffset.Y += e.Y - _lastMousePos.Y;
                _lastMousePos = e.Location;
                Invalidate();
            }
        }

        private void DungeonForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isPanning = false;
                this.Cursor = Cursors.Default;
            }
        }



        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_dungeon != null)
            {
                Graphics g = e.Graphics;

                g.TranslateTransform(_panOffset.X, _panOffset.Y);
                g.ScaleTransform(_scale, _scale);

                DrawGrid(g);
                _dungeon.Draw(g);
            }
        }

        private void DrawGrid(Graphics g)
        {
            Pen gridPen = new Pen(Color.LightGray, 1);

            for (int x = 0; x <= ConfigManager.dungeonWidth; x += ConfigManager.tileSize)
            {
                g.DrawLine(gridPen, x, 0, x, ConfigManager.dungeonHeight);
            }

            for (int y = 0; y <= ConfigManager.dungeonHeight; y += ConfigManager.tileSize)
            {
                g.DrawLine(gridPen, 0, y, ConfigManager.dungeonWidth, y);
            }

            gridPen.Dispose();
        }
    }
}
