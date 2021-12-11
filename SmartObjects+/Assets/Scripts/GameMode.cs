using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameModes
{
  Default,
  DataCollection
}

public class GameMode : MonoBehaviour
{
  [SerializeField]
  GameModes gameMode;
  public static GameModes mode;


  void Awake()
  {
    mode = gameMode;
  }
}
