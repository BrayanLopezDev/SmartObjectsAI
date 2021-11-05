using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//global static world class that contains information about world
public class World : MonoBehaviour
{
    [SerializeField]
    Terrain terrain;
    public float size;

				private void Awake()
				{
								terrain = GameObject.FindObjectOfType<Terrain>().GetComponent<Terrain>();
								size = terrain.terrainData.size.x;
				}

				// Start is called before the first frame update
				void Start()
    {
				}

    // Update is called once per frame
    void Update()
    {
    }
}
