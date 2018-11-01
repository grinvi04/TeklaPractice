using System;
using Tekla.Structures.Dialog;
using TSG = Tekla.Structures.Geometry3d;
using TSM = Tekla.Structures.Model;

namespace TeklaCommon
{
    /// <summary>
    /// TSM.Boolean의 하위의 클래스를 모듈화 한 클래스
    /// <para>
    /// YWBoolean 클래스는 부분 잘라 내기, 추가, 맞춤 또는 절단 기준면과 같은 부울 연산의 추상 기본 클래스입니다.
    /// </para>
    /// </summary>
    public class YWBoolean
    {
        /// <summary>
        /// BooleanPart 생성
        /// <para>
        /// BooleanPart 클래스는 부분 잘라 내기 또는 추가를 나타냅니다.
        /// 즉 모델 객체는 예를 들어 파트 인스턴스로 잘라서 아버지 파트에 대한 void를 만듭니다.
        /// 일반적으로 작동 부분은 작동 후에 삭제됩니다.
        /// </para>
        /// </summary>
        /// <param name="fatherObject"></param>
        /// <param name="operativePart"></param>
        /// <param name="type"></param>
        /// <returns>BooleanPart Insert Result</returns>
        public static bool CreateBooleanPart(TSM.ModelObject fatherObject, TSM.Part operativePart, TSM.BooleanPart.BooleanTypeEnum type = TSM.BooleanPart.BooleanTypeEnum.BOOLEAN_CUT)
        {
            bool isCreate = false;
            TSM.BooleanPart booleanPart = new TSM.BooleanPart();

            try
            {
                booleanPart.Father = fatherObject;
                booleanPart.SetOperativePart(operativePart);
                booleanPart.Type = type;

                if (booleanPart.Insert())
                {
                    if (operativePart.Delete())
                    {
                        isCreate = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorDialog.Show(ex, ErrorDialog.Severity.ERROR);
            }
            finally
            {
                new TSM.Model().CommitChanges();
            }

            return isCreate;
        }

        /// <summary>
        /// CutPlane 생성
        /// <para>
        /// CutPlane 클래스는 파트의 끝이 평면으로 절단되는 방법을 정의합니다.
        /// 절단면은 원래 파트의 경계를 확장 할 수 없기 때문에 피팅과 다릅니다.
        /// </para>
        /// </summary>
        /// <param name="fatherObject"></param>
        /// <param name="originPoint"></param>
        /// <param name="axisX"></param>
        /// <param name="axisY"></param>
        /// <returns>CutPlane Insert Result</returns>
        public static bool CreateCutPlane(TSM.ModelObject fatherObject, TSG.Point originPoint, TSG.Vector axisX, TSG.Vector axisY)
        {
            bool isCreate = false;
            TSM.CutPlane cutPlane = new TSM.CutPlane();

            try
            {
                cutPlane.Plane.Origin = originPoint;
                cutPlane.Plane.AxisX = axisX;
                cutPlane.Plane.AxisY = axisY;

                cutPlane.Father = fatherObject;

                isCreate = cutPlane.Insert();
            }
            catch (Exception ex)
            {
                ErrorDialog.Show(ex, ErrorDialog.Severity.ERROR);
            }

            return isCreate;
        }

        /// <summary>
        /// EdgeChamfer 생성
        /// <para>
        /// EdgeChamfer 클래스는 파트 모서리가 모따기되는 방식을 정의합니다.
        /// </para>
        /// </summary>
        /// <param name="fatherObject"></param>
        /// <param name="name"></param>
        /// <param name="firstEndPoint"></param>
        /// <param name="secondEndPoint"></param>
        /// <param name="chamfer"></param>
        /// <param name="firstChamferEndType"></param>
        /// <param name="firstBevelDimension"></param>
        /// <param name="secondChamferEndType"></param>
        /// <param name="secondBevelDimension"></param>
        /// <returns>EdgeChamfer Insert Result</returns>
        public static bool CreateEdgeChamfer(TSM.ModelObject fatherObject, string name,
            TSG.Point firstEndPoint, TSG.Point secondEndPoint, TSM.Chamfer chamfer,
            TSM.EdgeChamfer.ChamferEndTypeEnum firstChamferEndType = TSM.EdgeChamfer.ChamferEndTypeEnum.FULL, double firstBevelDimension = 0.0,
            TSM.EdgeChamfer.ChamferEndTypeEnum secondChamferEndType = TSM.EdgeChamfer.ChamferEndTypeEnum.FULL, double secondBevelDimension = 0.0)
        {
            bool isCreate = false;

            try
            {
                TSM.EdgeChamfer edgeChamfer = new TSM.EdgeChamfer(firstEndPoint, secondEndPoint);

                // 모따기거리 속성
                // Chamfer 속성은 type, 모따기거리(X방향의 컷 거리, Y방향의 컷 거리가 포함)
                // edgeChamfer에 들어가는 chamfer의 타입은 TSM.Chamfer.ChamferTypeEnum.CHAMFER_LINE만 가능
                if (chamfer.Type != TSM.Chamfer.ChamferTypeEnum.CHAMFER_LINE)
                    chamfer.Type = TSM.Chamfer.ChamferTypeEnum.CHAMFER_LINE;

                edgeChamfer.Chamfer = chamfer;
                edgeChamfer.Father = fatherObject;
                edgeChamfer.Name = name;

                // 모따기 단부 속성
                // 첫 번째 끝 유형(전체, 직선형, 베벨)
                edgeChamfer.FirstChamferEndType = firstChamferEndType;
                if (firstChamferEndType == TSM.EdgeChamfer.ChamferEndTypeEnum.BEVELLED)
                {
                    edgeChamfer.FirstBevelDimension = firstBevelDimension;
                }
                // 두 번째 끝 유형(전체, 직선형, 베벨)
                edgeChamfer.SecondChamferEndType = secondChamferEndType;
                if (secondChamferEndType == TSM.EdgeChamfer.ChamferEndTypeEnum.BEVELLED)
                {
                    edgeChamfer.SecondBevelDimension = secondBevelDimension;
                }

                isCreate = edgeChamfer.Insert();
            }
            catch (Exception ex)
            {
                ErrorDialog.Show(ex, ErrorDialog.Severity.ERROR);
            }
            finally
            {
                new TSM.Model().CommitChanges();
            }

            return isCreate;
        }

        /// <summary>
        /// Fitting 생성
        /// <para>
        /// Fitting 클래스는 파트 끝이 평면에 맞춰지는 방식을 정의합니다. 피팅은 부품을 길거나 짧게 만들 수 있습니다.
        /// </para>
        /// </summary>
        /// <param name="fatherObject"></param>
        /// <param name="originPoint"></param>
        /// <param name="axisX"></param>
        /// <param name="axisY"></param>
        /// <returns>Fitting Insert Result</returns>
        public static bool CreateFitting(TSM.ModelObject fatherObject, TSG.Point originPoint, TSG.Vector axisX, TSG.Vector axisY)
        {
            bool isCreate = false;
            TSM.Fitting fitting = new TSM.Fitting();

            try
            {
                TSM.Plane plane = new TSM.Plane();
                plane.Origin = originPoint;
                plane.AxisX = axisX;
                plane.AxisY = axisY;

                fitting.Plane = plane;
                fitting.Father = fatherObject;

                isCreate = fitting.Insert();
            }
            catch (Exception ex)
            {
                ErrorDialog.Show(ex, ErrorDialog.Severity.ERROR);
            }
            finally
            {
                new TSM.Model().CommitChanges();
            }

            return isCreate;
        }
    }
}