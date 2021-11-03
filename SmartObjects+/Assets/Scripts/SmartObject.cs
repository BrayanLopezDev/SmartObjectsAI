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
}
