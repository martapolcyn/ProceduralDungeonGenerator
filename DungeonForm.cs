using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;


namespace ProceduralDungeonGenerator
{
    public partial class DungeonForm : Form
    {

        private Dungeon _dungeon;

        public DungeonForm(Dungeon dungeon)
        {
            InitializeComponent();
            this.ClientSize = new Size(Config.dungeonWidth, Config.dungeonHeight);
            _dungeon = dungeon;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            _dungeon.Draw(g);
        }
    }
}
