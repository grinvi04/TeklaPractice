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
                //TSM.ModelObject modelObject = picker.PickObject(TSM.UI.Picker.PickObjectEnum.PICK_ONE_PART, "1개의 부재를 선택하세요");                

                //if (null != modelObject)
                //{
                //    RotateVector();
                //}
                //else
                //{
                //    MessageBox.Show("부재가 선택되지 않았습니다.\r\n부재를 선택해주세요.");
                //    return;
                //}

                TSM.ModelObjectEnumerator modelObjectEnumerator = picker.PickObjects(TSM.UI.Picker.PickObjectsEnum.PICK_N_PARTS, "부재를 1개 이상 선택하세요.");

                List<TSM.Part> partList = new List<TSM.Part>();

                if (null != modelObjectEnumerator)
                {
                    if(modelObjectEnumerator.GetSize() >= 2)
                    {
                        while (modelObjectEnumerator.MoveNext())
                        {
                            if (modelObjectEnumerator.Current is TSM.Part)
                            {
                                partList.Add(modelObjectEnumerator.Current as TSM.Part);
                                TC.CreateOrientedBoundingBox(this.model, modelObjectEnumerator.Current as TSM.Part);
                            }
                                
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
    }
}
