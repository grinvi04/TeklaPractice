using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using TSM = Tekla.Structures.Model;
using TSG = Tekla.Structures.Geometry3d;

namespace Tekla_Practice
{
    public partial class Form4 : Form
    {
        private TSG.Point PickStartPoint;
        private TSG.Point PickEndPoint;

        public Form4()
        {
            InitializeComponent();

            this.PickStartPoint = new TSG.Point();
            this.PickEndPoint = new TSG.Point();
        }

        private void button_createBeam_Click(object sender, EventArgs e)
        {
            TSM.Model model = new TSM.Model();

            if (!model.GetConnectionStatus()) return;

            TSM.UI.Picker picker = new TSM.UI.Picker();

            this.PickStartPoint = picker.PickPoint();
            this.PickEndPoint = picker.PickPoint();
        }
    }
}
