using UnityEditor;
using UnityEngine;

namespace Common.Encoding.Editor
{
	public class MenuBinder
	{
		[MenuItem("Tools/Encoding")]
		public static void LaunchReskinWindow()
		{
			var window = EditorWindow.GetWindow<EncodingToolWindow>("Encoding");
			window.minSize = new Vector2(1000f, 710f);
		}
	}
}
