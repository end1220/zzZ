using UnityEngine;
using UnityEditor;

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

	public static void Ensure()
	{
		int fontSizeNormal = 12;
		int fontSizeInput = 14;
		int fontSizeLarge = 18;

		skin = Resources.Load<GUISkin>("FloatGUISkin");
		link = skin.FindStyle("Link");

		label = new GUIStyle(EditorStyles.label);
		label.fontSize = fontSizeNormal;

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
	}
}
