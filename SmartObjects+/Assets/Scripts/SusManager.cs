using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public enum Crimes //very similar to needs
{
				food,
				fun,
				pee,
				kill,
				undetermined, //Sim cant determine what crime it was, so it will defer to SusManager to figure it out
				crimesAmount
};

public static class GameObjectExtension
{
				public static T GetCopyOf<T>(this Component comp, T other) where T : Component
				{
								System.Type type = comp.GetType();
								if (type != other.GetType()) return null; // type mis-match
								BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
								PropertyInfo[] pinfos = type.GetProperties(flags);
								foreach (var pinfo in pinfos)
								{
												if (pinfo.CanWrite)
												{
																try
																{
																				pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
																}
																catch { }
												}
								}
								FieldInfo[] finfos = type.GetFields(flags);
								foreach (var finfo in finfos)
								{
												finfo.SetValue(comp, finfo.GetValue(other));
								}
								return comp as T;
				}

				public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component
				{
								return go.AddComponent<T>().GetCopyOf(toAdd) as T;
				}

}
public static class Utils
{

				public static T CopyComponent<T>(T original, T destination) where T : Component
				{
								System.Type type = original.GetType();
								//Component copy = destination.AddComponent(type);
								System.Reflection.FieldInfo[] fields = type.GetFields();
								foreach (System.Reflection.FieldInfo field in fields)
								{
												field.SetValue(destination, field.GetValue(original));
								}
								return destination;
				}
}

public class SusManager : MonoBehaviour
{
				//which sims are sus
				List<Sim> sussys;
				//how many other sims are sus of that sim
				List<int> refCounts;
				//what crimes have they committed? will be most recent crime
				List<Crimes> crimes;
				//different material of sus for each crime
				[SerializeField]
				Material[] sussyMats;

				//dead amongus model
				[SerializeField]
				GameObject deadAmongusPrefab;
				//set by script
				MeshRenderer deadAmongusMR;
				[SerializeField]
				MeshFilter deadAmongusMF;
				[SerializeField]
				Mesh deadAmongusMesh;
				//differences between ghost and amongus
				[SerializeField]
				GameObject ghostPrefab;
				[SerializeField]
				GameObject sussyPrefab;
				//set by script
				[SerializeField]
				Animator sussyAC;
				[SerializeField]
				Animator ghostAC;
				[SerializeField]
				SkinnedMeshRenderer sussySKM;
				[SerializeField]
				SkinnedMeshRenderer ghostSKM;

				// Start is called before the first frame update
				void Start()
				{
								sussyAC = sussyPrefab.GetComponent<Animator>();
								sussySKM = sussyPrefab.GetComponentInChildren<SkinnedMeshRenderer>();

								ghostAC = ghostPrefab.GetComponent<Animator>();
								ghostSKM = ghostPrefab.GetComponentInChildren<SkinnedMeshRenderer>();

								deadAmongusMR = deadAmongusPrefab.GetComponentInChildren<MeshRenderer>();
								deadAmongusMF = deadAmongusPrefab.GetComponentInChildren<MeshFilter>();

								sussys = new List<Sim>();
								refCounts = new List<int>();
								crimes = new List<Crimes>();
				}

				// Update is called once per frame
				void Update()
				{

				}
				//converts need to a crime
				public static Crimes NeedToCrime(Needs need)
				{
								return (Crimes)need;
				}

				//called when a Sim who is already sus of someone sees them doing some other sus thing
				public void OnResusUpdateCrime(Crimes crime, List<Sim> sus, KnowledgeType type)
				{
								if(type==KnowledgeType.SecondHand)
								{
												return;
								}
								int susCount = sus.Count;
								for(int i = 0; i < susCount; ++i)
								{
												crimes[i] = crime; //crime should never be undetermined
												ConvertToAmongus(sus[i], crime); //recolors amongus
								}
				}

				//called when a Sim notices someone being sus
				public void OnSus(Crimes crime, List<Sim> sus, KnowledgeType type)
				{
								int susCount = sus.Count;

								for (int i = 0; i < susCount; ++i)
								{
												bool alreadySus = false;
												for (int j = 0; j < sussys.Count; ++j)
												{
																if (sus[i] == sussys[j])
																{
																				//if (crime != Crimes.undetermined)
																				if(type != KnowledgeType.SecondHand)
																				{
																								crimes[j] = crime;
																								ConvertToAmongus(sus[i], crime); //recolors amongus
																				}
																				refCounts[j]++;
																				alreadySus = true;
																				break;
																}
												}
												if (!alreadySus)
												{
																sussys.Add(sus[i]);
																refCounts.Add(1);
																crimes.Add(crime); //this crime should never be undetermined
																ConvertToAmongus(sus[i], crime);
												}
								}
				}

				//called when a Sim dies
				public void OnDeath(Sim me, List<Sim> sus)
				{
								SortedSet<int> toUnsus = new SortedSet<int>(); //sims to unsus
								foreach (Sim sussy in sus)
								{
												for (int j = 0; j < sussys.Count; ++j)
												{
																if (sussy == sussys[j])
																{
																				refCounts[j]--;
																				if (refCounts[j] == 0)
																				{
																								toUnsus.Add(j);
																				}
																				break;
																}
												}
								}

								//if I was sussy
								if (sussys.Contains(me))
								{
												ConvertToDeadAmongus(me);
								}

								//important to reverse, otherwise wrong sims would get unsusified
								var simsToUnsus = toUnsus.Reverse();

								foreach (int index in simsToUnsus)
								{
												ConvertToGhost(sussys[index]);
												sussys.RemoveAt(index);
												crimes.RemoveAt(index);
												refCounts.RemoveAt(index);
								}
				}

				void ConvertToAmongus(Sim sim, Crimes crime)
				{
								if (!sim.IsAlive())
								{
												return;
								}

								//DestroyImmediate(sim.GetComponent<Animator>());
								//sim.gameObject.AddComponent<Animator>(sussyAC);
								//sim.GetComponent<Animator>().GetCopyOf<Animator>(sussyAC);
								//Animator ac = sim.GetComponent<Animator>();
								//Utils.CopyComponent<Animator>(sussyAC, ac);
								//ac.runtimeAnimatorController = sussyAC.runtimeAnimatorController;
								//SkinnedMeshRenderer skm = sim.GetComponentInChildren<SkinnedMeshRenderer>();
								//safe to destroy before adding because its just marked as destroy, not actually destroyed right now
								//DestroyImmediate(skm);
								//SkinnedMeshRenderer newSKM = skm.gameObject.AddComponent<SkinnedMeshRenderer>(sussySKM);
								//skm.GetCopyOf<SkinnedMeshRenderer>(sussySKM);
								//var skm = Utils.CopyComponent<SkinnedMeshRenderer>(sussySKM, sim.GetComponentInChildren<SkinnedMeshRenderer>());
								SkinnedMeshRenderer skm = sim.GetComponentInChildren<SkinnedMeshRenderer>();
								//newSKM.material = sussyMats[(int)crime];
								skm.material = sussyMats[(int)crime];
				}

				void ConvertToDeadAmongus(Sim sim)
				{
								SkinnedMeshRenderer skm = sim.GetComponentInChildren<SkinnedMeshRenderer>();
								//skm.sharedMesh = deadAmongusMesh;
								//skm.materials = deadAmongusMR.sharedMaterials;
								//DestroyImmediate(skm);
								GameObject skinnedChild = skm.gameObject;
								Destroy(skinnedChild.GetComponent<SkinnedMeshRenderer>());
								MeshRenderer newMR = skinnedChild.AddComponent<MeshRenderer>(deadAmongusMR);
								MeshFilter mf = skinnedChild.AddComponent<MeshFilter>(deadAmongusMF);
								mf.mesh = deadAmongusMF.sharedMesh;
								newMR.materials = deadAmongusMR.sharedMaterials;
								newMR.material = skm.material;
								////make sure dead amongus has the same base material as alive amongus
								//newSKM.material = skm.material;
				}
				void ConvertToGhost(Sim sim)
				{
								if (!sim.IsAlive())
								{
												return;
								}

								//DestroyImmediate(sim.GetComponent<Animator>());
								//sim.gameObject.AddComponent<Animator>(ghostAC);
								//sim.GetComponent<Animator>().GetCopyOf<Animator>(ghostAC);

								//SkinnedMeshRenderer skm = sim.GetComponentInChildren<SkinnedMeshRenderer>();

								//Utils.CopyComponent<Animator>(ghostAC, sim.GetComponent<Animator>());
								var skm = Utils.CopyComponent<SkinnedMeshRenderer>(ghostSKM, sim.GetComponentInChildren<SkinnedMeshRenderer>());
								skm.material = ghostSKM.sharedMaterial;

								//safe to destroy before adding because its just marked as destroy, not actually destroyed right now
								//Destroy(skm);
								//skm.gameObject.AddComponent<SkinnedMeshRenderer>(ghostSKM);

				}
}
