using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TC = TeklaCommon.YWCommon;
using TSG = Tekla.Structures.Geometry3d;
using TSM = Tekla.Structures.Model;

namespace TeklaCommon
{
    /// <summary>
    /// 선택한 부재의 startPoint, endPoint, 부재의 각 solid에 대한 vertex Point를 표시
    /// </summary>
    public partial class DisplayPoint : Form
    {
        private TSM.Model model;
        private TSM.UI.Picker picker;

        public DisplayPoint()
        {
            InitializeComponent();

            this.model = new TSM.Model();

            if (!this.model.GetConnectionStatus()) return;
        }

        private void ButtonSelectPart_Click(object sender, EventArgs e)
        {
            picker = new TSM.UI.Picker();

            try
            {
                TSM.ModelObject modelObject = picker.PickObject(TSM.UI.Picker.PickObjectEnum.PICK_ONE_PART, "부재를 선택하세요");

                if (null != modelObject)
                {
                    this.CheckType(modelObject);
                }
                else
                {
                    MessageBox.Show("부재가 선택되지 않았습니다.\r\n부재를 선택해주세요.");
                    return;
                }
            } catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                // model 반영
                model.CommitChanges();
            }            
        }

        /// <summary>
        /// 선택한 부재의 타입을 체크
        /// </summary>
        /// <param name="modelObject"></param>
        private void CheckType(TSM.ModelObject modelObject)
        {
            Type type = modelObject.GetType();

            TSG.Point startPoint = null;
            TSG.Point endPoint = null;
            List<TSG.Point> vertexList = null;
            StringBuilder sb = null;

            if (type == typeof(TSM.Beam)) // 1. 빔(컬럼, 거더, 빔)
            {
                TSM.Beam beam = modelObject as TSM.Beam;

                // 시작, 끝 포인트
                startPoint = beam.StartPoint;
                endPoint = beam.EndPoint;

                // beam의 꼭지점 가져오기
                vertexList = TC.GetVertexList(beam);

                // 결과 텍스트 작성
                sb = new StringBuilder();
                sb.AppendFormat("1. Attributes");
                sb.Append(Environment.NewLine);

                sb.Append(' ', 5);
                sb.AppendFormat("Name : {0}, Type : {1}", beam.Name, beam.GetType().ToString());
                sb.Append(Environment.NewLine);
                sb.Append(' ', 5);
                sb.AppendFormat("Profile : {0}", beam.Profile.ProfileString);
                sb.Append(Environment.NewLine);
                sb.Append(' ', 5);
                sb.AppendFormat("Material : {0}", beam.Material.MaterialString);
                sb.Append(Environment.NewLine);
                sb.Append(' ', 5);
                sb.AppendFormat("Finish : {0}", beam.Finish);
                sb.Append(Environment.NewLine);
                sb.Append(' ', 5);
                sb.AppendFormat("Class : {0}", beam.Class);
                sb.Append(Environment.NewLine);


                sb.AppendFormat("2. StartPoint : {0}", beam.StartPoint.ToString());
                sb.Append(Environment.NewLine);
                sb.AppendFormat("3. EndPoint : {0}", beam.EndPoint.ToString());
                sb.Append(Environment.NewLine);

                string centerLineList = "";
                for (int i = 0, ii = beam.GetCenterLine(true).Count; i < ii; i++)
                {
                    centerLineList += beam.GetCenterLine(true)[i].ToString() + " ";
                }
                sb.AppendFormat("4. CenterLine : {0}", centerLineList);
                sb.Append(Environment.NewLine);

                sb.AppendFormat("5. Vertex List");
                sb.Append(Environment.NewLine);

                for (int i = 0, ii = vertexList.Count; i < ii; i++)
                {
                    sb.Append(' ', 5);
                    sb.AppendFormat("{0} : {1}", i+1, vertexList[i].ToString());
                    sb.Append(Environment.NewLine);
                }

                this.BooleanPartCheck(beam as TSM.Part, ref sb);
                this.ReinforcementCheck(beam as TSM.Part, ref sb);                

                TextBoxPointOfPart.Text = sb.ToString();

            } else if(type == typeof(TSM.PolyBeam)) // 2. 폴리빔
            {
                TSM.PolyBeam polyBeam = modelObject as TSM.PolyBeam;

                // 포인트 리스트
                ArrayList pointList = polyBeam.Contour.ContourPoints;
                
                // polybeam의 꼭지점 가져오기
                vertexList = TC.GetVertexList(polyBeam);

                // 결과 텍스트 작성
                sb = new StringBuilder();
                sb.AppendFormat("1. Name : {0}", polyBeam.Name);
                sb.Append(Environment.NewLine);

                sb.AppendFormat("2. ContourPoints");
                sb.Append(Environment.NewLine);

                for (int i = 0, ii = pointList.Count; i < ii; i++)
                {
                    sb.Append(' ', 5);
                    sb.AppendFormat("{0} : {1}", i + 1, pointList[i].ToString());
                    sb.Append(Environment.NewLine);
                }

                sb.AppendFormat("3. Vertex List");
                sb.Append(Environment.NewLine);

                for (int i = 0, ii = vertexList.Count; i < ii; i++)
                {
                    sb.Append(' ', 5);
                    sb.AppendFormat("{0} : {1}", i + 1, vertexList[i].ToString());
                    sb.Append(Environment.NewLine);
                }

                this.BooleanPartCheck(polyBeam as TSM.Part, ref sb);
                this.ReinforcementCheck(polyBeam as TSM.Part, ref sb);

                TextBoxPointOfPart.Text = sb.ToString();
            } else if(type == typeof(TSM.ContourPlate)) // 3. 플레이트(슬래브, 월)
            {
                TSM.ContourPlate contourPlate = modelObject as TSM.ContourPlate;

                // 포인트 리스트
                ArrayList pointList = contourPlate.Contour.ContourPoints;

                // polybeam의 꼭지점 가져오기
                vertexList = TC.GetVertexList(contourPlate);

                // 결과 텍스트 작성
                sb = new StringBuilder();
                sb.AppendFormat("1. Name : {0}", contourPlate.Name);
                sb.Append(Environment.NewLine);

                sb.AppendFormat("2. ContourPoints");
                sb.Append(Environment.NewLine);

                for (int i = 0, ii = pointList.Count; i < ii; i++)
                {
                    sb.Append(' ', 5);
                    sb.AppendFormat("{0} : {1}", i + 1, pointList[i].ToString());
                    sb.Append(Environment.NewLine);
                }

                sb.AppendFormat("3. Vertex List");
                sb.Append(Environment.NewLine);

                for (int i = 0, ii = vertexList.Count; i < ii; i++)
                {
                    sb.Append(' ', 5);
                    sb.AppendFormat("{0} : {1}", i + 1, vertexList[i].ToString());
                    sb.Append(Environment.NewLine);
                }

                this.BooleanPartCheck(contourPlate as TSM.Part, ref sb);
                this.ReinforcementCheck(contourPlate as TSM.Part, ref sb);

                TextBoxPointOfPart.Text = sb.ToString();
            }
        }

        

        /// <summary>
        /// 부재의 BooleanPart가 있는지 확인하고 정보를 조회한다.
        /// </summary>
        /// <param name="part"></param>
        private void BooleanPartCheck(TSM.Part part, ref StringBuilder sb)
        {
            List<TSG.Point> vertexList = null;

            // BooleanPart, CutPlane, EdgeChamfer, Fitting 체크
            TSM.ModelObjectEnumerator booleanEnumerator = part.GetBooleans();

            if (booleanEnumerator.GetSize() > 0)
            {
                sb.AppendFormat("=============BooleanPart=============");
                sb.Append(Environment.NewLine);
            }

            while (booleanEnumerator.MoveNext())
            {
                TSM.ModelObject booleanModelObject = booleanEnumerator.Current;

                sb.Append(' ', 5);
                sb.AppendFormat("Attributes");
                sb.Append(Environment.NewLine);

                if (booleanModelObject.GetType() == typeof(TSM.BooleanPart))
                {
                    TSM.BooleanPart booleanPart = booleanModelObject as TSM.BooleanPart;
                    TSM.Part operativePart = booleanPart.OperativePart;
                    Type operativePartType = operativePart.GetType();

                    sb.Append(' ', 10);
                    sb.AppendFormat("Name : {0}", operativePart.Name.ToString());
                    sb.Append(Environment.NewLine);
                    sb.Append(' ', 10);
                    sb.AppendFormat("Profile : {0}", operativePart.Profile.ProfileString);
                    sb.Append(Environment.NewLine);
                    sb.Append(' ', 10);
                    sb.AppendFormat("Depth : {0}, {1}", operativePart.Position.Depth.ToString(), operativePart.Position.DepthOffset.ToString());
                    sb.Append(Environment.NewLine);
                    sb.Append(' ', 10);
                    sb.AppendFormat("BooleanPart Type : {0}", booleanPart.Type.ToString());
                    sb.Append(Environment.NewLine);



                    if (operativePartType == typeof(TSM.Beam))
                    {
                        TSM.Beam operativePartBeam = operativePart as TSM.Beam;

                        sb.Append(' ', 10);
                        sb.AppendFormat("StartPoint : {0}", operativePartBeam.StartPoint.ToString());
                        sb.Append(Environment.NewLine);
                        sb.Append(' ', 10);
                        sb.AppendFormat("EndPoint : {0}", operativePartBeam.EndPoint.ToString());
                        sb.Append(Environment.NewLine);

                        string centerLineList = "";
                        for (int i = 0, ii = operativePartBeam.GetCenterLine(true).Count; i < ii; i++)
                        {
                            centerLineList += operativePartBeam.GetCenterLine(true)[i].ToString() + " ";
                        }
                        sb.AppendFormat("CenterLine : {0}", centerLineList);
                        sb.Append(Environment.NewLine);
                    }
                    else if (operativePartType == typeof(TSM.PolyBeam))
                    {
                        TSM.PolyBeam operativePartPolyBeam = operativePart as TSM.PolyBeam;
                        ArrayList pointList = operativePartPolyBeam.Contour.ContourPoints;

                        sb.Append(' ', 10);
                        sb.AppendFormat("ContourPoints");
                        sb.Append(Environment.NewLine);

                        for (int i = 0, ii = pointList.Count; i < ii; i++)
                        {
                            sb.Append(' ', 15);
                            sb.AppendFormat("{0} : {1}", i + 1, pointList[i].ToString());
                            sb.Append(Environment.NewLine);
                        }

                        string centerLineList = "";
                        for (int i = 0, ii = operativePartPolyBeam.GetCenterLine(true).Count; i < ii; i++)
                        {
                            centerLineList += operativePartPolyBeam.GetCenterLine(true)[i].ToString() + " ";
                        }
                        sb.AppendFormat("CenterLine : {0}", centerLineList);
                        sb.Append(Environment.NewLine);
                    }
                    else if (operativePartType == typeof(TSM.ContourPlate))
                    {
                        TSM.ContourPlate operativePartContourPlate = operativePart as TSM.ContourPlate;
                        ArrayList pointList = operativePartContourPlate.Contour.ContourPoints;

                        sb.Append(' ', 10);
                        sb.AppendFormat("ContourPoints");
                        sb.Append(Environment.NewLine);

                        for (int i = 0, ii = pointList.Count; i < ii; i++)
                        {
                            sb.Append(' ', 15);
                            sb.AppendFormat("{0} : {1}", i + 1, pointList[i].ToString());
                            sb.Append(Environment.NewLine);
                        }

                        string centerLineList = "";
                        for (int i = 0, ii = operativePartContourPlate.GetCenterLine(true).Count; i < ii; i++)
                        {
                            centerLineList += operativePartContourPlate.GetCenterLine(true)[i].ToString() + " ";
                        }
                        sb.AppendFormat("CenterLine : {0}", centerLineList);
                        sb.Append(Environment.NewLine);
                    }

                    vertexList = TC.GetVertexList(operativePart);

                    for (int i = 0, ii = vertexList.Count; i < ii; i++)
                    {
                        sb.Append(' ', 15);
                        sb.AppendFormat("{0} : {1}", i + 1, vertexList[i].ToString());
                        sb.Append(Environment.NewLine);
                    }
                }
            }
        }

        /// <summary>
        /// 부재의 철근정보를 확인하고 정보를 조회한다.
        /// </summary>
        /// <param name="part"></param>
        /// <param name="sb"></param>
        private void ReinforcementCheck(TSM.Part part, ref StringBuilder sb)
        {
            List<TSG.Point> vertexList = null;

            // 단일철근, 그룹철근 체크
            TSM.ModelObjectEnumerator reinforcementEnumerator = part.GetReinforcements();

            if (reinforcementEnumerator.GetSize() > 0)
            {
                sb.AppendFormat("=============철근정보=============");
                sb.Append(Environment.NewLine);
            }

            while (reinforcementEnumerator.MoveNext())
            {
                TSM.ModelObject reinforcementModelObject = reinforcementEnumerator.Current;

                if (reinforcementModelObject.GetType() == typeof(TSM.SingleRebar))
                {
                    TSM.SingleRebar singleRebar = reinforcementModelObject as TSM.SingleRebar;

                    sb.AppendFormat("1. Name : {0}", singleRebar.Name);
                    sb.Append(Environment.NewLine);

                    // singleRebar은 polygon으로 포인트를 나타낸다.
                    vertexList = singleRebar.Polygon.Points.Cast<TSG.Point>().ToList();

                    sb.AppendFormat("2. Points");
                    sb.Append(Environment.NewLine);

                    for (int i = 0, ii = vertexList.Count; i < ii; i++)
                    {
                        sb.Append(' ', 5);
                        sb.AppendFormat("{0} : {1}", i + 1, vertexList[i].ToString());
                        sb.Append(Environment.NewLine);
                    }

                    string onPlaneOffsets = "";

                    foreach (var onPlaneOffset in singleRebar.OnPlaneOffsets)
                    {
                        onPlaneOffsets += onPlaneOffset + " ";
                    }

                    sb.AppendFormat("3.1 Cover thickness On Plane : {0}", onPlaneOffsets);
                    sb.Append(Environment.NewLine);
                    sb.AppendFormat("3.2 Cover thickness From Plane : {0}", singleRebar.FromPlaneOffset.ToString());
                    sb.Append(Environment.NewLine);

                    sb.AppendFormat("4.1 Cover thickness Start OffsetType : {0}", singleRebar.StartPointOffsetType.ToString());
                    sb.Append(Environment.NewLine);
                    sb.AppendFormat("4.2 Cover thickness Start OffsetValue : {0}", singleRebar.StartPointOffsetValue.ToString());
                    sb.Append(Environment.NewLine);
                    sb.AppendFormat("5.1 Cover thickness End OffsetType: {0}", singleRebar.EndPointOffsetType.ToString());
                    sb.Append(Environment.NewLine);
                    sb.AppendFormat("5.2 Cover thickness End OffsetValue: {0}", singleRebar.EndPointOffsetValue.ToString());
                    sb.Append(Environment.NewLine);

                    List<TSM.RebarGeometry> rebarGeometryList = null;
                    rebarGeometryList = singleRebar.GetRebarGeometries(false).Cast<TSM.RebarGeometry>().ToList();

                    foreach (var rebarGeometry in rebarGeometryList)
                    {
                        vertexList = rebarGeometry.Shape.Points.Cast<TSG.Point>().ToList();

                        sb.AppendFormat("3. GeoMetry");
                        sb.Append(Environment.NewLine);

                        for (int i = 0, ii = vertexList.Count; i < ii; i++)
                        {
                            sb.Append(' ', 10);
                            sb.AppendFormat("{0} : {1}", i + 1, vertexList[i].ToString());
                            sb.Append(Environment.NewLine);
                        }
                    }
                }
                else if (reinforcementModelObject.GetType() == typeof(TSM.RebarGroup))
                {
                    TSM.RebarGroup rebarGroup = reinforcementModelObject as TSM.RebarGroup;
                    List<TSM.Polygon> polygonList = null;

                    sb.AppendFormat("1. Name : {0}", rebarGroup.Name);
                    sb.Append(Environment.NewLine);

                    sb.AppendFormat("2. StartPoint : {0}", rebarGroup.StartPoint.ToString());
                    sb.Append(Environment.NewLine);
                    sb.AppendFormat("3. EndPoint : {0}", rebarGroup.EndPoint.ToString());
                    sb.Append(Environment.NewLine);

                    sb.AppendFormat("4.1 Cover thickness On Plane : {0}", rebarGroup.StartFromPlaneOffset.ToString());
                    sb.Append(Environment.NewLine);
                    sb.AppendFormat("4.2 Cover thickness From Plane : {0}", rebarGroup.EndFromPlaneOffset.ToString());
                    sb.Append(Environment.NewLine);

                    sb.AppendFormat("5.1 Cover thickness Start OffsetType : {0}", rebarGroup.StartPointOffsetType.ToString());
                    sb.Append(Environment.NewLine);
                    sb.AppendFormat("5.2 Cover thickness Start OffsetValue : {0}", rebarGroup.StartPointOffsetValue.ToString());
                    sb.Append(Environment.NewLine);
                    sb.AppendFormat("6.1 Cover thickness End OffsetType: {0}", rebarGroup.EndPointOffsetType.ToString());
                    sb.Append(Environment.NewLine);
                    sb.AppendFormat("6.2 Cover thickness End OffsetValue: {0}", rebarGroup.EndPointOffsetValue.ToString());
                    sb.Append(Environment.NewLine);

                    polygonList = rebarGroup.Polygons.Cast<TSM.Polygon>().ToList();

                    sb.AppendFormat("7. Polygons");
                    sb.Append(Environment.NewLine);

                    for (int i = 0, ii = polygonList.Count; i < ii; i++)
                    {
                        vertexList = polygonList[i].Points.Cast<TSG.Point>().ToList();

                        for (int j = 0, jj = vertexList.Count; j < jj; j++)
                        {
                            sb.Append(' ', 5);
                            sb.AppendFormat("[{0}][{1}] : {2}", i + 1, j + 1, vertexList[j].ToString());
                            sb.Append(Environment.NewLine);
                        }
                    }

                    // 철근은 offsetvalue가 적용된 RebarGeometry로 받아서 사용해야 정확한 포인트가 나온다.
                    List<TSM.RebarGeometry> rebarGeometryList = null;
                    rebarGeometryList = rebarGroup.GetRebarGeometries(false).Cast<TSM.RebarGeometry>().ToList();

                    sb.AppendFormat("8. GeoMetry");
                    sb.Append(Environment.NewLine);

                    for (int i = 0, ii = rebarGeometryList.Count; i < ii; i++)
                    {
                        vertexList = rebarGeometryList[i].Shape.Points.Cast<TSG.Point>().ToList();

                        for (int j = 0, jj = vertexList.Count; j < jj; j++)
                        {
                            sb.Append(' ', 5);
                            sb.AppendFormat("[{0}][{1}] : {2}", i + 1, j + 1, vertexList[j].ToString());
                            sb.Append(Environment.NewLine);
                        }
                    }
                }
            }

        }
    }
}
