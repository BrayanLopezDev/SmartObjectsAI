using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyByCameraController : MonoBehaviour
{

    public float moveSpeed = 18; // the movement speed with WASD
    public float maxCamHeight = 100;
    public float minCamHeight = 0;
    private Rigidbody myBody; 


    // Start is called before the first frame update
    void Start()
    {
        myBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float xAxis = Input.GetAxis("Horizontal");
        float zAxis = Input.GetAxis("Vertical");
        float yAxis = Input.GetAxis("Fire1");

        Vector3 moveOffset = xAxis* transform.right+yAxis*transform.up+zAxis*transform.forward * moveSpeed * Time.deltaTime;
        //Vector3 moveOffset = new Vector3(xAxis, yAxis, zAxis) * moveSpeed * Time.deltaTime;


        if (((transform.position + moveOffset).y < minCamHeight) || ((transform.position + moveOffset).y > maxCamHeight))
        {
            moveOffset.y = 0;
        }


        myBody.transform.position = transform.position + moveOffset;
        //myBody.MovePosition(transform.position + moveOffset);

      
    }
}
