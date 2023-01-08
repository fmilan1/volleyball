using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{

    [SerializeField] float sensitivity;

    [SerializeField] Animator anim;

    Camera cam;

    [SerializeField] GameObject shootPoint;

    GameObject ball;

    Spawner spawner;

    Canvas canvas;

    void Start()
    {
        cam = Camera.main;
        canvas = GetComponentInChildren<Canvas>();

        spawner = GameObject.Find("Spawner").GetComponent<Spawner>();

        if (IsHost && IsOwner)
        {
            ball = spawner.SpawnBallServerRpc();
        }        
        else if (IsClient && IsOwner)
        {
            ball = GameObject.Find("volleyball(Clone)");
        }

        if (!IsOwner)
        {
            Destroy(GetComponentInChildren<Canvas>().gameObject);
            Destroy(transform.GetChild(4).gameObject);
        }
    }

    public void LeaveGame()
    {
        GameObject.Find("LobbyMenu").transform.GetChild(0).gameObject.SetActive(true);
    }

    public override void OnDestroy()
    {
        if (!IsOwner) return;
        GameObject.Find("LobbyMenu").transform.GetChild(0).gameObject.SetActive(true);        
    }

    void Update()
    {
        
        if (!IsOwner) return;

        

        ball.GetComponent<Rigidbody>().isKinematic = false;

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
        // else if (Input.GetMouseButton(0))


        if (Input.GetMouseButtonDown(0) && Input.GetMouseButtonDown(1))
        {
            Debug.Log("same time");
        }

        

    }

    void Shoot(int angle)
    {
        if (Vector3.Distance(ball.transform.position, transform.position) <= 3.5f)
        {
            ball.GetComponent<BallController>().ShootServerRpc(shootPoint.transform.position.x, shootPoint.transform.position.z, angle);
        }  
    }


    void LateUpdate()
    {
        if (!IsOwner) return;
        canvas.transform.LookAt(transform.position + cam.transform.forward);
    }

    
     
}
