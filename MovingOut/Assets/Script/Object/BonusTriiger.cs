using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusTriiger : MonoBehaviour
{
    [SerializeField] private float bonusPoint;
    public bool isPassed = false;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isPassed)
        {
            isPassed = true;
            other.GetComponent<Agent>().Bonus(bonusPoint);
        }
    }
}
