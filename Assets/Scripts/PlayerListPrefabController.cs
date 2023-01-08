using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListPrefabController : NetworkBehaviour
{
    public TMPro.TMP_Text playerNameText;
    [SerializeField] Button kickBtn;
    public PlayerController player;

    void Start()
    {
        kickBtn.onClick.AddListener(delegate{Kick();});
        
    }


    async void Kick()
    {
        var lobby = GameObject.Find("LobbyManager").GetComponent<LobbyManager>().createdLobby;

        Debug.Log(player.OwnerClientId + ", count: " + lobby.Players.Count);
        NetworkManager.Singleton.DisconnectClient(player.OwnerClientId);
        Destroy(gameObject);



        try
        {
            await LobbyService.Instance.RemovePlayerAsync(lobby.Id, lobby.Players[(int)player.OwnerClientId].Id);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

}
