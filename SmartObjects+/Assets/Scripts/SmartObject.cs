using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum Needs
{
    food,
    fun,
    pee,
    shade,
    needsAmount
};

public struct SmartObjectServiceResponse
{
    public bool response; //false means no
    public Sim[] servicing; //if no, servicing is the Sims this SmartObject is servicing
    public Needs service; //service this SmartObject provides
}

public class SmartObject : MonoBehaviour
{
    [SerializeField]
    Needs provides; //need this smart object provides for our sims, also name of animation to play
    [SerializeField]
    int capacity; //how many sims can benefit at same time
    [SerializeField]
    float fillPerSecond; //how much it will feel the need per second
    [SerializeField]
    Sim[] providing; //the Sims this smartie is providing for right now
    [SerializeField]
    bool isSus; //is this a sussy task?
    

    // Start is called before the first frame update
    void Start()
    {
        providing = new Sim[capacity];
    }

    // Update is called once per frame
    void Update()
    {
        Service();
    }

    void Service()
    {
        foreach(Sim sim in providing)
        {
            if(sim)
            {
                //play the animations
                AnimateSim(sim);
                AnimateMe();

                //service their need
                sim.ServiceNeed(provides, fillPerSecond * Time.deltaTime);
												}
								}
				}

    public Needs GetProvides()
    {
        return provides;
				}
    public void RequestRemovalOfService(Sim sim)
    {
        for (int i = 0; i < capacity; ++i)
        {
            if(providing[i] == sim)
            {
                providing[i] = null;
                //return it back to its usual animation state
                sim.GetComponent<Animator>().SetBool(provides.ToString(), false);
                break;
												}
								}

        //if I'm not servicing anyone
        if(!IsServicingAnyone())
        {
            //stop my animation
            UnanimateMe();
								}

				}
    void AnimateSim(Sim sim)
    {
        Animator simAC = sim.GetComponent<Animator>();

        if(!simAC)
        {
            return;
								}

        //make sure their animator isn't showing any other animations
        foreach (AnimatorControllerParameter parameter in simAC.parameters)
        {
            simAC.SetBool(parameter.name, false);
        }
        //turn on the animation for the need I provide
        simAC.SetBool(provides.ToString(), true);
    }

    void AnimateMe()
    {
        Animator ac = GetComponent<Animator>();

        if(ac)
        {
            ac.SetBool("inUse", true);
								}
				}

    void UnanimateMe()
    {
        Animator ac = GetComponent<Animator>();

        if (ac)
        {
            ac.SetBool("inUse", false);
        }

    }

    private void OnTriggerEnter(Collider other)
				{
								if(other.CompareTag("Sim"))
        {
            Sim sim = other.GetComponent<Sim>();
            if(sim && sim.IsAlive())
            {
                sim.ReceiveMessage(new ProvideNeedMessage { type = MessageType.ProvideNeed, payload = provides, pos = transform.position });
												}
								}
				}
    //SmartObject can provide for multiple Sims at once, this returns index of an open spot, -1 if there are no open spots
    int GetEmptyServiceSpot()
    {
        for(int i = 0; i < capacity; ++i)
        {
            if(!providing[i])
            {
                return i;
												}
								}
        return -1;
				}

    bool IsServicingAnyone()
    {
        for (int i = 0; i < capacity; ++i)
        {
            if (providing[i])
            {
                return true;
            }
        }
        return false;
    }

    //Sim asks to request service to get need provided for
    //if not already providing for a Sim, then provide for that one and return to them if they're chosen or not
    public SmartObjectServiceResponse RequestService(Sim requester)
    {
        SmartObjectServiceResponse response = new SmartObjectServiceResponse();
        response.service = provides;
        int openSpot = GetEmptyServiceSpot();

        //if found open spot
        if (openSpot != -1)
        {
            providing[openSpot] = requester;
            response.response = true;
								}
								else
								{
            response.response = false;
            response.servicing = providing;
								}
        return response;
				}

    public void Kill(Sim sim)
    {
        //remove from servicing
        RequestRemovalOfService(sim);
        //kill
        KillSim(sim);
				}
    void KillSim(Sim sim)
    {
        //let the Sim know they're dead
        sim.Die();
        //get rid of Sim's navmeshagent so it cant try to move anywhere
        Destroy(sim.GetComponent<NavMeshAgent>());
        //make Sim suceptible to physics
        Rigidbody rb = sim.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;

        //unanimate sim
        sim.GetComponent<Animator>().enabled = false;

    }

    public bool IsSus()
    {
        return isSus;
				}
}
