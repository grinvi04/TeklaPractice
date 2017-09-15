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

using ETG = EngSoft.TSEngine.Geometry;

namespace Tekla_Practice
{
    public partial class Form2 : Form
    {
        private TSG.Point PickStartPoint;
        private TSG.Point PickEndPoint;
        private TSG.Vector BaseVector;
        private TSG.Vector BaseVector90;

        public Form2()
        {
            InitializeComponent();

            this.PickStartPoint = new TSG.Point();
            this.PickEndPoint = new TSG.Point();
            this.BaseVector = new TSG.Vector();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            TSM.Model model = new TSM.Model();

            if(!model.GetConnectionStatus()) return;

            TSM.UI.Picker picker = new TSM.UI.Picker();
            this.PickStartPoint = picker.PickPoint();
            this.PickEndPoint = picker.PickPoint();
            this.BaseVector = new TSG.Vector(this.PickEndPoint - this.PickStartPoint).GetNormal();
            this.BaseVector90 = ETG.Geometry2D.Rotate90(this.BaseVector) * -1;

            TSM.ContourPlate contourPlate = this.CreateContourPlate();
            this.Cut(contourPlate);

            //this.CreateSingleRebar(contourPlate);
            this.CreateRebarGroup(contourPlate, (TSM.ContourPoint)contourPlate.Contour.ContourPoints[2], (TSM.ContourPoint)contourPlate.Contour.ContourPoints[3], 500, true);
            this.CreateRebarGroup(contourPlate, (TSM.ContourPoint)contourPlate.Contour.ContourPoints[2], (TSM.ContourPoint)contourPlate.Contour.ContourPoints[3], 1000, true);
            this.CreateRebarGroup(contourPlate, (TSM.ContourPoint)contourPlate.Contour.ContourPoints[6], (TSM.ContourPoint)contourPlate.Contour.ContourPoints[7], 500, false);
            this.CreateRebarGroup(contourPlate, (TSM.ContourPoint)contourPlate.Contour.ContourPoints[6], (TSM.ContourPoint)contourPlate.Contour.ContourPoints[7], 1000, false);

            model.CommitChanges();
        }

        /// <summary>
        /// 더블T 슬래브 생성
        /// </summary>
        /// <returns></returns>
        private TSM.ContourPlate CreateContourPlate()
        {
            TSM.ContourPlate contourPlate = new TSM.ContourPlate();
            ArrayList sizeList = this.GetSizeList();
            ArrayList polygon = this.GetContourPlatePolygon(sizeList);

            double slabLengh = new TSG.Vector(this.PickEndPoint - this.PickStartPoint).GetLength();

            contourPlate.Class = "4";
            contourPlate.Material.MaterialString = "C40";
            contourPlate.Name = "DTS";
            contourPlate.Profile.ProfileString = slabLengh.ToString();
            contourPlate.Position.Depth = TSM.Position.DepthEnum.BEHIND;
            contourPlate.Contour.ContourPoints.AddRange(polygon);

            contourPlate.Insert();

            return contourPlate;
        }

        private ArrayList GetSizeList()
        {
            ArrayList list = new ArrayList();

            int size1, size2, size3, size4, size5, size6, size7, size8, size9;
            int height1, height2;

            if(Int32.TryParse(textBox_size1.Text, out size1))
            {
                list.Add(size1);
            }

            if (Int32.TryParse(textBox_size2.Text, out size2))
            {
                list.Add(size2);
            }

            if (Int32.TryParse(textBox_size3.Text, out size3))
            {
                list.Add(size3);
            }

            if (Int32.TryParse(textBox_size4.Text, out size4))
            {
                list.Add(size4);
            }

            if (Int32.TryParse(textBox_size5.Text, out size5))
            {
                list.Add(size5);
            }

            if (Int32.TryParse(textBox_size6.Text, out size6))
            {
                list.Add(size6);
            }

            if (Int32.TryParse(textBox_size7.Text, out size7))
            {
                list.Add(size7);
            }

            if (Int32.TryParse(textBox_size8.Text, out size8))
            {
                list.Add(size8);
            }

            if (Int32.TryParse(textBox_size9.Text, out size9))
            {
                list.Add(size9);
            }

            if (Int32.TryParse(textBox_height1.Text, out height1))
            {
                list.Add(height1);
            }

            if (Int32.TryParse(textBox_height2.Text, out height2))
            {
                list.Add(height2);
            }

            return list;
        }

        /// <summary>
        /// 더블T 슬래브 생성 시 polygon 구하는 메소드
        /// </summary>
        /// <returns></returns>
        private ArrayList GetContourPlatePolygon(ArrayList sizeList)
        {
            ArrayList polygon = new ArrayList();

            TSG.Vector vectorX = new TSG.Vector(this.BaseVector90);
            TSG.Vector vectorZ = new TSG.Vector(0, 0, 1);

            TSG.Point point = new TSG.Point(this.PickStartPoint);
            // 전체 길이
            int totalSize = 0;

            //foreach(Object size in sizeList)
            //{
            //    totalSize += Int32.Parse(size.ToString());
            //}

            for(int i = 0, ii = sizeList.Count-2; i < ii; i++)
            {
                totalSize += Int32.Parse(sizeList[i].ToString());
            }

            double chamfer = 0.0;
            Double.TryParse(textBox_chamfer.Text, out chamfer);

            // 시작포인트
            polygon.Add(new TSM.ContourPoint(point, null));

            point += vectorX * Int32.Parse(sizeList[0].ToString());
            polygon.Add(new TSM.ContourPoint(point, null));

            point += vectorX * Int32.Parse(sizeList[1].ToString());
            point -= vectorZ * Int32.Parse(sizeList[10].ToString());
            polygon.Add(new TSM.ContourPoint(point, new TSM.Chamfer(chamfer, 0, TSM.Chamfer.ChamferTypeEnum.CHAMFER_ROUNDING)));

            point += vectorX * Int32.Parse(sizeList[2].ToString());
            polygon.Add(new TSM.ContourPoint(point, new TSM.Chamfer(chamfer, 0, TSM.Chamfer.ChamferTypeEnum.CHAMFER_ROUNDING)));

            point += vectorX * Int32.Parse(sizeList[3].ToString());
            point += vectorZ * Int32.Parse(sizeList[10].ToString());
            polygon.Add(new TSM.ContourPoint(point, null));

            point += vectorX * Int32.Parse(sizeList[4].ToString());
            polygon.Add(new TSM.ContourPoint(point, null));

            point += vectorX * Int32.Parse(sizeList[5].ToString());
            point -= vectorZ * Int32.Parse(sizeList[10].ToString());
            polygon.Add(new TSM.ContourPoint(point, new TSM.Chamfer(chamfer, 0, TSM.Chamfer.ChamferTypeEnum.CHAMFER_ROUNDING)));

            point += vectorX * Int32.Parse(sizeList[6].ToString());
            polygon.Add(new TSM.ContourPoint(point, new TSM.Chamfer(chamfer, 0, TSM.Chamfer.ChamferTypeEnum.CHAMFER_ROUNDING)));

            point += vectorX * Int32.Parse(sizeList[7].ToString());
            point += vectorZ * Int32.Parse(sizeList[10].ToString());
            polygon.Add(new TSM.ContourPoint(point, null));

            point += vectorX * Int32.Parse(sizeList[8].ToString());
            polygon.Add(new TSM.ContourPoint(point, null));

            point += vectorZ * Int32.Parse(sizeList[9].ToString());
            polygon.Add(new TSM.ContourPoint(point, null));

            point -= vectorX * totalSize;
            polygon.Add(new TSM.ContourPoint(point, null));

            return polygon;
        }

        private void Cut(TSM.Part mainPart)
        {
            TSG.Point pointS = null;
            TSG.Point pointE = null;

            TSG.Point pointS_2 = null;
            TSG.Point pointE_2 = null;

            if (mainPart is TSM.ContourPlate)
            {
                TSM.ContourPlate plate = mainPart as TSM.ContourPlate;
                pointS = (TSM.ContourPoint)plate.Contour.ContourPoints[0];
                pointE = (TSM.ContourPoint)plate.Contour.ContourPoints[11];
            }
            else if(mainPart is TSM.Beam)
            {
                TSM.Beam beam = mainPart as TSM.Beam;
            }


            TSG.Vector vectorX = new TSG.Vector(1, 0, 0);
            TSG.Vector vectorY = new TSG.Vector(0, 1, 0);
            TSG.Vector vectorZ = new TSG.Vector(0, 0, 1);

            double booleanPart_w = 0.0, booleanPart_h = 0.0;

            Double.TryParse(textBox_booleanPart_width.Text, out booleanPart_w);
            Double.TryParse(textBox_booleanPart_height.Text, out booleanPart_h);

            // S만구하고 E는 높이값으로 구하도록 한다.
            pointS += vectorX * (booleanPart_w / 2);
            pointS += vectorY * (booleanPart_h / 2);

            pointE += vectorX * (booleanPart_w / 2);
            pointE += vectorY * (booleanPart_h / 2);

            this.CutBoolen(mainPart, this.CutMember(pointS, pointE));

            // PickEndPoint로 반대편 point 찾기
            pointS_2 = this.PickEndPoint;
            pointE_2 = pointE;
            pointE_2.Y = this.PickEndPoint.Y;

            //this.CutBoolen(mainPart, this.CutMember(pointS_2, pointE_2));            
        }

        private TSM.Beam CutMember(TSG.Point startPoint, TSG.Point endPoint)
        {
            TSM.Beam cut = new TSM.Beam();

            cut.StartPoint = startPoint;
            cut.EndPoint = endPoint;
            
            cut.Name = "Column";

            //cut.Class = "2";
            cut.Material.MaterialString = "C24";

            cut.Position.Depth = TSM.Position.DepthEnum.MIDDLE;
            cut.Position.Plane = TSM.Position.PlaneEnum.MIDDLE;
            cut.Position.Rotation = TSM.Position.RotationEnum.TOP;
            //cut.Profile.ProfileString = "400X600";
            cut.Profile.ProfileString = textBox_booleanPart_width.Text + "X" + textBox_booleanPart_height.Text;

            cut.Insert();

            cut.Class = TSM.BooleanPart.BooleanOperativeClassName;

            return cut;
        }

        private void CutBoolen(TSM.Part mainPart, TSM.Beam cut)
        {
            TSM.BooleanPart boolenPart = new TSM.BooleanPart();
            boolenPart.Father = mainPart; // 잘릴부재
            boolenPart.SetOperativePart(cut as TSM.Part); // 자를 부재
            //boolenPart.Type = TSM.BooleanPart.BooleanTypeEnum.BOOLEAN_CUT;
            boolenPart.Insert();

            cut.Delete();
        }

        private void CreateSingleRebar(TSM.ContourPlate contourPlate)
        {
            TSM.SingleRebar singleRebar = new TSM.SingleRebar();

            singleRebar.Class = 2;
            singleRebar.Grade = "SD400";
            singleRebar.Name = "SingleRebar";
            singleRebar.Father = contourPlate;
            singleRebar.Polygon = this.GetSingleRebarPolygon(contourPlate.Contour.ContourPoints);
            singleRebar.EndHook.Shape = TSM.RebarHookData.RebarHookShapeEnum.HOOK_90_DEGREES;
            singleRebar.Size = "19";

            singleRebar.Insert();            
        }

        private TSM.Polygon GetSingleRebarPolygon(ArrayList contourPoints)
        {
            TSG.Vector vectorX = new TSG.Vector(1, 0, 0);
            TSG.Vector vectorY = new TSG.Vector(0, 1, 0);
            TSG.Vector vectorZ = new TSG.Vector(0, 0, 1);

            TSM.Polygon polygon = new TSM.Polygon();            

            TSM.ContourPoint contourPoint1 = (TSM.ContourPoint)contourPoints[2];
            TSM.ContourPoint contourPoint2 = (TSM.ContourPoint)contourPoints[3];

            TSG.Point point = new TSG.Point(contourPoint1.X, contourPoint1.Y, contourPoint1.Z);

            double distance = 0.0;
            distance = (contourPoint2.X - contourPoint1.X) / 2;
            point += vectorX * distance;
            polygon.Points.Add(point);

            point += vectorZ * 1000;
            polygon.Points.Add(point);

            return polygon;
        }

        private void CreateRebarGroup(TSM.ContourPlate contourPlate, TSM.ContourPoint startPoint, TSM.ContourPoint endPoint, double height, Boolean isEndHookReverse)
        {
            TSM.RebarGroup rebarGroup = new TSM.RebarGroup();

            rebarGroup.Name = "RebarGroup";
            rebarGroup.NumberingSeries.Prefix = "RG";
            rebarGroup.NumberingSeries.StartNumber = 1;
            rebarGroup.Class = 3;
            rebarGroup.Size = "19";
            rebarGroup.RadiusValues.Add(60.0);
            rebarGroup.Grade = "SD400";
            rebarGroup.SpacingType = TSM.BaseRebarGroup.RebarGroupSpacingTypeEnum.SPACING_TYPE_TARGET_SPACE;
            rebarGroup.Spacings.Add(200.0);

            rebarGroup.StartHook.Shape = TSM.RebarHookData.RebarHookShapeEnum.NO_HOOK;
            rebarGroup.EndHook.Shape = TSM.RebarHookData.RebarHookShapeEnum.CUSTOM_HOOK;

            double angle = 0.0, radius = 0.0, length = 0.0;
            angle = 90.0;
            radius = 60.0;
            length = 230.0;

            if (isEndHookReverse)
            {                
                rebarGroup.EndHook.Angle = angle;
                rebarGroup.EndHook.Radius = radius;
                rebarGroup.EndHook.Length = length;
            } else
            {
                rebarGroup.EndHook.Angle = -angle;
                rebarGroup.EndHook.Radius = radius;
                rebarGroup.EndHook.Length = length;
            }
            

            rebarGroup.StartPointOffsetType = TSM.Reinforcement.RebarOffsetTypeEnum.OFFSET_TYPE_COVER_THICKNESS;
            rebarGroup.EndPointOffsetType = TSM.Reinforcement.RebarOffsetTypeEnum.OFFSET_TYPE_COVER_THICKNESS;
            rebarGroup.StartPointOffsetValue = 40.0;
            rebarGroup.EndPointOffsetValue = 40.0;

            rebarGroup.Father = contourPlate;

            //rebarGroup.FromPlaneOffset = 40.0;
            rebarGroup.ExcludeType = TSM.BaseRebarGroup.ExcludeTypeEnum.EXCLUDE_TYPE_NONE;

            TSM.ContourPoint point = new TSM.ContourPoint();

            //rebarGroup.StartPoint = new TSG.Point(10000, 10000, 1000);
            //rebarGroup.EndPoint = new TSG.Point(10000, -10000, 1000); ;

            rebarGroup.Polygons.Add(this.GetRebarGroupPolygon(contourPlate, rebarGroup, startPoint, endPoint, height));

            rebarGroup.Insert();
        }

        private TSM.Polygon GetRebarGroupPolygon(TSM.ContourPlate contourPlate, TSM.RebarGroup rebarGroup, TSM.ContourPoint startPoint, TSM.ContourPoint endPoint, double height)
        {
            TSG.Vector baseVector = new TSG.Vector(startPoint - endPoint).GetNormal();

            TSG.Vector vectorX = new TSG.Vector(1, 0, 0);
            TSG.Vector vectorY = new TSG.Vector(this.BaseVector);
            TSG.Vector vectorZ = new TSG.Vector(0, 0, 1);

            TSM.Polygon polygon = new TSM.Polygon();

            TSG.Point point = new TSG.Point(startPoint.X, startPoint.Y, startPoint.Z);

            double distance = 0.0;
            double length = 0.0;

            distance = (endPoint.X - startPoint.X) / 2;
            point += vectorX * distance;
            polygon.Points.Add(point);            

            point += vectorZ * height;
            polygon.Points.Add(point);

            TSG.Point rebarGroupStartPoint = new TSG.Point(point.X, point.Y, point.Z);
            rebarGroup.StartPoint = rebarGroupStartPoint;

            Double.TryParse(contourPlate.Profile.ProfileString, out length);

            point += vectorY * length;
            rebarGroup.EndPoint = point;

            return polygon;
        }
    }
}
