using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Services.Lobbies.Models;

public class LobbyPrefabController : MonoBehaviour
{
    [SerializeField] LobbyManager lobbyManager;
    [SerializeField] RelayManager relayManager;
    public TMPro.TMP_Text lobbyName;
    public TMPro.TMP_Text maxPlayers;
    public Lobby lobby;

    void Start()
    {
        lobbyManager = GameObject.Find("LobbyManager").GetComponent<LobbyManager>();
        relayManager = GameObject.Find("RelayManager").GetComponent<RelayManager>();
        GetComponent<Button>().onClick.AddListener(delegate
        {
            lobbyManager.JoinLobby(lobby);
        });
    }
}
