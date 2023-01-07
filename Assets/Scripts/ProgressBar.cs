using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{

    public float value = 0;

    int c = 1;
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.position += new Vector3(350 * c * Time.deltaTime, 0, 0);
        }
        else if (Input.GetKey(KeyCode.LeftControl))
            transform.position -= new Vector3(350 * Time.deltaTime, 0, 0);
        
        transform.localPosition = new Vector3(Mathf.Clamp(transform.localPosition.x, -1.25f, 1.25f), 0, 0);

        
        if (transform.localPosition.x <= -0.5f) value = 0;
        else if (transform.localPosition.x > -0.5f && transform.localPosition.x < 0.5f) value = 1;
        else value = 2;

    }
}
