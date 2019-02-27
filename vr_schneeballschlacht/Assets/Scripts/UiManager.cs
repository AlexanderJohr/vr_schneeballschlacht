using Prototype.NetworkLobby;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour {
    
    public void PlayGameButtonPress() {
        SceneManager.LoadScene("Network Lobby");
    }

    public void RematchButtonPress()
    {
        LobbyManager existingLobbyManager = GameObject.FindObjectOfType<LobbyManager>();
        if (existingLobbyManager != null)
        {
            existingLobbyManager.backDelegate();
        }
    }
}
