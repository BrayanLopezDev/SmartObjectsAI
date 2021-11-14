using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MessageType
{
    ProvideNeed, //message of need I provide (fulfill)
    SusInfo //message on sims I'm sus of
};

public enum KnowledgeType
{
    FirstHand, //I saw this happen
    SecondHand //someone else told me this happened
}
public class Message
{
    public MessageType type;
}

public class ProvideNeedMessage : Message
{
    public Needs payload;
    public Vector3 pos;
}
public class SusInfoMessage: Message
{
    public List<Sim> payload;
    public Crimes crime;
    public KnowledgeType who;
}
