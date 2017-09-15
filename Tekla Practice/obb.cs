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
using System.Collections;

namespace Tekla_Practice
{
    // OBB 클래스를 사용해서 부재의 중심포인트를 구할 수 있다.
    // 또한 중심포인트에서 부재의 크기를 구할 수 있는데
    // extent0, extent1, extent2가 중심포인트에서 부재의 모양까지 Axis0, Axis1, Axis2 방향으로 X, Y, Z만큼
    // 떨어져있다.
    // 좌표계변환을 이용해야 한다.
    public partial class obb : Form
    {
        TSM.Model model;
        private TSM.Beam beam;
        TSG.OBB obb1;
        

        public obb()
        {
            InitializeComponent();

            this.model = new TSM.Model();
            if (!this.model.GetConnectionStatus()) return;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Column 선택(1개만)
            TSM.UI.Picker picker = new TSM.UI.Picker();

            this.beam = picker.PickObject(TSM.UI.Picker.PickObjectEnum.PICK_ONE_PART, "beam을 선택하세요") as TSM.Beam;

            if (null == this.beam)
            {
                MessageBox.Show("선택한 beam이 없습니다.");
                return;
            }

            CreateOrientedBoundingBox(this.beam);
        }

        private TSG.Point CalculateCenterPoint(TSG.Point min, TSG.Point max)
        {
            double x = min.X + ((max.X - min.X) / 2);
            double y = min.Y + ((max.Y - min.Y) / 2);
            double z = min.Z + ((max.Z - min.Z) / 2);

            return new TSG.Point(x, y, z);
        }

        private TSG.OBB CreateOrientedBoundingBox(TSM.Beam beam)
        {
            if(null != beam)
            {
                TSM.WorkPlaneHandler workPlaneHandler = model.GetWorkPlaneHandler();
                TSM.TransformationPlane transformationPlane = workPlaneHandler.GetCurrentTransformationPlane();

                TSM.Solid solid = beam.GetSolid();

                TSG.Point minPointInCurrentPlane = solid.MinimumPoint;
                TSG.Point maxPointInCurrentPlane = solid.MaximumPoint;
                TSG.Point centerPoint = CalculateCenterPoint(minPointInCurrentPlane, maxPointInCurrentPlane);

                TSG.CoordinateSystem coordinateSystem = beam.GetCoordinateSystem();
                TSM.TransformationPlane localtransformationPlane = new TSM.TransformationPlane(coordinateSystem);
                workPlaneHandler.SetCurrentTransformationPlane(localtransformationPlane);

                solid = beam.GetSolid();

                TSG.Point minPoint = solid.MinimumPoint;
                TSG.Point maxPoint = solid.MaximumPoint;

                double extent0 = (maxPoint.X - minPoint.X) / 2;
                double extent1 = (maxPoint.Y - minPoint.Y) / 2;
                double extent2 = (maxPoint.Z - minPoint.Z) / 2;

                workPlaneHandler.SetCurrentTransformationPlane(transformationPlane);
                obb1 = new TSG.OBB(centerPoint, coordinateSystem.AxisX, coordinateSystem.AxisY, coordinateSystem.AxisX.Cross(coordinateSystem.AxisY), extent0, extent1, extent2);
                TSG.Point[] list = obb1.ComputeVertices(); // beam의 꼭지점 list를 가져온다.
            }

            return obb1;
        }
    }
}
