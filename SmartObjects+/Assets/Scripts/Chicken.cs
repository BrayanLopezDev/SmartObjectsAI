using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
//using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.AI;

public class Chicken : MonoBehaviour
{
				VoidDelegate ForceShutdownSmartObject;
				[SerializeField]
				float maxFood;
				[SerializeField]
				float food;
				[SerializeField]
				float maxScale;
				[SerializeField]
				float minScale;
				[SerializeField]
				float foodDecayPerProvidePerSecond;
				[SerializeField]
				SmartObject smartie;
				[SerializeField]
				NavMeshAgent agent;
				[SerializeField]
				ProximitySensor proxSensor;
				[SerializeField]
				World world;


				// Start is called before the first frame update
				void Start()
				{
								smartie = GetComponent<SmartObject>();

								ForceShutdownSmartObject = smartie.GetShutdown();
								smartie.RegisterWhileIdle(Wander);
								smartie.RegisterWhileProvide(Provide);

								food = maxFood;

								proxSensor = GetComponentInChildren<ProximitySensor>();
								proxSensor.RegisterOnProximityCall(OnProximity);
								proxSensor.RegisterOnNoProximityCall(OnNoProximity);

								world = GameObject.FindObjectOfType<World>();

								agent = GetComponent<NavMeshAgent>();

								transform.localScale = new Vector3(maxScale,maxScale,maxScale);
				}

				// Update is called once per frame
				void Update()
				{

				}

				void Wander()
				{
								if(ReachedDestination())
								{
												agent.SetDestination(world.GetRandomSpotWithinTerrainBounds());
								}
				}

				void Provide()
				{
								food -= foodDecayPerProvidePerSecond * Time.deltaTime;
								transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(minScale,minScale,minScale), foodDecayPerProvidePerSecond * .25f * Time.deltaTime);								

								if(food <= 0f)
								{
												OnDeath();
								}
				}

				void OnDeath()
				{
								//make sure SmartObject cant provide for anyone anymore
								ForceShutdownSmartObject();
								//turn large trigger sphere into small collider sphere
								SphereCollider col = GetComponent<SphereCollider>();
								col.isTrigger = false;
								col.radius = 1f;
								col.center = new Vector3(0f, 1f, 0f);
								//disable the smartobject
								smartie.enabled = false;
								//roll around because its funny
								Rigidbody rb = GetComponent<Rigidbody>();
								rb.isKinematic = false;
								rb.useGravity = true;
								rb.AddForce(new Vector3(0f, 1000f, 0f));
								//disable navmeshagent
								agent.enabled = false;
								//disable this script
								this.enabled = false;
				}

				void OnProximity()
				{
								agent.isStopped = true;
				}

				void OnNoProximity()
				{
								agent.isStopped = false;
				}

				bool ReachedDestination()
				{
								bool closeEnough = CloseEnough(agent.destination);
								if (closeEnough)
								{
												agent.destination = transform.position;
								}
								//if agent doesnt have either a complete path nor an incomplete path, or its close enough to have reached destination
								return !agent.hasPath && !agent.pathPending || closeEnough;
				}
				bool CloseEnough(Vector3 destination)
				{
								return (destination - transform.position).sqrMagnitude <= 4f;
				}

}
