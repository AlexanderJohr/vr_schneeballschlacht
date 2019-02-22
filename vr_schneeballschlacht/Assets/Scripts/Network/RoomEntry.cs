using UnityEngine;
using UnityEngine.UI;

public class RoomEntry : MonoBehaviour {

	[SerializeField] Text _roomNameText;
	[SerializeField] Text _numberOFPlayerInRoom;
	[SerializeField] Button button;

	// Use this for initialization
	void Start () {
		GameObject lobbyCanvasObj = MainCanvasManager.Instance.GetLobbyCanvas().gameObject;
		GameObject openRoomCanvasObj = MainCanvasManager.Instance.GetOpenRoomCanvas().gameObject;
		if (lobbyCanvasObj == null)
		{
			return;
		}

		LobbyCanvas lobbyCanvas = lobbyCanvasObj.GetComponent<LobbyCanvas>();
		Button button = GetComponentInChildren<Button>();
		button.onClick.AddListener(() => lobbyCanvas.OnClickJoinRoom(GetRoomName().text));
		button.onClick.AddListener(() => lobbyCanvasObj.gameObject.SetActive(false));
		button.onClick.AddListener(() => openRoomCanvasObj.gameObject.SetActive(true));
	}

	private void OnDestroy()
	{
		Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
        }
	}

	public bool Updated { get; set; }

	public void SetRoomName(string value)
	{
		_roomNameText.text = value;
	}

	public Text GetRoomName()
	{
		return _roomNameText;
	}

	public void SetNumberOfPlayerInRoom(string value)
	{
		_numberOFPlayerInRoom.text = value;
	}

	public Text GetNumberOfPlayerInRoom()
	{
		return _numberOFPlayerInRoom;
	}
}
