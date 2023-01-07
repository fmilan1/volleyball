using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkUI : MonoBehaviour
{

    [SerializeField] private Button ServerBTN;
    [SerializeField] private Button HostBTN;
    [SerializeField] private Button ClientBTN;
    




    void Awake()
    {
        ServerBTN.onClick.AddListener(() => 
        {
            NetworkManager.Singleton.StartServer();
        });

        HostBTN.onClick.AddListener(() => 
        {
            NetworkManager.Singleton.StartHost();
        });

        ClientBTN.onClick.AddListener(() => 
        {
            NetworkManager.Singleton.StartClient();
        });
    }
}
