using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDE2.Assets
{
    //Use this interface for easier asset loading.
    public interface ICustomAsset
    {
        public string TypeRegistryName { get; }
    }
}
