using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimRef : MonoBehaviour
{
  public Sim sim;
  public SelectSim simSelector;
  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }

  public void Select()
  {
    simSelector.SelectAndFollow(sim);
  }
}
