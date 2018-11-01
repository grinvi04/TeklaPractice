using System;
using System.Collections.Generic;
using Tekla.Structures.Dialog;
using TSG = Tekla.Structures.Geometry3d;
using TSM = Tekla.Structures.Model;

namespace TeklaCommon
{
    /// <summary>
    /// TSM.Boolean의 하위의 클래스를 모듈화 한 클래스
    /// <para>
    /// Part 클래스는 모델의 일부를 나타냅니다. 파트는 보, 폴리 빔 또는 윤곽 플레이트 일 수 있습니다.
    /// </para>
    /// </summary>
    public class YWPart
    {
        /// <summary>
        /// Beam 생성
        /// <para>
        /// Beam 클래스는 모델의 단일 보를 나타냅니다. 보에는 단일 시작점과 끝점이 있습니다.
        /// </para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="color"></param>
        /// <param name="material"></param>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="startPointOffset"></param>
        /// <param name="endPointOffset"></param>
        /// <param name="type"></param>
        /// <param name="positionDepth"></param>
        /// <param name="positionDepthOffset"></param>
        /// <param name="positionPlane"></param>
        /// <param name="positionPlaneOffset"></param>
        /// <param name="positionRotation"></param>
        /// <param name="positionRotationOffset"></param>
        /// <param name="assemblyStartNumber"></param>
        /// <param name="assemblyPrefix"></param>
        /// <param name="partStartNumber"></param>
        /// <param name="partPrefix"></param>
        /// <param name="castUnitType"></param>
        /// <param name="pourPhase"></param>
        /// <param name="finish"></param>
        /// <returns></returns>
        public static TSM.Beam CreateBeam(string name, string color, string material, double height, double width,
            TSG.Point startPoint, TSG.Point endPoint, TSM.Offset startPointOffset = null, TSM.Offset endPointOffset = null,
            TSM.Beam.BeamTypeEnum type = TSM.Beam.BeamTypeEnum.BEAM,
            TSM.Position.DepthEnum positionDepth = TSM.Position.DepthEnum.MIDDLE, double positionDepthOffset = 0.0,
            TSM.Position.PlaneEnum positionPlane = TSM.Position.PlaneEnum.MIDDLE, double positionPlaneOffset = 0.0,
            TSM.Position.RotationEnum positionRotation = TSM.Position.RotationEnum.FRONT, double positionRotationOffset = 0.0,
            int assemblyStartNumber = 1, string assemblyPrefix = "", int partStartNumber = 1, string partPrefix = "",
            TSM.Part.CastUnitTypeEnum castUnitType = TSM.Part.CastUnitTypeEnum.PRECAST, int pourPhase = 0,
            string finish = ""
            )
        {
            TSM.Beam beam = new TSM.Beam(type);

            try
            {
                beam.Name = name;
                beam.Class = color;
                beam.Material.MaterialString = material;
                beam.Profile.ProfileString = height + "X" + width;
                beam.StartPoint = startPoint;
                beam.StartPointOffset = startPointOffset;
                beam.EndPoint = endPoint;
                beam.EndPointOffset = endPointOffset;

                beam.Position.Depth = positionDepth;
                beam.Position.DepthOffset = positionDepthOffset;
                beam.Position.Plane = positionPlane;
                beam.Position.PlaneOffset = positionPlaneOffset;
                beam.Position.Rotation = positionRotation;
                beam.Position.RotationOffset = positionRotationOffset;

                beam.AssemblyNumber.StartNumber = assemblyStartNumber;
                beam.AssemblyNumber.Prefix = assemblyPrefix;
                beam.PartNumber.StartNumber = partStartNumber;
                beam.PartNumber.Prefix = partPrefix;

                beam.CastUnitType = castUnitType;
                beam.PourPhase = beam.CastUnitType == TSM.Part.CastUnitTypeEnum.CAST_IN_PLACE ? pourPhase : 0;
                beam.Finish = finish;

                beam.Insert();
            }
            catch (Exception ex)
            {
                ErrorDialog.Show(ex, ErrorDialog.Severity.ERROR);
            }
            finally
            {
                new TSM.Model().CommitChanges();
            }

            return beam;
        }

        /// <summary>
        /// ContourPlate 생성
        /// <para>
        /// ContourPlate 클래스는 콘크리트 슬래브와 같이 윤곽선으로 만들어진 파트를 나타냅니다.
        /// </para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="color"></param>
        /// <param name="material"></param>
        /// <param name="length"></param>
        /// <param name="contourPoints"></param>
        /// <param name="positionDepth"></param>
        /// <param name="positionDepthOffset"></param>
        /// <param name="assemblyStartNumber"></param>
        /// <param name="assemblyPrefix"></param>
        /// <param name="partStartNumber"></param>
        /// <param name="partPrefix"></param>
        /// <param name="castUnitType"></param>
        /// <param name="pourPhase"></param>
        /// <param name="finish"></param>
        /// <returns></returns>
        public static TSM.ContourPlate CreateContourPlate(string name, string color, string material, double length, List<TSM.ContourPoint> contourPoints,
            TSM.Position.DepthEnum positionDepth = TSM.Position.DepthEnum.FRONT, double positionDepthOffset = 0.0,
            int assemblyStartNumber = 1, string assemblyPrefix = "", int partStartNumber = 1, string partPrefix = "",
            TSM.Part.CastUnitTypeEnum castUnitType = TSM.Part.CastUnitTypeEnum.PRECAST, int pourPhase = 0,
            string finish = "")
        {
            TSM.ContourPlate contourPlate = new TSM.ContourPlate();

            try
            {
                contourPlate.Name = name;
                contourPlate.Class = color;
                contourPlate.Material.MaterialString = material;
                contourPlate.Profile.ProfileString = string.Format("{0}", length);

                foreach (TSM.ContourPoint contourPoint in contourPoints)
                {
                    contourPlate.AddContourPoint(contourPoint);
                }

                contourPlate.Position.Depth = positionDepth;
                contourPlate.Position.DepthOffset = positionDepthOffset;

                contourPlate.AssemblyNumber.StartNumber = assemblyStartNumber;
                contourPlate.AssemblyNumber.Prefix = assemblyPrefix;
                contourPlate.PartNumber.StartNumber = partStartNumber;
                contourPlate.PartNumber.Prefix = partPrefix;

                contourPlate.CastUnitType = castUnitType;
                contourPlate.PourPhase = contourPlate.CastUnitType == TSM.Part.CastUnitTypeEnum.CAST_IN_PLACE ? pourPhase : 0;
                contourPlate.Finish = finish;

                contourPlate.Insert();
            }
            catch (Exception ex)
            {
                ErrorDialog.Show(ex, ErrorDialog.Severity.ERROR);
            }
            finally
            {
                new TSM.Model().CommitChanges();
            }

            return contourPlate;
        }
    }
}