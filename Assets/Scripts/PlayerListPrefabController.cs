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
    public string playerOwnerId;

    public string playerLobbyId;

    void Start()
    {
        kickBtn.onClick.AddListener(Kick);
    }


    async void Kick()
    {
        var lobby = GameObject.Find("LobbyManager").GetComponent<LobbyManager>().createdLobby;
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(lobby.Id, playerLobbyId);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
        NetworkManager.Singleton.DisconnectClient((ulong)int.Parse(playerOwnerId));
        Destroy(gameObject);


    }

}
