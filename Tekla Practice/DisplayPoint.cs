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

            TSM.ModelObject modelObject = picker.PickObject(TSM.UI.Picker.PickObjectEnum.PICK_ONE_PART, "부재를 선택하세요");

            if(null != modelObject)
            {
                this.CheckType(modelObject);
            } else
            {
                MessageBox.Show("부재가 선택되지 않았습니다.\r\n부재를 선택해주세요.");
                return;
            }

            // model 반영
            model.CommitChanges();
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
            List<TSM.Polygon> polygonList = null;
            StringBuilder sb = null;

            if (type == typeof(TSM.Beam)) // 1. 빔(컬럼, 거더, 빔)
            {
                TSM.Beam beam = modelObject as TSM.Beam;

                // 시작, 끝 포인트
                startPoint = beam.StartPoint;
                endPoint = beam.EndPoint;

                // beam의 꼭지점 가져오기
                vertexList = this.GetVertexList(beam);

                // 결과 텍스트 작성
                sb = new StringBuilder();
                sb.AppendFormat("1. Name : {0}", beam.Name);
                sb.Append(Environment.NewLine);
                sb.AppendFormat("2. StartPoint : {0}", startPoint.ToString());
                sb.Append(Environment.NewLine);
                sb.AppendFormat("3. EndPoint : {0}", endPoint.ToString());
                sb.Append(Environment.NewLine);
                sb.AppendFormat("4. Vertex List");
                sb.Append(Environment.NewLine);

                foreach (var item in vertexList)
                {
                    sb.AppendFormat("vertex : {0}", item.ToString());
                    sb.Append(Environment.NewLine);
                }

                // 단일철근, 그룹철근 체크
                TSM.ModelObjectEnumerator modelObjectEnumerator = beam.GetReinforcements();

                if(modelObjectEnumerator.GetSize() > 0)
                {
                    sb.AppendFormat("=============철근정보=============");
                    sb.Append(Environment.NewLine);
                }

                while (modelObjectEnumerator.MoveNext())
                {
                    TSM.ModelObject childModelObject = modelObjectEnumerator.Current;

                    if (childModelObject.GetType() == typeof(TSM.SingleRebar))
                    {
                        TSM.SingleRebar singleRebar = childModelObject as TSM.SingleRebar;

                        sb.AppendFormat("1. Name : {0}", singleRebar.Name);
                        sb.Append(Environment.NewLine);

                        // singleRebar은 polygon으로 포인트를 나타낸다.
                        vertexList = singleRebar.Polygon.Points.Cast<TSG.Point>().ToList();

                        foreach (var item in vertexList)
                        {
                            sb.AppendFormat("Points : {0}", item.ToString());
                            sb.Append(Environment.NewLine);
                        }

                        List<TSM.RebarGeometry> rebarGeometryList = null;
                        rebarGeometryList = singleRebar.GetRebarGeometries(false).Cast<TSM.RebarGeometry>().ToList();

                        foreach (var rebarGeometry in rebarGeometryList)
                        {
                            vertexList = rebarGeometry.Shape.Points.Cast<TSG.Point>().ToList();

                            foreach (var item in vertexList)
                            {
                                sb.AppendFormat("GeoMetry : {0}", item.ToString());
                                sb.Append(Environment.NewLine);
                            }
                        }
                    }
                    else if (childModelObject.GetType() == typeof(TSM.RebarGroup))
                    {
                        TSM.RebarGroup rebarGroup = childModelObject as TSM.RebarGroup;

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
                        sb.AppendFormat("5.1 Cover thickness Start OffsetValue : {0}", rebarGroup.StartPointOffsetValue.ToString());
                        sb.Append(Environment.NewLine);
                        sb.AppendFormat("5.2 Cover thickness End OffsetType: {0}", rebarGroup.EndPointOffsetType.ToString());
                        sb.Append(Environment.NewLine);
                        sb.AppendFormat("5.2 Cover thickness End OffsetValue: {0}", rebarGroup.EndPointOffsetValue.ToString());
                        sb.Append(Environment.NewLine);


                        //polygonList = rebarGroup.Polygons.Cast<TSM.Polygon>().ToList();

                        //foreach(var polygon in polygonList)
                        //{
                        //    vertexList = polygon.Points.Cast<TSG.Point>().ToList();

                        //    foreach (var item in vertexList)
                        //    {
                        //        sb.AppendFormat("Points : {0}", item.ToString());
                        //        sb.Append(Environment.NewLine);
                        //    }
                        //}

                        // 철근은 offsetvalue가 적용된 RebarGeometry로 받아서 사용해야 정확한 포인트가 나온다.
                        List<TSM.RebarGeometry> rebarGeometryList = null;
                        rebarGeometryList = rebarGroup.GetRebarGeometries(false).Cast<TSM.RebarGeometry>().ToList();

                        foreach (var rebarGeometry in rebarGeometryList)
                        {
                            vertexList = rebarGeometry.Shape.Points.Cast<TSG.Point>().ToList();

                            foreach (var item in vertexList)
                            {
                                sb.AppendFormat("GeoMetry : {0}", item.ToString());
                                sb.Append(Environment.NewLine);
                            }
                        }
                    }
                }

                TextBoxPointOfPart.Text = sb.ToString();

            } else if(type == typeof(TSM.PolyBeam)) // 2. 폴리빔
            {
                TSM.PolyBeam polyBeam = modelObject as TSM.PolyBeam;

                // 포인트 리스트
                ArrayList pointList = polyBeam.Contour.ContourPoints;
                
                // polybeam의 꼭지점 가져오기
                vertexList = this.GetVertexList(polyBeam);

                // 결과 텍스트 작성
                sb = new StringBuilder();
                sb.AppendFormat("1. Name : {0}", polyBeam.Name);
                sb.Append(Environment.NewLine);
                

                foreach (var item in pointList)
                {
                    sb.AppendFormat("ContourPoints : {0}", item.ToString());
                    sb.Append(Environment.NewLine);
                }               

                sb.AppendFormat("2. Vertex List");
                sb.Append(Environment.NewLine);

                foreach (var item in vertexList)
                {
                    sb.AppendFormat("vertex : {0}", item.ToString());
                    sb.Append(Environment.NewLine);
                }

                TextBoxPointOfPart.Text = sb.ToString();
            } else if(type == typeof(TSM.ContourPlate)) // 3. 플레이트(슬래브, 월)
            {
                TSM.ContourPlate contourPlate = modelObject as TSM.ContourPlate;

                // 포인트 리스트
                ArrayList pointList = contourPlate.Contour.ContourPoints;

                // polybeam의 꼭지점 가져오기
                vertexList = this.GetVertexList(contourPlate);

                // 결과 텍스트 작성
                sb = new StringBuilder();
                sb.AppendFormat("1. Name : {0}", contourPlate.Name);
                sb.Append(Environment.NewLine);


                foreach (var item in pointList)
                {
                    sb.AppendFormat("ContourPoints : {0}", item.ToString());
                    sb.Append(Environment.NewLine);
                }

                sb.AppendFormat("2. Vertex List");
                sb.Append(Environment.NewLine);

                foreach (var item in vertexList)
                {
                    sb.AppendFormat("vertex : {0}", item.ToString());
                    sb.Append(Environment.NewLine);
                }

                TextBoxPointOfPart.Text = sb.ToString();
            } else if(type == typeof(TSM.Reinforcement)) // 4. 철근(단일철근, 철근그룹, 메쉬)
            {
                
            }
        }

        /// <summary>
        /// 부재의 vertex list를 찾아서 return한다.
        /// Part Class(Beam, PolyBeam, ContourPlate Class만 처리한다)
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        private List<TSG.Point> GetVertexList(TSM.Part part)
        {
            List<TSG.Point> faceList = new List<TSG.Point>();
            List<TSG.Point> vertexList = new List<TSG.Point>();
            List<TSG.Point> list = new List<TSG.Point>();

            // 부재의 좌표정보 가져오기
            TSM.Solid solid = part.GetSolid();

            TSS.FaceEnumerator faceenum = solid.GetFaceEnumerator();

            while (faceenum.MoveNext())
            {
                // 면 정보 추출
                TSS.Face face = faceenum.Current as TSS.Face;

                if (null != face)
                {
                    faceList.Add(face.Normal);
                    TSS.LoopEnumerator loopenum = face.GetLoopEnumerator();

                    while (loopenum.MoveNext())
                    {
                        TSS.Loop loop = loopenum.Current as TSS.Loop;

                        if (null != loop)
                        {
                            // 꼭지점 정보 가져오기
                            TSS.VertexEnumerator vertexenum = loop.GetVertexEnumerator();
                            while (vertexenum.MoveNext())
                            {
                                TSG.Point vertex = vertexenum.Current;
                                if (null != vertex)
                                {
                                    vertexList.Add(vertex);
                                }
                            }

                        }
                    }
                }
            }

            // 중복제거
            IEnumerable<TSG.Point> distinctVertex = vertexList.Distinct();
            // list clear
            list.Clear();

            // 중복제거한 포인트를 list에 추가
            foreach (var vertex in distinctVertex)
            {
                list.Add(vertex);
            }

            // list 정렬
            list.Sort();

            return list;
        }
    }
}
