using System.Collections;
using System.Collections.Generic;
using Script;
using UnityEngine;

public class MyObj : MonoBehaviour, IInteractable
{
    public List<Agent> agents;

    public void Interact(Agent agent)
    {
        agents.Add(agent);
    }

    public void DeInteract(Agent agent)
    {
        agents.Remove(agent);
    }
}
