using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sim : MonoBehaviour
{
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
				Vector3[] providers;
				//list of Sims I'm sus of
				SortedSet<Transform> sussys;

				// Start is called before the first frame update
				void Start()
				{
								sussys = new SortedSet<Transform>();
								providers = new Vector3[(int)Needs.needsAmount];
								needs = new float[(int)Needs.needsAmount];


								needs[(int)Needs.food] = Random.Range(lows[(int)Needs.food], maxs[(int)Needs.food]);
								needs[(int)Needs.fun] = Random.Range(lows[(int)Needs.fun], maxs[(int)Needs.fun]);
								needs[(int)Needs.pee] = Random.Range(lows[(int)Needs.pee], maxs[(int)Needs.pee]);
								needs[(int)Needs.shade] = Random.Range(lows[(int)Needs.shade], maxs[(int)Needs.shade]);

				}

				// Update is called once per frame
				void Update()
				{
								for(int i = 0; i < (int)Needs.needsAmount; ++i)
								{
												needs[i] -= Time.deltaTime * decays[i];
								}

				}

				public void ReceiveMessage(Message ms)
				{
								switch (ms.type)
								{
												case MessageType.ProvideNeed:
																{
																				ProvideNeedMessage pnms = (ProvideNeedMessage)ms;
																				int needsIndex = (int)pnms.type;

																				providers[needsIndex] = pnms.pos;
																				break;
																}

												case MessageType.SusInfo:
																{
																				//Sims = Sus Info Message? sus
																				SusInfoMessage sims = (SusInfoMessage)ms;
																				int needsIndex = (int)sims.type;

																				foreach(var sussy in sims.payload)
																				{
																								sussys.Add(sussy);
																				}
																				break;
																}

								}
				}
}
