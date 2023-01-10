using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    [SerializeField] GameObject ballPrefab;


    [ServerRpc]
    public void SpawnBallServerRpc()
    {
        GameObject g = Instantiate(ballPrefab, new Vector3(0, 1, 0), Quaternion.identity);
        g.GetComponent<NetworkObject>().Spawn();
    }
}
