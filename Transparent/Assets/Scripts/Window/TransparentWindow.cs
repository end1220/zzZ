using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;


[RequireComponent(typeof(Camera))]
public class TransparentWindow : MonoBehaviour
{
	/*[SerializeField]
	private Material m_Material;

	[SerializeField]*/
	private Camera mainCamera;

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
	private static extern int ModifyStyleEx(uint nIndex, uint dwNewLong);

	[DllImport("user32.dll")]
	static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

	[DllImport("user32.dll", EntryPoint = "SetLayeredWindowAttributes")]
	static extern int SetLayeredWindowAttributes(IntPtr hwnd, int crKey, byte bAlpha, int dwFlags);

	[DllImport("user32.dll", EntryPoint = "SetWindowPos")]
	private static extern int SetWindowPos(IntPtr hwnd, int hwndInsertAfter, int x, int y, int cx, int cy, int uFlags);

	[DllImport("Dwmapi.dll")]
	private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

	const int GWL_STYLE = -16;
	const int GWL_EXSTYLE = -20;
	const uint WS_POPUP = 0x80000000;
	const uint WS_VISIBLE = 0x10000000;
	//const uint WS_EX_APPWINDOW = 0x00040000;
	const uint WS_EX_LAYERED = 0x00080000;
	const uint WS_EX_TRANSPARENT = 0x00000020;
	const uint WS_EX_TOOLWINDOW = 0x00000080;
	const int HWND_TOPMOST = -1;
	const uint SWP_FRAMECHANGED = 0x0020;
	const uint SWP_SHOWWINDOW = 0x0040;
	const uint SWP_HIDEWINDOW = 0x0080;

	Vector3 lastMousePosition;
	bool lastHoverModel = true;

	float wndPosX;
	float wndPosY;
	int wndWidth;
	int wndHeight;
	IntPtr hwnd;
	MARGINS margins;
	Texture2D textureClick;


	void Start()
	{
		mainCamera = GetComponent<Camera>();
		textureClick = new Texture2D(1, 1, TextureFormat.RGBA32, false);
		lastMousePosition = Input.mousePosition;

		PlayerPrefs.SetFloat("WindowPosX", 0);
		PlayerPrefs.SetFloat("WindowPosY", 0);

		wndPosX = PlayerPrefs.GetFloat("WindowPosX");
		wndPosY = PlayerPrefs.GetFloat("WindowPosY");
		wndWidth = Screen.width;
		wndHeight = Screen.height;
		margins = new MARGINS() { cxLeftWidth = -1 };

#if !UNITY_EDITOR
		InitWnd();
#endif
		StartCoroutine(TestMouseCollision());
	}

	WaitForEndOfFrame endofframe = new WaitForEndOfFrame();
	IEnumerator TestMouseCollision()
	{
		while (true)
		{
			bool hoverModel = true;
			Vector3 mousePosition = Input.mousePosition;
			if (mousePosition != lastMousePosition)
			{
				if (mousePosition.x >= 0 && mousePosition.x < wndWidth && mousePosition.y >= 0 && mousePosition.y < wndHeight)
				{
					textureClick.ReadPixels(new Rect(mousePosition.x, mousePosition.y, 1, 1), 0, 0);
					Color color = textureClick.GetPixel(0, 0);
					hoverModel = color != Color.clear;
				}
				else
				{
					hoverModel = false;
				}
				/*Ray ray = mainCamera.ScreenPointToRay(mousePosition);
				RaycastHit hit = new RaycastHit();
				hoverModel = Physics.Raycast(ray, out hit, 100);*/
				lastMousePosition = mousePosition;

				if (hoverModel != lastHoverModel)
				{
#if !UNITY_EDITOR
				SetTransparent(!hoverModel);
#else
					//Debug.Log(hoverModel ? "enter" : "leave");
#endif
					lastHoverModel = hoverModel;
				}
			}
			yield return endofframe;
		}
	}

	/*void OnRenderImage(RenderTexture from, RenderTexture to)
	{
		Graphics.Blit(from, to, m_Material);
	}*/

	private void InitWnd()
	{
		hwnd = GetActiveWindow();

		SetWindowLong(hwnd, GWL_STYLE, WS_POPUP | WS_VISIBLE);
		SetWindowPos(hwnd, HWND_TOPMOST, (int)wndPosX, (int)wndPosY, wndWidth, wndHeight, (int)(SWP_FRAMECHANGED | SWP_SHOWWINDOW));
		OnResize(wndWidth, wndHeight);
		DwmExtendFrameIntoClientArea(hwnd, ref margins);
	}

	private void SetTransparent(bool transparent)
	{
		if (transparent)
		{
			SetWindowLong(hwnd, GWL_STYLE, WS_POPUP | WS_VISIBLE);
			SetWindowLong(hwnd, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT/* | WS_EX_TOOLWINDOW*/);
			SetLayeredWindowAttributes(hwnd, 0, 255, 2);// Transparency=51=20%, LWA_ALPHA=2
			SetWindowPos(hwnd, HWND_TOPMOST, (int)wndPosX, (int)wndPosY, wndWidth, wndHeight, (int)(SWP_FRAMECHANGED | SWP_SHOWWINDOW));
		}
		else
		{
			SetWindowLong(hwnd, GWL_EXSTYLE, ~(WS_EX_LAYERED | WS_EX_TRANSPARENT));
			SetWindowPos(hwnd, HWND_TOPMOST, (int)wndPosX, (int)wndPosY, wndWidth, wndHeight, (int)(SWP_FRAMECHANGED | SWP_SHOWWINDOW));
		}
	}

	public void MoveWindow(float dx, float dy)
	{
		Debug.Log("move " + dx + ", " + dy);
#if !UNITY_EDITOR
		wndPosX += dx;
		wndPosY += dy;
		SetWindowPos(hwnd, HWND_TOPMOST, (int)wndPosX, (int)wndPosY, wndWidth, wndHeight, (int)(SWP_FRAMECHANGED | SWP_SHOWWINDOW));
		PlayerPrefs.SetFloat("WindowPosX", wndPosX);
		PlayerPrefs.SetFloat("WindowPosY", wndPosY);
#endif
	}

	public void SetWindowVisible(bool visible)
	{
		if (visible)
		{
			Application.runInBackground = true;
			SetWindowPos(hwnd, HWND_TOPMOST, (int)wndPosX, (int)wndPosY, wndWidth, wndHeight, (int)(SWP_FRAMECHANGED | SWP_SHOWWINDOW));
		}
		else
		{
			Application.runInBackground = false;
			SetWindowPos(hwnd, HWND_TOPMOST, (int)wndPosX, (int)wndPosY, wndWidth, wndHeight, (int)(SWP_HIDEWINDOW));
		}
	}

	public void SetWindowSize(int width, int height)
	{
		//OnResize(width, height);
	}

	protected void OnResize(int newWidth, int newHeight)
	{
		
	}

}