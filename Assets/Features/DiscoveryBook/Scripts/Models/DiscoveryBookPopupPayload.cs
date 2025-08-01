using System;
using System.Collections.Generic;

namespace Features.DiscoveryBook.Scripts.Models
{
    [Serializable]
    public class DiscoveryBookPopupPayload
    {
        public Dictionary<DiscoveryBookTabType, List<DiscoveryBookSectionData>> Tabs = new();
    }
}