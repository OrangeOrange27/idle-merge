using System.Linq;
using UnityEngine;

namespace Common.Utils.Extensions
{
	public static class TransformExtensions
	{
		public static Transform[] GetAllChildren(this Transform transform, bool onlyActive = false)
		{
			return transform.Cast<Transform>().Where(child => !onlyActive || child.gameObject.activeInHierarchy).ToArray();
		}
	}
}
