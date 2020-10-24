using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class control1 : MonoBehaviour
{
    public Rigidbody RG;
    public float speed = 500;
    public float horizontalF;
    public float verticalF;
    public float jump;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            RG.AddForce(new Vector3(500, 0, 0));
            transform.eulerAngles = Vector3.zero;
            Debug.LogError("coucou");
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            RG.AddForce(new Vector3(-500, 0, 0));
            transform.eulerAngles = new Vector3(0,180,0);
            Debug.LogError("coucou");
        }*/

        horizontalF = Input.GetAxis("Horizontal");
        verticalF = Input.GetAxis("Vertical");
        jump = Input.GetAxis("Jump");
        RG.AddForce(new Vector3(horizontalF*speed, jump*speed, verticalF*speed));

    }
}
