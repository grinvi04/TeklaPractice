using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TSM = Tekla.Structures.Model;
using TSG = Tekla.Structures.Geometry3d;
using System.Collections;

namespace Tekla_Practice
{
    public class Intersect
    {
        public Intersect()
        {

        }

        public List<TSG.Point> GetIntersectPoint(TSM.Part mainPart, TSG.Point pointS, TSG.Point pointE)
        {
            TSM.Solid solid = mainPart.GetSolid();


            //무한한 선
            //TSG.Line

            //범위가 있는 선
            //TSG.LineSegment

            //교차점 찾아오는것
            //TSG.Intersection

            //간섭 찾아오는 것
            //solid.Intersect            


            TSG.LineSegment segment = new TSG.LineSegment(pointS, pointE);
            ArrayList intersect = solid.Intersect(segment);

            List<TSG.Point> pointList = new List<TSG.Point>();

            for (int index = 0; index < intersect.Count; index++)
            {

            }

            return pointList;
        }
    }
}
