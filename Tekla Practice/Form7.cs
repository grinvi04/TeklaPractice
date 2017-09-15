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
using System.Collections;

namespace Tekla_Practice
{
    public partial class Form7 : Form
    {
        private TSM.Beam column;
        private List<TSM.Beam> girderList;

        TSM.Model model;

        public Form7()
        {
            // column과 girder 연결부분에 singleRebar 추가하는 클래스
            InitializeComponent();

            this.model = new TSM.Model();
            if (!this.model.GetConnectionStatus()) return;
        }

        private void button_selectColumn_Click(object sender, EventArgs e)
        {
            // Column 선택(1개만)
            TSM.UI.Picker picker = new TSM.UI.Picker();

            this.column = picker.PickObject(TSM.UI.Picker.PickObjectEnum.PICK_ONE_PART, "column을 선택하세요") as TSM.Beam;

            if (null == this.column)
            {
                MessageBox.Show("선택한 column이 없습니다.");
                return;
            }
        }

        private void button_selectGirder_Click(object sender, EventArgs e)
        {
            this.girderList = new List<TSM.Beam>();
            // Girder 선택(최대 4개)
            TSM.UI.Picker picker = new TSM.UI.Picker();

            TSM.ModelObjectEnumerator moe = picker.PickObjects(TSM.UI.Picker.PickObjectsEnum.PICK_N_OBJECTS, "girder을 선택하세요");

            if (moe.GetSize() < 1)
            {
                MessageBox.Show("선택한 girder가 없습니다.");
                return;
            }

            if (moe.GetSize() > 4)
            {
                MessageBox.Show("최대 4개만 선택하세요");
                return;
            }

            while (moe.MoveNext())
            {
                if (moe.Current.GetType() == typeof(TSM.Beam))
                {
                    TSM.Beam a = moe.Current as TSM.Beam;

                    if(null != a)
                    {
                        girderList.Add(a);                        
                    }
                }
            }
        }

        private void button_directionCheck_Click(object sender, EventArgs e)
        {
            List<TSG.Point> list = this.GetIntersection(this.column, this.girderList);

            // model 반영
            model.CommitChanges();
        }

        private List<TSG.Point> GetIntersection(TSM.Beam column, List<TSM.Beam> girderList)
        {
            List<TSG.Point> resultList = new List<TSG.Point>();

            // 선택한 column과 girder의 교차점을 구한다.
            foreach (var item in girderList)
            {
                TSG.LineSegment line = new TSG.LineSegment(item.StartPoint, item.EndPoint);
                TSM.Solid columnSolid = column.GetSolid();

                ArrayList intersectList = columnSolid.Intersect(line);

                // girder startPoint, endPoint 기준으로 겹치는 부분이 없을 경우
                // startPoint, endPoint의 Z를 column의 startPoint, endPoint의 z로 바꿔서 체크
                // 그래도 없으면 겹치는 부분 없음
                if (intersectList.Count == 0)
                {
                    item.StartPoint.Z = column.EndPoint.Z;
                    item.EndPoint.Z = column.EndPoint.Z;

                    line = new TSG.LineSegment(item.StartPoint, item.EndPoint);
                    intersectList = columnSolid.Intersect(line);

                    if (intersectList.Count == 0)
                    {
                        item.StartPoint.Z = column.StartPoint.Z;
                        item.EndPoint.Z = column.StartPoint.Z;

                        line = new TSG.LineSegment(item.StartPoint, item.EndPoint);
                        intersectList = columnSolid.Intersect(line);

                        if(intersectList.Count == 0)
                        {
                            MessageBox.Show(girderList.IndexOf(item)+1 + "번째 거더는 겹치는 부분이 없습니다.");
                            continue;
                        }
                    }
                }

                // 교차점 갯수가 0이면 에러, 2이면 girder가 column의 윗면을 완전히 덮은 상태이므로 에러
                if (intersectList.Count > 0)
                {
                    if(intersectList.Count == 1)
                    {
                        TSG.Point intersectPoint = intersectList[0] as TSG.Point;

                        TSG.Point startPoint = item.StartPoint;
                        TSG.Point endPoint = item.EndPoint;
                        
                        double length = 0.0;

                        Double.TryParse(textBox_singleRebar_length.Text, out length);
                        length = length / 2;

                        TSG.Point s_point = new TSG.Point();
                        TSG.Point e_point = new TSG.Point();

                        // column의 startPoint -> 교차점, endPoint -> 교차점의 vector, length
                        TSG.Vector columnToStartVector = new TSG.Vector(this.column.StartPoint - intersectPoint).GetNormal();
                        double columnToStartLength = new TSG.Vector(this.column.StartPoint - intersectPoint).GetLength();
                        TSG.Vector columnToEndVector = new TSG.Vector(this.column.EndPoint - intersectPoint).GetNormal();
                        double columnToEndLength = new TSG.Vector(this.column.EndPoint - intersectPoint).GetLength();

                        bool isStart = false;
                        bool isEnd = false;

                        int compare = columnToStartLength.CompareTo(columnToEndLength);

                        if(compare < 0)
                        {
                            isStart = true;
                        } else if(compare > 0)
                        {
                            isEnd = true;
                        } else
                        {
                            isEnd = true;
                        }


                        // startPoint -> 교차점, endPoint -> 교차점의 vector, length
                        TSG.Vector intersectToStartVector = new TSG.Vector(startPoint - intersectPoint).GetNormal();
                        double intersectToStartLength = new TSG.Vector(startPoint - intersectPoint).GetLength();
                        TSG.Vector intersectToEndVector = new TSG.Vector(endPoint - intersectPoint).GetNormal();
                        double intersectToEndLength = new TSG.Vector(endPoint - intersectPoint).GetLength();

                        TSG.Vector baseVector = null;

                        // intersectToStartLength가 0이거나 intersectToEndLength가 0이면 교차점과 startPoint 또는 endPoint가 일치하므로
                        // 교차점과 startPoint 또는 endPoint의 vector가 0이므로 vector가 0이되는 startPoint(endPoint)와 endPoint(startPoint)의 vector을 구한다.
                        if (intersectToStartLength == 0)
                        {
                            baseVector = new TSG.Vector(endPoint - startPoint).GetNormal();
                            s_point += intersectPoint + (baseVector * length);
                            e_point += intersectPoint + (baseVector * length * -1);

                            if (baseVector.X <= 0 && baseVector.Y <= 0 && baseVector.Z <= 0)
                            {
                                if (isStart)
                                    this.CreateSingleRebar(this.column, s_point, e_point, length, "S");
                                else
                                    this.CreateSingleRebar(this.column, s_point, e_point, length, "E");
                            }                            
                            else
                            {
                                if (isStart)
                                    this.CreateSingleRebar(this.column, s_point, e_point, length, "S");
                                else
                                    this.CreateSingleRebar(this.column, s_point, e_point, length, "E");
                            }

                            continue;
                        }

                        if (intersectToEndLength == 0)
                        {
                            baseVector = new TSG.Vector(startPoint - endPoint).GetNormal();
                            s_point += intersectPoint + (baseVector * length);
                            e_point += intersectPoint + (baseVector * length * -1);

                            if(baseVector.X <= 0 && baseVector.Y <= 0 && baseVector.Z <= 0)
                            {
                                if(isEnd)
                                    this.CreateSingleRebar(this.column, s_point, e_point, length, "E");
                                else
                                    this.CreateSingleRebar(this.column, s_point, e_point, length, "S");
                            }
                            else
                            {
                                if (isEnd)
                                    this.CreateSingleRebar(this.column, s_point, e_point, length, "E");
                                else
                                    this.CreateSingleRebar(this.column, s_point, e_point, length, "S");
                            }

                            continue;
                        }

                        int compare2 = intersectToStartLength.CompareTo(intersectToEndLength);

                        // 교차점에서 startPoint가 더 가깝다
                        if (compare2 < 0)
                        {
                            baseVector = intersectToStartVector;
                            s_point += intersectPoint + (baseVector * length * -1);
                            e_point += intersectPoint + (baseVector * length);

                            if (baseVector.X <= 0 && baseVector.Y <= 0 && baseVector.Z <= 0)
                            {
                                if (isStart)
                                    this.CreateSingleRebar(this.column, s_point, e_point, length, "S");
                                else
                                    this.CreateSingleRebar(this.column, s_point, e_point, length, "E");
                            }
                            else
                            {
                                if (isStart)
                                    this.CreateSingleRebar(this.column, s_point, e_point, length, "S");
                                else
                                    this.CreateSingleRebar(this.column, s_point, e_point, length, "E");
                            }

                            continue;
                        }

                        if (compare2 > 0)
                        {
                            baseVector = intersectToEndVector;
                            s_point += intersectPoint + (baseVector * length * -1);
                            e_point += intersectPoint + (baseVector * length);

                            if (baseVector.X <= 0 && baseVector.Y <= 0 && baseVector.Z <= 0)
                            {
                                if (isEnd)
                                    this.CreateSingleRebar(this.column, s_point, e_point, length, "E");
                                else
                                    this.CreateSingleRebar(this.column, s_point, e_point, length, "S");
                            }
                            else
                            {
                                if (isEnd)
                                    this.CreateSingleRebar(this.column, s_point, e_point, length, "E");
                                else
                                    this.CreateSingleRebar(this.column, s_point, e_point, length, "S");
                            }

                            continue;
                        }
                    }
                }
            }

            return resultList;
        }

        private void CreateSingleRebar(TSM.Beam beam, TSG.Point startPoint, TSG.Point endPoint, double length, string locate)
        {
            TSM.SingleRebar rebar = new TSM.SingleRebar();
            rebar.Class = 2;

            rebar.Grade = "SD400";
            rebar.Name = "singleRebar";

            rebar.StartHook.Shape = TSM.RebarHookData.RebarHookShapeEnum.NO_HOOK;

            // hook은 방향을 자유자재로 조절이 안되므로 3개 point로 꺽은 선 모양을 그린다.
            //rebar.EndHook.Shape = TSM.RebarHookData.RebarHookShapeEnum.CUSTOM_HOOK;
            //TSG.Vector direction = new TSG.Vector(startPoint - endPoint).GetNormal();
            //rebar.EndHook.Angle = isMinusAngle ? -90.0 : 90.0;
            //rebar.EndHook.Radius = 90.0;
            //rebar.EndHook.Length = 500.0;
            
            rebar.Father = beam;
            rebar.Polygon = this.GetSingleRebarPolygon(startPoint, endPoint, length, locate);
            rebar.Size = "19";
            rebar.OnPlaneOffsets.Add(0.0);
            rebar.FromPlaneOffset = 0;
            rebar.StartPointOffsetValue = 0;
            rebar.EndPointOffsetValue = 0;
            bool isInsert = rebar.Insert();
        }

        private TSM.Polygon GetSingleRebarPolygon(TSG.Point startPoint, TSG.Point endPoint, double length, string locate)
        {
            TSG.Vector vectorX = new TSG.Vector(1, 0, 0);
            TSG.Vector vectorY = new TSG.Vector(0, 1, 0);
            TSG.Vector vectorZ = new TSG.Vector(0, 0, 1);

            if ("S".Equals(locate)) vectorZ *= 1;
            if ("E".Equals(locate)) vectorZ *= -1;

            TSM.Polygon polygon = new TSM.Polygon();

            polygon.Points.Add(startPoint);
            polygon.Points.Add(endPoint);

            endPoint += vectorZ * length * 2;
            polygon.Points.Add(endPoint);

            return polygon;
        }
    }
}
