using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RiskyMod
{
    class Assets //Meant to be used to hold assetbundle shit... this mod does not use that, though, but still
    {
        internal static string assemblyDir
        {
            get
            {
                return Path.GetDirectoryName(RiskyMod.pluginInfo.Location);
            }
        }
    }
}
