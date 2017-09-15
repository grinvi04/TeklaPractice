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
    public partial class Form8 : Form
    {
        private TSM.Model model;

        private TSM.WorkPlaneHandler workPlaneHandler;
        private TSM.TransformationPlane currentPlane;
        private TSM.TransformationPlane partPlane;

        public Form8()
        {
            // 5가지 부재(column, beam, contourPlate, panel, ?)을 선택해서
            // 각 면의 가운데를 원기둥 모양으로 잘라낸다.
            InitializeComponent();

            this.model = new TSM.Model();

            this.workPlaneHandler = model.GetWorkPlaneHandler();
            this.currentPlane = workPlaneHandler.GetCurrentTransformationPlane();

            if (!this.model.GetConnectionStatus()) return;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            TSM.UI.Picker picker = new TSM.UI.Picker();

            TSM.ModelObject modelObject = picker.PickObject(TSM.UI.Picker.PickObjectEnum.PICK_ONE_PART, "부재를 선택하세요");

            this.CheckType(modelObject);

            // model 반영
            model.CommitChanges();
        }

        private void CheckType(TSM.ModelObject modelObject)
        {
            Type type = modelObject.GetType();

            // 부재좌표계로 설정후에 X축은 부재의 시작포인트->끝포인트 방향이다.
            this.SetCoordinateSystem(modelObject, true);

            double beamHeight = 0.0;
            Double.TryParse(textBox_beamHeight.Text, out beamHeight);

            TSG.Point startPoint = null;
            TSG.Point endPoint = null;

            TSG.Point basePoint = new TSG.Point(0, 0, 0);

            TSG.Vector vectorX = new TSG.Vector(1, 0, 0);
            TSG.Vector vectorY = new TSG.Vector(0, 1, 0);
            TSG.Vector vectorZ = new TSG.Vector(0, 0, 1);

            if (type == typeof(TSM.Beam))
            {
                TSM.Beam beam = modelObject as TSM.Beam;
                double size = new TSG.Vector(beam.EndPoint - beam.StartPoint).GetLength();

                double width = 0.0, length = 0.0;
                Double.TryParse(beam.Profile.ProfileString.Split('X')[0], out length);
                Double.TryParse(beam.Profile.ProfileString.Split('X')[1], out width);

                startPoint = basePoint;
                endPoint = basePoint + vectorX * beamHeight;// new TSG.Point(beamHeight, 0, 0);
                this.CreateBeam(startPoint, endPoint);

                startPoint = basePoint + vectorX * size;// new TSG.Point(length, 0, 0);
                endPoint = basePoint + (vectorX * size) - (vectorX * beamHeight);// new TSG.Point(length - beamHeight, 0, 0);
                this.CreateBeam(startPoint, endPoint);

                startPoint = basePoint + vectorX * (size / 2);
                startPoint += basePoint + vectorY * (length / 2);// new TSG.Point(length / 2, (height / 2), 0);
                endPoint = basePoint + vectorX * (size / 2);
                endPoint += basePoint + vectorY * (length / 2) - (vectorY * beamHeight);// new TSG.Point(length / 2, (height / 2) - beamHeight, 0);
                this.CreateBeam(startPoint, endPoint);

                startPoint = basePoint + vectorX * (size / 2);
                startPoint += basePoint + vectorY * (length / 2) * -1;// new TSG.Point(length / 2, (height / 2) * -1, 0);
                endPoint = basePoint + vectorX * (size / 2);
                endPoint += basePoint + vectorY * (length / 2) * -1 + (vectorY * beamHeight);//new TSG.Point(length / 2, ((height / 2) - beamHeight) * -1, 0);
                this.CreateBeam(startPoint, endPoint);

                startPoint = basePoint + vectorX * (size / 2);
                startPoint += basePoint + vectorZ * (width / 2); // new TSG.Point(length / 2, 0, (width / 2));
                endPoint = basePoint + vectorX * (size / 2);
                endPoint += basePoint + vectorZ * (width / 2) - (vectorZ * beamHeight);// new TSG.Point(length / 2, 0, (width / 2) - beamHeight);
                this.CreateBeam(startPoint, endPoint);

                startPoint = basePoint + vectorX * (size / 2);
                startPoint += basePoint + vectorZ * (width / 2) * -1;// new TSG.Point(length / 2, 0, (width / 2) * -1);
                endPoint = basePoint + vectorX * (size / 2);
                endPoint += basePoint + vectorZ * (width / 2) * -1 + (vectorZ * beamHeight);// new TSG.Point(length / 2, 0, ((width / 2) - beamHeight) * -1);
                this.CreateBeam(startPoint, endPoint);
            } else if (type == typeof(TSM.ContourPlate))
            {
                TSM.ContourPlate contourPlate = modelObject as TSM.ContourPlate;

                // 사각형 기준
                ArrayList pointList = contourPlate.Contour.ContourPoints;
                TSG.Point point1 = this.partPlane.TransformationMatrixToLocal.Transform(currentPlane.TransformationMatrixToGlobal.Transform(pointList[0] as TSG.Point));
                TSG.Point point2 = this.partPlane.TransformationMatrixToLocal.Transform(currentPlane.TransformationMatrixToGlobal.Transform(pointList[1] as TSG.Point));
                TSG.Point point3 = this.partPlane.TransformationMatrixToLocal.Transform(currentPlane.TransformationMatrixToGlobal.Transform(pointList[2] as TSG.Point));
                TSG.Point point4 = this.partPlane.TransformationMatrixToLocal.Transform(currentPlane.TransformationMatrixToGlobal.Transform(pointList[3] as TSG.Point));                

                double height = 0.0;
                Double.TryParse(contourPlate.Profile.ProfileString, out height);

                double width = new TSG.Vector(point2 - point1).GetLength();
                double length = new TSG.Vector(point3 - point2).GetLength();

                startPoint = basePoint;
                startPoint += vectorX * (width / 2);
                endPoint = basePoint + vectorX * (width / 2);
                endPoint += vectorY * beamHeight;
                this.CreateBeam(startPoint, endPoint);

                startPoint = basePoint + (vectorX * width);
                startPoint += vectorY * (length / 2);
                endPoint = basePoint + (vectorX * width);
                endPoint += vectorY * (length / 2);
                endPoint += vectorX * beamHeight * -1;
                this.CreateBeam(startPoint, endPoint);

                startPoint = basePoint + vectorX * (width / 2);
                startPoint += vectorY * length;
                endPoint = basePoint + vectorX * (width / 2);
                endPoint += vectorY * length;
                endPoint += vectorY * beamHeight * -1;
                this.CreateBeam(startPoint, endPoint);
                
                startPoint = basePoint + vectorY * (length / 2);
                endPoint = basePoint + vectorY * (length / 2);
                endPoint += vectorX * beamHeight;
                this.CreateBeam(startPoint, endPoint);

                startPoint = basePoint + vectorZ * (height / 2);
                startPoint += vectorX * (width / 2);
                startPoint += vectorY * (length / 2);
                endPoint = basePoint + vectorZ * (height / 2);
                endPoint += vectorX * (width / 2);
                endPoint += vectorY * (length / 2);
                endPoint += vectorZ * beamHeight * -1;
                this.CreateBeam(startPoint, endPoint);

                startPoint = basePoint + vectorZ * (height / 2) * -1;
                startPoint += vectorX * (width / 2);
                startPoint += vectorY * (length / 2);
                endPoint = basePoint + vectorZ * (height / 2) * -1;
                endPoint += vectorX * (width / 2);
                endPoint += vectorY * (length / 2);
                endPoint += vectorZ * beamHeight;
                this.CreateBeam(startPoint, endPoint);
            } else if (type == typeof(TSM.PolyBeam))
            {
                TSM.PolyBeam polyBeam = modelObject as TSM.PolyBeam;

                ArrayList pointList = polyBeam.Contour.ContourPoints;
                double length = 0.0;
                // 총 포인트 수 - 1이 하나의 개체
                // 마지막 포인트와 첫번째 포인트 연결은 생성안됨
                for(int i = 0; i < pointList.Count - 1; i++)
                {
                    TSG.Point s_point = this.partPlane.TransformationMatrixToLocal.Transform(currentPlane.TransformationMatrixToGlobal.Transform(pointList[i] as TSG.Point));
                    TSG.Point e_point = this.partPlane.TransformationMatrixToLocal.Transform(currentPlane.TransformationMatrixToGlobal.Transform(pointList[i+1] as TSG.Point));

                    length = new TSG.Vector(e_point - s_point).GetLength();
                    TSG.Vector vector = new TSG.Vector(e_point - s_point).GetNormal();
                    TSG.Vector vector90 = new TSG.Vector(-vector.Z, 0, vector.X);

                    double height = 0.0, width = 0.0;
                    Double.TryParse(polyBeam.Profile.ProfileString.Split('X')[0], out height);
                    Double.TryParse(polyBeam.Profile.ProfileString.Split('X')[1], out width);
                    height = height / 2;
                    width = width / 2;

                    startPoint = s_point + (vector * (length / 2));
                    startPoint += vectorY * height;

                    endPoint = s_point + (vector * (length / 2));
                    endPoint += vectorY * height;
                    endPoint += vector90 * beamHeight;
                    this.CreateBeam(startPoint, endPoint);

                    //startPoint = s_point + (vector * (length / 2));
                    //startPoint += vectorY * height;
                    //startPoint += vector90 * width;

                    //endPoint = s_point + (vector * (length / 2));
                    //endPoint += vectorY * height;
                    //endPoint += vector90 * width;
                    //endPoint += vector90 * beamHeight * -1;
                    //this.CreateBeam(startPoint, endPoint);


                    //startPoint += vector90 * (width * 2) * -1;
                    //endPoint = startPoint + vector90 * beamHeight;

                    //this.CreateBeam(startPoint, endPoint);
                }
            } else
            {
                MessageBox.Show("Beam, ContourPlate, Plane만 선택하세요.");
                return;
            }

            this.SetCoordinateSystem(modelObject, false);
        }

        private void SetCoordinateSystem(TSM.ModelObject modelObject, bool isPart)
        {
            this.partPlane = new TSM.TransformationPlane(modelObject.GetCoordinateSystem());

            if (isPart)
            {
                // 부재 좌표계로 설정
                this.workPlaneHandler.SetCurrentTransformationPlane(this.partPlane);
            } else
            {
                // 글로벌 좌표계로 설정
                this.workPlaneHandler.SetCurrentTransformationPlane(this.currentPlane);
            }
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
            column.Profile.ProfileString = "300X300";
            bool isInsert = column.Insert();

            return column;
        }
    }
}
