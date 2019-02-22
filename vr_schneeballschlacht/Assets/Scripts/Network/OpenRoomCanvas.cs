using UnityEngine;

public class OpenRoomCanvas : MonoBehaviour {

		private int selectedLevel = 2;

		public void OnClickStartLevel()
		{
			if (PhotonNetwork.isMasterClient) {
				PhotonNetwork.room.IsOpen = false;
				PhotonNetwork.room.IsVisible = false;
				PhotonNetwork.LoadLevel(selectedLevel);
			}
		}

		public void SetSelectedLevel(int value)
		{
			selectedLevel = value;
		}

		public int GetSelectedLevel()
		{
			return selectedLevel;
		}

	}

