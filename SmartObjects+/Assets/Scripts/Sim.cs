using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Random = UnityEngine.Random;

public enum SimState
{
				Wandering,
				TravelingToProvider,
				InteractingWithProvider
}

public class Sim : MonoBehaviour
{
				//important info about the world
				[SerializeField]
				World world;
				//max a Sim can get of needs
				[SerializeField]
				float[] maxs;
				//when Sim is critically low on need, they will do anything to satisfy need
				[SerializeField]
				float[] crits;
				//when Sim will start to consider the need
				[SerializeField]
				float[] lows;
				//when Sim is caught being sus, at what point they will try to kill their witness
				[SerializeField]
				float[] enoughToKills;
				//needs, 1-1 with Needs enum
				[SerializeField]
				float[] needs;
				//rate of decay for needs (per second)
				[SerializeField]
				float[] decays;
				//most recently seen smart object (at their position because some can move) which fulfills need
				[SerializeField]
				Vector3[] providers;
				//list of Sims I'm sus of
				[SerializeField]
				SortedSet<Transform> sussys;
				//my navigation agent
				[SerializeField]
				NavMeshAgent agent;
				[SerializeField]
				SmartObject providingMe; //Smart Object thats providing a need for me right now
				//if Start() has been called yet, needed because receiving messages can happen before Start() gets called
				bool started = false;
				[SerializeField]
				SimState state = SimState.Wandering;
				//what Sim needs the most right now
				[SerializeField]
				Needs mostNeed;
				[SerializeField]
				bool isAlive = true;

				// Start is called before the first frame update
				void Start()
				{
								if (started)
								{
												return;
								}

								world = GameObject.FindObjectOfType<World>();

								agent = GetComponent<NavMeshAgent>();

								sussys = new SortedSet<Transform>();
								providers = new Vector3[(int)Needs.needsAmount];
								needs = new float[(int)Needs.needsAmount];

								needs[(int)Needs.food] = Random.Range(lows[(int)Needs.food], maxs[(int)Needs.food]);
								needs[(int)Needs.fun] = Random.Range(lows[(int)Needs.fun], maxs[(int)Needs.fun]);
								needs[(int)Needs.pee] = Random.Range(lows[(int)Needs.pee], maxs[(int)Needs.pee]);
								needs[(int)Needs.shade] = Random.Range(lows[(int)Needs.shade], maxs[(int)Needs.shade]);

								started = true;
				}

				// Update is called once per frame
				void Update()
				{
								if (!isAlive)
								{
												return;
								}

								for (int i = 0; i < (int)Needs.needsAmount; ++i)
								{
												needs[i] -= Time.deltaTime * decays[i];
												if (needs[i] <= 0f)
												{
																SmartObject.KillWhileNotBeingServiced(this);
																return;
												}
								}

								Simulate();
				}

				void Simulate()
				{
								//if agent doesnt have either a complete path nor an incomplete path
								if (!agent.hasPath && !agent.pathPending)
								{
												//if was traveling to needs provider, means reached destination, now time to interact
												if (state == SimState.TravelingToProvider)
												{
																SmartObject provider = null;
																Collider[] cols = Physics.OverlapSphere(transform.position, 10f);

																foreach (Collider col in cols)
																{
																				provider = col.GetComponent<SmartObject>();

																				if (provider)
																				{
																								ArriveAtProvider(provider);
																								break;
																				}
																}

																//what if there was no provider?
																if (!provider)
																{
																				AskForDirectionsFromNearbySims(cols);
																}
												}
												else
												{
																//if Sim is interacting with provider, aka getting serviced
																if(state == SimState.InteractingWithProvider)
																{
																				InteractingWithProvider();
																				return;
																}

																int biggestNeed = GetBiggestNeed();
																
																//if Sim has no needs right now, then wander about and explore
																if (biggestNeed == -1)
																{
																				state = SimState.Wandering;
																				agent.SetDestination(world.GetRandomSpotWithinTerrainBounds());
																				return;
																}

																mostNeed = (Needs)biggestNeed;

																if (providers[biggestNeed] != Vector3.zero)
																{
																				state = SimState.TravelingToProvider;
																				agent.SetDestination(providers[biggestNeed]);
																}
																else
																{
																				agent.SetDestination(world.GetRandomSpotWithinTerrainBounds());
																}
												}
								}
				}

				int GetBiggestNeed()
				{
								int biggestNeed = -1;
								float smallestNumber = float.MaxValue;
								for (int i = 0; i < (int)Needs.needsAmount; ++i)
								{
												if (needs[i] < smallestNumber && needs[i] <= lows[i])
												{
																biggestNeed = i;
												}
								}
								return biggestNeed;
				}
				void InteractingWithProvider()
				{
								if(needs[(int)mostNeed] >= maxs[(int)mostNeed])
								{
												if(providingMe)
												{
																providingMe.RequestRemovalOfService(this);
																providingMe = null;
																state = SimState.Wandering;
												}
								}
				}
				void ArriveAtProvider(SmartObject provider)
				{
								SmartObjectServiceResponse response = provider.RequestService(this);
								//if the smart object said no
								if (!response.response)
								{
												//if we're desparate because we REALLY need this				
												if (needs[(int)response.service] <= crits[(int)response.service])
												{
																int min = 0;
																int max = response.servicing.Length;

																int toKill = Random.Range(min, max);

																//kill them, the SmartObject knows how to do this because they're smart
																provider.Kill(response.servicing[toKill]);

																//request again, should grant me a spot right?
																response = provider.RequestService(this);
																//checking, just to make sure theres actually a spot for me
																if (response.response)
																{
																				//miraculously, theres a spot for me, totally not sus at all :)
																				ProviderAcceptedMe(provider, response);
																}
												}
								}
								else
								{
												ProviderAcceptedMe(provider, response);
								}

				}

				void ProviderAcceptedMe(SmartObject provider, SmartObjectServiceResponse response)
				{
								state = SimState.InteractingWithProvider;
								//update this just to make 100% sure it matches with the provider
								mostNeed = response.service;
								//update my provider
								providingMe = provider;
				}

				void AskForDirectionsFromNearbySims(Collider[] cols)
				{
								//my now wrong knowledge of provider location
								Vector3 outdated = providers[(int)mostNeed];
								//my knowledge of nearest provider is now invalid
								providers[(int)mostNeed] = Vector3.zero;
								//ask everyone nearby for directions to nearest provider of what I need the most
								foreach (Collider col in cols)
								{
												Sim other = col.GetComponent<Sim>();
												//if there is someone there
												if (other)
												{
																//ask them for directions
																Vector3 directions = other.RequestDirectionsToProvider(mostNeed);
																//if no one has given me directions yet
																if (providers[(int)mostNeed] == Vector3.zero)
																{
																				//accept their directions as real and true...
																				//runs risk of multiple sims giving each other the same false information after arriving to same wrong spot at same time
																				providers[(int)mostNeed] = directions;
																}
																else
																{
																				Vector3 prevDirections = providers[(int)mostNeed];
																				Vector3 prevDirectionsVec = new Vector3(prevDirections.x - transform.position.x, prevDirections.y - transform.position.y, prevDirections.z - transform.position.z);
																				Vector3 newDirectionsVec = new Vector3(directions.x - transform.position.x, directions.y - transform.position.y, directions.z - transform.position.z);
																				Vector3 newToOutdatedDiff = new Vector3(directions.x - outdated.x, directions.y - outdated.y, directions.z - outdated.z);
																				//if new directions are closer to me than previous ones, and its far enough away that I dont consider it to be the same as my outdated knowledge of it
																				if (newDirectionsVec.sqrMagnitude < prevDirectionsVec.sqrMagnitude && newToOutdatedDiff.sqrMagnitude > 25f)
																				{
																								//update location of my knowledge of provider to this closer one
																								providers[(int)mostNeed] = directions;
																				}
																}
												}
								}
								//if I didn't get what I thought were valid directions
								if (providers[(int)mostNeed] == Vector3.zero)
								{
												//go somewhere random
												providers[(int)mostNeed] = world.GetRandomSpotWithinTerrainBounds();
												agent.SetDestination(providers[(int)mostNeed]);
								}
				}
				public Vector3 RequestDirectionsToProvider(Needs service)
				{
								return providers[(int)service];
				}
				public void ReceiveMessage(Message ms)
				{
								if (!started)
								{
												Start();
								}
								switch (ms.type)
								{
												case MessageType.ProvideNeed:
																{
																				ProvideNeedMessage pnms = (ProvideNeedMessage)ms;
																				int needsIndex = (int)pnms.payload;

																				providers[needsIndex] = pnms.pos;
																				break;
																}

												case MessageType.SusInfo:
																{
																				//Sims = Sus Info Message? sus
																				SusInfoMessage sims = (SusInfoMessage)ms;

																				foreach (var sussy in sims.payload)
																				{
																								sussys.Add(sussy);
																				}
																				break;
																}

								}
				}

				public void Die()
				{
								isAlive = false;
				}

				public void ServiceNeed(Needs need, float amount)
				{
								needs[(int)need] += amount;
				}
}
