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

				public Vector3 GetRandomSpotOnTerrain()
				{
								while(true)
								{
												float raycastStartY = 5f*terrain.terrainData.size.y + transform.position.y;

												Vector3 rayStart = new Vector3(Random.Range(0.5f * -size, 0.5f * size), raycastStartY, Random.Range(0.5f * -size, 0.5f * size));
												Vector3 rayEnd = new Vector3(rayStart.x, -2f * rayStart.y, rayStart.z);

												RaycastHit hit;
												if (Physics.Raycast(new Ray(rayStart, rayEnd), out hit, 2f * raycastStartY) && hit.collider.gameObject == gameObject)
												{
																return hit.point;
												}
								}
				}

				public Vector3 GetRandomSpotWithinTerrainBounds()
				{
								while (true)
								{
												float raycastStartY = terrain.terrainData.size.y + transform.position.y;

												Vector3 rayStart = new Vector3(Random.Range(0.5f * -size, 0.5f * size), raycastStartY, Random.Range(0.5f * -size, 0.5f * size));
												Vector3 rayEnd = new Vector3(rayStart.x, -2f * rayStart.y, rayStart.z);

												RaycastHit hit;
												if (Physics.Raycast(new Ray(rayStart, rayEnd), out hit, 2f * raycastStartY) && !hit.collider.CompareTag("Unwalkable"))
												{
																return hit.point;
												}
								}
				}
}
