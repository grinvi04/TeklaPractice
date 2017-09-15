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
using Tekla.Structures.Model;

namespace Tekla_Practice
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TSM.Model model = new TSM.Model();

            if(model.GetConnectionStatus()== false)
            {
                return;
            }

            //TSM.Beam beam = this.CreateBeam();
            TSM.ContourPlate plate = this.CreateContourPlate();

            //this.CreateSingleRebar(beam, plate);
            //this.CreatePolyBeam();

            //TSM.Beam beam2 = this.CreateBeamRebarGroup();
            //this.CreateRebarGroup(beam2);


            model.CommitChanges();
        }

        private void CreateSingleRebar(TSM.Beam beam, TSM.ContourPlate plate)
        {
            TSM.SingleRebar rebar = new TSM.SingleRebar();
            rebar.Class = 2;      
           
            rebar.Grade = "SD400";
            rebar.Name = "singleRebar";

            rebar.EndHook.Shape = TSM.RebarHookData.RebarHookShapeEnum.CUSTOM_HOOK;
            rebar.StartHook.Shape = TSM.RebarHookData.RebarHookShapeEnum.NO_HOOK;
            rebar.Father = beam;
            rebar.Polygon = this.GetRebarPolygon();
            rebar.Size = "19";
            rebar.OnPlaneOffsets.Add(0.0);
            rebar.FromPlaneOffset = 0;
            rebar.StartPointOffsetValue = 0;
            rebar.EndPointOffsetValue = 0;
            bool isInsert =  rebar.Insert();
        }

        private TSM.Beam CreateBeamRebarGroup()
        {
            TSG.Point startPoint = new TSG.Point(15000, 10000, 0);
            TSG.Point endPoiunt = new TSG.Point(25000, 10000, 0);


            TSM.Beam column = new TSM.Beam();
            column.Name = "Girder";
            column.StartPoint = startPoint;
            column.EndPoint = endPoiunt;

            column.Class = "6";
            column.Material.MaterialString = "C24";
            column.Finish = "G1";

            column.Position.Depth = TSM.Position.DepthEnum.MIDDLE;
            column.Position.Plane = TSM.Position.PlaneEnum.MIDDLE;
            column.Position.Rotation = TSM.Position.RotationEnum.FRONT;
            column.Profile.ProfileString = "800X600";
            bool isInsert = column.Insert();

            return column;
        }

        private void CreateRebarGroup(TSM.Beam beam)
        {
            TSM.RebarGroup rebarGroup = new TSM.RebarGroup();

            rebarGroup.Name = "rebarGroup";
            rebarGroup.NumberingSeries.Prefix = "S";
            rebarGroup.NumberingSeries.StartNumber = 1;
            //요기
            rebarGroup.StartPoint = beam.StartPoint;
            rebarGroup.EndPoint = beam.EndPoint;
            rebarGroup.Class = 3;
            rebarGroup.Size = "19";
            rebarGroup.RadiusValues.Add(60.0);
            rebarGroup.Grade = "SD400";

            rebarGroup.SpacingType = RebarGroup.RebarGroupSpacingTypeEnum.SPACING_TYPE_TARGET_SPACE;
            rebarGroup.Spacings.Add(200.0);

            rebarGroup.StartHook.Shape = TSM.RebarHookData.RebarHookShapeEnum.NO_HOOK;
            rebarGroup.EndHook.Shape = TSM.RebarHookData.RebarHookShapeEnum.NO_HOOK;

            rebarGroup.StartPointOffsetType = TSM.Reinforcement.RebarOffsetTypeEnum.OFFSET_TYPE_COVER_THICKNESS;
            rebarGroup.EndPointOffsetType = TSM.Reinforcement.RebarOffsetTypeEnum.OFFSET_TYPE_COVER_THICKNESS;

            rebarGroup.StartPointOffsetValue = 40.0;
            rebarGroup.EndPointOffsetValue = 40.0;

            rebarGroup.Father = beam;

            rebarGroup.FromPlaneOffset = 40.0;
            rebarGroup.ExcludeType = BaseRebarGroup.ExcludeTypeEnum.EXCLUDE_TYPE_NONE;


            TSG.Vector baseVector = new TSG.Vector(beam.EndPoint - beam.StartPoint).GetNormal();

            String[] profileStringArray = beam.Profile.ProfileString.Split('X');

            double beamHeight, beamWidth;

            Double.TryParse(profileStringArray[0], out beamHeight);
            Double.TryParse(profileStringArray[1], out beamWidth);

            TSG.Vector vector90 = ETG.Geometry2D.Rotate90(baseVector);
            TSG.Vector vectorZ = new TSG.Vector(0, 0, 1);

            TSG.Point s_point = beam.StartPoint;
            TSG.Point e_point = beam.EndPoint;

            TSM.Polygon polygon = new TSM.Polygon();

            TSG.Point point1 = s_point + vector90 * beamWidth * -0.5;
            point1 += vectorZ * beamHeight * 0.5;
            polygon.Points.Add(point1);

            TSG.Point point2 = s_point + vector90 * beamWidth * -0.5;
            point2 += vectorZ * beamHeight * -0.5;
            polygon.Points.Add(point2);

            TSG.Point point3 = s_point + vector90 * beamWidth * 0.5;
            point3 += vectorZ * beamHeight * -0.5;
            polygon.Points.Add(point3);

            TSG.Point point4 = s_point + vector90 * beamWidth * 0.5;
            point4 += vectorZ * beamHeight * 0.5;
            polygon.Points.Add(point4);
            
            rebarGroup.Polygons.Add(polygon);

            bool bInsert = rebarGroup.Insert();
        }

        private void CreatePolyBeam()
        {
            TSM.PolyBeam polybeam = new TSM.PolyBeam();

            polybeam.Class = "1";
            polybeam.Name = "Beam";
            polybeam.Profile.ProfileString = "H200X100X5.5X8";
            polybeam.Material.MaterialString = "SS400";
            polybeam.Contour.ContourPoints.AddRange(this.GetPolybeamPolygon());

            polybeam.Insert();

        }

        private ArrayList GetRebarGroupPolygon()
        {
            TSG.Vector vectorX = new TSG.Vector(1, 0, 0);
            TSG.Vector vectorY = new TSG.Vector(0, 1, 0);
            TSG.Vector vectorZ = new TSG.Vector(0, 0, 1);

            TSG.Point point = new TSG.Point(30000, 20000, 0);

            ArrayList polygon = new ArrayList();
            polygon.Add(new TSM.ContourPoint(point, null));

            point += vectorX * 1000;
            polygon.Add(new TSM.ContourPoint(point, null));

            point += vectorY * 1000;
            polygon.Add(new TSM.ContourPoint(point, null));

            point -= vectorX * 1000;

            point += vectorX * 1000;
            point += vectorY * 1000;
            polygon.Add(new TSM.ContourPoint(point, null));

            return polygon;
        }

        private ArrayList GetPolybeamPolygon()
        {
            TSG.Vector vectorX = new TSG.Vector(1, 0, 0);
            TSG.Vector vectorY = new TSG.Vector(0, 1, 0);
            TSG.Vector vectorZ = new TSG.Vector(0, 0, 1);

            TSG.Point point = new TSG.Point(30000, 20000, 0);

            ArrayList polygon = new ArrayList();
            polygon.Add(new TSM.ContourPoint(point, null));
  
            point += vectorX * 1000;
            polygon.Add(new TSM.ContourPoint(point, null));

            point += vectorY * 1000;
            polygon.Add(new TSM.ContourPoint(point, null));

            point -= vectorX * 1000;
            polygon.Add(new TSM.ContourPoint(point, new TSM.Chamfer(0, 0, TSM.Chamfer.ChamferTypeEnum.CHAMFER_ARC_POINT)));

            point += vectorX * 1000;
            point += vectorY * 1000;
            polygon.Add(new TSM.ContourPoint(point, null));

            return polygon;
        }


        private TSM.ContourPlate CreateContourPlate()
        {
            ArrayList polygon = this.GetPolygon();

            TSM.ContourPlate plate = new TSM.ContourPlate();
            plate.Class = "4";
            plate.Contour.ContourPoints.AddRange(polygon);
            plate.Material.MaterialString = "C40";
            plate.Name = "Slab";
            plate.Profile.ProfileString = "10000";
            plate.Position.Depth = TSM.Position.DepthEnum.FRONT;
            plate.Insert();

            return plate;

        }

        private TSM.Polygon GetRebarPolygon()
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

            polygon.Points.Add(new TSG.Point(1000, 1000, 1000));
            polygon.Points.Add(new TSG.Point(2000, 1000, 1000));
            polygon.Points.Add(new TSG.Point(2000, 2000, 1000));
            polygon.Points.Add(new TSG.Point(1000, 2000, 1000));
            polygon.Points.Add(new TSG.Point(1000, 3000, 1000));

            //polygon.Points.Add(new TSM.ContourPoint(new TSG.Point(1000, 3000, 1000), ));

            return polygon;
        }

        private ArrayList GetPolygon()
        {
            TSG.Vector vectorX = new TSG.Vector(1, 0, 0);
            TSG.Vector vectorY = new TSG.Vector(0, 1, 0);
            TSG.Vector vectorZ = new TSG.Vector(0, 0, 1);

            TSG.Point point = new TSG.Point(10000, 10000, 1000);

            ArrayList polygon = new ArrayList();
            polygon.Add(new TSM.ContourPoint(point, new TSM.Chamfer(10,0, TSM.Chamfer.ChamferTypeEnum.CHAMFER_ROUNDING)));

  
            point += vectorX * 1000;
            polygon.Add(new TSM.ContourPoint(point, null));

            point += vectorX * 200;
            point -= vectorZ * 1000;
            polygon.Add(new TSM.ContourPoint(point, null));

            point += vectorX * 800;
            polygon.Add(new TSM.ContourPoint(point, null));

            point += vectorX * 200;            
            point += vectorZ * 1000;
            polygon.Add(new TSM.ContourPoint(point, null));

            point += vectorX * 1000;
            polygon.Add(new TSM.ContourPoint(point, null));

            point += vectorX * 200;
            point -= vectorZ * 1000;
            polygon.Add(new TSM.ContourPoint(point, null));

            point += vectorX * 800;
            polygon.Add(new TSM.ContourPoint(point, null));

            point += vectorX * 200;            
            point += vectorZ * 1000;
            polygon.Add(new TSM.ContourPoint(point, null));

            point += vectorX * 1000;
            polygon.Add(new TSM.ContourPoint(point, null));

            point += vectorZ * 300;
            polygon.Add(new TSM.ContourPoint(point, null));

            point -= vectorX * 5400;
            polygon.Add(new TSM.ContourPoint(point, null));

            return polygon;
        }

        private TSM.Beam CreateBeam()
        {
            TSG.Point startPoint = new TSG.Point(0, 0, 0);
            TSG.Point endPoiunt = new TSG.Point(0, 0, 1000);


            TSM.Beam column = new TSM.Beam();
            column.Name = "Column";
            column.StartPoint = startPoint;
            column.EndPoint = endPoiunt;

            column.Class = "2";
            column.Material.MaterialString = "C40";

            column.Position.Depth = TSM.Position.DepthEnum.MIDDLE;
            column.Position.Plane = TSM.Position.PlaneEnum.MIDDLE;
            column.Position.Rotation = TSM.Position.RotationEnum.TOP;
            column.Profile.ProfileString = "D1000";
            bool isInsert = column.Insert();

            return column;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            TSM.UI.ModelObjectSelector mos = new TSM.UI.ModelObjectSelector();

            TSM.ModelObjectEnumerator moe = mos.GetSelectedObjects();

            while(moe.MoveNext())
            {
                if(moe.Current.GetType() == typeof(RebarGroup))
                {
                    ModelObject a = moe.Current;
                }
                
            }
        }
    }
}
