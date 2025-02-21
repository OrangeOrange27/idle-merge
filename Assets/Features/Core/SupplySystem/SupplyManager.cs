using Features.Core.GridSystem;
using UnityEngine;

namespace Features.Core.SupplySystem
{
    public class SupplyManager : MonoBehaviour
    {
        private static Vector3 Offset = new(0.5f, 0.5f, -1);
        
        [SerializeField] private GridManager _gridManager;
        [SerializeField] private GameAreaObject _testPrefab;
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var tile = _gridManager.GetRandomFreeTile();
                if(tile==null)
                    return;

                var pos = tile.Position + Offset;
                Instantiate(_testPrefab, pos, Quaternion.identity);
                tile.Occupy(_testPrefab);
            }
        }
    }
}