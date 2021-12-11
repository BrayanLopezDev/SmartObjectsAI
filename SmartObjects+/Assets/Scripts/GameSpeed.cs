using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSpeed : MonoBehaviour
{
  Slider gameSpeedSlider;
  // Start is called before the first frame update
  void Start()
  {
    gameSpeedSlider = GetComponent<Slider>();

    if (GameMode.mode == GameModes.DataCollection)
    {
      SpeedChanged(10f);
      gameSpeedSlider.SetValueWithoutNotify(10f);
    }
  }

  // Update is called once per frame
  void Update()
  {

  }

  public void SpeedChanged(float speed)
  {
    Time.timeScale = speed;
  }
}
