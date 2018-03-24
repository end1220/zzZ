using UnityEngine;
using UnityEditor;

public static class FloatGUIStyle
{
	private static GUISkin skin;
	public static GUIStyle label;
	public static GUIStyle boldLabel;
	public static GUIStyle textField;
	public static GUIStyle textArea;
	public static GUIStyle toggle;
	public static GUIStyle button;
	public static GUIStyle helpBox;
	public static GUIStyle link { get; private set; }

	public static void Ensure()
	{
		int fontSizeNormal = 12;
		int fontSizeInput = 14;

		skin = Resources.Load<GUISkin>("FloatGUISkin");
		link = skin.FindStyle("Link");

		label = new GUIStyle(EditorStyles.label);
		label.fontSize = fontSizeNormal;

		boldLabel = new GUIStyle(EditorStyles.label);
		boldLabel.fontStyle = FontStyle.Bold;
		boldLabel.fontSize = fontSizeNormal;

		textField = new GUIStyle(EditorStyles.textField);
		textField.fontSize = fontSizeInput;

		textArea = new GUIStyle(EditorStyles.textArea);
		textArea.fontSize = fontSizeInput;

		helpBox = new GUIStyle(EditorStyles.helpBox);
		helpBox.fontSize = fontSizeNormal;

	}
}
