
using System.Collections;
using Steamworks;


namespace Float
{
	public class SteamManager
	{
		private static SteamManager s_instance;
		public static SteamManager Instance
		{
			get
			{
				if (s_instance == null) s_instance = new SteamManager();
				return s_instance;
			}
		}

		private static bool s_EverInialized;

		private bool m_bInitialized;
		public static bool Initialized
		{
			get
			{
				return Instance.m_bInitialized;
			}
		}

		private SteamAPIWarningMessageHook_t m_SteamAPIWarningMessageHook;
		private static void SteamAPIDebugTextHook(int nSeverity, System.Text.StringBuilder pchDebugText)
		{
			Log.Warning(pchDebugText.ToString());
		}

		public void Init()
		{
			if (s_EverInialized)
			{
				throw new System.Exception("Tried to Initialize the SteamAPI twice in one session!");
			}

			if (!Packsize.Test())
			{
				Log.Error("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.");
			}

			if (!DllCheck.Test())
			{
				Log.Error("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.");
			}

			try
			{
				// If Steam is not running or the game wasn't started through Steam, SteamAPI_RestartAppIfNecessary starts the
				// Steam client and also launches this game again if the User owns it. This can act as a rudimentary form of DRM.

				// Once you get a Steam AppID assigned by Valve, you need to replace AppId_t.Invalid with it and
				// remove steam_appid.txt from the game depot. eg: "(AppId_t)480" or "new AppId_t(480)".
				// See the Valve documentation for more information: https://partner.steamgames.com/documentation/drm#FAQ
				if (SteamAPI.RestartAppIfNecessary(AppId_t.Invalid))
				{
					App.Instance.Shutdown();
					return;
				}
			}
			catch (System.DllNotFoundException e)
			{
				Log.Error("[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib. It's likely not in the correct location. Refer to the README for more details.\n" + e);

				App.Instance.Shutdown();
				return;
			}

			// Initialize the SteamAPI, if Init() returns false this can happen for many reasons.
			// Some examples include:
			// Steam Client is not running.
			// Launching from outside of steam without a steam_appid.txt file in place.
			// Running under a different OS User or Access level (for example running "as administrator")
			// Ensure that you own a license for the AppId on your active Steam account
			// If your AppId is not completely set up. Either in Release State: Unavailable, or if it's missing default packages.
			// Valve's documentation for this is located here:
			// https://partner.steamgames.com/documentation/getting_started
			// https://partner.steamgames.com/documentation/example // Under: Common Build Problems
			// https://partner.steamgames.com/documentation/bootstrap_stats // At the very bottom

			// If you're running into Init issues try running DbgView prior to launching to get the internal output from Steam.
			// http://technet.microsoft.com/en-us/sysinternals/bb896647.aspx
			m_bInitialized = SteamAPI.Init();
			if (!m_bInitialized)
			{
				Log.Error("[Steamworks.NET] SteamAPI_Init() failed. Refer to Valve's documentation or the comment above this line for more information.");
				return;
			}

			s_EverInialized = true;

			if (m_SteamAPIWarningMessageHook == null)
			{
				// Set up our callback to recieve warning messages from Steam.
				// You must launch with "-debug_steamapi" in the launch args to recieve warnings.
				m_SteamAPIWarningMessageHook = new SteamAPIWarningMessageHook_t(SteamAPIDebugTextHook);
				SteamClient.SetWarningMessageHook(m_SteamAPIWarningMessageHook);
			}
		}

		public void Destroy()
		{
			s_instance = null;

			if (m_bInitialized)
			{
				SteamAPI.Shutdown();
			}
		}

		public void Update()
		{
			if (!m_bInitialized)
			{
				return;
			}

			// Run Steam client callbacks
			SteamAPI.RunCallbacks();
		}
	}

}