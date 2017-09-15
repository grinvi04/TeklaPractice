using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using TSD = Tekla.Structures.Drawing;
using Tekla.Structures.Drawing.Tools;
using Tekla.Structures.Drawing.UI;
using TSG = Tekla.Structures.Geometry3d;

namespace Tekla_Practice
{
    public partial class drawing : Form
    {
        
        private Picker picker;
        private TSD.ViewBase viewBase;
        private TSD.View view;
        private TSG.Point point;

        public drawing()
        {
            InitializeComponent();

            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TSD.DrawingHandler drawingHandler = new TSD.DrawingHandler();

            if (drawingHandler.GetConnectionStatus())
            {
                this.picker = drawingHandler.GetPicker();

                this.viewBase = null;
                this.point = new TSG.Point();
                string prompt = "포인트를 선택하세요.";
                picker.PickPoint(prompt, out point, out viewBase);

                //this.view = (TSD.View)this.viewBase.GetView();
                this.view = viewBase as TSD.View;
            }

            this.CreateSymbol(this.view, this.point);
        }

        private bool CreateSymbol(TSD.View view, TSG.Point startPoint)
        {
            double tempScale;
            Double.TryParse(this.scale.Text, out tempScale);

            //스케일 
            double scale = view.Attributes.Scale * tempScale;

            //벡터
            TSG.Vector vectorX = new TSG.Vector(1, 0, 0); //PrecastCommon.GetVectorX(this.Data.RotationAngle);
            TSG.Vector vectorY = new TSG.Vector(0, 1, 0); //ETG.Geometry2D.Rotate90(vectorX);


            //원
            TSD.Circle circle = new TSD.Circle(view, startPoint, tempScale);
            circle.Insert();

            TSD.Text text = new TSD.Text(view, startPoint, "7");
            text.Attributes.Font.Height = tempScale;

            text.Insert();

            return true;
        }
    }
}
