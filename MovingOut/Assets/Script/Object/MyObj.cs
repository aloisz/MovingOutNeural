using System;
using System.Collections;
using System.Collections.Generic;
using Script;
using UnityEngine;

public class MyObj : MonoBehaviour, IInteractable
{
    public List<Agent> agents;
    private Vector3 baseTransform;
    private Quaternion baseQuaternion;

    private void Awake()
    {
        AgentManager.Instance.myObjs.Add(this);
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(.2f);
        ;
    }

    public void Interact(Agent agent)
    {
        if(agents.Contains(agent)) return;
        agents.Add(agent);
        agent.obj = this;
        agent.AddScoreByPassingCheckpoint(50);
        StartCoroutine(RemoveAgent(agent));
    }

    IEnumerator RemoveAgent(Agent agent)
    {
        yield return new WaitForSeconds(1);
        agent.obj = null;
        agents.Remove(agent);
    }

    private bool isOn;
    public void ResetList()
    {
        if (!isOn)
        {
            isOn = true;
            baseTransform = transform.position;
            baseQuaternion = transform.rotation;
        }
        agents.Clear();
        transform.position = baseTransform;
        transform.rotation = baseQuaternion;
    }
}
