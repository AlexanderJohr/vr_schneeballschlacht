using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCanvasManager : MonoBehaviour {

	static MainCanvasManager instance;

	[SerializeField] LobbyCanvas lobbyCanvas;
	[SerializeField] OpenRoomCanvas openRoomCanvas;

	void Awake () {
		// if the singleton hasn't been initialized yet
		if (instance != null && instance != this)
		{
			Destroy(this.gameObject);
		}

		instance = this;
	}

	public static MainCanvasManager Instance { get { return instance; } }

	public LobbyCanvas GetLobbyCanvas()
	{
		return lobbyCanvas;
	}

	public OpenRoomCanvas GetOpenRoomCanvas()
	{
		return openRoomCanvas;
	}
}