using UnityEngine;
using UnityEngine.UI;

public class LobbyCanvas : MonoBehaviour {

	[SerializeField] ListofRooms listofRooms;

	public void OnClickJoinRoom(string roomName)
	{
		if (PhotonNetwork.JoinRoom(roomName))
		{
			Debug.Log("Player could join to the room: " + roomName);
		}
		else
		{
			print("Join room failed.");
		}
	}

	public ListofRooms GetListofRooms()
	{
		return listofRooms;
	}
}