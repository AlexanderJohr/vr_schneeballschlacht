using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ManagementForScene : MonoBehaviour {

    static ManagementForScene Instance;
    private int playersInGame;
    public PhotonView PhotonView;

    void Awake()
    {

        // if the singleton hasn't been initialized yet
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }

        Instance = this;
        DontDestroyOnLoad(this);
    }

    // Use this for initialization
    void Start () {
        playersInGame = 0;
        PhotonView = GetComponent<PhotonView>();
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    private void OnEnable()
    {
        
    }

    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {

        if (scene.name != "MainMenu")
        {
            if (PhotonNetwork.isMasterClient)
                MasterClientLoadedGame(scene.buildIndex);
            else
                ClientLoadedGame();
        }
    }

    private void MasterClientLoadedGame(int scene)
    {
        PhotonView.RPC("RPC_LoadedGameScene", PhotonTargets.MasterClient);
        PhotonView.RPC("RPC_LoadGameOthers", PhotonTargets.Others, scene);
    }

    private void ClientLoadedGame()
    {
        PhotonView.RPC("RPC_LoadedGameScene", PhotonTargets.MasterClient);
    }

    [PunRPC]
    private void RPC_LoadGameOthers(int scene)
    {
        PhotonNetwork.LoadLevel(scene);
    }

    [PunRPC]
    private void RPC_LoadedGameScene()
    {
        playersInGame++;
        if (playersInGame == PhotonNetwork.playerList.Length)
        {
            print("All players are in the game scene.");
            PhotonView.RPC("RPC_CreatePlayer", PhotonTargets.All);
        }
    }

    [PunRPC]
    private void RPC_CreatePlayer()
    {
        Debug.Log("NetworkPlayer will createt");
        PhotonNetwork.Instantiate("NetworkPlayer", Vector3.zero, Quaternion.identity, 0);
    }

}
