using System.Windows.Forms;
using TSM = Tekla.Structures.Model;
using TSG = Tekla.Structures.Geometry3d;
using TSD = Tekla.Structures.Datatype;
using TSS = Tekla.Structures.Solid;
using YC = TeklaCommon.YWCommon;
using YCO = TeklaCommon.YWControlObject;
using YP = TeklaCommon.YWPart;

using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

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
                    //_Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(coordinateSystem));

                    if (part is TSM.Beam)
                    {
                        TSM.Beam beam = part as TSM.Beam;

                        TSG.OBB obb = YC.CreateOrientedBoundingBox(_Model, part);

                        startPoint = obb.Center - (obb.Axis0 * obb.Extent0) - (obb.Axis1 * obb.Extent1);
                        endPoint = obb.Center + (obb.Axis0 * obb.Extent0) - (obb.Axis1 * obb.Extent1);

                        polygon.Points.Add(startPoint);
                        polygon.Points.Add(startPoint + (obb.Axis1 * obb.Extent1 * 2));
                        polygons.Add(polygon);

                    }
                    else if (part is TSM.PolyBeam)
                    {

                    }

                    //YC.CreateRebarGroup("SD400", "19", "main", "mainRebar", 3, part, 1, "m", startPoint, endPoint, polygons, onPlaneOffset, distances, rebarGroupSpacingType);

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                //_Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(transformationPlaneCurrent);
                _Model.CommitChanges();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (!_Model.GetConnectionStatus()) throw new Exception("Tekla Structures와 연결되지 않았습니다.");

                TSM.ModelObject mo = _Picker.PickObject(TSM.UI.Picker.PickObjectEnum.PICK_ONE_OBJECT, "철근간섭을 체크할 부재를 선택하세요");

                TSM.ModelObject mo2 = _Picker.PickObject(TSM.UI.Picker.PickObjectEnum.PICK_ONE_OBJECT, "선택한 부재와 간섭된 부재를 선택하세요");

                if(null == mo)
                {
                    MessageBox.Show("선택된 부재가 없습니다.");
                    return;
                }

                if (null == mo2)
                {
                    MessageBox.Show("선택된 부재가 없습니다.");
                    return;
                }

                TSM.Part part = mo as TSM.Part;
                TSM.Part otherPart = mo2 as TSM.Part;

                // 철근이 속한 부재
                TSM.ModelObjectEnumerator childrenEnumerator = part.GetChildren();

                while (childrenEnumerator.MoveNext())
                {
                    TSM.ModelObject children = childrenEnumerator.Current;

                    Dictionary<int, ArrayList> dic = new Dictionary<int, ArrayList>();
                    ArrayList rebarGeometries = null;
                    TSG.OBB obb = YC.CreateOrientedBoundingBox(new TSM.Model(), otherPart);

                    // 부재의 자식 개체가 철근그룹이면
                    if (children is TSM.RebarGroup)
                    {
                        TSM.RebarGroup rebarGroup = children as TSM.RebarGroup;
                        // 제외목록에 포함된 이름이면 로직 수행 안함
                        //if (null != exclusionRebarNameList && exclusionRebarNameList.Contains(rebarGroup.Name)) continue;
                        rebarGeometries = rebarGroup.GetRebarGeometries(false);

                        // rebarGeometries와 Intersection 계산
                        for (int i = 0; i < rebarGeometries.Count; i++)
                        {
                            TSM.RebarGeometry rebarGeometry = rebarGeometries[i] as TSM.RebarGeometry;

                            if (rebarGeometry.Shape.Points.Count == 2)
                            {

                                TSG.Point point1 = rebarGeometry.Shape.Points[0] as TSG.Point;
                                TSG.Point point2 = rebarGeometry.Shape.Points[1] as TSG.Point;
                                TSG.LineSegment lineSegment = new TSG.LineSegment(point1, point2);

                                bool isIntersect = obb.Intersects(lineSegment);

                                TSG.LineSegment intersectionSegment = TSG.Intersection.LineSegmentToObb(lineSegment, obb);
                                if (null != intersectionSegment)
                                {
                                    if (point1 == intersectionSegment.Point1 && point2 == intersectionSegment.Point2) continue;
                                    ArrayList intersects = new ArrayList();

                                    intersects.Add(intersectionSegment.Point1);
                                    intersects.Add(intersectionSegment.Point2);

                                    dic.Add(i, intersects);
                                }
                            }
                            else
                            {
                                for (int j = 0; j < rebarGeometry.Shape.Points.Count - 1; j++)
                                {
                                    TSG.Point point1 = rebarGeometry.Shape.Points[j] as TSG.Point;
                                    TSG.Point point2 = rebarGeometry.Shape.Points[j + 1] as TSG.Point;
                                    TSG.LineSegment lineSegment = new TSG.LineSegment(point1, point2);

                                    bool isIntersect = obb.Intersects(lineSegment);

                                    TSG.LineSegment intersectionSegment = TSG.Intersection.LineSegmentToObb(lineSegment, obb);
                                    if (null != intersectionSegment)
                                    {
                                        if (point1 == intersectionSegment.Point1 && point2 == intersectionSegment.Point2) continue;
                                        ArrayList intersects = new ArrayList();

                                        intersects.Add(intersectionSegment.Point1);
                                        intersects.Add(intersectionSegment.Point2);

                                        dic.Add(i, intersects);
                                    }
                                }
                            }
                        }

                        if (dic.Count > 0)
                        {
                            TSG.Vector moveVector = new TSG.Vector(rebarGroup.EndPoint - rebarGroup.StartPoint).GetNormal();
                            TSG.Vector vectorZ = new TSG.Vector(0, 0, 1);

                            double onPlaneOffSet = (double)rebarGroup.OnPlaneOffsets[0];
                            double fromPlaneOffset = (double)rebarGroup.FromPlaneOffset;

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


                            #region 철근그룹의 형태가 직선철근일 경우 아래 로직 수행
                            if (singleRebars[0].Polygon.Points.Count == 2)
                            {
                                List<TSM.SingleRebar> firstSingleRebars = new List<TSM.SingleRebar>();
                                List<TSM.SingleRebar> secondSingleRebars = new List<TSM.SingleRebar>();

                                for (int i = 0; i < keyList.Count; i++)
                                {
                                    TSM.SingleRebar singleRebar = singleRebars[keyList[i]];
                                    ArrayList singleRebarGeometries = singleRebar.GetRebarGeometries(false);
                                    TSM.RebarGeometry singleRebarGeometry = singleRebarGeometries[0] as TSM.RebarGeometry;
                                    TSG.Point p1 = singleRebarGeometry.Shape.Points[0] as TSG.Point;
                                    TSG.Point p2 = singleRebarGeometry.Shape.Points[1] as TSG.Point;
                                    TSG.Point dicPoint1 = dic[keyList[i]][0] as TSG.Point;
                                    TSG.Point dicPoint2 = dic[keyList[i]][1] as TSG.Point;

                                    double halfDiameter = 0.0;
                                    halfDiameter = singleRebar.GetSingleRebar(0, false).Diameter * 0.5;


                                    TSG.Vector polygonVector = new TSG.Vector(p2 - p1).GetNormal();
                                    TSG.Vector onPlaneVector = polygonVector.Cross(moveVector);
                                    TSG.Point movePoint = new TSG.Point();

                                    // 간섭이 철근 시작포인트보다 바깥쪽에 있거나 안쪽에 있는 경우
                                    if (dicPoint1.CompareTo(p1) == 0)
                                    {
                                        if (onPlaneOffSet < 0)
                                        {
                                            movePoint = dicPoint2 + onPlaneVector * -1 * (onPlaneOffSet - halfDiameter);

                                            if(fromPlaneOffset != 0)
                                            {
                                                movePoint += moveVector * -1 * (fromPlaneOffset + halfDiameter);
                                            }                                            
                                        }
                                        else if (onPlaneOffSet > 0)
                                        {
                                            movePoint = dicPoint2 + onPlaneVector * -1 * (onPlaneOffSet + halfDiameter);

                                            if (fromPlaneOffset != 0)
                                            {
                                                movePoint += moveVector * -1 * (fromPlaneOffset + halfDiameter);
                                            }    
                                        }
                                        else
                                        {
                                            if (fromPlaneOffset != 0)
                                            {
                                                movePoint = dicPoint2 + moveVector * -1 * (fromPlaneOffset + halfDiameter);
                                            } else
                                            {
                                                movePoint = dicPoint2;
                                            }                                                
                                        }

                                        singleRebar.Polygon.Points[0] = movePoint;

                                        singleRebar.Class = 2;
                                        singleRebar.Modify();

                                        firstSingleRebars.Add(singleRebar);

                                    }
                                    else if (dicPoint1.CompareTo(p1) == -1)
                                    {
                                        if (onPlaneOffSet < 0)
                                        {
                                            movePoint = dicPoint1 + onPlaneVector * -1 * (onPlaneOffSet - halfDiameter);

                                            if(fromPlaneOffset != 0)
                                            {
                                                movePoint += moveVector * -1 * (fromPlaneOffset + halfDiameter);
                                            }                                            
                                        }
                                        else if (onPlaneOffSet > 0)
                                        {
                                            movePoint = dicPoint1 + onPlaneVector * -1 * (onPlaneOffSet + halfDiameter);

                                            if (fromPlaneOffset != 0)
                                            {
                                                movePoint += moveVector * -1 * (fromPlaneOffset + halfDiameter);
                                            }                                                
                                        }
                                        else
                                        {
                                            if (fromPlaneOffset != 0)
                                            {
                                                movePoint = dicPoint1 + moveVector * -1 * (fromPlaneOffset + halfDiameter);
                                            } else
                                            {
                                                movePoint = dicPoint1;
                                            }
                                        }

                                        singleRebar.Polygon.Points[1] = movePoint;

                                        singleRebar.Class = 3;
                                        singleRebar.Modify();

                                        firstSingleRebars.Add(singleRebar);
                                    }
                                    else
                                    {
                                        if(onPlaneOffSet < 0)
                                        {
                                            movePoint = dicPoint1 + onPlaneVector * -1 * (onPlaneOffSet - halfDiameter);

                                            if(fromPlaneOffset != 0)
                                            {
                                                movePoint += moveVector * -1 * (fromPlaneOffset + halfDiameter);
                                            }                                            
                                        }
                                        else if (onPlaneOffSet > 0)
                                        {
                                            movePoint = dicPoint1 + onPlaneVector * -1 * (onPlaneOffSet + halfDiameter);

                                            if (fromPlaneOffset != 0)
                                            {
                                                movePoint += moveVector * -1 * (fromPlaneOffset + halfDiameter);
                                            }                                                
                                        }
                                        else
                                        {
                                            if (fromPlaneOffset != 0)
                                            {
                                                movePoint = dicPoint1 + moveVector * -1 * (fromPlaneOffset + halfDiameter);
                                            } else
                                            {
                                                movePoint = dicPoint1;
                                            }                                                
                                        }


                                        singleRebar.Polygon.Points[1] = movePoint;

                                        singleRebar.Class = 4;
                                        singleRebar.Modify();

                                        firstSingleRebars.Add(singleRebar);
                                    }


                                    if (dicPoint1.CompareTo(p1) != 0)
                                    {
                                        if (dicPoint2.CompareTo(p2) == 0)
                                        {
                                            if (onPlaneOffSet < 0)
                                            {
                                                movePoint = dicPoint1 + onPlaneVector * -1 * (onPlaneOffSet - halfDiameter);

                                                if(fromPlaneOffset != 0)
                                                {
                                                    movePoint += moveVector * -1 * (fromPlaneOffset + halfDiameter);
                                                }                                                
                                            }
                                            else if (onPlaneOffSet > 0)
                                            {
                                                movePoint = dicPoint1 + onPlaneVector * -1 * (onPlaneOffSet + halfDiameter);

                                                if (fromPlaneOffset != 0)
                                                {
                                                    movePoint += moveVector * -1 * (fromPlaneOffset + halfDiameter);
                                                }                                                    
                                            }
                                            else
                                            {
                                                if (fromPlaneOffset != 0)
                                                {
                                                    movePoint = dicPoint1 + moveVector * -1 * (fromPlaneOffset + halfDiameter);
                                                } else
                                                {
                                                    movePoint = dicPoint1;
                                                }                                                    
                                            }

                                            singleRebar.Polygon.Points[1] = movePoint;

                                            singleRebar.Class = 5;
                                            singleRebar.Modify();

                                            firstSingleRebars.Add(singleRebar);
                                        }
                                        else if (dicPoint2.CompareTo(p2) == -1)
                                        {
                                            TSM.SingleRebar newSingleRebar = TSM.Operations.Operation.CopyObject(singleRebar, polygonVector) as TSM.SingleRebar;

                                            if(onPlaneOffSet < 0)
                                            {
                                                movePoint = p2 + onPlaneVector * -1 * (onPlaneOffSet - halfDiameter);

                                                if(fromPlaneOffset != 0)
                                                {
                                                    movePoint += moveVector * -1 * (fromPlaneOffset + halfDiameter);
                                                }                                                
                                            }
                                            else if (onPlaneOffSet > 0)
                                            {
                                                movePoint = p2 + onPlaneVector * -1 * (onPlaneOffSet + halfDiameter);

                                                if (fromPlaneOffset != 0)
                                                {
                                                    movePoint += moveVector * -1 * (fromPlaneOffset + halfDiameter);
                                                }                                                    
                                            }
                                            else
                                            {
                                                if (fromPlaneOffset != 0)
                                                {
                                                    movePoint = p2 + moveVector * -1 * (fromPlaneOffset + halfDiameter);
                                                } else
                                                {
                                                    movePoint = p2;
                                                }                                                    
                                            }

                                            newSingleRebar.Polygon.Points[0] = movePoint;

                                            if (onPlaneOffSet < 0)
                                            {
                                                movePoint = dicPoint2 + onPlaneVector * -1 * (onPlaneOffSet - halfDiameter);

                                                if (fromPlaneOffset != 0)
                                                {
                                                    movePoint += moveVector * -1 * (fromPlaneOffset + halfDiameter);
                                                }
                                                    
                                            }
                                            else if (onPlaneOffSet > 0)
                                            {
                                                movePoint = dicPoint2 + onPlaneVector * -1 * (onPlaneOffSet + halfDiameter);

                                                if (fromPlaneOffset != 0)
                                                {
                                                    movePoint += moveVector * -1 * (fromPlaneOffset + halfDiameter);
                                                }                                                    
                                            }
                                            else
                                            {
                                                if (fromPlaneOffset != 0)
                                                {
                                                    movePoint = dicPoint2 + moveVector * -1 * (fromPlaneOffset + halfDiameter);
                                                } else
                                                {
                                                    movePoint = dicPoint2;
                                                }                                                    
                                            }

                                            newSingleRebar.Polygon.Points[1] = movePoint;

                                            newSingleRebar.OnPlaneOffsets[0] = onPlaneOffSet * -1;

                                            newSingleRebar.Class = 6;
                                            newSingleRebar.Modify();

                                            secondSingleRebars.Add(newSingleRebar);
                                        }
                                        else
                                        {
                                            TSM.SingleRebar newSingleRebar = TSM.Operations.Operation.CopyObject(singleRebar, polygonVector) as TSM.SingleRebar;

                                            if(onPlaneOffSet < 0)
                                            {
                                                movePoint = p2 + onPlaneVector * -1 * (onPlaneOffSet - halfDiameter);

                                                if (fromPlaneOffset != 0)
                                                {
                                                    movePoint += moveVector * -1 * (fromPlaneOffset + halfDiameter);
                                                }
                                            }
                                            else if (onPlaneOffSet > 0)
                                            {
                                                movePoint = p2 + onPlaneVector * -1 * (onPlaneOffSet + halfDiameter);

                                                if (fromPlaneOffset != 0)
                                                {
                                                    movePoint += moveVector * -1 * (fromPlaneOffset + halfDiameter);
                                                }                                                    
                                            }
                                            else
                                            {
                                                if (fromPlaneOffset != 0)
                                                {
                                                    movePoint = p2 + moveVector * -1 * (fromPlaneOffset + halfDiameter);
                                                } else
                                                {
                                                    movePoint = p2;
                                                }                                                    
                                            }

                                            newSingleRebar.Polygon.Points[0] = movePoint;

                                            if(onPlaneOffSet < 0)
                                            {
                                                movePoint = dicPoint2 + onPlaneVector * -1 * (onPlaneOffSet - halfDiameter);

                                                if (fromPlaneOffset != 0)
                                                {
                                                    movePoint += moveVector * -1 * (fromPlaneOffset + halfDiameter);
                                                }                                                    
                                            }
                                            else if (onPlaneOffSet > 0)
                                            {
                                                movePoint = dicPoint2 + onPlaneVector * -1 * (onPlaneOffSet + halfDiameter);

                                                if (fromPlaneOffset != 0)
                                                {
                                                    movePoint += moveVector * -1 * (fromPlaneOffset + halfDiameter);
                                                }                                                    
                                            }
                                            else
                                            {
                                                if (fromPlaneOffset != 0)
                                                {
                                                    movePoint = dicPoint2 + moveVector * -1 * (fromPlaneOffset + halfDiameter);
                                                } else
                                                {
                                                    movePoint = dicPoint2;
                                                }                                                    
                                            }

                                            newSingleRebar.Polygon.Points[1] = movePoint;

                                            newSingleRebar.OnPlaneOffsets[0] = onPlaneOffSet * -1;

                                            newSingleRebar.Class = 7;
                                            newSingleRebar.Modify();


                                            secondSingleRebars.Add(newSingleRebar);
                                        }
                                    }
                                }

                                firstSingleRebars = firstSingleRebars.Distinct().ToList();
                                secondSingleRebars = secondSingleRebars.Distinct().ToList();

                                TSM.Operations.Operation.Group(firstSingleRebars);
                                TSM.Operations.Operation.Group(secondSingleRebars);
                            }
                            #endregion 철근그룹의 형태가 직선철근일 경우 아래 로직 수행

                            #region 철근그룹의 형태가 스트럽(4 or 5 point)인 경우 아래 로직 수행
                            if (singleRebars[0].Polygon.Points.Count > 2)
                            {
                                for (int i = 0; i < keyList.Count; i++)
                                {
                                    TSM.SingleRebar singleRebar = singleRebars[keyList[i]];
                                    singleRebar.Delete();
                                }
                            }
                            #endregion 철근그룹의 형태가 스트럽(4 or 5 point)인 경우 아래 로직 수행


                            // 삭제는 끝에서부터해야 기존 철근그룹이 오류가 없다.
                            keyList.Reverse();
                            for (int i = 0; i < keyList.Count; i++)
                            {
                                singleRebars.RemoveAt(keyList[i]);
                            }

                            TSM.Operations.Operation.Group(singleRebars);
                        }
                    }
                    else if (children is TSM.SingleRebar)
                    {
                        TSM.SingleRebar SingleRebar = children as TSM.SingleRebar;
                        // 제외목록에 포함된 이름이면 로직 수행 안함
                        //if (null != exclusionRebarNameList && exclusionRebarNameList.Contains(SingleRebar.Name)) continue;
                        rebarGeometries = SingleRebar.GetRebarGeometries(false);

                        // rebarGeometries와 Intersection 계산
                        for (int i = 0; i < rebarGeometries.Count; i++)
                        {
                            TSM.RebarGeometry rebarGeometry = rebarGeometries[i] as TSM.RebarGeometry;

                            if (rebarGeometry.Shape.Points.Count == 2)
                            {

                                TSG.Point point1 = SingleRebar.Polygon.Points[0] as TSG.Point;
                                TSG.Point point2 = SingleRebar.Polygon.Points[1] as TSG.Point;
                                TSG.LineSegment lineSegment = new TSG.LineSegment(point1, point2);

                                bool isIntersect = obb.Intersects(lineSegment);

                                TSG.LineSegment intersectionSegment = TSG.Intersection.LineSegmentToObb(lineSegment, obb);
                                if (null != intersectionSegment)
                                {
                                    if (point1 == intersectionSegment.Point1 && point2 == intersectionSegment.Point2) continue;
                                    ArrayList intersects = new ArrayList();

                                    intersects.Add(intersectionSegment.Point1);
                                    intersects.Add(intersectionSegment.Point2);

                                    dic.Add(i, intersects);
                                }
                            }
                            else
                            {
                                for (int j = 0; j < rebarGeometry.Shape.Points.Count - 1; j++)
                                {
                                    TSG.Point point1 = rebarGeometry.Shape.Points[j] as TSG.Point;
                                    TSG.Point point2 = rebarGeometry.Shape.Points[j + 1] as TSG.Point;
                                    TSG.LineSegment lineSegment = new TSG.LineSegment(point1, point2);

                                    bool isIntersect = obb.Intersects(lineSegment);

                                    TSG.LineSegment intersectionSegment = TSG.Intersection.LineSegmentToObb(lineSegment, obb);
                                    if (null != intersectionSegment)
                                    {
                                        if (point1 == intersectionSegment.Point1 && point2 == intersectionSegment.Point2) continue;
                                        ArrayList intersects = new ArrayList();

                                        intersects.Add(intersectionSegment.Point1);
                                        intersects.Add(intersectionSegment.Point2);

                                        dic.Add(i, intersects);
                                    }
                                }
                            }
                        }

                        if (dic.Count > 0)
                        {
                            TSG.Vector moveVector = new TSG.Vector((SingleRebar.Polygon.Points[1] as TSG.Point) - (SingleRebar.Polygon.Points[0] as TSG.Point)).GetNormal();
                            TSG.Vector moveVector90 = new TSG.Vector(moveVector.Y, -moveVector.X, moveVector.Z);
                            double onPlaneOffSet = (double)SingleRebar.OnPlaneOffsets[0];
                            double fromPlaneOffset = (double)SingleRebar.FromPlaneOffset;

                            List<TSM.SingleRebar> singleRebars = new List<TSM.SingleRebar>();

                            singleRebars.Add(SingleRebar);

                            // 간섭된 철근 index와 point정보를 갖고 있는 dictionary
                            List<int> keyList = new List<int>(dic.Keys);
                            keyList.Sort();


                            #region 철근그룹의 형태가 직선철근일 경우 아래 로직 수행
                            if (singleRebars[0].Polygon.Points.Count == 2)
                            {
                                List<TSM.SingleRebar> firstSingleRebars = new List<TSM.SingleRebar>();
                                List<TSM.SingleRebar> secondSingleRebars = new List<TSM.SingleRebar>();

                                for (int i = 0; i < keyList.Count; i++)
                                {
                                    TSM.SingleRebar singleRebar = singleRebars[keyList[i]];
                                    ArrayList singleRebarGeometries = singleRebar.GetRebarGeometries(false);
                                    TSM.RebarGeometry singleRebarGeometry = singleRebarGeometries[0] as TSM.RebarGeometry;
                                    TSG.Point p1 = singleRebar.Polygon.Points[0] as TSG.Point;
                                    TSG.Point p2 = singleRebar.Polygon.Points[1] as TSG.Point;
                                    TSG.Point dicPoint1 = dic[keyList[i]][0] as TSG.Point;
                                    TSG.Point dicPoint2 = dic[keyList[i]][1] as TSG.Point;

                                    double halfDiameter = 0.0;
                                    halfDiameter = singleRebar.GetSingleRebar(0, false).Diameter * 0.5;


                                    TSG.Vector polygonVector = new TSG.Vector(p2 - p1).GetNormal();
                                    TSG.Vector polygonVector90 = new TSG.Vector(polygonVector.Y, -polygonVector.X, polygonVector.Z);
                                    TSG.Point movePoint = new TSG.Point();

                                    if (polygonVector.CompareTo(polygonVector90) == 0)
                                    {
                                        polygonVector90 = YC.Rotate(polygonVector, 90, (int)moveVector.X, (int)moveVector.Y, (int)moveVector.Z);
                                        polygonVector90 = polygonVector90 * -1;
                                    }

                                    // 간섭이 철근 시작포인트보다 바깥쪽에 있거나 안쪽에 있는 경우
                                    if (dicPoint1.CompareTo(p1) == 0)
                                    {
                                        singleRebar.Polygon.Points[0] = dicPoint2;

                                        singleRebar.Class = 2;
                                        singleRebar.Modify();

                                        firstSingleRebars.Add(singleRebar);

                                    }
                                    else if (dicPoint1.CompareTo(p1) == -1)
                                    {
                                        singleRebar.Polygon.Points[1] = dicPoint1;

                                        singleRebar.Class = 2;
                                        singleRebar.Modify();

                                        firstSingleRebars.Add(singleRebar);
                                    }
                                    else
                                    {
                                        singleRebar.Polygon.Points[1] = dicPoint1;

                                        singleRebar.Class = 2;
                                        singleRebar.Modify();

                                        firstSingleRebars.Add(singleRebar);
                                    }


                                    if (dicPoint1.CompareTo(p1) != 0)
                                    {
                                        if (dicPoint2.CompareTo(p2) == 0)
                                        {
                                            singleRebar.Polygon.Points[1] = dicPoint1;

                                            singleRebar.Class = 2;
                                            singleRebar.Modify();

                                            firstSingleRebars.Add(singleRebar);
                                        }
                                        else if (dicPoint2.CompareTo(p2) == -1)
                                        {
                                            TSM.SingleRebar newSingleRebar = TSM.Operations.Operation.CopyObject(singleRebar, polygonVector) as TSM.SingleRebar;

                                            newSingleRebar.Polygon.Points[0] = p2;
                                            newSingleRebar.Polygon.Points[1] = dicPoint2;

                                            newSingleRebar.Class = 2;
                                            newSingleRebar.Modify();

                                            secondSingleRebars.Add(newSingleRebar);
                                        }
                                        else
                                        {
                                            TSM.SingleRebar newSingleRebar = TSM.Operations.Operation.CopyObject(singleRebar, polygonVector) as TSM.SingleRebar;

                                            newSingleRebar.Polygon.Points[0] = p2;
                                            newSingleRebar.Polygon.Points[1] = dicPoint2;

                                            newSingleRebar.Class = 2;
                                            newSingleRebar.Modify();

                                            secondSingleRebars.Add(newSingleRebar);
                                        }
                                    }
                                }

                                firstSingleRebars = firstSingleRebars.Distinct().ToList();
                                secondSingleRebars = secondSingleRebars.Distinct().ToList();

                                TSM.Operations.Operation.Group(firstSingleRebars);
                                TSM.Operations.Operation.Group(secondSingleRebars);
                            }
                            #endregion 철근그룹의 형태가 직선철근일 경우 아래 로직 수행

                            #region 철근그룹의 형태가 스트럽(4 or 5 point)인 경우 아래 로직 수행
                            if (singleRebars[0].Polygon.Points.Count > 2)
                            {
                                for (int i = 0; i < keyList.Count; i++)
                                {
                                    TSM.SingleRebar singleRebar = singleRebars[keyList[i]];
                                    singleRebar.Delete();
                                }
                            }
                            #endregion 철근그룹의 형태가 스트럽(4 or 5 point)인 경우 아래 로직 수행


                            // 삭제는 끝에서부터해야 기존 철근그룹이 오류가 없다.
                            keyList.Reverse();
                            for (int i = 0; i < keyList.Count; i++)
                            {
                                singleRebars.RemoveAt(keyList[i]);
                            }

                            TSM.Operations.Operation.Group(singleRebars);
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
                TSM.Beam beam = YP.CreateBeam("girder", "2", "C40", 400, 700, new TSG.Point(3000, 0, 0), new TSG.Point(6000, 0, 0), null, null, TSM.Beam.BeamTypeEnum.BEAM);

                YCO.CreateControlPlane(new TSG.Point(5000, 3000, 0), new TSG.Vector(1000, 0, 0), new TSG.Vector(0, 1000, 0));
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

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                List<TSM.ContourPoint> contourPoints = new List<TSM.ContourPoint>();

                contourPoints.Add(new TSM.ContourPoint(new TSG.Point(0, 0, 0), null));
                contourPoints.Add(new TSM.ContourPoint(new TSG.Point(3000, 0, 0), null));
                contourPoints.Add(new TSM.ContourPoint(new TSG.Point(3000, 4000, 0), null));
                contourPoints.Add(new TSM.ContourPoint(new TSG.Point(0, 4000, 0), null));

                TSM.ContourPlate contourPlate = YP.CreateContourPlate("slab", "3", "C40", 400, contourPoints);
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

        private void button7_Click(object sender, EventArgs e)
        {
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

                    // polygon에 사용될 철근의 시작, 끝 포인트
                    TSG.Point point = new TSG.Point();
                    TSG.Point basePoint = new TSG.Point();

                    // 철근의 형상 포인트
                    TSM.Polygon polygon = new TSM.Polygon();

                    // 철근 시작평면
                    double fromPlaneOffset = 0.0;                    

                    // 철근 평면
                    ArrayList onPlaneOffset = new ArrayList();

                    // 부재의 local coordinate로 변환
                    TSG.CoordinateSystem coordinateSystem = part.GetCoordinateSystem();
                    // model에 workplane 설정
                    _Model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TSM.TransformationPlane(coordinateSystem));

                    if (part is TSM.Beam)
                    {
                        TSM.Beam beam = part as TSM.Beam;

                        TSG.OBB obb = YC.CreateOrientedBoundingBox(_Model, part);

                        //startPoint = obb.Center - (obb.Axis0 * obb.Extent0);
                        //endPoint = obb.Center + (obb.Axis0 * obb.Extent0);

                        basePoint = obb.Center - (obb.Axis0 * obb.Extent0);

                        point = basePoint + obb.Axis2 * obb.Extent2;
                        polygon.Points.Add(point);

                        point = point + obb.Axis2 * obb.Extent2;
                        polygon.Points.Add(point);

                        TSG.Vector vector = new TSG.Vector((polygon.Points[1] as TSG.Point) - (polygon.Points[0] as TSG.Point)).GetNormal();
                        TSG.Vector vectorAxisX = YC.Rotate(vector, 58, 1, 0, 0);

                        point = point + vectorAxisX * obb.Extent2;
                        polygon.Points.Add(point);

                    }
                    else if (part is TSM.PolyBeam)
                    {

                    }

                    YWReinforcement.CreateSingleRebar("SD400", "19", "main", "mainRebar", 3, part, polygon);

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

        /// <summary>
        /// WALL 생성
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                TSM.Beam beam = YP.CreateBeam("WALL", "1", "C24", 6000, 250,
                    new TSG.Point(6000, 0, 0), new TSG.Point(6000, 6000, 0), null, null, TSM.Beam.BeamTypeEnum.PANEL,
                    TSM.Position.DepthEnum.FRONT, 0, TSM.Position.PlaneEnum.MIDDLE, 0, TSM.Position.RotationEnum.TOP, 0);
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