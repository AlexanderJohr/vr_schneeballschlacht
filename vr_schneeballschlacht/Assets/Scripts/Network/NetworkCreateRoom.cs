using UnityEngine.UI;
using UnityEngine;

public class NetworkCreateRoom : MonoBehaviour {

	int maxPlayer = 2;
	[SerializeField] Text roomName;


	public void CreateOneRoom(){
		RoomOptions roomOptions = new RoomOptions (){ IsVisible = true, IsOpen = true, MaxPlayers = (byte)maxPlayer };
		if (PhotonNetwork.CreateRoom (GetRoomNameText (), roomOptions, TypedLobby.Default)) {
			Debug.Log ("create room successfully sent to server");
		} else {
			Debug.Log ("create room failed sent to server");
		}
	}

	private void OnPhotonCreateRoomFailed(object[] codeAndMsg)
	{
		Debug.Log("create room failed: " + codeAndMsg[1]);
	}

	private void OnCreatedRoom()
	{
		Debug.Log("Room create was successfully");
	}

	public void SetRoomNameText(string text)
	{
		roomName.text = text;
	}

	public string GetRoomNameText()
	{
		return roomName.text;
	}

	public void ChangeMaxPlayerValue(int value)
	{
		if (value % 2 == 0) {
			maxPlayer = value;
		}
	}
}
