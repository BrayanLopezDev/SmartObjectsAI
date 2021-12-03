using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectSim : MonoBehaviour
{
  Sim prev;
  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetMouseButtonDown(0))
    {

      var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

      RaycastHit hit;

      

      if (Physics.Raycast(ray, out hit,10000000000f, (1 << 7)))
      {
        if (hit.collider.CompareTag("Sim"))
        {
          Sim sim = hit.collider.GetComponent<Sim>();
          if (prev != sim)
          {
            if (prev)
            {
              prev.Deselect();
            }
            sim.Select();
            prev = sim;
          }
        }
      }
    }
  }

}

