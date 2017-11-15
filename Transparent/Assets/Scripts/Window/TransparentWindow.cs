using System;
using System.Runtime.InteropServices;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TransparentWindow : MonoBehaviour
{
	[SerializeField]
	private Material m_Material;

	[SerializeField]
	private Camera mainCamera;

	private Vector3 lastMousePosition;
	private bool lastHoverModel = true;

	private struct MARGINS
	{
		public int cxLeftWidth;
		public int cxRightWidth;
		public int cyTopHeight;
		public int cyBottomHeight;
	}

	[DllImport("user32.dll")]
	private static extern IntPtr GetActiveWindow();

	[DllImport("user32.dll")]
	private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

	[DllImport("user32.dll")]
	static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

	[DllImport("user32.dll", EntryPoint = "SetLayeredWindowAttributes")]
	static extern int SetLayeredWindowAttributes(IntPtr hwnd, int crKey, byte bAlpha, int dwFlags);

	[DllImport("user32.dll", EntryPoint = "SetWindowPos")]
	private static extern int SetWindowPos(IntPtr hwnd, int hwndInsertAfter, int x, int y, int cx, int cy, int uFlags);

	[DllImport("Dwmapi.dll")]
	private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

	const int GWL_STYLE = -16;
	const uint WS_POPUP = 0x80000000;
	const uint WS_VISIBLE = 0x10000000;
	const int HWND_TOPMOST = -1;

	int fWidth;
	int fHeight;
	IntPtr hwnd;
	MARGINS margins;

	void Start()
	{
		mainCamera = GetComponent<Camera>();
		lastMousePosition = Input.mousePosition;

#if !UNITY_EDITOR
		InitWnd();
#endif
	}

	void Update()
	{
		//int layer = LayerMask.NameToLayer("Default");

		bool hoverModel = true;
		Vector3 mousePosition = Input.mousePosition;
		if (mousePosition != lastMousePosition)
		{
			Ray ray = mainCamera.ScreenPointToRay(mousePosition);
			RaycastHit hit = new RaycastHit();
			hoverModel = Physics.Raycast(ray, out hit, 100);
			lastMousePosition = mousePosition;

			if (hoverModel != lastHoverModel)
			{
#if !UNITY_EDITOR
				SetTransparent(!hoverModel);
#endif
				lastHoverModel = hoverModel;
			}
		}

	}

	void OnRenderImage(RenderTexture from, RenderTexture to)
	{
		Graphics.Blit(from, to, m_Material);
	}

	private void InitWnd()
	{
		fWidth = Screen.width;
		fHeight = Screen.height;
		margins = new MARGINS() { cxLeftWidth = -1 };
		hwnd = GetActiveWindow();

		SetWindowLong(hwnd, GWL_STYLE, WS_POPUP | WS_VISIBLE);
		SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, fWidth, fHeight, 32 | 64); //SWP_FRAMECHANGED = 0x0020 (32); //SWP_SHOWWINDOW = 0x0040 (64)
		DwmExtendFrameIntoClientArea(hwnd, ref margins);
	}

	private void SetTransparent(bool transparent)
	{
		if (transparent)
		{
			SetWindowLong(hwnd, GWL_STYLE, WS_POPUP | WS_VISIBLE);
			SetWindowLong(hwnd, -20, (uint)524288 | (uint)32);//GWL_EXSTYLE=-20; WS_EX_LAYERED=524288=&h80000, WS_EX_TRANSPARENT=32=0x00000020L
			SetLayeredWindowAttributes(hwnd, 0, 255, 2);// Transparency=51=20%, LWA_ALPHA=2
			SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, fWidth, fHeight, 32 | 64); //SWP_FRAMECHANGED = 0x0020 (32); //SWP_SHOWWINDOW = 0x0040 (64)
		}
		else
		{
			SetWindowLong(hwnd, -20, ~(((uint)524288) | ((uint)32)));//GWL_EXSTYLE=-20; WS_EX_LAYERED=524288=&h80000, WS_EX_TRANSPARENT=32=0x00000020L
			SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, fWidth, fHeight, 32 | 64); //SWP_FRAMECHANGED = 0x0020 (32); //SWP_SHOWWINDOW = 0x0040 (64)
		}
	}
}