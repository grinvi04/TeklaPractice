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
    // Matrix class와 MatrixFactory class를 이용하여 Point를 주어진 좌표계에 맞게 변형할수 있다.
    // OBB와 마찬가지로 Matrix를 이용해서 beam이 중심이 되서 start, end point를 구할 수 있다.
    public partial class matrix : Form
    {
        TSM.Model model;
        private TSM.Beam beam;

        public matrix()
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

            TSM.WorkPlaneHandler workPlaneHandler = model.GetWorkPlaneHandler();
            TSG.CoordinateSystem coordinate = beam.GetCoordinateSystem();

            Console.WriteLine("Pick Part : " + beam.StartPoint + ", " + beam.EndPoint);

            // To Example
            TSG.Matrix transformTo = TSG.MatrixFactory.ToCoordinateSystem(coordinate);
            TSG.Point toStartPoint = transformTo.Transform(beam.StartPoint);
            TSG.Point toEndPoint = transformTo.Transform(beam.EndPoint);

            Console.WriteLine("TransForm : " + toStartPoint + ", " + toEndPoint);

            transformTo.Transpose();
            toStartPoint = transformTo.Transform(beam.StartPoint);
            toEndPoint = transformTo.Transform(beam.EndPoint);
            Console.WriteLine("Transpose : " + toStartPoint + ", " + toEndPoint);

            CreateBeam(toStartPoint, toEndPoint);
        }

        private TSM.Beam CreateBeam(TSG.Point startPoint, TSG.Point endPoint)
        {
            TSG.Point s_point = startPoint;
            TSG.Point e_point = endPoint;

            TSM.Beam column = new TSM.Beam();
            column.Name = "Column";
            column.StartPoint = startPoint;
            column.EndPoint = endPoint;

            column.Class = "3";
            column.Material.MaterialString = "C24";
            column.Finish = "C1";

            column.Position.Depth = TSM.Position.DepthEnum.MIDDLE;
            column.Position.Plane = TSM.Position.PlaneEnum.MIDDLE;
            column.Position.Rotation = TSM.Position.RotationEnum.TOP;
            column.Profile.ProfileString = "800X600";
            bool isInsert = column.Insert();

            return column;
        }
    }
}
