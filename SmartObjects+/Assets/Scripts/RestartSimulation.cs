using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RestartSimulation : MonoBehaviour
{
  [SerializeField]
  Text spawnAmountText;
  [SerializeField]
  int spawnAmount;

  bool started = false;

  // Start is called before the first frame update
  void Start()
  {
    int oldSpawnAmt = WorldSpawner.GetSimSpawnAmount();
    if(oldSpawnAmt == 0)
    {
      WorldSpawner.SetSimSpawnAmount(spawnAmount);
    }
    else
    {
      spawnAmount = oldSpawnAmt;
    }
    spawnAmountText.text = spawnAmount.ToString();

    GameObject.FindObjectOfType<WorldSpawner>().WorldStart();

    started = true;
  }

  public void SetSpawnAmount(int amt)
  {
    spawnAmount = amt;
    OnSpawnAmountChanged();
  }

  void OnSpawnAmountChanged()
  {
    WorldSpawner.SetSimSpawnAmount(spawnAmount);
    spawnAmountText.text = spawnAmount.ToString();
  }

  public void Restart()
  {
    OnSpawnAmountChanged();
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
  }

  void EnsureStarted()
  {
    if(!started)
    {
      Start();
    }
  }
  public int GetSpawnAmount()
  {
    EnsureStarted();
    return spawnAmount;
  }
}
