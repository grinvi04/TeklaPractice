using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using TS = Tekla.Structures;
using TSG = Tekla.Structures.Geometry3d;
using TSM = Tekla.Structures.Model;
using TSS = Tekla.Structures.Solid;

namespace TeklaCommon.Common
{
    /// <summary>
    /// Tekla Component 개발에 사용되는 공통 클래스
    /// </summary>
    class Common
    {
        /// <summary>
        /// 부재의 vertex list를 찾아서 return한다.
        /// Part Class(Beam, PolyBeam, ContourPlate Class만 처리한다)
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public static List<TSG.Point> GetVertexList(TSM.Part part)
        {
            List<TSG.Point> list = new List<TSG.Point>();
            List<TSG.Point> faceList = new List<TSG.Point>();
            List<TSG.Point> vertexList = new List<TSG.Point>();

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
                    faceList.Add(face.Normal);
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

        public static bool IsClashCheck(TSM.Model model, TSM.Part part1, TSM.Part part2, out ArrayList intersectionList)
        {
            bool isClash = false;
            TSM.ClashCheckHandler clashCheckHandler = model.GetClashCheckHandler();

            // 선택한 부재들을 비교해서 간섭되었는지 체크
            intersectionList = clashCheckHandler.GetIntersectionBoundingBoxes(part1.Identifier, part2.Identifier);
            
            // count가 0이면 간섭되지 않음
            if (null == intersectionList || intersectionList.Count < 1)
            {
                isClash = false;
            } else
            {
                isClash = true;
            }

            return isClash;
        }


        /// <summary>
        /// 주어진 각도와 회전할 축을 기준으로 반시계방향으로 vector를 회전시킨다.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="degree"></param>
        /// <param name="axisX"></param>
        /// <param name="axisY"></param>
        /// <param name="axisZ"></param>
        /// <returns></returns>
        public static TSG.Vector Rotate(TSG.Vector vector, double degree = 90.0, int axisX = 0, int axisY = 0, int axisZ = 1)
        {
            // 단위는 라디안이며 90은 회전할 각도
            double angle = (Math.PI / 180.0) * degree;
            // 벡터 파라미터는 회전축에 해당하는 벡터에 1을 입력하면 축방향을 기준으로 반시계방향으로 회전.
            // 아래는 z축을 기준으로 반시계 방향으로 회전하는 즉 X,Y만 바뀐다.
            TSG.Matrix rotateMatrix = TSG.MatrixFactory.Rotate(angle, new TSG.Vector(axisX, axisY, axisZ));

            // 매트릭스는 [4,3]으로 고정된 행렬이다.
            TSG.Matrix matrix = rotateMatrix.GetTranspose();

            // 회전
            TSG.Point point = matrix.Transform(vector);

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

                // beam의 꼭지점 list를 가져온다.
                //TSG.Point[] list = obb.ComputeVertices();
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
            double x = min.X + ((max.X - min.X) / 2);
            double y = min.Y + ((max.Y - min.Y) / 2);
            double z = min.Z + ((max.Z - min.Z) / 2);

            return new TSG.Point(x, y, z);
        }

        public static TSM.Beam CreateBeam(TSG.Point startPoint, TSG.Point endPoint, TSM.Position.PlaneEnum plane = TSM.Position.PlaneEnum.MIDDLE, double height = 600.0, double width = 600.0)
        {            
            TSM.Beam beam = new TSM.Beam();

            beam.Name = "beam";
            beam.StartPoint = startPoint;
            beam.EndPoint = endPoint;

            beam.Class = "6";
            beam.Material.MaterialString = "C24";

            beam.Position.Depth = TSM.Position.DepthEnum.FRONT;
            beam.Position.Plane = plane;
            beam.Position.Rotation = TSM.Position.RotationEnum.TOP;
            beam.Profile.ProfileString = height + "X" + width;
            bool isInsert = beam.Insert();

            return beam;
        }
    }
}
