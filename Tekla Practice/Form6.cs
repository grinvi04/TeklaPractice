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
using TSS = Tekla.Structures.Solid;
using ETG = EngSoft.TSEngine.Geometry;
using System.Collections;

namespace Tekla_Practice
{
    public partial class Form6 : Form
    {
        private TSG.Point PickPoint;

        public Form6()
        {
            InitializeComponent();

            this.PickPoint = new TSG.Point();
        }

        private void button_createBeam_Click(object sender, EventArgs e)
        {
            TSM.Model model = new TSM.Model();

            if (!model.GetConnectionStatus()) return;

            TSM.UI.Picker picker = new TSM.UI.Picker();

            this.PickPoint = picker.PickPoint();

            TSM.Beam beam = this.CreateBeam(this.PickPoint);

            TSM.WorkPlaneHandler workPlaneHandler = model.GetWorkPlaneHandler();
            TSM.TransformationPlane currentPlane = workPlaneHandler.GetCurrentTransformationPlane();
            TSM.TransformationPlane beamPlane = new TSM.TransformationPlane(beam.GetCoordinateSystem());

            // 부재 좌표계로 설정된 BeamPoint
            TSG.Point startPoint = beamPlane.TransformationMatrixToLocal.Transform(currentPlane.TransformationMatrixToGlobal.Transform(beam.StartPoint));
            TSG.Point endPoint = beamPlane.TransformationMatrixToLocal.Transform(currentPlane.TransformationMatrixToGlobal.Transform(beam.EndPoint));

            // 부재 좌표계로 설정
            workPlaneHandler.SetCurrentTransformationPlane(beamPlane);

            // beam의 꼭지점 가져오기
            List<TSG.Point> vertexList = new List<TSG.Point>();
            vertexList = this.GetVertexList(beam);

            // 중복제거
            IEnumerable<TSG.Point> distinctVertex = vertexList.Distinct();

            List<TSG.Point> list = new List<TSG.Point>();

            foreach (var vertex in distinctVertex)
            {
                // beam의 endPoint의 X와 같은 point 찾는다
                if(vertex.X.Equals(endPoint.X))
                {
                    list.Add(vertex);
                }
            }

            list.Sort();

            // endPoint와 4개 점들의 단위vector
            List<TSG.Vector> vectorList = new List<TSG.Vector>();
            TSG.Vector vector = new TSG.Vector();

            foreach(var point in list)
            {
                vector = new TSG.Vector(point - endPoint).GetNormal();
                vectorList.Add(vector);
            }

            // 입력받은 철근의 x,y좌표값으로 포인트 생성
            double rebarPointX = 0.0;
            double rebarPointY = 0.0;

            Double.TryParse(textBox_pointX.Text, out rebarPointX);
            Double.TryParse(textBox_pointY.Text, out rebarPointY);

            TSG.Point rebarStartPoint = new TSG.Point(startPoint.X, rebarPointX, rebarPointY);
            TSG.Point rebarEndPoint = new TSG.Point(endPoint.X, rebarPointX, rebarPointY);

            // 생성된 rebarPoint의 단위vector
            TSG.Vector rebarVector = new TSG.Vector(rebarEndPoint - endPoint).GetNormal();

            // 생성된 rebarPoint위치가 어느 영역에 속해있는지 확인
            double dot1 = 0.0, dot2 = 0.0, dot3 = 0.0, dot4 = 0.0;

            dot1 = TSG.Vector.Dot(vectorList[0], rebarVector);
            dot2 = TSG.Vector.Dot(vectorList[1], rebarVector);
            dot3 = TSG.Vector.Dot(vectorList[2], rebarVector);
            dot4 = TSG.Vector.Dot(vectorList[3], rebarVector);

            if(dot1 < 0)
            {
                if(dot2 >= 0)
                {
                    // B영역에 있음
                    this.CreateSingleRebar(beam, rebarVector, rebarStartPoint, rebarEndPoint);
                }
            }

            if (dot2 < 0)
            {
                if (dot3 >= 0)
                {
                    // C영역에 있음
                    this.CreateSingleRebar(beam, rebarVector, rebarStartPoint, rebarEndPoint);
                }
            }

            if (dot3 < 0)
            {
                if (dot4 >= 0)
                {
                    // D영역에 있음
                    this.CreateSingleRebar(beam, rebarVector, rebarStartPoint, rebarEndPoint);
                }
            }

            if (dot4 < 0)
            {
                if (dot1 >= 0)
                {
                    // A영역에 있음
                    this.CreateSingleRebar(beam, rebarVector, rebarStartPoint, rebarEndPoint);
                }
            }

            //foreach (var baseVector in vectorList)
            //{
            //    dot = TSG.Vector.Dot(baseVector, rebarVector);
            //    if (dot < 0) isLeft = true;
            //    if (dot > 0) isRight = true;
            //    //Console.WriteLine("baseVector : {0}, rebarVector : {1}, dot : {2}, dotCeiling : {3}", baseVector, rebarVector, dot, Math.Ceiling(dot));
            //}


            // 글로벌 좌표계로 설정
            workPlaneHandler.SetCurrentTransformationPlane(currentPlane);

            // model 반영
            model.CommitChanges();
        }

        private TSM.Beam CreateBeam(TSG.Point point)
        {
            TSG.Point s_point = point;
            TSG.Point e_point = new TSG.Point();

            TSM.Beam column = new TSM.Beam();
            column.Name = "Column";
            column.StartPoint = s_point;

            e_point.X = s_point.X;
            e_point.Y = s_point.Y;
            e_point.Z = s_point.Z + 6000;
            column.EndPoint = e_point;

            column.Class = "13";
            column.Material.MaterialString = "C24";
            column.Finish = "C1";

            column.Position.Depth = TSM.Position.DepthEnum.MIDDLE;
            column.Position.Plane = TSM.Position.PlaneEnum.MIDDLE;
            column.Position.Rotation = TSM.Position.RotationEnum.TOP;
            column.Profile.ProfileString = "600X600";
            bool isInsert = column.Insert();

            return column;
        }

        private List<TSG.Point> GetVertexList(TSM.Beam beam)
        {
            List<TSG.Point> faceList = new List<TSG.Point>();
            List<TSG.Point> vertexList = new List<TSG.Point>();
            

            // 부재의 좌표정보 가져오기
            TSM.Solid solid = beam.GetSolid();

            TSS.FaceEnumerator faceenum = solid.GetFaceEnumerator();

            while(faceenum.MoveNext())
            {
                // 면 정보 추출
                TSS.Face face = faceenum.Current as TSS.Face;

                if(null != face)
                {
                    faceList.Add(face.Normal);
                    TSS.LoopEnumerator loopenum = face.GetLoopEnumerator();

                    while (loopenum.MoveNext())
                    {
                        TSS.Loop loop = loopenum.Current as TSS.Loop;

                        if(null != loop)
                        {
                            // 꼭지점 정보 가져오기
                            TSS.VertexEnumerator vertexenum = loop.GetVertexEnumerator();
                            while (vertexenum.MoveNext())
                            {
                                TSG.Point vertex = vertexenum.Current;
                                if(null != vertex)
                                {
                                    vertexList.Add(vertex);
                                }
                            }

                        }
                    }
                }
            }


            return vertexList;
        }

        private void CreateSingleRebar(TSM.Beam beam, TSG.Vector rebarVector, TSG.Point startPoint, TSG.Point endPoint)
        {
            TSM.SingleRebar rebar = new TSM.SingleRebar();
            rebar.Class = 2;

            rebar.Grade = "SD400";
            rebar.Name = "singleRebar";

            rebar.EndHook.Shape = TSM.RebarHookData.RebarHookShapeEnum.NO_HOOK;
            rebar.StartHook.Shape = TSM.RebarHookData.RebarHookShapeEnum.NO_HOOK;
            rebar.Father = beam;
            rebar.Polygon = this.GetSingleRebarPolygon(rebarVector, startPoint, endPoint);
            rebar.Size = "19";
            rebar.OnPlaneOffsets.Add(0.0);
            rebar.FromPlaneOffset = 0;
            rebar.StartPointOffsetValue = 0;
            rebar.EndPointOffsetValue = 0;
            bool isInsert = rebar.Insert();
        }

        private TSM.Polygon GetSingleRebarPolygon(TSG.Vector rebarVector, TSG.Point startPoint, TSG.Point endPoint)
        {
            TSG.Vector vectorX = new TSG.Vector(1, 0, 0);
            TSG.Vector vectorY = new TSG.Vector(0, 1, 0);
            TSG.Vector vectorZ = new TSG.Vector(0, 0, 1);

            TSM.Polygon polygon = new TSM.Polygon();

            //TSM.Chamfer chamfer = new TSM.Chamfer(1000, 0, TSM.Chamfer.ChamferTypeEnum.CHAMFER_ROUNDING);

            //TSG.Point point = new TSG.Point(1000, 1000, 1000);
            //polygon.Points.Add(new TSM.ContourPoint(point, chamfer));

            //polygon.Points.Add(new TSM.ContourPoint(point, null));

            //point = new TSG.Point(2000, 1000, 1000);
            //polygon.Points.Add(new TSM.ContourPoint(point, chamfer));

            //point = new TSG.Point(2000, 2000, 1000);
            //polygon.Points.Add(new TSM.ContourPoint(point, chamfer));

            //point = new TSG.Point(1000, 2000, 1000);
            //polygon.Points.Add(new TSM.ContourPoint(point, null));

            polygon.Points.Add(startPoint);
            polygon.Points.Add(endPoint);

            //TSG.Vector vector = rebarVector;
            TSG.Point point = new TSG.Point(endPoint);
            //if (rebarVector.Y > rebarVector.Z)
            //{
            //    if(rebarVector.Y > 0) point.Y += 2000;
            //    else point.Y -= 2000;

            //    polygon.Points.Add(point);
            //} else
            //{
            //    if (rebarVector.Z > 0) point.Z += 2000;
            //    else point.Z -= 2000;
            //    polygon.Points.Add(point);
            //}

            if(rebarVector.Y > 0)
            {
                if(rebarVector.Z > 0)
                {
                    if(Math.Abs(rebarVector.Y) > Math.Abs(rebarVector.Z))
                    {
                        point.Y += 2000;
                    } else
                    {
                        point.Z += 2000;
                    }
                } else
                {
                    if (Math.Abs(rebarVector.Y) > Math.Abs(rebarVector.Z))
                    {
                        point.Z -= 2000;
                    } else
                    {
                        point.Y += 2000;
                    }
                }
            } else
            {
                if (rebarVector.Z > 0)
                {
                    if (Math.Abs(rebarVector.Y) > Math.Abs(rebarVector.Z))
                    {
                        point.Y -= 2000;
                    } else
                    {
                        point.Z += 2000;
                    }
                } else
                {
                    if (Math.Abs(rebarVector.Y) > Math.Abs(rebarVector.Z))
                    {
                        point.Y -= 2000;
                    } else
                    {
                        point.Z -= 2000;
                    }
                }
            }

            polygon.Points.Add(point);

            //TSG.Point point = new TSG.Point(endPoint.X, endPoint.Y, endPoint.Z + 500);
            //polygon.Points.Add(point);

            //polygon.Points.Add(new TSM.ContourPoint(new TSG.Point(1000, 3000, 1000), ));

            return polygon;
        }
    }
}
