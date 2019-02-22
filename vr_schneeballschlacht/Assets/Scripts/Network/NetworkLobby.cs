using UnityEngine;
using UnityEngine.UI;

public class NetworkLobby : MonoBehaviour {

	// FFFFFF64  BCBCBC64   			000000C8

	public InputField inputFieldPlayerName;
	public GameObject logInPanel;
	public Text statusInfo;
	public Text nameOfPlayerInfo;
	public GameObject lobbyPanel;


	public void StartConnectingOverButton()
	{
		if (inputFieldPlayerName.text.Length > 3) {
			ConnectionToServer ();
		}
	}

	private void ConnectionToServer()
	{
		PhotonNetwork.playerName = inputFieldPlayerName.text;
		Debug.Log("Try connecting to server");
		PhotonNetwork.ConnectUsingSettings("0.2");

	}

	private void OnConnectedToMaster()
	{
		Debug.Log("Connected to master");
	//	PhotonNetwork.automaticallySyncScene = false;
		PhotonNetwork.JoinLobby(TypedLobby.Default);
		UpdateCanvasUI ();
	}

	private void OnJoinedLobby()
	{
		if (!PhotonNetwork.inRoom)
		{
			//GameObject lobbyCanvasObj = NetworkMainCanvasManager.Instance.GetLobbyCanvas().gameObject;
			//GameObject openRoomCanvasObj = NetworkMainCanvasManager.Instance.GetOpenRoomCanvas().gameObject;
			//lobbyCanvasObj.SetActive(true);
			//openRoomCanvasObj.SetActive(false);
		}
		Debug.Log("Join to the Lobby");
	}

	private void UpdateCanvasUI(){
		statusInfo.text = "Online";
		nameOfPlayerInfo.text = inputFieldPlayerName.text;
		logInPanel.SetActive (false);
		lobbyPanel.SetActive (true);
	}


}
