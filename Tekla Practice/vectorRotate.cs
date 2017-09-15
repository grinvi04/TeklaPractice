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
    public partial class vectorRotate : Form
    {
        TSM.Model model;
        private TSM.Beam beam;

        public vectorRotate()
        {
            InitializeComponent();

            //this.model = new TSM.Model();
            //if (!this.model.GetConnectionStatus()) return;
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

            RoteteVector(this.beam, Math.Round(Math.PI / 180.0 * 90.0, 3));
            //rotate();
        }

        private TSG.Vector rotate(TSG.Vector vector, double degree=90.0, int axisX=0, int axisY=0, int axisZ=1)
        {
            // 단위는 라디안이며 90은 회전할 각도
            double angle = Math.Round(Math.PI * degree / 180.0, 3);
            // 벡터 파라미터는 회전축에 해당하는 벡터에 1을 입력하면 반시계방향으로 회전.
            // 아래는 z축을 기준으로 반시계 방향으로 회전하는 즉 X,Y만 바뀐다.
            TSG.Matrix rotateMatrix = TSG.MatrixFactory.Rotate(angle, new TSG.Vector(axisX, axisY, axisZ));

            // 매트릭스는 [4,3]으로 고정된 행렬이다.
            TSG.Matrix matrix = rotateMatrix.GetTranspose();

            // matrix값을 소수점 3자리로 제한하기 위해 아래 작업 수행
            for (int i = 0; i < 4; i++)
            {
                for(int j = 0; j < 3; j++)
                {
                    matrix[i,j] = Math.Round(matrix[i,j], 3);
                }
            }

            // 회전
            vector = new TSG.Vector(1000, 500, 0); // vector
            TSG.Point point = matrix.Transform(vector);

            return new TSG.Vector(point);
        }

        private void RoteteVector(TSM.Beam beam, double degree)
        {
            TSG.Vector v = new TSG.Vector(beam.EndPoint - beam.StartPoint).GetNormal();
            double length = new TSG.Vector(beam.EndPoint - beam.StartPoint).GetLength();

            TSG.Vector rotate = new TSG.Vector(v.X * Math.Cos(degree) - v.Y * Math.Sin(degree), v.X * Math.Sin(degree) + v.Y * Math.Cos(degree), 0);
            TSG.Vector rotate90 = new TSG.Vector(rotate.Y, -rotate.X, 0);

            TSG.Point sp = new TSG.Point(beam.StartPoint.X, beam.StartPoint.Y, beam.StartPoint.Z);
            //TSG.Point ep = new TSG.Point(beam.EndPoint.X * x, beam.EndPoint.Y * y, beam.EndPoint.Z);

            TSG.Point x = new TSG.Point(beam.StartPoint + (rotate * length));

            TSG.Point ep = new TSG.Point(x);
            



            CreateBeam(sp, ep);


            model.CommitChanges();

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

            column.Position.Depth = TSM.Position.DepthEnum.BEHIND;
            column.Position.Plane = TSM.Position.PlaneEnum.MIDDLE;
            column.Position.Rotation = TSM.Position.RotationEnum.TOP;
            column.Profile.ProfileString = "800X600";
            bool isInsert = column.Insert();

            return column;
        }
    }
}
