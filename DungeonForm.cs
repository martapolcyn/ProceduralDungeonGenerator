using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ProceduralDungeonGenerator.Configuration;
using ProceduralDungeonGenerator.Model;


namespace ProceduralDungeonGenerator
{
    public partial class DungeonForm : Form
    {

        private ComboBox styleComboBox;
        private Button generateButton;
        private Dungeon? _dungeon;
        private float _scale = 1.0f;
        private Point _panOffset = new Point(0, 0);
        private Point _lastMousePos;
        private bool _isPanning = false;

        public DungeonForm()
        {
            InitializeComponent();
            this.Text = "Procedural Dungeon Generator";
            this.ClientSize = new Size(ConfigManager.dungeonWidth, ConfigManager.dungeonHeight + 50);
            
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
            styleComboBox.Items.AddRange(new[] { "Dungeon", "Spaceship" });
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

            DoubleBuffered = true;
        }

        private void GenerateButton_Click(object? sender, EventArgs e)
        {
            string selectedStyle = styleComboBox.SelectedItem?.ToString() ?? "Dungeon";
            ConfigManager.LoadAllConfigs(selectedStyle);

            IDungeonStyle style = selectedStyle switch
            {
                "Dungeon" => new DungeonStyleDungeon(),
                "Spaceship" => new DungeonStyleSpaceship(),
                _ => throw new NotImplementedException()
            };

            _dungeon = new Dungeon(style);
            _dungeon.GenerateDungeon();
            Invalidate();
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

                _dungeon.Draw(g);
            }
        }
    }
}
