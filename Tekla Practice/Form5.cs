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
using ETG = EngSoft.TSEngine.Geometry;
using System.Collections;

namespace Tekla_Practice
{
    public partial class Form5 : Form
    {
        private TSG.Point PickStartPoint;
        private TSG.Point PickEndPoint;
        private TSG.Vector BaseVector;
        private TSG.Vector BaseVector90;
        private TSM.Beam cut;

        public Form5()
        {
            InitializeComponent();

            this.PickStartPoint = new TSG.Point();
            this.PickEndPoint = new TSG.Point();
            this.BaseVector = new TSG.Vector();
        }

        private void button_createBeam_Click(object sender, EventArgs e)
        {
            TSM.Model model = new TSM.Model();

            if (!model.GetConnectionStatus()) return;

            TSM.UI.Picker picker = new TSM.UI.Picker();

            this.PickStartPoint = picker.PickPoint();
            this.PickEndPoint = picker.PickPoint();

            this.BaseVector = new TSG.Vector(this.PickEndPoint - this.PickStartPoint).GetNormal();
            this.BaseVector90 = ETG.Geometry2D.Rotate90(this.BaseVector) * -1;

            TSM.Beam beam = this.CreateBeam(this.PickStartPoint, this.PickEndPoint);

            TSM.TransformationPlane beamPlane = new TSM.TransformationPlane(beam.GetCoordinateSystem());

            model.GetWorkPlaneHandler();


            


            TSM.RebarGroup rebarGroup = this.CreateRebarGroup(beam);            

            this.Cut(beam);

            this.ModifyRebarPolygonByIntersection(cut, rebarGroup);

            //List<TSG.Point> list = this.GetIntersection(cut, rebarGroup);

            //if(list.Count > 0)
            //{
            //    ArrayList rebarList = rebarGroup.GetRebarGeometries(false);

            //    TSM.RebarGeometry rg = null;
            //    TSG.Point point = new TSG.Point();                

            //    rg = rebarList[2] as TSM.RebarGeometry;
            //    point = rg.Shape.Points[0] as TSG.Point;
            //    point.Y = cut.EndPoint.Y;
            //    point.Z = cut.EndPoint.Z;
            //    this.CreateSingleRebar(beam, rg.Shape.Points[1] as TSG.Point, rg.Shape.Points[0] as TSG.Point);

            //    rg = rebarList[3] as TSM.RebarGeometry;
            //    point = rg.Shape.Points[0] as TSG.Point;
            //    point.Y = cut.EndPoint.Y;
            //    point.Z = cut.EndPoint.Z;
            //    this.CreateSingleRebar(beam, rg.Shape.Points[1] as TSG.Point, rg.Shape.Points[0] as TSG.Point);


            //    rebarGroup.SpacingType = TSM.BaseRebarGroup.RebarGroupSpacingTypeEnum.SPACING_TYPE_EXACT_SPACINGS;
            //    rebarGroup.Spacings.Clear();
            //    rebarGroup.Spacings.Add(200.0);
            //    rebarGroup.Modify();
            //}            

            cut.Delete();


            model.CommitChanges();
        }

        private TSM.Beam CreateBeam(TSG.Point s_point, TSG.Point e_Point)
        {
            TSG.Point startPoint = s_point;
            TSG.Point endPoiunt = e_Point;

            TSM.Beam column = new TSM.Beam();
            column.Name = "Column";
            column.StartPoint = startPoint;
            column.EndPoint = endPoiunt;

            column.Class = "6";
            column.Material.MaterialString = "C24";

            column.Position.Depth = TSM.Position.DepthEnum.MIDDLE;
            column.Position.Plane = TSM.Position.PlaneEnum.MIDDLE;
            column.Position.Rotation = TSM.Position.RotationEnum.TOP;
            column.Profile.ProfileString = "1200X1200";
            bool isInsert = column.Insert();

            return column;
        }

        private void CreateSingleRebar(TSM.Beam beam, TSG.Point s_point, TSG.Point e_point)
        {
            TSM.SingleRebar singleRebar = new TSM.SingleRebar();

            singleRebar.Class = 2;
            singleRebar.Grade = "SD400";
            singleRebar.Name = "SingleRebar";
            singleRebar.Father = beam;
            singleRebar.Polygon = this.GetSingleRebarPolygon(s_point, e_point);
            singleRebar.EndHook.Shape = TSM.RebarHookData.RebarHookShapeEnum.NO_HOOK;
            singleRebar.Size = "19";

            singleRebar.Insert();
        }

        private TSM.Polygon GetSingleRebarPolygon(TSG.Point s_point, TSG.Point e_point)
        {
            TSG.Vector vectorX = new TSG.Vector(1, 0, 0);
            TSG.Vector vectorY = new TSG.Vector(this.BaseVector);
            TSG.Vector vectorZ = new TSG.Vector(0, 0, 1);

            //vectorX = ETG.Geometry2D.Rotate90(this.BaseVector) * -1;

            TSM.Polygon polygon = new TSM.Polygon();

            TSG.Point point = new TSG.Point(s_point.X, s_point.Y, s_point.Z);            

            double distance = 0.0;
            //distance = (e_point.X - s_point.X);
            distance = new TSG.Vector(s_point - e_point).GetLength();
            point += this.BaseVector90 * distance;
            polygon.Points.Add(point);


            vectorX = ETG.Geometry2D.Rotate90(this.BaseVector) * -1;
            //distance = (e_point.Y - s_point.Y);
            point += vectorY * distance;
            polygon.Points.Add(point);

            return polygon;
        }

        private TSM.Polygon GetIntersecdSingleRebarPolygon(TSG.Point s_point, TSG.Point e_point)
        {
            TSG.Vector vectorX = new TSG.Vector(1, 0, 0);
            TSG.Vector vectorY = new TSG.Vector(this.BaseVector);
            TSG.Vector vectorZ = new TSG.Vector(0, 0, 1);

            //vectorX = ETG.Geometry2D.Rotate90(this.BaseVector) * -1;

            TSM.Polygon polygon = new TSM.Polygon();

            TSG.Point point = new TSG.Point(s_point.X, s_point.Y, s_point.Z);

            //double distance = 0.0;
            //distance = (e_point.X - s_point.X);
            //distance = new TSG.Vector(s_point - e_point).GetLength();
            //point += this.BaseVector90 * distance;
            //polygon.Points.Add(point);
            polygon.Points.Add(s_point);


            //vectorX = ETG.Geometry2D.Rotate90(this.BaseVector) * -1;
            //distance = (e_point.Y - s_point.Y);
            //point += vectorY * distance;
            //polygon.Points.Add(point);
            polygon.Points.Add(e_point);

            return polygon;
        }

        private TSM.RebarGroup CreateRebarGroup(TSM.Beam beam)
        {
            TSM.RebarGroup rebarGroup = new TSM.RebarGroup();

            rebarGroup.Name = "RebarGroup";
            rebarGroup.NumberingSeries.Prefix = "RG";
            rebarGroup.NumberingSeries.StartNumber = 1;
            rebarGroup.Class = 3;
            rebarGroup.Size = "19";
            rebarGroup.RadiusValues.Add(60.0);
            rebarGroup.Grade = "SD400";

            //rebarGroup.SpacingType = TSM.BaseRebarGroup.RebarGroupSpacingTypeEnum.SPACING_TYPE_EXACT_SPACINGS;
            //rebarGroup.Spacings.Add(200.0);
            //rebarGroup.Spacings.Add(200.0);
            //rebarGroup.Spacings.Add(200.0);
            //rebarGroup.Spacings.Add(200.0);
            //rebarGroup.Spacings.Add(200.0);
            //rebarGroup.Spacings.Add(200.0);

            rebarGroup.SpacingType = TSM.BaseRebarGroup.RebarGroupSpacingTypeEnum.SPACING_TYPE_TARGET_SPACE;
            rebarGroup.Spacings.Add(200.0);

            rebarGroup.StartHook.Shape = TSM.RebarHookData.RebarHookShapeEnum.NO_HOOK;
            rebarGroup.EndHook.Shape = TSM.RebarHookData.RebarHookShapeEnum.NO_HOOK;

            rebarGroup.StartPointOffsetType = TSM.Reinforcement.RebarOffsetTypeEnum.OFFSET_TYPE_COVER_THICKNESS;
            rebarGroup.EndPointOffsetType = TSM.Reinforcement.RebarOffsetTypeEnum.OFFSET_TYPE_COVER_THICKNESS;
            rebarGroup.StartPointOffsetValue = 40.0;
            rebarGroup.EndPointOffsetValue = 40.0;

            rebarGroup.Father = beam;

            //rebarGroup.FromPlaneOffset = 40.0;
            rebarGroup.ExcludeType = TSM.BaseRebarGroup.ExcludeTypeEnum.EXCLUDE_TYPE_NONE;

            TSM.ContourPoint point = new TSM.ContourPoint();

            rebarGroup.Polygons.Add(this.GetRebarGroupPolygon(beam, rebarGroup));

            rebarGroup.Insert();

            return rebarGroup;
        }

        private TSM.Polygon GetRebarGroupPolygon(TSM.Beam beam, TSM.RebarGroup rebarGroup)
        {
            TSG.Vector baseVector = new TSG.Vector(beam.StartPoint - beam.EndPoint).GetNormal();

            TSG.Vector vectorX = new TSG.Vector(1, 0, 0);
            TSG.Vector vectorY = new TSG.Vector(this.BaseVector);
            TSG.Vector vectorZ = new TSG.Vector(0, 0, 1);

            vectorX = ETG.Geometry2D.Rotate90(this.BaseVector) * -1;

            TSM.Polygon polygon = new TSM.Polygon();

            TSG.Point s_point = new TSG.Point(beam.StartPoint.X, beam.StartPoint.Y, beam.StartPoint.Z);
            TSG.Point e_point = new TSG.Point(beam.EndPoint.X, beam.EndPoint.Y, beam.EndPoint.Z);

            double height = 0.0;
            double width = 0.0;
            double distance = 0.0;

            string[] beamProfile = beam.Profile.ProfileString.Split('X');
            Double.TryParse(beamProfile[0], out height);
            Double.TryParse(beamProfile[1], out width);            

            distance = new TSG.Vector(e_point - s_point).GetLength();

            //distance = (beam.EndPoint.X - beam.StartPoint.X) / 2;
            s_point += vectorX * (width / 2) * -1;
            //s_point += vectorY * (height / 2) * -1;
            polygon.Points.Add(s_point);

            e_point += vectorX * ((width / 2) * -1);
            //e_point += vectorY * (height / 2) * -1;
            
            polygon.Points.Add(e_point);

            rebarGroup.StartPoint = beam.StartPoint + (vectorX * ((width / 2) * -1));
            rebarGroup.EndPoint = beam.StartPoint + (vectorX * ((width / 2)));

            return polygon;
        }

        private List<TSG.Point> GetIntersection(TSM.Beam part1, TSM.RebarGroup part2)
        {
            TSM.Solid solid = part1.GetSolid();

            //ArrayList intersect = solid.Intersect(part2.StartPoint, part2.EndPoint);

            // 기준벡터
            TSG.Vector part1Vector = new TSG.Vector(part1.EndPoint - part1.StartPoint).GetNormal();
            TSG.Vector part2Vector = new TSG.Vector(part2.EndPoint - part2.StartPoint).GetNormal();

            //TSG.Vector check1Vector = ETG.Geometry2D.Rotate90(part1Vector) * -1;

            TSG.Point point = new TSG.Point();

            //point += part1.StartPoint + (check1Vector * -20000);

            ArrayList list = new ArrayList();
            List<TSG.Point> resultList = new List<TSG.Point>();

            list = solid.Intersect(part1.StartPoint, part2.StartPoint);            

            for(int i = 0, ii = list.Count; i < ii; i++)
            {
                resultList.Add(list[i] as TSG.Point);
            }
            

            return resultList;
        }

        private void ModifyRebarPolygonByIntersection(TSM.Beam part1, TSM.RebarGroup rebarGroup)
        {
            TSM.Solid solid = part1.GetSolid();

            List<TSG.Point> resultList = new List<TSG.Point>();

            int intersectCount = 0;

            ArrayList rebarShapeList = rebarGroup.GetRebarGeometries(false);
            for (int index = 0; index < rebarShapeList.Count; index++)
            {
                TSM.RebarGeometry geometry = rebarShapeList[index] as TSM.RebarGeometry;
                TSG.Point shapePoint1 = geometry.Shape.Points[0] as TSG.Point;
                TSG.Point shapePoint2 = geometry.Shape.Points[1] as TSG.Point;

                ArrayList intersect = solid.Intersect(shapePoint1, shapePoint2);

                if (intersect.Count > 0)
                {
                    intersectCount++;

                    if (intersect.Count == 1)
                    {
                        TSG.Point intersectPoint = intersect[0] as TSG.Point;
                        this.CreateIntersecdtSingleRebar(shapePoint1, intersectPoint);
                    }
                    else if (intersect.Count == 2)
                    {
                        TSG.Point intersectPoint1 = intersect[0] as TSG.Point;
                        this.CreateIntersecdtSingleRebar(shapePoint1, intersectPoint1);

                        TSG.Point intersectPoint2 = intersect[1] as TSG.Point;
                        this.CreateIntersecdtSingleRebar(shapePoint2, intersectPoint2);
                    }
                }
            }

            if (rebarGroup.Spacings.Count - 2 <= intersectCount)
            {
                //rebarGroup.Delete();
                return;
            }

            for (int index = 0; index < intersectCount; index++)
            {
                rebarGroup.Spacings.RemoveAt(rebarGroup.Spacings.Count - 1);
            }
            rebarGroup.Modify();


            //return resultList;
        }

        private void CreateIntersecdtSingleRebar(TSG.Point s_point, TSG.Point e_Point)
        {
            TSM.SingleRebar singleRebar = new TSM.SingleRebar();

            singleRebar.Class = 2;
            singleRebar.Grade = "SD400";
            singleRebar.Name = "SingleRebar";
            //singleRebar.Father = beam;
            singleRebar.Polygon = this.GetIntersecdSingleRebarPolygon(s_point, e_Point);
            singleRebar.EndHook.Shape = TSM.RebarHookData.RebarHookShapeEnum.NO_HOOK;
            singleRebar.Size = "19";

            singleRebar.Insert();
        }

        private void Cut(TSM.Part mainPart)
        {
            TSG.Point pointS = null;
            TSG.Point pointE = null;

            if (mainPart is TSM.Beam)
            {
                TSM.Beam beam = mainPart as TSM.Beam;

                pointS = beam.StartPoint;
                pointE = beam.EndPoint;
            }

            TSG.Vector vectorX = new TSG.Vector(1, 0, 0);
            TSG.Vector vectorY = new TSG.Vector(this.BaseVector);
            TSG.Vector vectorZ = new TSG.Vector(0, 0, 1);

            

            double length = new TSG.Vector(pointS - pointE).GetLength();

            double booleanPart_w = 0.0, booleanPart_h = 0.0;

            Double.TryParse(textBox_booleanPart_width.Text, out booleanPart_w);
            Double.TryParse(textBox_booleanPart_height.Text, out booleanPart_h);

            // S만구하고 E는 높이값으로 구하도록 한다.
            //pointS += vectorZ * (booleanPart_h / 2);
            //pointS += this.BaseVector90 * (booleanPart_w / 2);

            //pointE += vectorZ * (booleanPart_h / 2);
            //pointE += this.BaseVector90 * (booleanPart_w / 2);
            //pointE += vectorY * (booleanPart_h / 2 - 1000);

            this.CutBoolen(mainPart, this.CutMember(pointS, pointE));
        }

        private TSM.Beam CutMember(TSG.Point startPoint, TSG.Point endPoint)
        {
            TSM.Beam cut = new TSM.Beam();

            TSG.Vector vectorX = new TSG.Vector(1, 0, 0);
            TSG.Vector vectorY = new TSG.Vector(this.BaseVector);
            TSG.Vector vectorZ = new TSG.Vector(0, 0, 1);

            double length = new TSG.Vector(startPoint - endPoint).GetLength();

            startPoint += vectorY * (length / 2);
            endPoint += vectorY * -1 * (length / 2 + 2000);

            cut.StartPoint = startPoint;
            cut.EndPoint = endPoint;

            cut.Name = "Column";

            //cut.Class = "2";
            cut.Material.MaterialString = "C24";

            cut.Position.Depth = TSM.Position.DepthEnum.MIDDLE;
            cut.Position.Plane = TSM.Position.PlaneEnum.MIDDLE;
            cut.Position.Rotation = TSM.Position.RotationEnum.TOP;
            //cut.Profile.ProfileString = "400X600";

            double booleanPart_w = 0.0, booleanPart_h = 0.0;

            Double.TryParse(textBox_booleanPart_width.Text, out booleanPart_w);
            Double.TryParse(textBox_booleanPart_height.Text, out booleanPart_h);


            cut.Profile.ProfileString = booleanPart_h + "X" + booleanPart_w;

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

            this.cut = cut;

            //cut.Delete();
        }
    }
}
