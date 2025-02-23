using Common.Encoding.Implementation;
using Common.Encoding.Infrastructure;
using UnityEditor;
using UnityEngine;

namespace Common.Encoding.Editor
{
	public class EncodingToolWindow : EditorWindow
	{
		private readonly IEncoder[] _encoders = new IEncoder[] 
			{ new CryptEncoder(), new GenericEncoder(), new HtmlEncoder() };

		private string[] _encoderNames;
		private string _input = string.Empty;
		private string _output = string.Empty;
		private int _selectedEncoder = 0;

		private void Awake()
		{
			FillEncoderNames();
		}

		private void OnGUI()
		{
			GUILayout.BeginVertical();
			{
				DrawSelectEncoder();
				GUILayout.BeginHorizontal();
				{
					DrawInputSection();
					DrawOutputSection();
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndVertical();
		}

		private void DrawSelectEncoder()
		{
			GUILayout.Space(20f);
			_selectedEncoder = EditorGUILayout.Popup("Select Encoder:", _selectedEncoder, _encoderNames);
			GUILayout.Space(20f);
		}

		private void DrawInputSection()
		{
			GUILayout.BeginVertical();
			{
				GUILayout.Label("Input");
				_input = GUILayout.TextArea(_input, GUILayout.MinWidth(400), GUILayout.MinHeight(600));

				if (GUILayout.Button("DECODE"))
				{
					_output = _encoders[_selectedEncoder].Decode(_input);
				}
			}
			GUILayout.EndVertical();
		}

		private void DrawOutputSection()
		{
			GUILayout.BeginVertical();
			{
				GUILayout.Label("Output");
				_output = GUILayout.TextArea(_output, GUILayout.MinWidth(400), GUILayout.MinHeight(600));

				if (GUILayout.Button("ENCODE"))
				{
					_output = _encoders[_selectedEncoder].Encode(_input);
				}
			}
			GUILayout.EndVertical();
		}

		private void FillEncoderNames()
		{
			_encoderNames = new string[_encoders.Length];

			for (var i = 0; i < _encoders.Length; i++)
			{
				_encoderNames[i] = _encoders[i].GetType().Name;
			}
		}
	}
}
