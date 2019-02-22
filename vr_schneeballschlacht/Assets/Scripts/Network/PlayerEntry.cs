using UnityEngine.UI;
using UnityEngine;

public class PlayerEntry : MonoBehaviour {

	public PhotonPlayer PhotonPlayer { get; private set; }

	[SerializeField] Text _playerName;

	public void ApplyPhotonPlayer(PhotonPlayer photonPlayer)
	{
		PhotonPlayer = photonPlayer;
		GetPlayerName().text = photonPlayer.NickName;
	}

	private Text GetPlayerName()
	{
		return _playerName;
	}
}
