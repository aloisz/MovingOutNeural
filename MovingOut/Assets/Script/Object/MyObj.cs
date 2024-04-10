using System;
using System.Collections;
using System.Collections.Generic;
using Script;
using UnityEngine;

public class MyObj : MonoBehaviour, IInteractable
{
    public List<Agent> agents;

    private void Awake()
    {
        AgentManager.Instance.myObjs.Add(this);
    }

    public void Interact(Agent agent)
    {
        if(agents.Contains(agent)) return;
        agents.Add(agent);
        agent.AddScoreByPassingCheckpoint(50);
        StartCoroutine(RemoveAgent(agent));
    }

    IEnumerator RemoveAgent(Agent agent)
    {
        yield return new WaitForSeconds(1);
        agents.Remove(agent);
    }

    public void ResetList()
    {
        agents.Clear();
    }
}
