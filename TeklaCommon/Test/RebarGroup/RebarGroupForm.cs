using System.Windows.Forms;
using TSM = Tekla.Structures.Model;
using TSG = Tekla.Structures.Geometry3d;
using TSD = Tekla.Structures.Datatype;
using TSS = Tekla.Structures.Solid;
using TC = TeklaCommon.Common;
using System;
using System.Collections.Generic;
using System.Collections;

namespace TeklaCommon.Test
{
    public partial class RebarGroupForm : Form
    {
        private TSM.Model _Model = new TSM.Model();
        private TSM.UI.Picker _Picker = new TSM.UI.Picker();

        public RebarGroupForm()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            
        }

        private void SetItemsComboBox(ComboBox comboBox, string[] names)
        {
            comboBox.Items.AddRange(names);
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            //TSG.Vector v1 = new TSG.Vector(0, 0, 0);
            //TSG.Vector v2 = TC.Common.Rotate(v1, 45);
            //TSG.Vector v3 = TC.Common.Rotate(v1, -45);
            //TSG.Vector v4 = TC.Common.Rotate(v1, 90);
            //TSG.Vector v5 = TC.Common.Rotate(v1, -90);

            // global workplane 선언
            TSM.TransformationPlane transformationPlaneCurrent = _Model.GetWorkPlaneHandler().GetCurrentTransformationPlane();

            try
            {
                if (!_Model.GetConnectionStatus()) throw new Exception("Tekla Structures와 연결되지 않았습니다.");

                TSM.ModelObject mo = _Picker.PickObject(TSM.UI.Picker.PickObjectEnum.PICK_ONE_OBJECT, "철근을 생성할 부재를 선택하세요");

                if (null == mo)
                {
                    MessageBox.Show("부재가 선택되지 않았습니다.\r\n부재를 선택해주세요.");
                    return;
                }

                if (null != mo)
                {
                    TSM.Part part = mo as TSM.Part;

                    // 철근그룹의 시작, 끝 포인트
                    TSG.Point startPoint = new TSG.Point();
                    TSG.Point endPoint = new TSG.Point();

                    // 철근그룹의 형상 포인트
                    List<TSM.Polygon> polygons = new List<TSM.Polygon>();
                    TSM.Polygon polygon = new TSM.Polygon();

                    // 철근그룹 간격
                    TSD.DistanceList distances = new TSD.DistanceList();

                    // 자동일 경우(첫번째는 fromPlaneOffset 값으로 들어감)
                    distances.Add(new TSD.Distance(100));
                    distances.Add(new TSD.Distance(250));
                    TSM.BaseRebarGroup.RebarGroupSpacingTypeEnum rebarGroupSpacingType = TSM.BaseRebarGroup.RebarGroupSpacingTypeEnum.SPACING_TYPE_EXACT_SPACE_FLEX_AT_BOTH;
                    // 수동일 경우
                    //distances = TSD.DistanceList.Parse("3*200 400 5*150");
                    //TSM.BaseRebarGroup.RebarGroupSpacingTypeEnum rebarGroupSpacingType = TSM.BaseRebarGroup.RebarGroupSpacingTypeEnum.SPACING_TYPE_EXACT_SPACINGS;

                    // 철근그룹 평면
                    ArrayList onPlaneOffset = new ArrayList();

                    // 부재의 local coordinate로 변환
                    TSG.CoordinateSystem coordinateSystem = part.GetCoordinateSystem();
                    // model에 workplane 설정
                    _Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(coordinateSystem));

                    if (part is TSM.Beam)
                    {
                        TSM.Beam beam = part as TSM.Beam;

                        TSG.OBB obb = TC.Common.CreateOrientedBoundingBox(_Model, part);

                        startPoint = obb.Center - (obb.Axis0 * obb.Extent0);
                        endPoint = obb.Center + (obb.Axis0 * obb.Extent0);

                        polygon.Points.Add(startPoint + (obb.Axis2 * obb.Extent2));
                        polygon.Points.Add(startPoint - (obb.Axis2 * obb.Extent2));
                        polygons.Add(polygon);

                    }
                    else if (part is TSM.PolyBeam)
                    {

                    }

                    TC.Common.CreateRebarGroup("SD400", "19", "main", "mainRebar", 3, part, 1, "m", startPoint, endPoint, polygons, onPlaneOffset, distances, rebarGroupSpacingType);

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                _Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(transformationPlaneCurrent);
                _Model.CommitChanges();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (!_Model.GetConnectionStatus()) throw new Exception("Tekla Structures와 연결되지 않았습니다.");

                TSM.ModelObject mo = _Picker.PickObject(TSM.UI.Picker.PickObjectEnum.PICK_ONE_OBJECT, "철근간섭을 체크할 부재를 선택하세요");

                TSM.ModelObjectEnumerator moe = _Picker.PickObjects(TSM.UI.Picker.PickObjectsEnum.PICK_N_OBJECTS, "선택한 부재와 간섭된 부재를 선택하세요");

                if(null == mo)
                {
                    MessageBox.Show("선택된 부재가 없습니다.");
                    return;
                }

                if (null == moe)
                {
                    MessageBox.Show("선택된 부재가 없습니다.");
                    return;
                }

                TSM.Part part = mo as TSM.Part;

                // 철근이 속한 부재
                if(part is TSM.Beam)
                {
                    TSM.Beam beam = part as TSM.Beam;
                    
                    Dictionary<int, ArrayList> dic = new Dictionary<int, ArrayList>();

                    TSM.ModelObjectEnumerator childrenEnumerator = beam.GetChildren();

                    while (childrenEnumerator.MoveNext())
                    {
                        TSM.ModelObject children = childrenEnumerator.Current;

                        // 부재의 자식 개체가 철근그룹이면
                        if (children is TSM.RebarGroup)
                        {
                            TSM.RebarGroup rebarGroup = children as TSM.RebarGroup;
                            ArrayList rebarGeometries = rebarGroup.GetRebarGeometries(false);

                            while (moe.MoveNext())
                            {
                                TSM.ModelObject modelObject = moe.Current;
                                TSM.Part otherPart = modelObject as TSM.Part;

                                TSG.OBB obb = TC.Common.CreateOrientedBoundingBox(_Model, otherPart);

                                for (int i = 0; i < rebarGeometries.Count; i++)
                                {
                                    TSM.RebarGeometry rebarGeometry = rebarGeometries[i] as TSM.RebarGeometry;
                                    TSG.Point point1 = rebarGeometry.Shape.Points[0] as TSG.Point;
                                    TSG.Point point2 = rebarGeometry.Shape.Points[1] as TSG.Point;
                                    TSG.LineSegment lineSegment = new TSG.LineSegment(point1, point2);

                                    bool isIntersect = obb.Intersects(lineSegment);

                                    TSG.LineSegment intersectionSegment = TSG.Intersection.LineSegmentToObb(lineSegment, obb);
                                    if(null != intersectionSegment)
                                    {
                                        ArrayList intersects = new ArrayList();

                                        intersects.Add(intersectionSegment.Point1);
                                        intersects.Add(intersectionSegment.Point2);

                                        dic.Add(i, intersects);
                                    }
                                }
                            }

                            if (dic.Count > 0)
                            {
                                // 철근그룹 ungroup해서 singleRebar List로 변환
                                TSM.ModelObjectEnumerator rebarEnumerator = TSM.Operations.Operation.Ungrouping(rebarGroup);
                                List<TSM.SingleRebar> singleRebars = new List<TSM.SingleRebar>();                             

                                while (rebarEnumerator.MoveNext())
                                {
                                    TSM.ModelObject rebar = rebarEnumerator.Current;
                                    TSM.SingleRebar singleRebar = rebar as TSM.SingleRebar;
                                    singleRebars.Add(singleRebar);
                                }

                                // 간섭된 철근 index와 point정보를 갖고 있는 dictionary
                                List<int> keyList = new List<int>(dic.Keys);
                                keyList.Sort();


                                List<TSM.SingleRebar> firstSingleRebars = new List<TSM.SingleRebar>();

                                for (int i = 0; i < keyList.Count; i++)
                                {
                                    TSG.Point p = singleRebars[keyList[i]].Polygon.Points[1] as TSG.Point;
                                    TSG.Point p2 = dic[keyList[i]][0] as TSG.Point;

                                    p.Y = p2.Y;
                                    p.Z = p2.Z;
                                    singleRebars[keyList[i]].Polygon.Points[1] = p;
                                    singleRebars[keyList[i]].Modify();
                                }

                                TSM.Operations.Operation.Group(singleRebars);
                                
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                _Model.CommitChanges();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                TSM.Beam beam = TC.Common.CreateBeam("girder", "2", "C40", 400, 700, new TSG.Point(3000, 0, 0), new TSG.Point(6000, 0, 0));
            } catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                _Model.CommitChanges();
            }
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                List<TSM.ContourPoint> contourPoints = new List<TSM.ContourPoint>();

                contourPoints.Add(new TSM.ContourPoint(new TSG.Point(0, 0, 0), null));
                contourPoints.Add(new TSM.ContourPoint(new TSG.Point(3000, 0, 0), null));
                contourPoints.Add(new TSM.ContourPoint(new TSG.Point(3000, 4000, 0), null));
                contourPoints.Add(new TSM.ContourPoint(new TSG.Point(0, 4000, 0), null));

                TSM.ContourPlate contourPlate = TC.Common.CreateContourPlate("slab", "3", "C40", 400, contourPoints);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                _Model.CommitChanges();
            }
        }
    }
}

// 부재 간섭 체크해서 해당영역에 beam생성하는 코드
//List<TSM.Part> partList = new List<TSM.Part>();

//                        while (moe.MoveNext())
//                        {
//                            if (moe.Current is TSM.Part)
//                            {
//                                partList.Add(moe.Current as TSM.Part);
//                            }
//                        }

//                        TSM.Part part1 = partList[0];
//TSM.Part part2 = partList[1];

//TSG.Point minPoint, maxPoint, centerPoint;
//// 두 부재 사이의 간섭 체크
//bool isClashCheck = TC.Common.IsClashCheck(_Model, part1, part2, out minPoint, out maxPoint, out centerPoint);
//TSG.Point startPoint = new TSG.Point();
//TSG.Point endPoint = new TSG.Point();

//double extentX = (maxPoint.X - minPoint.X) / 2;
//double extentY = (maxPoint.Y - minPoint.Y) / 2;
//double extentZ = (maxPoint.Z - minPoint.Z) / 2;
//startPoint = centerPoint + (new TSG.Vector(0, 0, -1) * extentZ);
//                        endPoint = centerPoint + (new TSG.Vector(0, 0, 1) * extentZ);

//                        TC.Common.CreateBeam(startPoint, endPoint, TSM.Position.PlaneEnum.MIDDLE, extentX* 2, extentY* 2);