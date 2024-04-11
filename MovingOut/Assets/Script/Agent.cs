using System;
using System.Collections;
using System.Collections.Generic;
using Script;
using UnityEngine;
using UnityEngine.Serialization;

public class Agent : MonoBehaviour
{
    public NeuralNetwork net;
    public float fitness;

    [SerializeField] private float raylenght = 10;
    [Space]
    [SerializeField] private float rayRange = 5;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Rigidbody rb;
    
    [Space]
    public Vector3 nextCheckpoint;
    public float velocityValue = 0f;
    
    [SerializeField] float[] inputs;

    public Vector3 basePosition;

    public MyObj obj;
    
    [Space]
    [SerializeField] private float dotProduct;
    [SerializeField] private float crossMagnitude;
    private Vector3 cross;
    private Vector3 dirToTarget;

    private Vector3 transformForward;
    private Vector3 transformRight;
    private void Start()
    {
        transformForward = transform.forward;
        transformRight = transform.right;
    }

    public void ResetAgent(Vector3 offSet, Transform checkpoint)
    {
        inputs = new float[net.layers[0]]; // Init input 
        transform.position = offSet;
        basePosition = transform.position;
        transform.rotation = Quaternion.identity;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
        playerController.Reset();
        obj = null;
        nextCheckpoint = checkpoint.position; // posiiton de la caisse

        isGoingWrongWay = 0;
        isTouched = 0;
        bonus = 0;
        IsTouchingObj(false);
        RemainingTime = baseTime;

        dotProduct = 0;
        crossMagnitude = 0;
        dirToTarget = Vector3.zero;
        cross = Vector3.zero;
        velocityValue = 0;

        colliderValue = 0;
        valuevelocity = 0;
        
        fitness = 0;
    }

    private void FixedUpdate()
    {
        InputUpdate();
        OutputUpdate();
        FitnessUpdate();
    }

    private Vector3 pos;
    private void InputUpdate()
    {
        pos = transform.position;
        var setUpPos = Vector3.up * 0.02f;
        
        // Front
        inputs[0] = RaySensor(pos + setUpPos, transformForward, raylenght);
        
        // Sides
        
        
        inputs[1] = RaySensor(pos + setUpPos, transformRight, raylenght);
        inputs[2] = RaySensor(pos + setUpPos, -transformRight, raylenght);
        
        // Diagonals
        inputs[3] = RaySensor(pos + setUpPos, transformForward + transformRight, raylenght);
        inputs[4] = RaySensor(pos + setUpPos, transformForward + -transformRight, raylenght);
        
        // back
        inputs[5] = RaySensor(pos + setUpPos, -transformForward, raylenght);

        inputs[6] = 1;

        inputs[7] = IsCatching(pos + setUpPos, transform.forward, .2f);

        inputs[8] = Dot();

        inputs[9] = Cross();

        inputs[9] = RaySensorCollider(pos + setUpPos, transform.forward, 2f);
    }

    private RaycastHit hit;
    private float RaySensor(Vector3 origin, Vector3 dir, float lenght)
    {
        float value = 0;
        if (Physics.Raycast(origin, dir, out hit, rayRange * lenght, layerMask))
        {
            value = 1 - hit.distance / (rayRange * lenght);
            Debug.DrawRay(origin, dir * hit.distance, Color.Lerp(Color.green, Color.red, value));
        }
        return value;
    }
    
    private float IsCatching(Vector3 origin, Vector3 dir, float lenght)
    {
        float returnValue = 0f;
        if (Physics.Raycast(origin, dir, out hit, rayRange * lenght, layerMask))
        {
            returnValue = hit.transform.GetComponent<IInteractable>() != null ? 1 : 0f;
            Debug.DrawRay(origin, dir * hit.distance, Color.Lerp(Color.red, Color.green, 1 ));
        }
        return returnValue;
    }

    private float RaySensorCollider(Vector3 origin, Vector3 dir, float lenght)
    {
        
        if (Physics.Raycast(origin, dir, out hit, rayRange * lenght, layerMask))
        {
            if (hit.transform.GetComponent<Collider>() != null)
            {
                isTouched = -1f;
                Debug.DrawRay(origin, dir * hit.distance, Color.Lerp(Color.red, Color.green, isTouched));
            }
            
        }
        else isTouched = 1f;
        
        return isTouched;
    }
    
            
    private float Dot()
    {  
        dirToTarget = Vector3.Normalize(nextCheckpoint - transform.position);
        dotProduct = Vector3.Dot(transform.forward, dirToTarget);
        return dotProduct;
    }
    
    private float Cross()
    {  
        cross = Vector3.Cross(transform.forward, dirToTarget.normalized);
        crossMagnitude = cross.y;
        return crossMagnitude;
    }

    private void OutputUpdate()
    {
        net.FeedForward(inputs);
        playerController.horizontalInput = net.neurons[^1][0];
        playerController.verticalInput = net.neurons[^1][1];
        playerController.holdingInput = net.neurons[^1][2];
    }

    [SerializeField] float isGoingWrongWay;
    [SerializeField] float valuevelocity;
    [SerializeField] float colliderValue;
    private void FitnessUpdate()
    {
        isGoingWrongWay = 1 - (nextCheckpoint -  transform.position).magnitude;
        if (transform.position.magnitude >20)
        {
            velocityValue += transform.position.magnitude;
        }
        else
        {
            velocityValue -= transform.position.magnitude;
        }

        valuevelocity = Mathf.Clamp(velocityValue, -100, 10);
        colliderValue = isTouched;
        
        //if (fitness < distanceTraveled) fitness = distanceTraveled;
        RemainingTime -= (Time.fixedDeltaTime % 60) * 10;
        
        fitness =  bonus + isGoingWrongWay + RemainingTime; //+ colliderValue
    }

    [SerializeField] float isTouched;
    private void OnCollisionEnter(Collision other)
    {
        if (LayerMask.GetMask(LayerMask.LayerToName(other.gameObject.layer)) == layerMask)
        {
            //IsTouchingObj(true);
        }
    }

    private void IsTouchingObj(bool touching)
    {
        if(touching) isTouched -= 50;
        else isTouched += 50;
    }


    public void SetFirstMat()
    {
        //_meshRenderer.material = firstMat;
    }
    
    public void SetDefaultMat()
    {
        //_meshRenderer.material = defaulttMat;
    }
    public void SetMutatedMat()
    {
        //_meshRenderer.material = mutatedMat;
    }

    [FormerlySerializedAs("checkPoints")] [SerializeField] private float bonus;
    public float Bonus(float points)
    {
        return this.bonus += points;
    }

    [SerializeField] private float baseTime = 5;
    [SerializeField] private float RemainingTime = 5;
    public void ResetTimer()
    {
        RemainingTime = baseTime;
    }

    private void OnDrawGizmos()
    {
        
    }
}
