using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyByCameraController : MonoBehaviour
{

  public float moveSpeed = 18; // the movement speed with WASD
  public float maxCamHeight = 100;
  public float minCamHeight = 0;


  // Start is called before the first frame update
  void Start()
  {
  }

  // Update is called once per frame
  void Update()
  {
    float xAxis = Input.GetAxisRaw("Horizontal");
    float zAxis = Input.GetAxisRaw("Vertical");
    float yAxis = Input.GetAxisRaw("Fire1");

    Vector3 moveOffset = xAxis * transform.right + yAxis * transform.up + zAxis * transform.forward * moveSpeed * Time.unscaledDeltaTime;
    //Vector3 moveOffset = new Vector3(xAxis, yAxis, zAxis) * moveSpeed * Time.deltaTime;


    transform.position += moveOffset;
    transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, minCamHeight, maxCamHeight),transform.position.z);
  }
}
