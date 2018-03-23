using UnityEngine;
using UnityEditor;

public static class FloatGUIStyle
{
	private static GUISkin skin;
	public static GUIStyle label { get { return skin.label; } }
	public static GUIStyle textField { get { return skin.textField; } }
	public static GUIStyle textArea { get { return skin.textArea; } }
	public static GUIStyle toggle { get { return skin.toggle; } }
	public static GUIStyle button { get { return skin.button; } }
	public static GUIStyle styleLink { get; private set; }

	public static void Ensure()
	{
		skin = Resources.Load<GUISkin>("FloatGUISkin");
		styleLink = skin.FindStyle("Link");
	}
}
