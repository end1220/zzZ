
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using Debug = UnityEngine.Debug;


public class Hook : MonoBehaviour, IManager
{
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

	[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
	private static extern int CallNextHookEx(int idHook, int nCode, int wParam, int lParam);

	private delegate int HookProc(int nCode, int wParam, int lParam);

	int idHook = 0;
	bool isHooked = false;
	GCHandle gc;
	private const int WH_CALLWNDPROC = 4;

	public unsafe struct IPC_Head
	{
		public int wVersion;
		public int wPacketSize;
		public int wMainCmdID;
		public int wSubCmdID;
	}

	private const int IPC_BUFFER = 10240;

	public unsafe struct IPC_Buffer
	{
		public IPC_Head Head;
		public fixed byte cbBuffer[IPC_BUFFER];
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
		CommandHandler.Register();
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
			//避免被垃圾回收
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
			IntPtr ptr = new IntPtr(lParam);
			CWPSTRUCT m = (CWPSTRUCT)Marshal.PtrToStructure(ptr, typeof(CWPSTRUCT));
			if (m.message == 0x004A)
			{
				Log.Error("HookCallback 1");
				COPYDATASTRUCT copydata = (COPYDATASTRUCT)Marshal.PtrToStructure((IntPtr)m.lparam, typeof(COPYDATASTRUCT));
				IPC_Buffer ipc = (IPC_Buffer)Marshal.PtrToStructure(copydata.lpData, typeof(IPC_Buffer));

				IntPtr intp = new IntPtr(ipc.cbBuffer);
				int length = copydata.cbData - (ushort)Marshal.SizeOf(typeof(IPC_Head));
				byte[] data = new byte[length];
				Marshal.Copy(intp, data, 0, length);
				
				Lite.ByteBuffer buffer = new Lite.ByteBuffer(data);
				Lite.Packet packet = new Lite.Packet();
				packet.length = (ushort)data.Length;
				packet.msgId = buffer.ReadShort();
				packet.stamp = 0;
				packet.data = buffer.ReadBytes();
				Log.Error("HookCallback 2");
				CommandHandler.Handle(packet);
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
