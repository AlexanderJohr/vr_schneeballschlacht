using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListofRooms : MonoBehaviour {

	[SerializeField] GameObject _roomListingPrefab;
	List<RoomEntry> _roomListingButtons = new List<RoomEntry>(); 

	//Method from Photon
	private void OnReceivedRoomListUpdate()
	{
		RoomInfo[] rooms = PhotonNetwork.GetRoomList();
		foreach(RoomInfo room in rooms)
		{
			RoomReceived(room);
		}
		RemoveOldRooms();
	}

	// Check if the room still exist
	private void RoomReceived(RoomInfo room)
	{
		int index = GetRoomListingButtons().FindIndex(x => x.GetRoomName().text == room.Name);
		if(index == -1)
		{
			if(room.IsVisible && room.PlayerCount < room.MaxPlayers)
			{
				GameObject networkRoomEntryObj = Instantiate(GetRoomListingPrefab());
				networkRoomEntryObj.transform.SetParent(transform, false);

				RoomEntry networkRoomEntry = networkRoomEntryObj.GetComponent<RoomEntry>();
				GetRoomListingButtons().Add(networkRoomEntry);
				index = (GetRoomListingButtons().Count - 1);
			}
		}
		if(index != -1)
		{
			RoomEntry roomEntry = GetRoomListingButtons()[index];
			roomEntry.SetRoomName(room.Name);
			roomEntry.Updated = true;
		}
	}

	// Delete the rooms which are not exist anymore
	private void RemoveOldRooms()
	{
		List<RoomEntry> removesRooms = new List<RoomEntry>();
		foreach (RoomEntry roomEntry in GetRoomListingButtons())
		{
			if (!roomEntry.Updated)
			{
				removesRooms.Add(roomEntry);
			} else
			{
				roomEntry.Updated = false;
			}
		}

		foreach (RoomEntry roomEntry in removesRooms)
		{
			GameObject roomEntryObj = roomEntry.gameObject;
			GetRoomListingButtons().Remove(roomEntry);
			Destroy(roomEntryObj);
		}
	}

	public GameObject GetRoomListingPrefab()
	{
		return _roomListingPrefab;
	}

	public List<RoomEntry> GetRoomListingButtons()
	{
		return _roomListingButtons;
	}
}
