using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using Unity.Networking.Transport.Relay;
using Unity.Netcode.Transports.UTP;
// using ParrelSync;

public class LobbyManager : NetworkBehaviour
{

    [SerializeField] Button MakeBtn;
    [SerializeField] Button RefreshBtn;

    [SerializeField] Button CreateMenuBtn;
    [SerializeField] GameObject LobbyPrefab;
    [SerializeField] Transform LobbyListContent;

    [SerializeField] TMPro.TMP_Text newLobbyName;
    [SerializeField] TMPro.TMP_Text newLobbyPassword;
    [SerializeField] TMPro.TMP_Text newLobbyMaxPlayer;
    [SerializeField] RelayManager relayManager;
    public Lobby createdLobby;

    async void Start()
    {

        var options = new InitializationOptions();

        // #if UNITY_EDITOR
        //     options.SetProfile(ClonesManager.IsClone() ? ClonesManager.GetArgument() : "Primary");
        // #endif

        await UnityServices.InitializeAsync(options);


        
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in as " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        GameObject.Find("PlayerName").transform.GetComponent<TMPro.TMP_InputField>().text = PlayerPrefs.GetString("playerName");
        AssignButtons();
        ListLobbies();
    }


    void AssignButtons()
    {
        MakeBtn.onClick.AddListener(CreateLobbyAndRelay);
        RefreshBtn.onClick.AddListener(ListLobbies);
    }

    async void CreateLobbyAndRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(int.Parse(newLobbyMaxPlayer.text) - 1);
            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            CreateLobbyOptions options = new CreateLobbyOptions()
            {
                Player = GetPlayer()
            };
            options.Data = new Dictionary<string, DataObject>()
            {
                {
                    "joinCode", new DataObject(
                        visibility: DataObject.VisibilityOptions.Public,
                        value: joinCode
                    )
                }                
            };



            var lobby = await LobbyService.Instance.CreateLobbyAsync(newLobbyName.text, int.Parse(newLobbyMaxPlayer.text), options);
            createdLobby = lobby;
            Debug.Log("Lobby created witch joincode " + lobby.Data["joinCode"].Value + " and lobbId: " + lobby.Id);
            StartCoroutine(HeartbeatLobbyCoroutine(lobby.Id, 25));
            NetworkManager.Singleton.StartHost();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void JoinLobby(Lobby lobby)
    {
        try
        {
            JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions()
            {
                Player = GetPlayer()
            };

            await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id, joinLobbyByIdOptions);
            Debug.Log($"joined lobby with id {lobby.Id}");
            
            relayManager.JoinRelay(lobby.Data["joinCode"].Value);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    Player GetPlayer()
    {
        string playerName = GameObject.Find("PlayerName").transform.GetComponentInChildren<TMPro.TMP_Text>().text;
        PlayerPrefs.SetString("playerName", playerName);
        PlayerPrefs.Save();
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                {
                    "playerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)
                }
            }
        };

    }

    private async void UpdateLobby(string id)
    {
        if (id == "") return;
        createdLobby = await LobbyService.Instance.GetLobbyAsync(id);
    }

    async void ListLobbies()
    {
        foreach (Transform child in LobbyListContent)
        {
            Destroy(child.gameObject);
        }
        QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
        if (queryResponse.Results.Count == 0)
        {
            CreateMenuBtn.Select();
            CreateMenuBtn.onClick.Invoke();
            return;
        }
        foreach (Lobby lobby in queryResponse.Results)
        {
            GameObject prefab = Instantiate(LobbyPrefab, LobbyListContent);
            var controller = prefab.GetComponent<LobbyPrefabController>();
            controller.lobbyName.text = lobby.Name;
            controller.maxPlayers.text = lobby.Players.Count + "/" + lobby.MaxPlayers;
            controller.lobby = lobby;
        }
    }



    IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
    {
        var delay = new WaitForSecondsRealtime(waitTimeSeconds);

        while (true)
        {
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }


    // async IEnumerator PollLobbyCoroutine(string lobbyId, float waitTimeSeconds)
    // {
    //     var delay = new WaitForSecondsRealtime(waitTimeSeconds);

    //     while (true)
    //     {
    //         // LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
    //         await LobbyService.Instance.UpdateLobbyAsync(lobbyId);
    //         yield return delay;
    //     }
    // }    
}
