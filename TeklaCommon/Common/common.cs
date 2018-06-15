using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using TSG = Tekla.Structures.Geometry3d;
using TSM = Tekla.Structures.Model;
using TSS = Tekla.Structures.Solid;
using TSC = Tekla.Structures.Catalogs;
using TSD = Tekla.Structures.Datatype;

namespace TeklaCommon.Common
{
    /// <summary>
    /// Tekla Component 개발에 사용되는 공통 클래스
    /// </summary>
    class Common
    {
        public static List<TSS.Face> GetFaceList(TSM.Part part)
        {
            List<TSS.Face> faceList = new List<TSS.Face>();

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

            return resultList;
        }

        /// <summary>
        /// 부재 간 간섭여부 체크 및 간섭영역(AABB)의 min, max, center point를 List로 return
        /// 단, 간섭
        /// </summary>
        /// <param name="model"></param>
        /// <param name="part1"></param>
        /// <param name="part2"></param>
        /// <param name="intersectionList"></param>
        /// <returns></returns>
        public static bool IsClashCheck(TSM.Model model, TSM.Part part1, TSM.Part part2, out TSG.Point minPoint, out TSG.Point maxPoint, out TSG.Point centerPoint)
        {
            minPoint = new TSG.Point();
            maxPoint = new TSG.Point();
            centerPoint = new TSG.Point();
            bool isClash = false;
            TSM.ClashCheckHandler clashCheckHandler = model.GetClashCheckHandler();

            // 선택한 부재들을 비교해서 간섭되었는지 체크
            ArrayList list = clashCheckHandler.GetIntersectionBoundingBoxes(part1.Identifier, part2.Identifier);

            // count가 0이면 간섭되지 않음
            if (null == list || list.Count < 1)
            {
                isClash = false;
            } else
            {
                TSG.AABB aabb = list[0] as TSG.AABB;

                minPoint = aabb.MinPoint;
                maxPoint = aabb.MaxPoint;
                centerPoint = aabb.GetCenterPoint();

                isClash = true;
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
            // 단위는 라디안이며 degree는 회전할 각도
            double angle = (Math.PI / 180.0) * degree;
            // 벡터 파라미터는 회전축에 해당하는 벡터에 1을 입력하면 축방향을 기준으로 시계방향으로 회전.
            // 기본 parameter는 z축을 기준으로 반시계 방향으로 회전하는 즉 X,Y만 바뀐다.
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


        /// <summary>
        /// Beam 생성
        /// </summary>
        /// <param name="name">이름</param>
        /// <param name="color">클래스</param>
        /// <param name="material">재질</param>
        /// <param name="height">높이</param>
        /// <param name="width">폭</param>
        /// <param name="startPoint">시작점</param>
        /// <param name="endPoint">끝점</param>
        /// <param name="assemblyStartNumber">시작넘버</param>
        /// <param name="assemblyPrefix">접두사</param>
        /// <param name="positionDepth">위치 - 깊이</param>
        /// <param name="positionDepthOffset">위치 - 깊이 오프셋</param>
        /// <param name="positionPlane">위치 - 평면</param>
        /// <param name="positionPlaneOffset">위치 - 평면 오프셋</param>
        /// <param name="positionRotation">위치 - 회전</param>
        /// <param name="positionRotationOffset">위치 - 회전 오프셋</param>
        /// <returns></returns>
        public static TSM.Beam CreateBeam(string name, string color, string material, double height, double width, TSG.Point startPoint, TSG.Point endPoint, 
            int assemblyStartNumber = 1, string assemblyPrefix = "A",
            TSM.Position.DepthEnum positionDepth = TSM.Position.DepthEnum.MIDDLE, double positionDepthOffset = 0.0,
            TSM.Position.PlaneEnum positionPlane = TSM.Position.PlaneEnum.MIDDLE, double positionPlaneOffset = 0.0,
            TSM.Position.RotationEnum positionRotation = TSM.Position.RotationEnum.FRONT, double positionRotationOffset = 0.0
            )
        {
            TSM.Beam beam = new TSM.Beam();

            beam.Name = name;
            beam.Class = color;
            beam.Material.MaterialString = material;
            beam.Profile.ProfileString = height + "X" + width;

            beam.StartPoint = startPoint;
            beam.EndPoint = endPoint;

            beam.AssemblyNumber.StartNumber = assemblyStartNumber;
            beam.AssemblyNumber.Prefix = assemblyPrefix;
            beam.CastUnitType = TSM.Part.CastUnitTypeEnum.PRECAST;

            beam.Position.Depth = positionDepth;
            beam.Position.DepthOffset = positionDepthOffset;
            beam.Position.Plane = positionPlane;
            beam.Position.PlaneOffset = positionPlaneOffset;
            beam.Position.Rotation = positionRotation;
            beam.Position.RotationOffset = positionRotationOffset;

            bool isInsert = beam.Insert();

            return beam;
        }

        public static TSM.ContourPlate CreateContourPlate(string name, string color, string material, double length)
        {
            TSM.ContourPlate contourPlate = new TSM.ContourPlate();

            contourPlate.Name = name;
            contourPlate.Class = color;
            contourPlate.Material.MaterialString = material;
            contourPlate.Profile.ProfileString = string.Format("{0}", length);
        }

        /// <summary>
        /// 철근그룹 생성
        /// </summary>
        /// <param name="grade">등급</param>
        /// <param name="size">크기</param>
        /// <param name="usage">메인 바, 타이 또는 스트럽</param>
        /// <param name="name">이름</param>
        /// <param name="color">클래스</param>
        /// <param name="part">철근그룹이 생성될 part</param>
        /// <param name="numberingStartNumber">시작넘버</param>
        /// <param name="numberingPrefix">접두사</param>
        /// <param name="startPoint">보강범위 시작점</param>
        /// <param name="endPoint">보강범위 끝점</param>
        /// <param name="polygons">철근형상 포인트</param>
        /// <param name="distances">철근간격</param>
        /// <param name="onPlaneOffsets">피복두께 - 평면</param>
        /// <param name="excludeType">그룹에 생성되지 않는 철근</param>
        /// <param name="startHookShape">시작 후크 형상</param>
        /// <param name="startHookAngle">시작 후크 각도</param>
        /// <param name="startHookRadius">시작 후크 반경</param>
        /// <param name="startHookLength">시작 후크 길이</param>
        /// <param name="endHookShape">끝 후크 형상</param>
        /// <param name="endHookAngle">끝 후크 각도</param>
        /// <param name="endHookRadius">끝 후크 반경</param>
        /// <param name="endHookLength">끝 후크 길이</param>
        /// <param name="startPointOffsetType">피복타입(피복두께, 레그길이)</param>
        /// <param name="startPointOffsetValue">시작 피복</param>
        /// <param name="endPointOffsetType">피복타입(피복두께, 레그길이)</param>
        /// <param name="endPointOffsetValue">끝 피복</param>
        /// <returns></returns>
        public static TSM.RebarGroup CreateRebarGroup(string grade, string size, string usage, string name, int color, TSM.Part part, int numberingStartNumber, string numberingPrefix,
            TSG.Point startPoint, TSG.Point endPoint, List<TSM.Polygon> polygons,
            ArrayList onPlaneOffsets,
            TSD.DistanceList distances, TSM.BaseRebarGroup.RebarGroupSpacingTypeEnum spacingType = TSM.BaseRebarGroup.RebarGroupSpacingTypeEnum.SPACING_TYPE_EXACT_SPACINGS,
            TSM.BaseRebarGroup.ExcludeTypeEnum excludeType = TSM.BaseRebarGroup.ExcludeTypeEnum.EXCLUDE_TYPE_NONE,
            TSM.RebarHookData.RebarHookShapeEnum startHookShape = TSM.RebarHookData.RebarHookShapeEnum.NO_HOOK, double startHookAngle = 0.0, double startHookRadius = 0.0, double startHookLength = 0.0, 
            TSM.RebarHookData.RebarHookShapeEnum endHookShape = TSM.RebarHookData.RebarHookShapeEnum.NO_HOOK, double endHookAngle = 0.0, double endHookRadius = 0.0, double endHookLength = 0.0,
            TSM.Reinforcement.RebarOffsetTypeEnum startPointOffsetType = TSM.Reinforcement.RebarOffsetTypeEnum.OFFSET_TYPE_COVER_THICKNESS, double startPointOffsetValue = 0.0, TSM.Reinforcement.RebarOffsetTypeEnum endPointOffsetType = TSM.Reinforcement.RebarOffsetTypeEnum.OFFSET_TYPE_COVER_THICKNESS, double endPointOffsetValue = 0.0
            )
        {
            TSM.RebarGroup rebarGroup = new TSM.RebarGroup();

            //TeklaRebar TeklaRebar = new TeklaRebar(size);
            TSC.RebarItem rebarItem = new TSC.RebarItem();
            bool isRebarSelect = rebarItem.Select(grade, size, usage);

            if (isRebarSelect)
            {
                rebarGroup.Grade = grade;
                rebarGroup.Size = size;

                rebarGroup.RadiusValues.Add(rebarItem.BendRadius);

                rebarGroup.Name = name;
                rebarGroup.Class = color;
                rebarGroup.Father = part;

                rebarGroup.NumberingSeries.StartNumber = numberingStartNumber;
                rebarGroup.NumberingSeries.Prefix = numberingPrefix;

                rebarGroup.StartPoint = startPoint;
                rebarGroup.EndPoint = endPoint;
                rebarGroup.Polygons.AddRange(polygons);

                rebarGroup.ExcludeType = excludeType;

                rebarGroup.StartHook.Shape = startHookShape;
                rebarGroup.StartHook.Angle = startHookAngle;
                rebarGroup.StartHook.Radius = startHookRadius;
                rebarGroup.StartHook.Length = startHookLength;

                rebarGroup.EndHook.Shape = endHookShape;
                rebarGroup.EndHook.Angle = endHookAngle;
                rebarGroup.EndHook.Radius = endHookRadius;
                rebarGroup.EndHook.Length = endHookLength;

                rebarGroup.FromPlaneOffset = distances[0].Value - (rebarItem.NominalDiameter * 0.5);
                rebarGroup.OnPlaneOffsets = onPlaneOffsets;

                rebarGroup.SpacingType = spacingType;
                rebarGroup.Spacings.AddRange(distances.Skip(1).Select(o => o.Value).ToArray());

                rebarGroup.StartPointOffsetType = startPointOffsetType;
                rebarGroup.StartPointOffsetValue = startPointOffsetValue;

                rebarGroup.EndPointOffsetType = endPointOffsetType;
                rebarGroup.EndPointOffsetValue = endPointOffsetValue;

                bool isInsert = rebarGroup.Insert();
            }

            return rebarGroup;
        }


        /**
         * 철근규격
         * */
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

    
