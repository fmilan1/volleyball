using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BallController : NetworkBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] GameObject targetPrefab;
    [SerializeField] GameObject target;

    Vector3 targetPosition;


    [ServerRpc(RequireOwnership = false)]
    public void ShootServerRpc(float x, float z, int angle)
    {
        rb.velocity = Vector3.zero;
        rb.AddForce(calcBallisticVelocityVector(transform.position, new Vector3(x, 0, z), angle), ForceMode.VelocityChange);        
        targetPosition = new Vector3(x, 0.001f, z);
        
    }

    void Start()
    {
        if (!IsHost) return;
        target = Instantiate(targetPrefab);
        target.GetComponent<NetworkObject>().Spawn();
    }

    void Update()
    {
        UpdateTargetPositionServerRpc();
    }


    [ServerRpc(RequireOwnership = false)]
    void UpdateTargetPositionServerRpc()
    {
        target.transform.position = targetPosition;
        target.transform.localRotation = Quaternion.Euler(-90, 0, 0);
    }

    Vector3 calcBallisticVelocityVector(Vector3 source, Vector3 target, float angle)
    {
        Vector3 direction = target - source;                            
        float h = direction.y;                                           
        direction.y = 0;                                               
        float distance = direction.magnitude;                           
        float a = angle * Mathf.Deg2Rad;                                
        direction.y = distance * Mathf.Tan(a);                            
        distance += h/Mathf.Tan(a);                                      

        // calculate velocity
        float velocity = Mathf.Sqrt(distance * Physics.gravity.magnitude / Mathf.Sin(2*a));
        return velocity * direction.normalized;    
    }
}
