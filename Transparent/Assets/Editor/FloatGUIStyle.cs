using UnityEngine;
using UnityEditor;

public static class FloatGUIStyle
{
	private static GUISkin skin;
	public static GUIStyle label;/* { get { return skin.label; } }*/
	public static GUIStyle boldLabel;
	public static GUIStyle textField;/* { get { return skin.textField; } }*/
	public static GUIStyle textArea;/* { get { return skin.textArea; } }*/
	public static GUIStyle toggle;/* { get { return skin.toggle; } }*/
	public static GUIStyle button;/* { get { return skin.button; } }*/
	public static GUIStyle helpBox;
	public static GUIStyle styleLink { get; private set; }

	public static void Ensure()
	{
		skin = Resources.Load<GUISkin>("FloatGUISkin");
		styleLink = skin.FindStyle("Link");

		label = new GUIStyle(EditorStyles.label);
		label.fontSize = 14;

		boldLabel = new GUIStyle(EditorStyles.boldLabel);
		boldLabel.fontSize = 14;

		textField = new GUIStyle(EditorStyles.textField);
		textField.fontSize = 14;

		textArea = new GUIStyle(EditorStyles.textArea);
		textArea.fontSize = 14;

		helpBox = new GUIStyle(EditorStyles.helpBox);
		helpBox.fontSize = 12;
	}
}
