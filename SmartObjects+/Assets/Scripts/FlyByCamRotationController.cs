using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyByCamRotationController : MonoBehaviour
{
    private Rigidbody myBody;
    public float cameraSens = 1;

    // Start is called before the first frame update
    void Start()
    {
        myBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float rotXAxis = Input.GetAxis("Mouse X");
        float rotYAxis = Input.GetAxis("Mouse Y");
        if (Input.GetKey(KeyCode.Mouse1))
        {
            Screen.lockCursor = true; // had to use this method instead of new method because new method is bugged
            transform.RotateAround(myBody.position, Vector3.up, rotXAxis * cameraSens );
            transform.RotateAround(myBody.position, -transform.right, rotYAxis * cameraSens);
        }
        else
        {
            if (Screen.lockCursor == true)
            {
                Screen.lockCursor = false;
            }
        }
    }
}
