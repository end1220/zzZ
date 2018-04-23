using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

namespace Float
{
	public static class FloatGUIStyle
	{
		private static GUISkin skin;
		public static GUIStyle label { get; private set; }
		public static GUIStyle boldLabel { get; private set; }
		public static GUIStyle largeLabel { get; private set; }
		public static GUIStyle textField { get; private set; }
		public static GUIStyle textFieldPath { get; private set; }
		public static GUIStyle textArea { get; private set; }
		public static GUIStyle toggle { get; private set; }
		public static GUIStyle button { get; private set; }
		public static GUIStyle helpBox { get; private set; }
		public static GUIStyle link { get; private set; }
		public static GUIStyle foldout { get; private set; }
		public static GUIStyle red { get; private set; }

		public static void Ensure()
		{
			if (skin != null)
				return;

			int fontSizeNormal = 12;
			int fontSizeInput = 14;
			int fontSizeLarge = 18;

			skin = Resources.Load<GUISkin>("FloatGUISkin");
			link = skin.FindStyle("Link");
			red = skin.FindStyle("red");

			label = new GUIStyle(EditorStyles.label);
			label.fontSize = fontSizeNormal;
			label.wordWrap = true;
			label.richText = true;

			boldLabel = new GUIStyle(EditorStyles.label);
			boldLabel.fontStyle = FontStyle.Bold;
			boldLabel.fontSize = fontSizeNormal;

			largeLabel = new GUIStyle(EditorStyles.largeLabel);
			largeLabel.fontStyle = FontStyle.Bold;
			largeLabel.fontSize = fontSizeLarge;

			textField = new GUIStyle(EditorStyles.textField);
			textField.fontSize = fontSizeInput;

			textFieldPath = new GUIStyle(EditorStyles.textField);
			textFieldPath.fontSize = fontSizeNormal;

			textArea = new GUIStyle(EditorStyles.textArea);
			textArea.fontSize = fontSizeInput;

			helpBox = new GUIStyle(EditorStyles.helpBox);
			helpBox.fontSize = fontSizeNormal;

			toggle = new GUIStyle(EditorStyles.toggle);
			toggle.fontSize = fontSizeNormal;

			button = new GUIStyle(EditorStyles.miniButton);
			button.fontStyle = FontStyle.Bold;
			button.fontSize = fontSizeInput;

			foldout = new GUIStyle(EditorStyles.foldout);
			foldout.fontStyle = FontStyle.Bold;
			foldout.fontSize = fontSizeInput;
			foldout.fixedWidth = 400;
			foldout.fixedHeight = 20;
		}

		// constants
		public const float spaceSize = 15f;
		public const float leftSpace = 10;
		public const float titleLen = 80;
		public const float textLen = 550;
		public const float buttonLen1 = 100;
		public const float buttonLen2 = 50;
		public const float buttonHeight = 30;


		public static string HandleCopyPaste(int controlID)
		{
			if (controlID == GUIUtility.keyboardControl)
			{
				if (Event.current.type == UnityEngine.EventType.KeyUp && (Event.current.modifiers == EventModifiers.Control || Event.current.modifiers == EventModifiers.Command))
				{
					if (Event.current.keyCode == KeyCode.C)
					{
						Event.current.Use();
						TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
						editor.Copy();
					}
					else if (Event.current.keyCode == KeyCode.V)
					{
						Event.current.Use();
						TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
						editor.Paste();
#if UNITY_5_3_OR_NEWER || UNITY_5_3
						return editor.text; //以及更高的unity版本中editor.content.text已经被废弃，需使用editor.text代替
#else
                    return editor.content.text;
#endif
					}
					else if (Event.current.keyCode == KeyCode.A)
					{
						Event.current.Use();
						TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
						editor.SelectAll();
					}
				}
			}
			return null;
		}

		public static string TextField(string value, params GUILayoutOption[] options)
		{
			int textFieldID = GUIUtility.GetControlID("TextField".GetHashCode(), FocusType.Keyboard) + 1;
			if (textFieldID == 0)
				return value;

			value = HandleCopyPaste(textFieldID) ?? value;

			return GUILayout.TextField(value, options);
		}

		public static string TextField(string value, GUIStyle style, params GUILayoutOption[] options)
		{
			int textFieldID = GUIUtility.GetControlID("TextField".GetHashCode(), FocusType.Keyboard) + 1;
			if (textFieldID == 0)
				return value;

			value = HandleCopyPaste(textFieldID) ?? value;

			return GUILayout.TextField(value, style, options);
		}

		public static string TextArea(string value, GUIStyle style, params GUILayoutOption[] options)
		{
			int textFieldID = GUIUtility.GetControlID("TextArea".GetHashCode(), FocusType.Keyboard) + 1;
			if (textFieldID == 0)
				return value;

			value = HandleCopyPaste(textFieldID) ?? value;

			return GUILayout.TextArea(value, style, options);
		}

		public static object EnumPopup(string title, Enum selected, params GUILayoutOption[] options)
		{
			int index = 0;
			var array = Enum.GetValues(selected.GetType());
			int length = array.Length;

			string[] enumString = new string[length];
			for (int i = 0; i < length; i++)
			{
				FieldInfo[] fields = selected.GetType().GetFields();
				foreach (FieldInfo field in fields)
				{
					if (field.Name.Equals(array.GetValue(i).ToString()))
					{
						object[] objs = field.GetCustomAttributes(typeof(Float.EnumLabelAttribute), true);
						if (objs != null && objs.Length > 0)
						{
							enumString[i] = ((Float.EnumLabelAttribute)objs[0]).label;
						}
					}
				}
			}

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel(title);
			index = EditorGUILayout.Popup(selected.GetHashCode(), enumString, options);
			EditorGUILayout.EndHorizontal();

			return Enum.ToObject(selected.GetType(), index);
		}

		public static object EnumPopup(Enum selected, params GUILayoutOption[] options)
		{
			int index = 0;
			var array = Enum.GetValues(selected.GetType());
			int length = array.Length;

			string[] enumString = new string[length];
			for (int i = 0; i < length; i++)
			{
				FieldInfo[] fields = selected.GetType().GetFields();
				foreach (FieldInfo field in fields)
				{
					if (field.Name.Equals(array.GetValue(i).ToString()))
					{
						object[] objs = field.GetCustomAttributes(typeof(Float.EnumLabelAttribute), true);
						if (objs != null && objs.Length > 0)
						{
							enumString[i] = ((Float.EnumLabelAttribute)objs[0]).label;
						}
					}
				}
			}

			index = EditorGUILayout.Popup(selected.GetHashCode(), enumString, options);

			return Enum.ToObject(selected.GetType(), index);
		}

		public static object EnumPopup2(Enum selected, params GUILayoutOption[] options)
		{
			int index = 0;
			var array = Enum.GetValues(selected.GetType());
			int length = array.Length;

			string[] enumString = new string[length];
			for (int i = 0; i < length; i++)
			{
				FieldInfo[] fields = selected.GetType().GetFields();
				foreach (FieldInfo field in fields)
				{
					if (field.Name.Equals(array.GetValue(i).ToString()))
					{
						object[] objs = field.GetCustomAttributes(typeof(Float.EnumLanguageAttribute), true);
						if (objs != null && objs.Length > 0)
						{
							enumString[i] = Language.Get(((Float.EnumLanguageAttribute)objs[0]).label);
						}
					}
				}
			}

			index = EditorGUILayout.Popup(selected.GetHashCode(), enumString, options);

			return Enum.ToObject(selected.GetType(), index);
		}

	}

}
