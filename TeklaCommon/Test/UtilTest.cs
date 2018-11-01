using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Tekla.Structures.Dialog;
using TSD = Tekla.Structures.Datatype;
using TSG = Tekla.Structures.Geometry3d;
using TSM = Tekla.Structures.Model;
using YWB = TeklaCommon.YWBoolean;
using YWC = TeklaCommon.YWCommon;
using YWCO = TeklaCommon.YWControlObject;

namespace TeklaCommon.Test
{
    public partial class UtilTest : Form
    {
        public UtilTest()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            bool isCreate = YWCO.CreateControlCircle(new TSG.Point(0, 0, 0), new TSG.Point(0, 1000, 0), new TSG.Point(1000, 0, 0), TSM.ControlCircle.ControlCircleColorEnum.BLUE);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            TSG.Point Point = new TSG.Point(0, 0, 0);
            TSG.Point Point2 = new TSG.Point(1000, 0, 0);

            TSM.Beam Beam = new TSM.Beam();
            Beam.StartPoint = Point;
            Beam.EndPoint = Point2;
            Beam.Profile.ProfileString = "600*600";
            Beam.Finish = "PAINT";

            if (Beam.Insert())
            {
                YWB.CreateCutPlane(Beam, new TSG.Point(400, 0, 0), new TSG.Vector(0, 1000, 0), new TSG.Vector(0, 0, 1000));
            }

            new TSM.Model().CommitChanges();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            TSG.Point point = new TSG.Point(0, 7000, 0);
            TSG.Point point2 = new TSG.Point(1000, 7000, 0);
            TSM.Beam Beam1 = new TSM.Beam();
            Beam1.StartPoint = point;
            Beam1.EndPoint = point2;
            Beam1.Profile.ProfileString = "800X600";
            Beam1.Insert();

            TSM.Beam Beam2 = new TSM.Beam();
            Beam2.StartPoint = new TSG.Point(500, 6000, 0);
            Beam2.EndPoint = new TSG.Point(500, 8000, 0);
            Beam2.Profile.ProfileString = "800X600";
            Beam2.Class = TSM.BooleanPart.BooleanOperativeClassName;
            Beam2.Insert();

            YWB.CreateBooleanPart(Beam1, Beam2, TSM.BooleanPart.BooleanTypeEnum.BOOLEAN_CUT);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            TSG.Point Point = new TSG.Point(0, 0, 0);
            TSG.Point Point2 = new TSG.Point(1000, 0, 0);

            TSM.Beam Beam = new TSM.Beam();
            Beam.StartPoint = Point;
            Beam.EndPoint = Point2;
            Beam.Profile.ProfileString = "400*400";
            Beam.Finish = "PAINT";
            Beam.Insert();

            TSM.Chamfer chamfer = new TSM.Chamfer(30, 50, TSM.Chamfer.ChamferTypeEnum.CHAMFER_LINE);
            YWB.CreateEdgeChamfer(Beam, "BeamEdge", new TSG.Point(0, -200, 0), new TSG.Point(0, 400, 0), chamfer, TSM.EdgeChamfer.ChamferEndTypeEnum.STRAIGHT, 0.0, TSM.EdgeChamfer.ChamferEndTypeEnum.BEVELLED, 30.0);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            TSG.Point Point = new TSG.Point(0, 0, 0);
            TSG.Point Point2 = new TSG.Point(1000, 0, 0);

            TSM.Beam Beam = new TSM.Beam();
            Beam.StartPoint = Point;
            Beam.EndPoint = Point2;
            Beam.Profile.ProfileString = "800*800";
            Beam.Finish = "PAINT";
            Beam.Insert();

            YWB.CreateFitting(Beam, new TSG.Point(500, 0, 0), new TSG.Vector(0, 1000, 0), new TSG.Vector(0, 0, -500));
        }

        private void button8_Click(object sender, EventArgs e)
        {
            TSM.Beam beam = YWPart.CreateBeam("test", "2", "C24", 800, 600, new TSG.Point(0, 2000, 0), new TSG.Point(2000, 2000, 0));
        }

        private void button9_Click(object sender, EventArgs e)
        {
            List<TSM.ContourPoint> contourPoints = new List<TSM.ContourPoint>();

            TSG.Point point = null;
            TSM.Chamfer chamfer = null;
            TSM.ContourPoint contourPoint = null;

            point = new TSG.Point(1000, 0, 0);
            contourPoint = new TSM.ContourPoint(point, chamfer);
            contourPoints.Add(contourPoint);

            point = new TSG.Point(3000, 0, 0);
            contourPoint = new TSM.ContourPoint(point, chamfer);
            contourPoints.Add(contourPoint);

            point = new TSG.Point(3000, 0, 1000);
            contourPoint = new TSM.ContourPoint(point, chamfer);
            contourPoints.Add(contourPoint);

            point = new TSG.Point(1000, 0, 1000);
            contourPoint = new TSM.ContourPoint(point, chamfer);
            contourPoints.Add(contourPoint);

            TSM.ContourPlate contourPlate = YWPart.CreateContourPlate("TestContourPlate", "3", "C24", 1000, contourPoints);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            TSM.ModelObject mo = YWC.GetPicker.PickObject(TSM.UI.Picker.PickObjectEnum.PICK_ONE_OBJECT, "철근을 생성할 부재를 선택하세요");

            if (null != mo)
            {
                TSM.Polygon polygon = new TSM.Polygon();

                if (mo is TSM.Part)
                {
                    TSM.Part part = mo as TSM.Part;

                    if (part is TSM.ContourPlate)
                    {
                        TSM.ContourPlate contourPlate = part as TSM.ContourPlate;
                        polygon.Points.Add(contourPlate.Contour.ContourPoints[0]);
                        polygon.Points.Add(contourPlate.Contour.ContourPoints[1]);

                        string usage = YWC.GetDescription(YWC.eUsage.tie_stirrup);
                        YWReinforcement.CreateSingleRebar("SD400", "19", usage, "mainRebar", 3, mo, polygon);
                    }
                }
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            TSM.ModelObject mo = YWC.GetPicker.PickObject(TSM.UI.Picker.PickObjectEnum.PICK_ONE_OBJECT, "철근그룹을 생성할 부재를 선택하세요");

            if (null != mo)
            {
                List<TSM.Polygon> polygons = new List<TSM.Polygon>();
                TSM.Polygon polygon = new TSM.Polygon();

                if (mo is TSM.Part)
                {
                    TSM.Part part = mo as TSM.Part;

                    if (part is TSM.ContourPlate)
                    {
                        TSM.ContourPlate contourPlate = part as TSM.ContourPlate;
                        polygon.Points.Add(contourPlate.Contour.ContourPoints[0]);
                        polygon.Points.Add(contourPlate.Contour.ContourPoints[1]);

                        polygons.Add(polygon);

                        TSD.DistanceList spaceList = TSD.DistanceList.Parse("200 50");

                        string usage = YWC.GetDescription(YWC.eUsage.tie_stirrup);
                        YWReinforcement.CreateSingleRebar("SD400", "19", usage, "mainRebar", 3, mo, polygon);
                        usage = YWC.GetDescription(YWC.eUsage.main);
                        YWReinforcement.CreateRebarGroup("SD400", "19", usage, "mainRebar", 2, mo, polygons, contourPlate.Contour.ContourPoints[0] as TSG.Point, contourPlate.Contour.ContourPoints[3] as TSG.Point, spaceList);
                    }
                }
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            try
            {
                if (!YWC.GetModel.GetConnectionStatus()) throw new Exception("Tekla Structures와 연결되지 않았습니다.");

                TSM.ModelObject mo1 = YWC.GetPicker.PickObject(TSM.UI.Picker.PickObjectEnum.PICK_ONE_OBJECT, "간섭체크 할 첫 번째 객체를 선택하세요.");
                TSM.ModelObject mo2 = YWC.GetPicker.PickObject(TSM.UI.Picker.PickObjectEnum.PICK_ONE_OBJECT, "간섭체크 할 두 번째 객체를 선택하세요.");

                TSG.AABB aabb;
                YWC.IsClashCheck(mo1.Identifier, mo2.Identifier, out aabb);
            }
            catch (Exception ex)
            {
                ErrorDialog.Show(ex, ErrorDialog.Severity.ERROR);
            }
            finally
            {
            }
        }
    }
}