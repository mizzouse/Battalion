using System;
using System.Collections.Generic;

using Infantry.Assets;
using Lidgren.Network;

namespace Infantry.Network
{
    public class SC_TotalAssetChecksum
    {
        public SC_TotalAssetChecksum(NetIncomingMessage inc)
        {
            Checksum.TotalChecksumValue = inc.ReadString();
        }
    }
}
