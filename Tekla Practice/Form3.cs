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
    public partial class Form3 : Form
    {
        private TSG.Point PickStartPoint;
        private TSG.Point PickEndPoint;

        public Form3()
        {
            InitializeComponent();

            this.PickStartPoint = new TSG.Point();
            this.PickEndPoint = new TSG.Point();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TSM.Model model = new TSM.Model();

            if (!model.GetConnectionStatus()) return;

            TSM.UI.Picker picker = new TSM.UI.Picker();
            //this.PickStartPoint = picker.PickPoint();
            //this.PickEndPoint = picker.PickPoint();

            TSM.Beam part1 = picker.PickObject(TSM.UI.Picker.PickObjectEnum.PICK_ONE_PART) as TSM.Beam;
            TSM.Beam part2 = picker.PickObject(TSM.UI.Picker.PickObjectEnum.PICK_ONE_PART) as TSM.Beam;

            //GetIntersection(part1, part2);
            string result = GetSolidIntersect(part1, part2);

            label2.Text = result;

            model.CommitChanges();
        }

        private List<TSG.Point> GetIntersection(TSM.Beam part1, TSM.Beam part2)
        {
            TSM.Solid solid = part2.GetSolid();

            //ArrayList intersect = solid.Intersect(part2.StartPoint, part2.EndPoint);

            // 기준벡터
            TSG.Vector part1Vector = new TSG.Vector(part1.EndPoint - part1.StartPoint).GetNormal();
            TSG.Vector part2Vector = new TSG.Vector(part2.EndPoint - part2.StartPoint).GetNormal();

            TSG.Vector check1Vector = new TSG.Vector(-part1Vector.Y, part1Vector.X, part1Vector.Z) * -1;//ETG.Geometry2D.Rotate90(part1Vector) * -1;

            TSG.Point point = new TSG.Point();

            point += part1.StartPoint + (check1Vector * -20000);

            TSG.Line line1 = new TSG.Line(part1.StartPoint, point);
            TSG.Line line2 = new TSG.Line(part2.StartPoint, part2.EndPoint);

            TSG.LineSegment a = TSG.Intersection.LineToLine(line1, line2);

            // a값이 있으면 교차점이 존재
            // 왼쪽, 오른쪽은 기준벡터 방향으로 체크
            // Line은 선이 무한정이라 왼쪽, 오른쪽 체크 기준 다시 확인 필요



            //무한한 선
            //TSG.Line

            //범위가 있는 선
            //TSG.LineSegment

            //교차점 찾아오는것
            //TSG.Intersection

            //간섭 찾아오는 것
            //solid.Intersect            


            //TSG.LineSegment segment = new TSG.LineSegment(pointS, pointE);
            //ArrayList intersect = solid.Intersect(segment);

            //List<TSG.Point> pointList = new List<TSG.Point>();

            //for (int index = 0; index < intersect.Count; index++)
            //{

            //}
            return null;        
        }

        private string GetSolidIntersect(TSM.Beam part1, TSM.Beam part2)
        {
            TSM.Solid solid = part2.GetSolid();

            //ArrayList intersect = solid.Intersect(part2.StartPoint, part2.EndPoint);

            // 기준벡터
            TSG.Vector part1Vector = new TSG.Vector(part1.EndPoint - part1.StartPoint).GetNormal();
            TSG.Vector part2Vector = new TSG.Vector(part2.EndPoint - part2.StartPoint).GetNormal();

            TSG.Vector check1Vector = new TSG.Vector(-part1Vector.Y, part1Vector.X, part1Vector.Z) * -1;//ETG.Geometry2D.Rotate90(part1Vector) * -1;

            TSG.Point point = new TSG.Point();
            ArrayList list = new ArrayList();

            point += part1.StartPoint + (check1Vector * -20000);
            list = solid.Intersect(part1.StartPoint, point);

            if(list.Count > 0)
            {
                return "왼쪽";

            }
            else
            {
                //오른쪽
                check1Vector = check1Vector * -1;
                point = new TSG.Point();

                point += part1.StartPoint + (check1Vector * -20000);
                list = solid.Intersect(part1.StartPoint, point);

                if(list.Count > 0)
                {
                    return "오른쪽";
                }
                else
                {
                    return "왼쪽, 오른쪽에 없음";
                }
            }
        }

        private TSM.Beam CreateBeam(TSG.Point s_point, TSG.Point e_Point)
        {
            TSG.Point startPoint = s_point;
            TSG.Point endPoiunt = e_Point;


            TSM.Beam column = new TSM.Beam();
            column.Name = "Column";
            column.StartPoint = startPoint;
            column.EndPoint = endPoiunt;

            column.Class = "3";
            column.Material.MaterialString = "SS400";

            column.Position.Depth = TSM.Position.DepthEnum.MIDDLE;
            column.Position.Plane = TSM.Position.PlaneEnum.MIDDLE;
            column.Position.Rotation = TSM.Position.RotationEnum.TOP;
            column.Profile.ProfileString = "H200X100X5.5X8";
            bool isInsert = column.Insert();

            return column;
        }

        private void RebarGroupEachPolygonShape()
        {
            TSM.RebarGroup rebar = new TSM.RebarGroup();

            ArrayList array = rebar.GetRebarGeometries(false)
;        }
    }
}
