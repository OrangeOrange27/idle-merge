using Features.DiscoveryBook.Scripts.Models;
using UnityEngine;

namespace Features.DiscoveryBook.Scripts.Views
{
    public class DiscoveryBookTabView : MonoBehaviour
    {
        [SerializeField] DiscoveryBookTabType _tabType;
        
        public DiscoveryBookTabType TabType => _tabType;
    }
}