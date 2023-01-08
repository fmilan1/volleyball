using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
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

    void Update()
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

            var players = NetworkManager.Singleton.ConnectedClients;
            foreach (var player in players)
            {
                GameObject g = Instantiate(playerListPrefab, content);
                g.GetComponent<PlayerListPrefabController>().playerNameText.text = player.Value.PlayerObject.name;
                g.GetComponent<PlayerListPrefabController>().player = player.Value.PlayerObject.GetComponent<PlayerController>();
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
