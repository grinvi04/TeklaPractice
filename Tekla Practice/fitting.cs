using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using TSG = Tekla.Structures.Geometry3d;
using TSM = Tekla.Structures.Model;

namespace Tekla_Practice
{
    public partial class fitting : Form
    {
        private TSM.Model model;

        public fitting()
        {
            InitializeComponent();

            this.model = new TSM.Model();
            if (!this.model.GetConnectionStatus()) return;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TSG.Point point1 = new TSG.Point(0, 500, 0);
            TSG.Point point2 = new TSG.Point(3000, 500, 0);

            TSM.Beam beam1 = new TSM.Beam();
            beam1.StartPoint = point1;
            beam1.EndPoint = point2;
            beam1.Profile.ProfileString = "H400X200X8X13";
            beam1.Material.MaterialString = "SS400";
            beam1.Insert();


            TSM.Fitting fitting = new TSM.Fitting();
            fitting.Plane = new TSM.Plane();
            fitting.Plane.Origin = new TSG.Point(500, 0, 0);
            fitting.Plane.AxisX = new TSG.Vector(0, 100, 0);
            fitting.Plane.AxisY = new TSG.Vector(0, 0, 100);
            fitting.Father = beam1;
            //fitting.Insert();

            // model 반영
            this.model.CommitChanges();
        }
    }
}
