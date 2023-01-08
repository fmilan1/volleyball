using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LobbyPrefabController : MonoBehaviour
{
    [SerializeField] LobbyManager lobbyManager;
    [SerializeField] RelayManager relayManager;
    public TMPro.TMP_Text lobbyName;
    public TMPro.TMP_Text maxPlayers;

    public string relayCode;
    public string id;

    void Start()
    {
        lobbyManager = GameObject.Find("LobbyManager").GetComponent<LobbyManager>();
        relayManager = GameObject.Find("RelayManager").GetComponent<RelayManager>();
        GetComponent<Button>().onClick.AddListener(delegate
        {
            relayManager.JoinRelayAndLobby(relayCode, id);
        });
    }
}
