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
  [SerializeField]
  GameObject sussyListUI;
  [SerializeField]
  GameObject sussyListParent;
  [SerializeField]
  GameObject sussyEntryPrefab;
  [SerializeField]
  SusManager susMan;

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
    if (!prev)
    {
      killButton.SetActive(false);
      followButton.SetActive(false);
      sussyListUI.SetActive(false);
    }
    else
    {
      killButton.SetActive(true);
      followButton.SetActive(true);
      sussyListUI.SetActive(true);
    }

    if (Input.GetMouseButtonDown(0))
    {
      var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

      RaycastHit hit;


      //sim is layer 7
      if (Physics.Raycast(ray, out hit, 10000000000f, (1 << 7)))
      {
        if (hit.collider.CompareTag("Sim"))
        {
          Sim sim = hit.collider.GetComponent<Sim>();
          Select(sim);
        }
      }
    }
  }

  public void Select(Sim sim)
  {
    if (prev != sim)
    {
      if (prev)
      {
        prev.Deselect();
      }
      sim.Select();
      prev = sim;
      ShowSussyList(prev);
    }
  }
  void ShowSussyList(Sim sim)
  {
    var sussys = sim.GetSussyList();

    foreach (var sussyPair in sussys)
    {
      GameObject sussyEntry = Instantiate(sussyEntryPrefab, sussyListParent.transform);

      Text sussyText = sussyEntry.GetComponent<Text>();

      sussyText.text = sussyPair.Key.GetID().ToString();
      sussyText.color = susMan.GetCrimeColor(sussyPair.Value);

      SimRef simRef = sussyEntry.GetComponent<SimRef>();

      simRef.sim = sussyPair.Key;
      simRef.simSelector = this;
    }
  }
  public void Follow()
  {
    if (transform.parent != null)
    {
      transform.parent = null;
      followButton.GetComponentInChildren<Text>().text = "Follow";
      return;
    }
    Vector3 vec = transform.position - prev.transform.position;
    vec = vec.normalized * followDist;

    transform.position = prev.transform.position + vec;
    transform.parent = prev.transform;
    transform.LookAt(prev.transform);

    followButton.GetComponentInChildren<Text>().text = "Unfollow";
  }

  public void SelectAndFollow(Sim sim)
  {
    Select(sim);

    transform.parent = null;
    Follow();
  }
  public void Kill()
  {
    prev.Kill();
  }

}

