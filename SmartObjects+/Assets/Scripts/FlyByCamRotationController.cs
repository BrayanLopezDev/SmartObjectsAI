using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyByCamRotationController : MonoBehaviour
{
    public float cameraSens = 1;
    public bool LockCursorOnRotate = true;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float rotXAxis = Input.GetAxis("Mouse X");
        float rotYAxis = Input.GetAxis("Mouse Y");
        if (Input.GetKey(KeyCode.Mouse1))
        {
            if (LockCursorOnRotate)
            {
                Screen.lockCursor = true; // had to use this method instead of new method because new method is bugged
            }    
            transform.RotateAround(transform.position, Vector3.up, rotXAxis * cameraSens * Time.unscaledDeltaTime );
            transform.RotateAround(transform.position, -transform.right, rotYAxis * cameraSens * Time.unscaledDeltaTime);
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
