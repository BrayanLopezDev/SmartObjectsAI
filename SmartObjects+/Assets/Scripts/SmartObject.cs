using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Needs
{
    food,
    fun,
    pee,
    shade,
    needsAmount
};

public class SmartObject : MonoBehaviour
{
    [SerializeField]
    Needs provides; //need this smart object provides for our sims, also name of animation to play
    [SerializeField]
    int capacity; //how many sims can benefit at same time
    [SerializeField]
    float fillPerSecond; //how much it will feel the need per second
    [SerializeField]
    Sim providing; //the Sim this smartie is providing a need for right now, null if none
    [SerializeField]
    bool isSus; //is this a sussy task?
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

				private void OnTriggerEnter(Collider other)
				{
								if(other.CompareTag("Sim"))
        {
            other.GetComponent<Sim>().ReceiveMessage(new ProvideNeedMessage { type = MessageType.ProvideNeed, payload = provides, pos = transform.position });
								}
				}

    //Sim asks to request service to get need provided for
    //if not already providing for a Sim, then provide for that one and return to them if they're chosen or not
    public bool RequestService(Sim requester)
    {
        if(!providing)
        {
            providing = requester;
								}
        return providing == requester;
				}

    public bool IsSus()
    {
        return isSus;
				}
}
