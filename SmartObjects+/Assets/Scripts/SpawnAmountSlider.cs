using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnAmountSlider : MonoBehaviour
{
  [SerializeField]
  RestartSimulation restarter;
  [SerializeField]
  Slider slider;

  void Start()
  {
    slider.value = restarter.GetSpawnAmount();
  }
  public void SetSpawnAmount()
  {
    restarter.SetSpawnAmount((int)slider.value);
  }
}
