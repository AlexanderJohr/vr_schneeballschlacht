using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListofPlayers : MonoBehaviour {

	[SerializeField] GameObject _playerListingPrefab;
	List<PlayerEntry> _playerListings = new List<PlayerEntry>();

	// Called by photon whenever this client join a room
	private void OnJoinedRoom()
	{
		foreach(Transform child in transform)
		{
			Destroy(child.gameObject);
		}

		PhotonPlayer[] photonPlayers = PhotonNetwork.playerList;
		for (int i =0; i < photonPlayers.Length; i++)
		{
			PlayerJoinedRoom(photonPlayers[i]);
		}
	}

	//Called by photon when a player joins the room.
	private void OnPhotonPlayerConnected(PhotonPlayer photonPlayer)
	{
		PlayerJoinedRoom(photonPlayer);
	}

	// Called by photon when a player leaves the room
	private void OnPhotonPlayerDisconnected(PhotonPlayer photonPlayer)
	{
		PlayerLeftRoom(photonPlayer);
	}

	private void PlayerJoinedRoom(PhotonPlayer photonPlayer)
	{
		if(photonPlayer == null)
		{
			return;
		}
		PlayerLeftRoom(photonPlayer);

		GameObject playerListingObj = Instantiate(GetPlayerListingPrefab());
		playerListingObj.transform.SetParent(transform, false);

		PlayerEntry playerEntry = playerListingObj.GetComponentInChildren<PlayerEntry>();
		playerEntry.ApplyPhotonPlayer(photonPlayer);

		GetPlayerListings().Add(playerEntry);
	}

	private void PlayerLeftRoom(PhotonPlayer photonPlayer)
	{
		int index = GetPlayerListings().FindIndex(x => x.PhotonPlayer == photonPlayer);
		if (index != -1)
		{
			Destroy(GetPlayerListings()[index].gameObject);
			GetPlayerListings().RemoveAt(index);
		}
	}

	// Called by photon whenever the master client is swithced.
	// Leave the master client the room should all player leave this room
	private void OnMasterClientSwitched(PhotonPlayer newMasterClient)
	{
		PhotonNetwork.LeaveRoom();
	}

	public void LeaveOpenRoom()
	{
		PhotonNetwork.LeaveRoom();
	}

	private GameObject GetPlayerListingPrefab()
	{
		return _playerListingPrefab;
	}

	private List<PlayerEntry> GetPlayerListings()
	{
		return _playerListings;
	}
}
