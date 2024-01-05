using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDE2.Assets
{
    public class TestAsset : Asset, ICustomAsset
    {
        private string _testValue;

        public string TestValue => _testValue;

        public string TypeRegistryName { get => "Test"; }

        public override string GetTypeFriendlyName()
        {
            string text = base.GetTypeFriendlyName();
            if (text.StartsWith("Test "))
            {
                text = text.Substring(5) + " Test";
            }
            return text;
        }

        public override void PopulateAsset(Bundle bundle, DatDictionary data, Local localization)
        {
            base.PopulateAsset(bundle, data, localization);

            _testValue = data.GetString("Test_Value", "fuck");
        }
    }
}
