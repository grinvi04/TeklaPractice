using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using TC = TeklaCommon.Common.Common;
using TSM = Tekla.Structures.Model;
using TSG = Tekla.Structures.Geometry3d;
using System.Collections;

namespace TeklaCommon.Test
{
    public partial class UtilTest : Form
    {
        private TSM.Model model;
        private TSM.UI.Picker picker;

        public UtilTest()
        {
            InitializeComponent();

            this.model = new TSM.Model();

            if (!this.model.GetConnectionStatus()) return;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            picker = new TSM.UI.Picker();

            try
            {
                TSM.ModelObject modelObject = picker.PickObject(TSM.UI.Picker.PickObjectEnum.PICK_ONE_PART, "1개의 부재를 선택하세요");

                if (null != modelObject)
                {
                    RotateVector();
                }
                else
                {
                    MessageBox.Show("부재가 선택되지 않았습니다.\r\n부재를 선택해주세요.");
                    return;
                }

                /*
                #region 두 부재(빔)간의 간섭을 체크하고 해당 영역을 BooleanPart을 사용한 부재절단으로 절단한다.
                TSM.ModelObjectEnumerator modelObjectEnumerator = picker.PickObjects(TSM.UI.Picker.PickObjectsEnum.PICK_N_PARTS, "부재를 2개 선택하세요.");

                

                if (null != modelObjectEnumerator)
                {
                    if(modelObjectEnumerator.GetSize() >= 2)
                    {
                        List<TSM.Part> partList = new List<TSM.Part>();

                        while (modelObjectEnumerator.MoveNext())
                        {
                            if (modelObjectEnumerator.Current is TSM.Part)
                            {
                                partList.Add(modelObjectEnumerator.Current as TSM.Part);
                            }                                
                        }

                        if(partList.Count != 2)
                        {
                            MessageBox.Show("두 개의 부재를 선택하세요.");
                            return;
                        }
                        
                        // 잘릴 부재, 자를 부재 순서로 선택
                        TSM.Part fatherPart = partList[0];
                        TSM.Part setOperativePart = partList[1];

                        ArrayList intersectionList;
                        // 두 부재 사이의 간섭 체크
                        bool isClashCheck = TC.IsClashCheck(this.model, fatherPart, setOperativePart, out intersectionList);

                        if (isClashCheck)
                        {
                            // intersectionList가 2개가 나오는 경우가 있는데 정확하게 어떻게해서 나오는지 확인 필요
                            TSG.AABB aABB = intersectionList[0] as TSG.AABB;

                            // 간섭영역의 크기
                            double addX = aABB.MaxPoint.X - aABB.MinPoint.X;
                            double addY = aABB.MaxPoint.Y - aABB.MinPoint.Y;
                            double addZ = aABB.MaxPoint.Z - aABB.MinPoint.Z;

                            // 계산에 사용할 단위 벡터
                            TSG.Vector vectorX = new TSG.Vector(1, 0, 0);
                            TSG.Vector vectorY = new TSG.Vector(0, 1, 0);
                            TSG.Vector vectorZ = new TSG.Vector(0, 0, 1);

                            TSG.Vector aABBVector = new TSG.Vector(aABB.MaxPoint - aABB.MinPoint).GetNormal();

                            // 잘릴 부재의 벡터를 구하기 위해 beam으로 캐스팅하고 단위벡터를 구한다.
                            TSM.Beam setOperativeBeam = setOperativePart as TSM.Beam;
                            TSG.Vector baseVector = new TSG.Vector(setOperativeBeam.EndPoint - setOperativeBeam.StartPoint).GetNormal();

                            // 잘릴 부재의 벡터와 구해진 간섭영역의 벡터로 각도(라디안)을 구한다.
                            double angle = baseVector.GetAngleBetween(aABBVector);
                            // degree로 변환한다.
                            double degree = angle * 180 / Math.PI;

                            // 자를 영역에 들어갈 beam의 Plane을 체크한다.
                            TSM.Position.PlaneEnum plane;
                            // 구해진 각도가 180보다 작으면 잘릴 부재의 벡터의 왼쪽, 크면 오른쪽이다.
                            if(degree < 180) plane = TSM.Position.PlaneEnum.LEFT;
                            else if(degree > 0) plane = TSM.Position.PlaneEnum.RIGHT;
                            else plane = TSM.Position.PlaneEnum.MIDDLE;                        

                            // 간섭영역의 벡터가 대각선으로 나오므로 minPoint에서 x방향으로 더해질 크기를 더한다.
                            TSG.Point tempPoint = aABB.MinPoint + (vectorX * addX);

                            TSM.BooleanPart booleanPart = new TSM.BooleanPart();
                            booleanPart.Father = fatherPart;

                            // 시작포인트는 minPoint이고 addY, addZ만큼 beam의 width, height를 지정한다.
                            TSM.Beam beam = TC.CreateBeam(aABB.MinPoint, tempPoint, plane, addZ, addY);

                            beam.Class = TSM.BooleanPart.BooleanOperativeClassName;
                            booleanPart.SetOperativePart(beam);                            

                            if (!booleanPart.Insert())
                            {
                                MessageBox.Show("부재를 자르지 못했습니다.");
                                return;
                            }

                            beam.Delete();
                        } else
                        {
                            MessageBox.Show("간섭된 영역이 없습니다.");
                            return;
                        }
                    } else
                    {
                        MessageBox.Show("부재를 1개 이상 선택해주세요.");
                        return;
                    }
                } else
                {
                    MessageBox.Show("부재가 선택되지 않았습니다.\r\n부재를 선택해주세요.");
                    return;
                }

                #endregion
                */

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                // model 반영
                model.CommitChanges();
            }
        }

        public void RotateVector()
        {
            TSG.Vector vector = new TSG.Vector(500, 1000, 0);
            TSG.Vector vectorAxisX = TC.Rotate(vector, 90, 1, 0, 0);
            TSG.Vector vectorAxisY = TC.Rotate(vector, 90, 0, 1, 0);
            TSG.Vector vectorAxisZ = TC.Rotate(vector, 90, 0, 0, 1);

            System.Console.WriteLine("vector : {0}", vector);
            System.Console.WriteLine("vectorAxisX : {0}", vectorAxisX);
            System.Console.WriteLine("vectorAxisY : {0}", vectorAxisY);
            System.Console.WriteLine("vectorAxisZ : {0}", vectorAxisZ);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            picker = new TSM.UI.Picker();

            try
            {
                TSM.ModelObject modelObject = picker.PickObject(TSM.UI.Picker.PickObjectEnum.PICK_ONE_PART, "1개의 부재를 선택하세요");

                if (null != modelObject)
                {

                }
                else
                {
                    MessageBox.Show("부재가 선택되지 않았습니다.\r\n부재를 선택해주세요.");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                // model 반영
                model.CommitChanges("수정되었습니다");
            }
        }
    }
}
