using System.Collections;
using System.Collections.Generic;
using System.IO;
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

  void Start()
  {
    if(started)
    {
      return;
    }

    if(GameMode.mode == GameModes.DataCollection)
    {
      using (TextReader tr = File.OpenText("./__SpawnAmount.txt"))
      {
        spawnAmount = int.Parse(tr.ReadLine());
      }
      WorldSpawner.SetSimSpawnAmount(spawnAmount);
    }
    else
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
    }

    spawnAmountText.text = spawnAmount.ToString();

    GameObject.FindObjectOfType<SusManager>().SetSimStartAmount(spawnAmount);
    GameObject.FindObjectOfType<WorldSpawner>().WorldStart();

    started = true;
  }

  public void StartNow()
  {
    Start();
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
