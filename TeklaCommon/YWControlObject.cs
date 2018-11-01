using System;
using Tekla.Structures.Dialog;
using TSG = Tekla.Structures.Geometry3d;
using TSM = Tekla.Structures.Model;

namespace TeklaCommon
{
    /// <summary>
    /// TSM.ModelObject 하위의 참고객체 관련 클래스를 모듈화 한 클래스
    /// </summary>
    public class YWControlObject
    {
        /// <summary>
        /// 참고객체 점 생성
        /// <para>
        /// ControlPoint 클래스는 모델링 작업을 돕는 사용자 정의 된 점을 정의합니다.
        /// </para>
        /// </summary>
        /// <param name="existPoint"></param>
        /// <returns>ControlPoint Insert Result</returns>
        public static bool CreateControlPoint(TSG.Point existPoint)
        {
            bool isCreate = false;

            try
            {
                TSM.ControlPoint controlPoint = new TSM.ControlPoint(existPoint);

                isCreate = controlPoint.Insert();
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
        /// 참고객체 선 생성
        /// <para>
        /// ControlLine 클래스는 모델링 작업을 돕는 사용자 정의 (가능하면 자기) 선을 정의합니다.
        /// </para>
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="color"></param>
        /// <returns>ControlLine Insert Result</returns>
        public static bool CreateControlLine(TSG.Point startPoint, TSG.Point endPoint, TSM.ControlLine.ControlLineColorEnum color)
        {
            bool isCreate = false;
            TSM.ControlLine controlLine = new TSM.ControlLine();

            try
            {
                TSG.LineSegment line = new TSG.LineSegment(startPoint, endPoint);

                controlLine.Line = line;
                controlLine.Color = color;

                isCreate = controlLine.Insert();
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
        /// 참고객체 평면 생성
        /// <para>
        /// ControlPlane 클래스는 모델링 작업을 돕는 사용자 정의 (가능하면 자기) 평면을 정의합니다.
        /// </para>
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="axisX"></param>
        /// <param name="axisY"></param>
        /// <returns>ControlPlane Insert Result</returns>
        public static bool CreateControlPlane(TSG.Point origin, TSG.Vector axisX, TSG.Vector axisY)
        {
            bool isCreate = false;
            TSM.Plane plane = new TSM.Plane();

            try
            {
                plane.Origin = origin;
                plane.AxisX = axisX;
                plane.AxisY = axisY;

                TSM.ControlPlane controlPlane = new TSM.ControlPlane(plane, true);

                isCreate = controlPlane.Insert();
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
        /// 참고객체 원 생성
        /// <para>
        /// ControlCircle 클래스는 모델링 작업을 돕는 사용자 정의 (자성이 아닌) 원을 정의합니다.
        /// 중복 된 입력 지점이 있거나 3 개의 입력 지점이 한 줄에 있으면 ControlCircle.Insert ()는 false를 반환합니다.
        /// ControlCircle.Select ()는 원의 중심점을 Point1로 되돌립니다.
        /// </para>
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="color"></param>
        /// <returns>ControlCircle Insert Result</returns>
        public static bool CreateControlCircle(TSG.Point p1, TSG.Point p2, TSG.Point p3, TSM.ControlCircle.ControlCircleColorEnum color)
        {
            bool isCreate = false;

            try
            {
                TSM.ControlCircle controlCircle = new TSM.ControlCircle(p1, p2, p3);

                controlCircle.Color = color;

                isCreate = controlCircle.Insert();
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