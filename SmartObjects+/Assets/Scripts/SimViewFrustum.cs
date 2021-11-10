using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimViewFrustum : MonoBehaviour
{
				//sim parent I'm attached to
				[SerializeField]
				Sim sim;
				//sims in my vision
				[SerializeField]
				SortedSet<Sim> inMyVision;
				//sussy smart object in my vision, assumes Sim will only see one at a time
				[SerializeField]
				SmartObject sussySmartie;
				//crimes I saw them commit
				[SerializeField]
				Crimes crime;
				//OnTriggerEnter() can be called before Start(), this is to make sure everything is initialized beforehand
				bool started = false;
				// Start is called before the first frame update
				void Start()
				{
								if(started)
								{
												return;
								}
				
								sim = GetComponentInParent<Sim>();
								inMyVision = new SortedSet<Sim>();

								crime = Crimes.undetermined;

								started = true;
				}

				void EnsureStarted()
				{
								Start();
				}

				void OnTriggerEnter(Collider other)
				{
								EnsureStarted();
								if (other.CompareTag("Sim") )
								{
												if(other.gameObject == transform.parent.gameObject)
												{
																return; //I wish I didnt have to do this but they become sus of themselves otherwise
												}
												Sim otherSim = other.GetComponent<Sim>();
												//if saw someone dead
												if (!otherSim.IsAlive())
												{
																//tell parent at end of frame
																crime = Crimes.kill;
												}
												else
												{
																//otherwise, keep track of who is in my vision
																inMyVision.Add(other.GetComponent<Sim>());
												}
								}
								if(other.gameObject.layer == 3) //SimView layer
								{
												SmartObject smartie = other.GetComponentInParent<SmartObject>();
												if(smartie.IsSus())
												{
																sussySmartie = smartie;
																crime = SusManager.NeedToCrime(smartie.GetProvides());
												}
								}
				}

				void OnTriggerExit(Collider other)
				{
								if (other.CompareTag("Sim"))
								{
												inMyVision.Remove(other.GetComponent<Sim>());
								}
								if(sussySmartie && other.gameObject == sussySmartie.gameObject)
								{
												//if(crime == SusManager.NeedToCrime(sussySmartie.GetProvides()))
												//{
												//				crime = Crimes.undetermined;
												//}
												crime = Crimes.undetermined;
												sussySmartie = null;
								}
				}

				void Update()
				{
								if (crime != Crimes.undetermined)
								{
												if(inMyVision.Count == 0)
												{
																return;
												}

												//int inmyvisionCount = inMyVision.Count;
												//for(int i = 0; i < inmyvisionCount; ++i)
												//{
												//				crimes.Add(Crimes.kill);
												//}

												sim.ReceiveMessage(new SusInfoMessage { type = MessageType.SusInfo, payload = inMyVision,crime=crime });
												inMyVision.Clear();
												//crime = Crimes.undetermined;
												//sussySmartie = null;
								}
				}
}
