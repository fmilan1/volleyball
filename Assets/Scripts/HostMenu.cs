using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.Services.Lobbies;
using UnityEngine;
using UnityEngine.UI;

public class HostMenu : NetworkBehaviour
{
    [SerializeField] Button respawnBallBtn;
    [SerializeField] GameObject playerListPrefab;
    [SerializeField] Transform content;
    [SerializeField] LobbyManager lobbyManager;


    void Start()
    {
        respawnBallBtn.onClick.AddListener(RespawnBall);

    }

    async void Update()
    {
        if (!IsServer) return;



        if (Input.GetKeyDown("h"))
        {
            GameObject hostMenu = transform.GetChild(0).gameObject;
            foreach (Transform t in content.transform)
            {
                Destroy(t.gameObject);
            }
            hostMenu.SetActive(!hostMenu.activeSelf);

            lobbyManager.createdLobby = await LobbyService.Instance.GetLobbyAsync(lobbyManager.createdLobby.Id);
            var players = lobbyManager.createdLobby.Players;
            var tmp = NetworkManager.ConnectedClientsList;
            int i = -1;
            foreach (var player in players)
            {
                i++;
                GameObject g = Instantiate(playerListPrefab, content);
                var controller = g.GetComponent<PlayerListPrefabController>();
                controller.playerNameText.text = player.Data["playerName"].Value;
                controller.playerOwnerId = tmp[i].PlayerObject.OwnerClientId.ToString();
                controller.playerLobbyId = player.Id;
            }
        }
    }

    void RespawnBall()
    {
        GameObject ball = GameObject.Find("volleyball(Clone)");
        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ball.transform.position = new Vector3(0, 1, 0);
    }
}
