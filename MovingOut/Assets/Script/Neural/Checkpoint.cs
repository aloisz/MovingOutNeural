using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Checkpoint : MonoBehaviour
{
    public Transform nextCheckpoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Agent>(out Agent agent))
        {
            if(agent.nextCheckpoint == transform)
            {
                agent.ResetTimer();
                agent.CheckpointReached(nextCheckpoint);
                agent.AddScoreByPassingCheckpoint(25);
            }
            else
            {
                agent.AddScoreByPassingCheckpoint(-200);
            }
        }
        if (other.TryGetComponent<MyObj>(out MyObj obj))
        {
            foreach (var agentt in obj.agents)
            {
                agentt.AddScoreByPassingCheckpoint(500);
            }
        }
        
    }
}
