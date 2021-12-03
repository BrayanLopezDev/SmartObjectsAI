using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectSim : MonoBehaviour
{
  [SerializeField]
  GameObject followButton;
  [SerializeField]
  GameObject killButton;

  Sim prev;

  [SerializeField]
  float followDist;
  
  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    if(!prev)
    {
      killButton.SetActive(false);
      followButton.SetActive(false);
    }
    else
    {
      killButton.SetActive(true);
      followButton.SetActive(true);
    }

    if (Input.GetMouseButtonDown(0))
    {
      var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

      RaycastHit hit;

      
      //sim is layer 7
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

  public void Follow()
{
    if(transform.parent != null)
    {
      transform.parent = null;
      followButton.GetComponentInChildren<Text>().text = "Follow";
      return;
    }
    Vector3 vec = transform.position - prev.transform.position;
    vec = vec.normalized * followDist;

    transform.position = prev.transform.position + vec;
    transform.parent = prev.transform;

    followButton.GetComponentInChildren<Text>().text = "Unfollow";
}

public void Kill()
{
    prev.Kill();
}

}

