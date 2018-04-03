
using UnityEngine;
using Steamworks;


public class SteamManager
{
	private static SteamManager inst;
	public static SteamManager Instance
	{
		get
		{
			if (inst == null)
				inst = new SteamManager();
			return inst;
		}
	}

	public bool Initialized { get; private set; }

	private SteamAPIWarningMessageHook_t m_SteamAPIWarningMessageHook;
	private static void SteamAPIDebugTextHook(int nSeverity, System.Text.StringBuilder pchDebugText)
	{
		Debug.LogWarning(pchDebugText);
	}

	public void Init()
	{
		if (Initialized)
			throw new System.Exception("Tried to Initialize the SteamAPI twice in one session!");

		if (!Packsize.Test())
			Debug.LogError("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.");

		if (!DllCheck.Test())
			Debug.LogError("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.");

		try
		{
			if (SteamAPI.RestartAppIfNecessary(AppId_t.Invalid))
			{
				Application.Quit();
				return;
			}
		}
		catch (System.DllNotFoundException e)
		{
			Debug.LogError("[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib. It's likely not in the correct location. Refer to the README for more details.\n" + e);
			Application.Quit();
			return;
		}

		Initialized = SteamAPI.Init();
		if (!Initialized)
		{
			//Debug.LogError("[Steamworks.NET] SteamAPI_Init() failed. Refer to Valve's documentation or the comment above this line for more information.");
			Debug.LogError("[Steamworks.NET] SteamAPI_Init() failed. Please make sure your steam client is running.");
			return;
		}

		if (m_SteamAPIWarningMessageHook == null)
		{
			// You must launch with "-debug_steamapi" in the launch args to recieve warnings.
			m_SteamAPIWarningMessageHook = new SteamAPIWarningMessageHook_t(SteamAPIDebugTextHook);
			SteamClient.SetWarningMessageHook(m_SteamAPIWarningMessageHook);
		}
	}


	public void Destroy()
	{
		if (!Initialized)
			return;

		Initialized = false;
		SteamAPI.Shutdown();
	}

	public void Update()
	{
		if (!Initialized)
			return;

		SteamAPI.RunCallbacks();
	}
}
