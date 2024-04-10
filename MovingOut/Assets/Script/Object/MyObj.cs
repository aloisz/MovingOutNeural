using System.Collections;
using System.Collections.Generic;
using Script;
using UnityEngine;

public class MyObj : MonoBehaviour, IInteractable
{
    public List<Agent> agents;

    public void Interact(Agent agent)
    {
        if(agents.Contains(agent)) return;
        agents.Add(agent);
    }

    public void DeInteract(Agent agent)
    {
        if(!agents.Contains(agent)) return;
        agents.Remove(agent);
    }

    public void ResetList()
    {
        agents.Clear();
    }
}
