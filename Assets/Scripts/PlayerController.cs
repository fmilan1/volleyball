using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{

    [SerializeField] float sensitivity;

    [SerializeField] Animator anim;

    Camera cam;

    [SerializeField] GameObject shootPoint;

    GameObject ball;

    [SerializeField] Canvas overHeadCanvas;

    public TMPro.TMP_Text nameText;

    Spawner spawner;

    void Start()
    {
        
        cam = Camera.main;

        spawner = GameObject.Find("Spawner").GetComponent<Spawner>();
        
        if (IsHost && IsOwnedByServer)
        {
            spawner.SpawnBallServerRpc();
        }

        if (!IsOwner)
        {
            Destroy(overHeadCanvas.transform.GetChild(0).gameObject);
            Destroy(shootPoint);
        }
        else 
        {
            Destroy(overHeadCanvas.transform.GetChild(1).gameObject);
        }

        if (IsOwner)
        {
            ball = GameObject.Find("volleyball(Clone)");
            string playerName = PlayerPrefs.GetString("playerName");
            GetComponent<PlayerController>().nameText.text = playerName;
            gameObject.name = playerName;
            UpdateNameServerRpc(gameObject, playerName);
            GameObject.Find("LobbyMenu").transform.GetChild(0).gameObject.SetActive(false);
        }
        UpdateHostNameServerRpc();
    }



    [ServerRpc(RequireOwnership = false)]
    public void UpdateNameServerRpc(NetworkObjectReference networkObjectReference, string nameStr)
    {
        GameObject g = (GameObject)networkObjectReference;
        g.name = nameStr;
        g.GetComponent<PlayerController>().nameText.text = nameStr;
        UpdateNameClientRpc(networkObjectReference, nameStr);        
    }

    [ClientRpc]
    void UpdateNameClientRpc(NetworkObjectReference networkObjectReference, string nameStr)
    {
        GameObject g = (GameObject)networkObjectReference;
        g.name = nameStr;
        g.GetComponent<PlayerController>().nameText.text = nameStr;
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateHostNameServerRpc()
    {
        UpdateNameClientRpc(gameObject, gameObject.name);
    }

    public void LeaveGame()
    {
        GameObject.Find("LobbyMenu").transform.GetChild(0).gameObject.SetActive(true);
    }

    public override void OnDestroy()
    {
        if (!IsOwner) return;
        LeaveGame();
    }

    void Update()
    {
        
        if (!IsOwner) return;

        
        

        // ball.GetComponent<Rigidbody>().isKinematic = false;

        Vector3 WASD = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        WASD.Normalize();
        Vector3 move = WASD;


        transform.Translate(move * Time.deltaTime * 10, Space.World);
        if (WASD != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(WASD, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 1000 * Time.deltaTime);
        }

        int angle = 0;

        if (!Input.GetKey("space"))
        {

            if (Input.GetMouseButtonDown(1))
            {
                anim.SetTrigger("alkar");
            }
            else if (Input.GetMouseButtonUp(1))
            {
                angle = 55;
                anim.SetTrigger("alkar_ut");
            }
            else if (Input.GetMouseButtonDown(0))
            { 
                anim.SetTrigger("kosar");
            }
            else if (Input.GetMouseButtonUp(0))
            {
                angle = 65;
                anim.SetTrigger("kosar_ut");
            }
        }

        if (WASD.magnitude > 0) anim.SetBool("jaras", true);
        else if (WASD.magnitude == 0) anim.SetBool("jaras", false);


        Vector2 mouse = Input.mousePosition;
        Ray ray = cam.ScreenPointToRay(mouse);
        if (Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            shootPoint.transform.position = raycastHit.point + new Vector3(0, 0.001f, 0);
        }


        if ((Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)) && !Input.GetKey("space"))
        {
            Shoot(angle);
        }

        

    }

    void Shoot(int angle)
    {
        if (Vector3.Distance(ball.transform.position, transform.position) <= 2f)
        {
            ball.GetComponent<BallController>().ShootServerRpc(shootPoint.transform.position.x, shootPoint.transform.position.z, angle);
        }  
    }


    void LateUpdate()
    {
        overHeadCanvas.transform.LookAt(overHeadCanvas.transform.position + cam.transform.forward);
    }

    
     
}
