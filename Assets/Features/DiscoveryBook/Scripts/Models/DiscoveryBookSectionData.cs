using System;
using System.Collections.Generic;

namespace Features.DiscoveryBook.Scripts.Models
{
    [Serializable]
    public class DiscoveryBookSectionData
    {
        public string Title;
        public List<DiscoveryBookItemData> Items;
        public bool IsProgressive;
    }
}