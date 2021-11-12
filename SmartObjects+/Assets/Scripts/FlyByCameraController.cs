using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyByCameraController : MonoBehaviour
{

    public float moveSpeed = 18; // the movement speed with WASD
    private Rigidbody myBody; 


    // Start is called before the first frame update
    void Start()
    {
        myBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float xAxis = Input.GetAxis("Horizontal");
        float zAxis = Input.GetAxis("Vertical");
        float yAxis = Input.GetAxis("Fire1");

        Vector3 moveOffset = new Vector3(xAxis, yAxis, zAxis) * moveSpeed * Time.deltaTime;

        myBody.MovePosition(transform.position + moveOffset);

    }
}
