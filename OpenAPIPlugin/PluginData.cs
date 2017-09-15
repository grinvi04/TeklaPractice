using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tekla.Structures;
using Tekla.Structures.Model;
using Tekla.Structures.Model.UI;
using Tekla.Structures.Plugins;

namespace OpenAPIPlugin
{
    public class PluginData
    {
        [StructuresField("Height")]
        public double Height;
    }

    [Plugin("OpenAPIPlugin")]
    [PluginUserInterface("OpenAPIPlugin.MainForm")]
    public class OpenAPIPlugin : PluginBase
    {
        private Model _Model;
        private PluginData _Data;

        public OpenAPIPlugin(PluginData Data)
        {
            this._Model = new Model();
            this._Data = Data;
        }

        public override List<InputDefinition> DefineInput()
        {
            List<InputDefinition> inputList = new List<InputDefinition>();

            Picker picker = new Picker();

            ModelObject modelObject = picker.PickObject(Picker.PickObjectEnum.PICK_ONE_PART, "Select MainPart");

            inputList.Add(new InputDefinition(modelObject.Identifier));

            return inputList;

            //throw new NotImplementedException();
        }

        public override bool Run(List<InputDefinition> Input)
        {
            try
            {
                Identifier identifier = Input[0].GetInput() as Identifier;
                Beam beam = _Model.SelectModelObject(identifier) as Beam;

                beam.Profile.ProfileString = "800X" + _Data.Height;

                beam.Modify();
                
            }
            catch
            {
                return false;

            }

            return true;
            //throw new NotImplementedException();
        }
    }
}
