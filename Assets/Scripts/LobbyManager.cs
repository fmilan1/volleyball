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


public class LobbyManager : MonoBehaviour
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

    async void Start()
    {
        // await UnityServices.InitializeAsync();



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
        
        AssignButtons();
        ListLobbies();
    }


    void AssignButtons()
    {
        MakeBtn.onClick.AddListener(CreateLobby);
        RefreshBtn.onClick.AddListener(ListLobbies);
    }


    async void CreateLobby()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(int.Parse(newLobbyMaxPlayer.text) - 1);
            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            CreateLobbyOptions options = new CreateLobbyOptions();
            options.Data = new Dictionary<string, DataObject>()
            {
                {
                    "joinCode", new DataObject(
                        visibility: DataObject.VisibilityOptions.Public,
                        value: joinCode
                    )
                },
            };



            var lobby = await LobbyService.Instance.CreateLobbyAsync(newLobbyName.text, int.Parse(newLobbyMaxPlayer.text), options);
            Debug.Log("Lobby created witch joincode " + lobby.Data["joinCode"].Value);
            StartCoroutine(HeartbeatLobbyCoroutine(lobby.Id, 25));
            NetworkManager.Singleton.StartHost();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
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
            controller.code = lobby.Data["joinCode"].Value;
            controller.id = lobby.Id;
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
}
