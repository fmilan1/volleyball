using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using Unity.Services.Authentication;
using UnityEngine;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;

public class RelayManager : MonoBehaviour
{

    string relayCode;

    public string RelayCode {get;}

    // async void Start()
    // {
    //     await UnityServices.InitializeAsync();

    //     AuthenticationService.Instance.SignedIn += () =>
    //     {
    //         Debug.Log("Signed in as " + AuthenticationService.Instance.PlayerId);
    //     };
        
    //     await AuthenticationService.Instance.SignInAnonymouslyAsync();
        
    // }


    async void CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();

            relayCode = joinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void JoinRelayAndLobby(string joinCode, string id)
    {
        try
        {
            // await LobbyService.Instance.JoinLobbyByIdAsync(id);
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
            Debug.Log("Joined with code " + joinCode);
            GameObject.Find("LobbyMenu").transform.GetChild(0).gameObject.SetActive(false);
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }    
}
