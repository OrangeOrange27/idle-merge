using UnityEngine;

namespace Utils
{
    public static class VectorExtensions
    {
        public static Vector3Int ToVector3Int(this Vector3 vector3)
        {
            return new Vector3Int((int) vector3.x, (int) vector3.y, (int) vector3.z);
        }
    }
}