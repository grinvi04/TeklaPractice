using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Tekla.Structures;
using Tekla.Structures.Dialog;
using TSG = Tekla.Structures.Geometry3d;
using TSM = Tekla.Structures.Model;
using TSS = Tekla.Structures.Solid;

namespace TeklaCommon
{
    /// <summary>
    /// Tekla Component 개발에 사용되는 공통 클래스
    /// </summary>
    public class YWCommon
    {
        private static TSM.Model model = new TSM.Model();
        private static TSM.UI.Picker picker = new TSM.UI.Picker();

        /// <summary>
        /// Tekla Structures 프로세스와의 연결여부를 체크한 후 TSM.Model Instance를 return하는 메서드
        /// </summary>
        public static TSM.Model GetModel
        {
            get
            {
                return model;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public static TSM.UI.Picker GetPicker
        {
            get
            {
                return picker;
            }
        }

        /// <summary>
        /// Part Instance로 만든 모델의 실제 객체의 Solid에서 단일면들을 List에 담아 return하는 메서드
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public static List<TSS.Face> GetFaceList(TSM.Part part)
        {
            List<TSS.Face> faceList = new List<TSS.Face>();

            try
            {
                if (null == part)
                {
                    System.Console.WriteLine("vertex를 조회할 부재가 없습니다.");
                    return faceList;
                }

                // 부재의 좌표정보 가져오기
                TSM.Solid solid = part.GetSolid();

                TSS.FaceEnumerator faceenum = solid.GetFaceEnumerator();

                while (faceenum.MoveNext())
                {
                    // 면 정보 추출
                    TSS.Face face = faceenum.Current as TSS.Face;

                    if (null != face)
                    {
                        faceList.Add(face);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorDialog.Show(ex, ErrorDialog.Severity.ERROR);
            }
            finally
            {
            }

            return faceList;
        }

        /// <summary>
        /// 부재의 vertex list를 찾아서 return한다.
        /// Part Class(Beam, PolyBeam, ContourPlate Class만 처리한다)
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public static List<TSG.Point> GetVertexList(TSM.Part part)
        {
            List<TSG.Point> list = new List<TSG.Point>();
            List<TSG.Point> resultList = new List<TSG.Point>();
            List<TSG.Point> vertexList = new List<TSG.Point>();

            try
            {
                if (null == part)
                {
                    System.Console.WriteLine("vertex를 조회할 부재가 없습니다.");
                    return list;
                }

                // 부재의 좌표정보 가져오기
                TSM.Solid solid = part.GetSolid();

                TSS.FaceEnumerator faceenum = solid.GetFaceEnumerator();

                while (faceenum.MoveNext())
                {
                    // 면 정보 추출
                    TSS.Face face = faceenum.Current as TSS.Face;

                    if (null != face)
                    {
                        TSS.LoopEnumerator loopenum = face.GetLoopEnumerator();

                        while (loopenum.MoveNext())
                        {
                            if (loopenum.Current is TSS.Loop loop)
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

                // 중복제거한 포인트를 list에 추가
                foreach (var vertex in distinctVertex)
                {
                    list.Add(vertex);
                }

                // list 정렬
                resultList = list.OrderBy(item => item.X).ThenBy(item => item.Y).ThenBy(item => item.Z).ToList();
            }
            catch (Exception ex)
            {
                ErrorDialog.Show(ex, ErrorDialog.Severity.ERROR);
            }
            finally
            {
            }

            return resultList;
        }

        /// <summary>
        /// 부재 간 간섭여부 체크 및 간섭영역(AABB) 객체 out
        /// </summary>
        /// <param name="identifier1"></param>
        /// <param name="identifier2"></param>
        /// <param name="aabb"></param>
        /// <returns></returns>
        public static bool IsClashCheck(Identifier identifier1, Identifier identifier2, out TSG.AABB aabb)
        {
            aabb = null;
            bool isClash = false;
            TSM.ClashCheckHandler clashCheckHandler = new TSM.Model().GetClashCheckHandler();

            try
            {
                clashCheckHandler.RunClashCheck();
                // 선택한 부재들을 비교해서 간섭되었는지 체크
                ArrayList list = clashCheckHandler.GetIntersectionBoundingBoxes(identifier1, identifier2);

                TSM.ClashCheckData clashCheckData = new TSM.ClashCheckData();
                TSM.ModelObject modelObject1 = clashCheckData.Object1;
                TSM.ModelObject modelObject2 = clashCheckData.Object2;

                clashCheckHandler.StopClashCheck();
                // count가 0이면 간섭되지 않음
                if (null != list || list.Count > 0)
                {
                    aabb = list[0] as TSG.AABB;
                    isClash = true;
                }
            }
            catch (Exception ex)
            {
                ErrorDialog.Show(ex, ErrorDialog.Severity.ERROR);
            }
            finally
            {
            }

            return isClash;
        }

        /// <summary>
        /// 주어진 각도와 회전할 축을 기준으로 축방향의 시계방향으로 vector를 회전시킨다.
        /// ex) TSG.Vector v1 = new TSG.Vector(0, 0, 0);
        ///     TSG.Vector v2 = TC.Common.Rotate(v1, 45);
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="degree"></param>
        /// <param name="axisX"></param>
        /// <param name="axisY"></param>
        /// <param name="axisZ"></param>
        /// <returns></returns>
        public static TSG.Vector Rotate(TSG.Vector vector, double degree = 90.0, int axisX = 0, int axisY = 0, int axisZ = 1)
        {
            TSG.Point point = new TSG.Point();

            try
            {
                // 단위는 라디안이며 degree는 회전할 각도
                double angle = (Math.PI / 180.0) * degree;
                // 벡터 파라미터는 회전축에 해당하는 벡터에 1을 입력하면 축방향을 기준으로 시계방향으로 회전.
                // 기본 parameter는 z축을 기준으로 반시계 방향으로 회전하는 즉 X,Y만 바뀐다.
                TSG.Matrix rotateMatrix = TSG.MatrixFactory.Rotate(angle, new TSG.Vector(axisX, axisY, axisZ));

                // 매트릭스는 [4,3]으로 고정된 행렬이다.
                TSG.Matrix matrix = rotateMatrix.GetTranspose();

                // 회전
                point = matrix.Transform(vector);
            }
            catch (Exception ex)
            {
                ErrorDialog.Show(ex, ErrorDialog.Severity.ERROR);
            }
            finally
            {
            }
            return new TSG.Vector(point);
        }

        /// <summary>
        /// OBB 클래스를 사용해서 부재의 중심포인트를 구할 수 있다. 또한 중심포인트에서 부재의 크기를 구할 수 있는데
        /// extent0, extent1, extent2가 중심포인트에서 부재의 모양까지 Axis0, Axis1, Axis2 방향으로 X, Y, Z만큼 떨어져있다.
        /// 좌표계변환을 이용해야 한다.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="part"></param>
        /// <returns></returns>
        public static TSG.OBB CreateOrientedBoundingBox(TSM.Model model, TSM.Part part)
        {
            TSG.OBB obb = new TSG.OBB();

            try
            {
                if (null != part)
                {
                    TSM.WorkPlaneHandler workPlaneHandler = model.GetWorkPlaneHandler();
                    TSM.TransformationPlane transformationPlane = workPlaneHandler.GetCurrentTransformationPlane();

                    TSM.Solid solid = part.GetSolid();

                    TSG.Point minPointInCurrentPlane = solid.MinimumPoint;
                    TSG.Point maxPointInCurrentPlane = solid.MaximumPoint;
                    TSG.Point centerPoint = CalculateCenterPoint(minPointInCurrentPlane, maxPointInCurrentPlane);

                    TSG.CoordinateSystem coordinateSystem = part.GetCoordinateSystem();
                    TSM.TransformationPlane localtransformationPlane = new TSM.TransformationPlane(coordinateSystem);
                    workPlaneHandler.SetCurrentTransformationPlane(localtransformationPlane);

                    solid = part.GetSolid();

                    TSG.Point minPoint = solid.MinimumPoint;
                    TSG.Point maxPoint = solid.MaximumPoint;

                    double extent0 = (maxPoint.X - minPoint.X) / 2;
                    double extent1 = (maxPoint.Y - minPoint.Y) / 2;
                    double extent2 = (maxPoint.Z - minPoint.Z) / 2;

                    workPlaneHandler.SetCurrentTransformationPlane(transformationPlane);
                    obb = new TSG.OBB(centerPoint, coordinateSystem.AxisX, coordinateSystem.AxisY, coordinateSystem.AxisX.Cross(coordinateSystem.AxisY), extent0, extent1, extent2);
                }
            }
            catch (Exception ex)
            {
                ErrorDialog.Show(ex, ErrorDialog.Severity.ERROR);
            }
            finally
            {
            }

            return obb;
        }

        /// <summary>
        /// CreateOrientedBoundingBox 메서드에서 중심Point를 찾기 위한 메서드
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private static TSG.Point CalculateCenterPoint(TSG.Point min, TSG.Point max)
        {
            double x = 0.0, y = 0.0, z = 0.0;

            try
            {
                x = min.X + ((max.X - min.X) / 2);
                y = min.Y + ((max.Y - min.Y) / 2);
                z = min.Z + ((max.Z - min.Z) / 2);
            }
            catch (Exception ex)
            {
                ErrorDialog.Show(ex, ErrorDialog.Severity.ERROR);
            }
            finally
            {
            }

            return new TSG.Point(x, y, z);
        }

        /// <summary>
        /// enum 정의 시 값의 Description 속성 문자열을 가져온다.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription(Enum value)
        {
            return value
                .GetType()
                .GetMember(value.ToString())
                .FirstOrDefault()
                ?.GetCustomAttribute<DescriptionAttribute>()
                ?.Description;
        }

        /// <summary>
        /// rebar_database파일의 Usage 속성
        /// </summary>
        internal enum eUsage
        {
            [Description("main")]
            main,

            [Description("tie/stirrup")]
            tie_stirrup,
        }

        /// <summary>
        /// 철근 규격
        /// </summary>
        internal enum eDeformedBarSize
        {
            HD10,
            HD13,
            HD16,
            HD19,
            HD22,
            HD25,
            HD29,
            HD32,
            HD35,
            HD38,
            HD41,
            HD51,
            SHD10,
            SHD13,
            SHD16,
            SHD19,
            SHD22,
            SHD25,
            SHD29,
            SHD32,
            SHD35,
            SHD38,
            SHD41,
            SHD51,
            UHD10,
            UHD13,
            UHD16,
            UHD19,
            UHD22,
            UHD25,
            UHD29,
            UHD32,
            UHD35,
            UHD38,
            UHD41,
            UHD51,
        }

        /// <summary>
        /// 강선 규격
        /// </summary>
        internal enum eStrandSize
        {
            D9_5,
            D11_1,
            D12_7,
            D15_2,
        }
    }
}