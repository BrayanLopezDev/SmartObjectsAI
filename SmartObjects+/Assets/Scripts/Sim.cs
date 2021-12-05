using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

using Random = UnityEngine.Random;

public enum SimState
{
  Wandering,
  TravelingToProvider,
  InteractingWithProvider
}

public class Sim : MonoBehaviour/*,IComparable<Sim>*/
{
  //to have something for the IComparable to sort them by, also to tell them apart
  [SerializeField]
  int id;
  //important info about the world
  [SerializeField]
  World world;
  //the sus manager stores important info about who is sus
  SusManager sussyMan;
  //max a Sim can get of needs
  [SerializeField]
  float[] maxs;
  //when Sim is critically low on need, they will do anything to satisfy need
  [SerializeField]
  float[] crits;
  //when Sim will start to consider the need
  [SerializeField]
  float[] lows;
  //when Sim is approached by another Sim while getting provided for, how much should their need be full before they decide to leave?
  [SerializeField]
  float[] enoughToLeaves;
  //needs, 1-1 with Needs enum
  [SerializeField]
  float[] needs;
  //rate of decay for needs (per second)
  [SerializeField]
  float[] decays;
  //most recently seen smart object (at their position because some can move) which fulfills need
  [SerializeField]
  Vector3[] providers;
  //Sims I'm sus of
  [SerializeField]
  Dictionary<Sim, Crimes> sussys;
  //my navigation agent
  [SerializeField]
  NavMeshAgent agent;
  //Smart Object thats providing a need for me right now
  [SerializeField]
  SmartObject providingMe;
  //if Start() has been called yet, needed because receiving messages can happen before Start() gets called
  bool started = false;
  [SerializeField]
  SimState state = SimState.Wandering;
  //what Sim needs the most right now
  [SerializeField]
  Needs mostNeed = Needs.food;
  [SerializeField]
  bool isAlive = true;
  //sim will need a bit of control over the animator as well as the smartobjects
  [SerializeField]
  Animator ac;
  //max distance Sim has to be to speak to another Sim
  [SerializeField]
  float speakRadius;
  //will usually be walking
  [SerializeField]
  float walkSpeed;
  //will run when critically low on a need, or to run to police or to run to witnesses
  [SerializeField]
  float runSpeed;
  //for debugging, why do they randomly stop walking???
  [SerializeField]
  bool reachedDestination;
  [SerializeField]
  GameObject simInfoUI;
  [SerializeField]
  Slider[] needsSliders;
  [SerializeField]
  Image[] sliderImages;

  // Start is called before the first frame update
  void Start()
  {
    if (started)
    {
      return;
    }

    id = Random.Range(int.MinValue, int.MaxValue);

    world = GameObject.FindObjectOfType<World>();

    sussyMan = GameObject.FindObjectOfType<SusManager>();

    agent = GetComponent<NavMeshAgent>();

    ac = GetComponent<Animator>();

    sussys = new Dictionary<Sim, Crimes>();
    providers = new Vector3[(int)Needs.needsAmount];
    needs = new float[(int)Needs.needsAmount];

    needs[(int)Needs.food] = Random.Range(lows[(int)Needs.food], maxs[(int)Needs.food]);
    needs[(int)Needs.fun] = Random.Range(lows[(int)Needs.fun], maxs[(int)Needs.fun]);
    needs[(int)Needs.pee] = Random.Range(lows[(int)Needs.pee], maxs[(int)Needs.pee]);
    needs[(int)Needs.shade] = Random.Range(lows[(int)Needs.shade], maxs[(int)Needs.shade]);

    started = true;

    walkSpeed = agent.speed;
    runSpeed = agent.speed * 2f;

    Reprioritize();
    Travel();
    //StartCoroutine(Reprioritize());
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

      //update sliders
      needsSliders[i].value = needs[i];
      sliderImages[i].color = Color.Lerp(Color.red, Color.green, needs[i]);

      if (needs[i] <= 0f)
      {
        //die
        Kill();
        return;
      }
    }

    Simulate();
  }
  //the animator can be destroyed and a different one can be added at an undetermined time, get AC this way
  Animator AC()
  {
    if (!ac)
    {
      ac = GetComponent<Animator>();
    }
    return ac;
  }
  void Simulate()
  {
    reachedDestination = ReachedDestination();
    //if sim has no destination because they reached it
    if (reachedDestination)
    {
      //if was traveling to needs provider, means reached destination, now time to interact
      if (state == SimState.TravelingToProvider)
      {
        SmartObject provider = null;
        Collider[] cols = Physics.OverlapSphere(transform.position, speakRadius);

        foreach (Collider col in cols)
        {
          provider = col.GetComponent<SmartObject>();

          if (provider && provider.enabled)
          {
            if (provider.GetProvides() == mostNeed && CloseEnough(provider.transform.position))
            {
              ArriveAtProvider(provider);
              break;
            }
            else
            {
              provider = null;
            }
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
        if (state == SimState.InteractingWithProvider)
        {
          InteractingWithProvider();
          return;
        }

      }
    }
    if (state == SimState.Wandering)
    {
      Reprioritize();
    }
    if (state == SimState.TravelingToProvider)
    {
      Travel();
    }
  }

  bool ReachedDestination()
  {
    bool closeEnough = false;
    if (mostNeed != Needs.needsAmount)
    {
      //solves the problem of sim thinking they reached their destination but they're 20ft away
      if (agent.destination != providers[(int)mostNeed])
      {
        agent.SetDestination(providers[(int)mostNeed]);
      }
      closeEnough = CloseEnough(providers[(int)mostNeed]);
      if (closeEnough)
      {
        agent.destination = transform.position;
      }
    }
    //if agent doesnt have either a complete path nor an incomplete path, or its close enough to have reached destination
    return !agent.hasPath && !agent.pathPending || closeEnough;
  }

  bool CloseEnough(Vector3 destination)
  {
    return (destination - transform.position).sqrMagnitude <= 6f;
  }

  bool CloseEnough(Vector3 vec1, Vector3 vec2, float sqrMag)
  {
    return (vec2 - vec1).sqrMagnitude <= sqrMag;
  }
  void Reprioritize()
  {
    int biggestNeed = GetBiggestNeed();

    mostNeed = (Needs)biggestNeed;

    if (providers[biggestNeed] != Vector3.zero)
    {
      state = SimState.TravelingToProvider;
      agent.SetDestination(providers[biggestNeed]);
    }
    else
    {
      AskForDirectionsFromNearbySims();
    }

  }

  int GetBiggestNeed()
  {
    int biggestNeed = -1;
    float smallestNumber = float.MaxValue;
    for (int i = 0; i < (int)Needs.needsAmount; ++i)
    {
      if (needs[i] < smallestNumber /*&& needs[i] <= lows[i]*/)
      {
        biggestNeed = i;
        smallestNumber = needs[i];
      }
    }
    return biggestNeed;
  }
  void InteractingWithProvider()
  {
    if (needs[(int)mostNeed] >= maxs[(int)mostNeed])
    {
      if (providingMe)
      {
        providingMe.RequestRemovalOfService(this);
        DoneInteractingWithProvider();
      }
    }
  }

  public void ForceRemoveFromService()
  {
    DoneInteractingWithProvider();
  }

  void DoneInteractingWithProvider()
  {
    providingMe = null;
    state = SimState.Wandering;
    Reprioritize();
    Travel();

  }
  void ArriveAtProvider(SmartObject provider)
  {
    SmartObjectServiceResponse response = provider.RequestService(this);
    //if the smart object said no
    if (!response.response)
    {
      //if we're desparate because we REALLY need this				
      if (IsCritical(response.service))
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

  void AskForDirectionsFromNearbySims()
  {
    Collider[] cols = Physics.OverlapSphere(transform.position, speakRadius);
    AskForDirectionsFromNearbySims(cols);
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
      if (other && other.IsAlive())
      {
        //ask them for directions
        Vector3 directions = other.RequestDirectionsToProvider(this, mostNeed);
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
    if (CloseEnough(providers[(int)mostNeed], outdated, 25f))
    {
      //go somewhere random
      providers[(int)mostNeed] = world.GetRandomSpotWithinTerrainBounds();
      agent.SetDestination(providers[(int)mostNeed]);
      state = SimState.TravelingToProvider;
    }
  }
  public Vector3 RequestDirectionsToProvider(Sim requester, Needs service)
  {
    EnsureStarted();
    //if I don't know, or I think they're sus
    if (providers[(int)service] == Vector3.zero || sussys.ContainsKey(requester))
    {
      //lie
      return world.GetRandomSpotWithinTerrainBounds();
    }
    return providers[(int)service];
  }

  void EnsureStarted()
  {
    Start();
  }
  public void ReceiveMessage(Message ms)
  {
    EnsureStarted();
    if (!isAlive)
    {
      return;
    }
    switch (ms.type)
    {
      case MessageType.ProvideNeed:
        {
          ProvideNeedMessage pnms = (ProvideNeedMessage)ms;
          int needsIndex = (int)pnms.payload;

          providers[needsIndex] = pnms.pos;
          //if this smartobject has what I need the most, go there instead
          if (mostNeed == pnms.payload && state == SimState.TravelingToProvider)
          {
            agent.SetDestination(pnms.pos);
            Travel();
          }
          break;
        }

      case MessageType.SusInfo:
        {
          //if too busy get needs met by SmartObject, ignore this message
          if (state == SimState.InteractingWithProvider)
          {
            break;
          }

          //Sims = Sus Info Message? sus
          SusInfoMessage sims = (SusInfoMessage)ms;
          //sims I became newly sus of after other sim told me who they are sus of
          List<Sim> newlySus = new List<Sim>();
          //repeat sussy offenders
          List<Sim> reSus = new List<Sim>();
          foreach (var sussy in sims.payload)
          {
            if (!sussys.ContainsKey(sussy))
            {
              newlySus.Add(sussy);
            }
            else
            {
              reSus.Add(sussy);
            }
            sussys[sussy] = sims.crime;
          }

          //the sussy manager must know all about sus
          sussyMan.OnSus(sims.crime, newlySus, sims.who);
          sussyMan.OnResusUpdateCrime(sims.crime, reSus, sims.who);
          break;
        }
      case MessageType.SimInfo:
        {
          SimInfoMessage sims = (SimInfoMessage)ms;
          int simCount = sims.payload.Count;

          for (int i = 0; i < simCount; ++i)
          {
            Sim other = sims.payload[i];
            if (sussys.ContainsKey(other)) //if im sus of em
            {
              //go somewhere else if I'm not too terribly low on the need I'm gettting served right now
              if (state == SimState.InteractingWithProvider && needs[(int)mostNeed] >= lows[(int)mostNeed])
              {
                providingMe.RequestRemovalOfService(this);
                providingMe = null;
                state = SimState.Wandering;
              }
              if (!IsCritical(mostNeed)) //if I'm not critically low on that need, go somewhere else
              {
                providers[(int)mostNeed] = world.GetRandomSpotWithinTerrainBounds();
                if (state == SimState.TravelingToProvider)
                {
                  agent.SetDestination(providers[(int)mostNeed]);
                }

              }
            }
            else if (state != SimState.InteractingWithProvider) //if im not busy interacting with provider
            {
              SendSusInfoMessage(other);
            }
          }

          break;
        }

    }
  }

  void SendSusInfoMessage(Sim other)
  {
    List<Sim> sus = new List<Sim>();

    foreach (var sussy in sussys)
    {
      if (Random.Range(0, 300) == 0) //1 in 600 chance
      {
        sus.Add(sussy.Key);
        other.ReceiveMessage(new SusInfoMessage { type = MessageType.SusInfo, crime = sussy.Value, payload = sus, who = KnowledgeType.SecondHand });
        sus.Clear();
      }
    }
  }
  bool IsCritical(Needs need)
  {
    return needs[(int)need] <= crits[(int)need];
  }

  void Travel()
  {
    if (IsCritical(mostNeed))
    {
      //play run animation
      AC().SetBool("run", true);
      AC().SetBool("walk", false);
      //set speed to run speed
      agent.speed = runSpeed;
    }
    else
    {
      //play walk animation
      AC().SetBool("run", false);
      AC().SetBool("walk", true);
      //set speed to walk speed
      agent.speed = walkSpeed;
    }
  }
  public void Die()
  {
    sussyMan.OnDeath(this, sussys.Keys.ToList<Sim>());
    GetComponentInChildren<SimViewFrustum>().OnDeath();
    isAlive = false;
    this.enabled = false;
  }

  public void Kill()
  {
    if (providingMe) //if a smart object is providing for me right now
    {
      //have them kill me
      providingMe.Kill(this);
    }
    else //find any smart object and have them kill me
    {
      SmartObject smartie = GameObject.FindObjectOfType<SmartObject>();

      if (smartie)
      {
        smartie.Kill(this);
      }
    }
  }

  public bool IsAlive()
  {
    EnsureStarted();
    return isAlive;
  }
  public void ServiceNeed(Needs need, float amount)
  {
    needs[(int)need] += amount;
  }

  public void Select()
  {
    simInfoUI.SetActive(true);
  }

  public void Deselect()
  {
    simInfoUI.SetActive(false);
  }

  public Dictionary<Sim, Crimes> GetSussyList()
  {
    return sussys;
  }

  public int GetID()
  {
    return id;
  }

  ////I don't care how they're sorted, I just wanted a Set but I can't have a Set unless its specifically a Sorted Set???
  //public int CompareTo(Sim other)
  //{
  //				return (int)(id - other.id); 
  //}
}
