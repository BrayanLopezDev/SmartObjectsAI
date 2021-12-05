using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class WorldSpawner : MonoBehaviour
{
  [SerializeField]
  GameObject[] smartObjects; //prefabs of smart objects
  [SerializeField]
  GameObject simprefab; //sim prefab
  [SerializeField]
  float[] maxSpawnsPerSim; //max amount of smart objects to spawn per sim

  static int simSpawnAmount; //how many sims to spawn, dictated by RestartSimulation()

  [SerializeField]
  float raycastStartY; // height at which to begin raycast
  [SerializeField]
  World world;

  // Start is called before the first frame update
  public void WorldStart()
  {

    world = GameObject.FindObjectOfType<World>().GetComponent<World>();
    int[] maxSpawns = new int[maxSpawnsPerSim.Length];

    List<Vector3> treePoss = new List<Vector3>();

    //spawn smart objects, can cause infinite loop because if raycast hits a trigger from a smart object, it wont count as it hitting the terrain
    for (int i = 0; i < smartObjects.Length; ++i)
    {
      maxSpawns[i] = Mathf.Max((int)(maxSpawnsPerSim[i] * simSpawnAmount), 1);
      for (int j = 0; j < maxSpawns[i]; ++j)
      {
        Vector3 spawnPoint = world.GetRandomSpotOnTerrain();
        GameObject smartie = Instantiate(smartObjects[i], spawnPoint, Quaternion.identity, world.transform);
        smartie.transform.Rotate(new Vector3(0f, Random.Range(0f, 360f), 0f));
        if (i == 0) //assumes tree is first in list
        {
          treePoss.Add(smartie.transform.position);
        }
      }
    }

    //spawn sims

    int simsPerTree = simSpawnAmount / treePoss.Count;

    for (int i = 0; i < simsPerTree; ++i)
    {
      for (int j = 0; j < treePoss.Count; ++j)
      {
        Instantiate(simprefab, treePoss[j], Quaternion.identity);
      }
    }

  }

  public static void SetSimSpawnAmount(int amt)
  {
    simSpawnAmount = amt;
  }

  public static int GetSimSpawnAmount()
  {
    return simSpawnAmount;
  }

}
