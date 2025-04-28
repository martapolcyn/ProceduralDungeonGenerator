using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;


namespace ProceduralDungeonGenerator
{
    public partial class DungeonForm : Form
    {

        private Dungeon dungeon;

        public DungeonForm()
        {
            InitializeComponent();
            this.ClientSize = new Size(800, 600);
            dungeon = new Dungeon();
            dungeon.GenerateDungeon(6);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics; 

            // Drawing every room
            foreach (var room in dungeon.Rooms)
            {
                Rectangle rect = new Rectangle(room.X, room.Y, room.Width, room.Height);

                g.FillRectangle(Brushes.LightBlue, rect);
                g.DrawRectangle(Pens.Black, rect);
            }
        }
    }
}
