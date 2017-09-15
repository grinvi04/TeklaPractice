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
    public partial class cutplane : Form
    {
        private TSM.Model model;

        public cutplane()
        {
            InitializeComponent();

            this.model = new TSM.Model();
            if (!this.model.GetConnectionStatus()) return;
        }

        private void button_cutplane_Click(object sender, EventArgs e)
        {
            TSG.Point point1 = new TSG.Point(0, 3000, 0);
            TSG.Point point2 = new TSG.Point(3000, 3000, 0);

            TSM.Beam beam1 = new TSM.Beam();
            beam1.StartPoint = point1;
            beam1.EndPoint = point2;
            beam1.Profile.ProfileString = "H400X200X8X13";
            beam1.Material.MaterialString = "SS400";
            beam1.Insert();


            TSM.CutPlane cutplane = new TSM.CutPlane();
            cutplane.Plane = new TSM.Plane();
            cutplane.Plane.Origin = new TSG.Point(1000, 3000, 0);
            cutplane.Plane.AxisX = new TSG.Vector(1, 1, 0);
            cutplane.Plane.AxisY = new TSG.Vector(0, 0, 1);

            TSG.Vector vector = cutplane.Plane.AxisX.Cross(cutplane.Plane.AxisY);

            cutplane.Father = beam1;
            cutplane.Insert();



            // model 반영
            this.model.CommitChanges();



        }
    }
}
