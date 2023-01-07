using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject ball;

    [ServerRpc]
    public GameObject SpawnBallServerRpc()
    {
        GameObject g = Instantiate(ball, Vector3.zero, Quaternion.identity);
        g.GetComponent<NetworkObject>().Spawn();
        return g;
    }
}
