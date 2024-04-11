using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjDetection : MonoBehaviour
{
    [SerializeField] private MyObj obj;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            obj.agents.Add(other.GetComponent<Agent>());
            other.GetComponent<Agent>().obj = obj;
            other.GetComponent<Agent>().Bonus(10000);
            other.GetComponent<Agent>().ResetTimer();
            //other.GetComponent<Agent>().nextCheckpoint = other.GetComponent<Agent>().basePosition;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Agent>().obj = null;
            obj.agents.Remove(other.GetComponent<Agent>());
            other.GetComponent<Agent>().Bonus(-50);
        }
    }
}
