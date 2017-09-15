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
    public partial class weld : Form
    {
        private TSM.Model model;

        public weld()
        {
            InitializeComponent();

            this.model = new TSM.Model();
            if (!this.model.GetConnectionStatus()) return;
        }

        private void button_weld_Click(object sender, EventArgs e)
        {
            TSG.Point point1 = new TSG.Point(0, 0, 0);
            TSG.Point point2 = new TSG.Point(3000, 0, 0);
            TSG.Point point3 = new TSG.Point(6000, 0, 0);

            TSM.Beam beam1 = new TSM.Beam(point1, point2);
            TSM.Beam beam2 = new TSM.Beam(point2, point3);

            beam1.Profile.ProfileString = "H400X200X8X13";
            beam1.Material.MaterialString = "SS400";
            beam1.Insert();

            beam2.Profile.ProfileString = "H400X200X8X13";
            beam2.Material.MaterialString = "SS400";
            beam2.Insert();

            TSM.Weld weld = new TSM.Weld();
            weld.MainObject = beam1;
            weld.SecondaryObject = beam2;
            weld.TypeAbove = TSM.BaseWeld.WeldTypeEnum.WELD_TYPE_BEVEL_GROOVE_SINGLE_V_BUTT;
            weld.SizeAbove = 9;
            weld.AngleAbove = 90;

            weld.AroundWeld = false;
            weld.ShopWeld = true;
            weld.Direction = new TSG.Vector(0, 0, 1);
            weld.Placement = TSM.BaseWeld.WeldPlacementTypeEnum.PLACEMENT_AUTO;
            weld.Preparation = TSM.BaseWeld.WeldPreparationTypeEnum.PREPARATION_AUTO;
            weld.Insert();

            // model 반영
            this.model.CommitChanges();
        }
    }
}
