using System;
using System.Collections;
using System.Collections.Generic;
using Script;
using UnityEngine;

public class MyObj : MonoBehaviour, IInteractable
{
    public List<Agent> agents;
    public Vector3 baseTransform;
    public Quaternion baseQuaternion;

    private void Awake()
    {
        AgentManager.Instance.myObjs.Add(this);
    }


    private void Start()
    {
        baseTransform = transform.localPosition;
        baseQuaternion = transform.localRotation;
    }

    public void Interact(Agent agent)
    {
        
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
            baseTransform = transform.localPosition;
            baseQuaternion = transform.localRotation;
            
            
            Debug.Log(" (!isOn)");
        }
        
        
        Debug.Log("        transform.localPosition = baseTransform;");
        agents.Clear();
        transform.localPosition = baseTransform;
        transform.localRotation = baseQuaternion;
        transform.GetComponent<Collider>().enabled = true;
        transform.GetComponent<Rigidbody>().isKinematic = false;
        //transform.GetChild(0).transform.transform.position = Vector3.zero;
    }
}
