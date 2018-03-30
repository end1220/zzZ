
using System.Collections;
using Steamworks;
using Float;

public class SteamScript
{
	protected Callback<GameOverlayActivated_t> m_GameOverlayActivated;

	private CallResult<NumberOfCurrentPlayers_t> m_NumberOfCurrentPlayers;

	void Init()
	{
		//if (SteamManager.Initialized)
		{
			string name = SteamFriends.GetPersonaName();
			Log.Info(name);

			m_GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);

			m_NumberOfCurrentPlayers = CallResult<NumberOfCurrentPlayers_t>.Create(OnNumberOfCurrentPlayers);
		}
	}

	private void Update()
	{
		SteamAPICall_t handle = SteamUserStats.GetNumberOfCurrentPlayers();
		m_NumberOfCurrentPlayers.Set(handle);
	}

	private void OnGameOverlayActivated(GameOverlayActivated_t pCallback)
	{
		if (pCallback.m_bActive != 0)
		{
			// pause game ?
			Log.Info("Steam Overlay has been activated");
		}
		else
		{
			// resume game?
			Log.Info("Steam Overlay has been closed");
		}
	}

	private void OnNumberOfCurrentPlayers(NumberOfCurrentPlayers_t pCallback, bool bIOFailure)
	{
		if (pCallback.m_bSuccess != 1 || bIOFailure)
		{
			Log.Info("There was an error retrieving the NumberOfCurrentPlayers.");
		}
		else
		{
			Log.Info("The number of players playing your game: " + pCallback.m_cPlayers);
		}
	}

}