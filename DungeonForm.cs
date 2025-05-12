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

        public DungeonForm()
        {
            InitializeComponent();
            this.Text = "Procedural Dungeon Generator";
            this.ClientSize = new Size(ConfigManager.dungeonWidth, ConfigManager.dungeonHeight + 50);

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


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_dungeon != null)
            {
                Graphics g = e.Graphics;
                _dungeon.Draw(g);
            }
        }
    }
}
