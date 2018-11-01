using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tekla.Structures.Dialog;
using TSC = Tekla.Structures.Catalogs;
using TSD = Tekla.Structures.Datatype;
using TSG = Tekla.Structures.Geometry3d;
using TSM = Tekla.Structures.Model;
using YWC = TeklaCommon.YWCommon;

namespace TeklaCommon
{
    /// <summary>
    /// Reinforcement 하위의 클래스를 모듈화한 클래스
    /// <para>
    /// Reinforcement 클래스는 모델의 보강을 나타냅니다. 보강재는 메쉬, 단일 보강 철근, 보강 철근 그룹 또는 가닥 일 수 있습니다.
    /// </para>
    /// </summary>
    public class YWReinforcement
    {
        /// <summary>
        /// SingleRebar 생성
        /// <para>
        /// SingleRebar 클래스는 단일 철근 막대를 나타냅니다.
        /// </para>
        /// </summary>
        /// <param name="grade"></param>
        /// <param name="size"></param>
        /// <param name="usage"></param>
        /// <param name="name"></param>
        /// <param name="color"></param>
        /// <param name="fatherObject"></param>
        /// <param name="polygon"></param>
        /// <param name="onPlaneOffsets"></param>
        /// <param name="fromPlaneOffset"></param>
        /// <param name="startHookShape"></param>
        /// <param name="startHookAngle"></param>
        /// <param name="startHookRadius"></param>
        /// <param name="startHookLength"></param>
        /// <param name="endHookShape"></param>
        /// <param name="endHookAngle"></param>
        /// <param name="endHookRadius"></param>
        /// <param name="endHookLength"></param>
        /// <param name="startPointOffsetType"></param>
        /// <param name="startPointOffsetValue"></param>
        /// <param name="endPointOffsetType"></param>
        /// <param name="endPointOffsetValue"></param>
        /// <param name="numberingStartNumber"></param>
        /// <param name="numberingPrefix"></param>
        /// <returns></returns>
        public static TSM.SingleRebar CreateSingleRebar(string grade, string size, string usage, string name, int color, TSM.ModelObject fatherObject,
            TSM.Polygon polygon, ArrayList onPlaneOffsets = null, double fromPlaneOffset = 0.0,
            TSM.RebarHookData.RebarHookShapeEnum startHookShape = TSM.RebarHookData.RebarHookShapeEnum.NO_HOOK, double startHookAngle = 0.0, double startHookRadius = 0.0, double startHookLength = 0.0,
            TSM.RebarHookData.RebarHookShapeEnum endHookShape = TSM.RebarHookData.RebarHookShapeEnum.NO_HOOK, double endHookAngle = 0.0, double endHookRadius = 0.0, double endHookLength = 0.0,
            TSM.Reinforcement.RebarOffsetTypeEnum startPointOffsetType = TSM.Reinforcement.RebarOffsetTypeEnum.OFFSET_TYPE_COVER_THICKNESS, double startPointOffsetValue = 0.0,
            TSM.Reinforcement.RebarOffsetTypeEnum endPointOffsetType = TSM.Reinforcement.RebarOffsetTypeEnum.OFFSET_TYPE_COVER_THICKNESS, double endPointOffsetValue = 0.0,
            int numberingStartNumber = 1, string numberingPrefix = "")
        {
            TSM.SingleRebar singleRebar = new TSM.SingleRebar();
            TSC.RebarItem rebarItem = new TSC.RebarItem();

            try
            {
                bool isRebarSelect = rebarItem.Select(grade, size, usage);

                if (isRebarSelect)
                {
                    singleRebar.Grade = grade;
                    singleRebar.Size = size;

                    singleRebar.RadiusValues.Add(rebarItem.BendRadius);

                    singleRebar.Name = name;
                    singleRebar.Class = color;
                    singleRebar.Father = fatherObject;

                    singleRebar.NumberingSeries.StartNumber = numberingStartNumber;
                    singleRebar.NumberingSeries.Prefix = numberingPrefix;

                    singleRebar.Polygon = polygon;

                    singleRebar.StartHook.Shape = startHookShape;
                    if (startHookShape == TSM.RebarHookData.RebarHookShapeEnum.CUSTOM_HOOK)
                    {
                        singleRebar.StartHook.Angle = startHookAngle;
                        singleRebar.StartHook.Radius = startHookRadius;
                        singleRebar.StartHook.Length = startHookLength;
                    }

                    singleRebar.EndHook.Shape = endHookShape;
                    if (endHookShape == TSM.RebarHookData.RebarHookShapeEnum.CUSTOM_HOOK)
                    {
                        singleRebar.EndHook.Angle = endHookAngle;
                        singleRebar.EndHook.Radius = endHookRadius;
                        singleRebar.EndHook.Length = endHookLength;
                    }

                    if (fromPlaneOffset > 0)
                    {
                        singleRebar.FromPlaneOffset = fromPlaneOffset - (rebarItem.NominalDiameter * 0.5);
                    }
                    else if (fromPlaneOffset < 0)
                    {
                        singleRebar.FromPlaneOffset = fromPlaneOffset + (rebarItem.NominalDiameter * 0.5);
                    }
                    else
                    {
                        singleRebar.FromPlaneOffset = fromPlaneOffset;
                    }

                    if (null != onPlaneOffsets && onPlaneOffsets.Count > 0)
                    {
                        singleRebar.OnPlaneOffsets = onPlaneOffsets;
                    }

                    singleRebar.StartPointOffsetType = startPointOffsetType;
                    singleRebar.StartPointOffsetValue = startPointOffsetValue;

                    singleRebar.EndPointOffsetType = endPointOffsetType;
                    singleRebar.EndPointOffsetValue = endPointOffsetValue;

                    singleRebar.Insert();
                }
            }
            catch (Exception ex)
            {
                ErrorDialog.Show(ex, ErrorDialog.Severity.ERROR);
            }
            finally
            {
                YWC.GetModel.CommitChanges();
            }

            return singleRebar;
        }

        /// <summary>
        /// RebarGroup 생성
        /// <para>
        /// RebarGroup 클래스는 강화 막대 그룹을 나타냅니다.
        /// </para>
        /// </summary>
        /// <param name="grade"></param>
        /// <param name="size"></param>
        /// <param name="usage"></param>
        /// <param name="name"></param>
        /// <param name="color"></param>
        /// <param name="fatherObject"></param>
        /// <param name="polygons"></param>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="spaceList"></param>
        /// <param name="spacingType"></param>
        /// <param name="onPlaneOffsets"></param>
        /// <param name="stirrupType"></param>
        /// <param name="excludeType"></param>
        /// <param name="startHookShape"></param>
        /// <param name="startHookAngle"></param>
        /// <param name="startHookRadius"></param>
        /// <param name="startHookLength"></param>
        /// <param name="endHookShape"></param>
        /// <param name="endHookAngle"></param>
        /// <param name="endHookRadius"></param>
        /// <param name="endHookLength"></param>
        /// <param name="startPointOffsetType"></param>
        /// <param name="startPointOffsetValue"></param>
        /// <param name="endPointOffsetType"></param>
        /// <param name="endPointOffsetValue"></param>
        /// <param name="numberingStartNumber"></param>
        /// <param name="numberingPrefix"></param>
        /// <returns></returns>
        public static TSM.RebarGroup CreateRebarGroup(string grade, string size, string usage, string name, int color, TSM.ModelObject fatherObject,
            List<TSM.Polygon> polygons, TSG.Point startPoint, TSG.Point endPoint,
            TSD.DistanceList spaceList, TSM.BaseRebarGroup.RebarGroupSpacingTypeEnum spacingType = TSM.BaseRebarGroup.RebarGroupSpacingTypeEnum.SPACING_TYPE_EXACT_SPACINGS,
            ArrayList onPlaneOffsets = null,
            TSM.RebarGroup.RebarGroupStirrupTypeEnum stirrupType = TSM.RebarGroup.RebarGroupStirrupTypeEnum.STIRRUP_TYPE_POLYGONAL,
            TSM.BaseRebarGroup.ExcludeTypeEnum excludeType = TSM.BaseRebarGroup.ExcludeTypeEnum.EXCLUDE_TYPE_NONE,
            TSM.RebarHookData.RebarHookShapeEnum startHookShape = TSM.RebarHookData.RebarHookShapeEnum.NO_HOOK, double startHookAngle = 0.0, double startHookRadius = 0.0, double startHookLength = 0.0,
            TSM.RebarHookData.RebarHookShapeEnum endHookShape = TSM.RebarHookData.RebarHookShapeEnum.NO_HOOK, double endHookAngle = 0.0, double endHookRadius = 0.0, double endHookLength = 0.0,
            TSM.Reinforcement.RebarOffsetTypeEnum startPointOffsetType = TSM.Reinforcement.RebarOffsetTypeEnum.OFFSET_TYPE_COVER_THICKNESS, double startPointOffsetValue = 0.0,
            TSM.Reinforcement.RebarOffsetTypeEnum endPointOffsetType = TSM.Reinforcement.RebarOffsetTypeEnum.OFFSET_TYPE_COVER_THICKNESS, double endPointOffsetValue = 0.0,
            int numberingStartNumber = 1, string numberingPrefix = "")
        {
            TSM.RebarGroup rebarGroup = new TSM.RebarGroup();
            TSC.RebarItem rebarItem = new TSC.RebarItem();

            try
            {
                bool isRebarSelect = rebarItem.Select(grade, size, usage);

                if (isRebarSelect)
                {
                    rebarGroup.Grade = grade;
                    rebarGroup.Size = size;

                    rebarGroup.RadiusValues.Add(rebarItem.BendRadius);

                    rebarGroup.Name = name;
                    rebarGroup.Class = color;
                    rebarGroup.Father = fatherObject;

                    rebarGroup.NumberingSeries.StartNumber = numberingStartNumber;
                    rebarGroup.NumberingSeries.Prefix = numberingPrefix;
                    rebarGroup.StirrupType = stirrupType;

                    rebarGroup.StartPoint = startPoint;
                    rebarGroup.EndPoint = endPoint;
                    rebarGroup.Polygons.AddRange(polygons);

                    rebarGroup.ExcludeType = excludeType;

                    rebarGroup.StartHook.Shape = startHookShape;
                    if (startHookShape == TSM.RebarHookData.RebarHookShapeEnum.CUSTOM_HOOK)
                    {
                        rebarGroup.StartHook.Angle = startHookAngle;
                        rebarGroup.StartHook.Radius = startHookRadius;
                        rebarGroup.StartHook.Length = startHookLength;
                    }

                    rebarGroup.EndHook.Shape = endHookShape;
                    if (endHookShape == TSM.RebarHookData.RebarHookShapeEnum.CUSTOM_HOOK)
                    {
                        rebarGroup.EndHook.Angle = endHookAngle;
                        rebarGroup.EndHook.Radius = endHookRadius;
                        rebarGroup.EndHook.Length = endHookLength;
                    }

                    rebarGroup.FromPlaneOffset = spaceList[0].Value - (rebarItem.NominalDiameter * 0.5);

                    if (null != onPlaneOffsets && onPlaneOffsets.Count > 0)
                    {
                        rebarGroup.OnPlaneOffsets = onPlaneOffsets;
                    }

                    rebarGroup.SpacingType = spacingType;
                    rebarGroup.Spacings.AddRange(spaceList.Skip(1).Select(o => o.Value).ToArray());

                    rebarGroup.StartPointOffsetType = startPointOffsetType;
                    rebarGroup.StartPointOffsetValue = startPointOffsetValue;

                    rebarGroup.EndPointOffsetType = endPointOffsetType;
                    rebarGroup.EndPointOffsetValue = endPointOffsetValue;

                    bool isInsert = rebarGroup.Insert();
                }
            }
            catch (Exception ex)
            {
                ErrorDialog.Show(ex, ErrorDialog.Severity.ERROR);
            }
            finally
            {
                YWC.GetModel.CommitChanges();
            }

            return rebarGroup;
        }
    }
}