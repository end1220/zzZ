
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using Debug = UnityEngine.Debug;


public class Hook : MonoBehaviour, IManager
{
	//钩子接收消息的结构
	public struct CWPSTRUCT
	{
		public int lparam;
		public int wparam;
		public uint message;
		public IntPtr hwnd;
	}

	[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
	private static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, uint dwThreadId);

	[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
	private static extern bool UnhookWindowsHookEx(int idHook);


	//把信息传递到下一个监听
	[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
	private static extern int CallNextHookEx(int idHook, int nCode, int wParam, int lParam);

	private delegate int HookProc(int nCode, int wParam, int lParam);

	int idHook = 0;
	bool isHooked = false;
	GCHandle gc;
	private const int WH_CALLWNDPROC = 4;  //钩子类型 全局钩子

	//定义结构和发送的结构对应
	public unsafe struct IPC_Head
	{
		public int wVersion;
		public int wPacketSize;
		public int wMainCmdID;
		public int wSubCmdID;
	}

	private const int IPC_BUFFER = 10240;//最大缓冲长度

	public unsafe struct IPC_Buffer
	{
		public IPC_Head Head;
		public fixed byte cbBuffer[IPC_BUFFER];  //json数据存的地方
	}

	public struct COPYDATASTRUCT
	{
		public int dwData;
		public int cbData;
		public IntPtr lpData;
	}

	public void Init()
	{
		AddHook();
	}

	public void Destroy()
	{
		RemoveHook();
	}

	public void Tick()
	{

	}


	private void AddHook()
	{
		HookProc lpfn = new HookProc(HookCallback);
		//关联进程的主模块
		IntPtr hInstance = IntPtr.Zero;// GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName);
		idHook = SetWindowsHookEx(WH_CALLWNDPROC, lpfn, hInstance, (uint)AppDomain.GetCurrentThreadId());
		if (idHook > 0)
		{
			Debug.Log("钩子[" + idHook + "]安装成功");
			isHooked = true;
			//保持活动 避免 回调过程 被垃圾回收
			gc = GCHandle.Alloc(lpfn);
		}
		else
		{
			Debug.Log("hook failed.");
			isHooked = false;
			UnhookWindowsHookEx(idHook);
		}

	}

	private void RemoveHook()
	{
		if (isHooked)
		{
			UnhookWindowsHookEx(idHook);
		}
	}

	private unsafe int HookCallback(int nCode, int wParam, int lParam)
	{
		try
		{
			IntPtr p = new IntPtr(lParam);
			CWPSTRUCT m = (CWPSTRUCT)Marshal.PtrToStructure(p, typeof(CWPSTRUCT));

			if (m.message == 74)
			{
				COPYDATASTRUCT entries = (COPYDATASTRUCT)Marshal.PtrToStructure((IntPtr)m.lparam, typeof(COPYDATASTRUCT));
				IPC_Buffer entries1 = (IPC_Buffer)Marshal.PtrToStructure((IntPtr)entries.lpData, typeof(IPC_Buffer));

				IntPtr intp = new IntPtr(entries1.cbBuffer);
				string str = new string((sbyte*)intp);
				Debug.Log("json数据：" + str);
			}

			return CallNextHookEx(idHook, nCode, wParam, lParam);
		}
		catch (Exception ex)
		{
			Debug.Log(ex.Message);
			return 0;
		}

	}
}
