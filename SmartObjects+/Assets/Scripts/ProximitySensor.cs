using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximitySensor : MonoBehaviour
{
  [SerializeField]
  List<Sim> nearMe;
  List<VoidDelegate> OnProximityCalls; //functions to call when a sim is near me
  List<VoidDelegate> OnNoProximityCalls; //functions to call when there are no sims near me
                                         //make sure lists get initialized before stuff gets added to them
  bool started = false;
  // Start is called before the first frame update
  void Start()
  {
    if (started)
    {
      return;
    }
    nearMe = new List<Sim>();
    OnProximityCalls = new List<VoidDelegate>();
    OnNoProximityCalls = new List<VoidDelegate>();

    StartCoroutine(ReevaluateNearMe());

    started = true;
  }

  void EnsureStarted()
  {
    Start();
  }

  IEnumerator ReevaluateNearMe()    //fix chicken not moving when someone nearby dies
  {
    while (true)
    {
      yield return new WaitForSeconds(5f);

      for (int i = 0; i < nearMe.Count; ++i)
      {
        if (!nearMe[i].IsAlive())
        {
          nearMe.RemoveAt(i);
          --i;
        }
      }
    }
  }

  void OnTriggerEnter(Collider other)
  {
    EnsureStarted();
    if (other.CompareTag("Sim"))
    {
      Sim sim = other.GetComponent<Sim>();

      if (sim.IsAlive())
      {
        if (nearMe.Count == 0)
        {
          OnProximity();
        }
        nearMe.Add(sim);
      }
    }
  }

  void OnTriggerExit(Collider other)
  {
    EnsureStarted();
    if (other.CompareTag("Sim"))
    {
      Sim sim = other.GetComponent<Sim>();

      if (sim.IsAlive())
      {
        nearMe.Remove(sim);
        if (nearMe.Count == 0)
        {
          OnNoProximity();
        }
      }
    }
  }


  void OnProximity()
  {
    foreach (VoidDelegate fn in OnProximityCalls)
    {
      fn();
    }
  }

  void OnNoProximity()
  {
    foreach (VoidDelegate fn in OnNoProximityCalls)
    {
      fn();
    }
  }

  public void RegisterOnProximityCall(VoidDelegate fn)
  {
    EnsureStarted();
    if (fn != null)
    {
      OnProximityCalls.Add(fn);
    }
  }

  public void RegisterOnNoProximityCall(VoidDelegate fn)
  {
    EnsureStarted();
    if (fn != null)
    {
      OnNoProximityCalls.Add(fn);
    }
  }

}
