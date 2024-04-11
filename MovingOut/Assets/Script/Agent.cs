using System.Collections;
using System.Collections.Generic;
using Script;
using UnityEngine;
using UnityEngine.Serialization;

public class Agent : MonoBehaviour
{
    public NeuralNetwork net;
    public float fitness;
    [Space]
    [SerializeField] private float rayRange = 5;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Rigidbody rb;
    
    [Space]
    public Vector3 nextCheckpoint;
    
    [SerializeField] float[] inputs;

    public Vector3 basePosition;

    public MyObj obj;
    
    [Space]
    [SerializeField] private float dotProduct;
    [SerializeField] private float crossMagnitude;
    private Vector3 cross;
    private Vector3 dirToTarget;
    private void Start()
    {
        //IsTouchingObj(false);
    }

    public void ResetAgent(Vector3 offSet, Transform checkpoint)
    {
        inputs = new float[net.layers[0]]; // Init input 
        basePosition = transform.position;
        transform.position = offSet;
        transform.rotation = Quaternion.identity;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
        playerController.Reset();
        Debug.Log("     obj = null;");
        obj = null;
        nextCheckpoint = checkpoint.position; // posiiton de la caisse

        isGoingWrongWay = 0;
        isTouched = 0;
        bonus = 0;
        //IsTouchingObj(false);
        RemainingTime = baseTime;

        dotProduct = 0;
        crossMagnitude = 0;
        dirToTarget = Vector3.zero;
        cross = Vector3.zero;
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
        var transformForward = transform.forward;
        var transformRight = transform.right;
        
        // Front
        inputs[0] = RaySensor(pos + setUpPos, transform.forward, 4f);
        
        // Sides
        inputs[1] = RaySensor(pos + setUpPos, transform.right, 1.5f);
        inputs[2] = RaySensor(pos + setUpPos, -transform.right, 1.5f);
        
        // Diagonals
        inputs[3] = RaySensor(pos + setUpPos, transform.forward + transform.right, 2f);
        inputs[4] = RaySensor(pos + setUpPos, transform.forward + -transform.right, 2f);

        inputs[5] = 1;

        inputs[6] = IsCatching(pos + setUpPos, transform.forward, .2f);

        inputs[7] = Dot();

        inputs[8] = Cross();
    }

    private RaycastHit hit;
    private float RaySensor(Vector3 origin, Vector3 dir, float lenght)
    {
        if (Physics.Raycast(origin, dir, out hit, rayRange * lenght, layerMask))
        {
            float value = 1 - hit.distance / (rayRange * lenght);
            Debug.DrawRay(origin, dir * hit.distance, Color.Lerp(Color.red, Color.green, value));
            return value;
        }
        else
        {
            if (Physics.Raycast(origin, dir, out hit, rayRange * lenght, layerMask))
            {
                float value = 1 - hit.distance / (rayRange * lenght);
                Debug.DrawRay(origin, dir * hit.distance, Color.Lerp(Color.red, Color.red, value));
            }
            return 0;
        }
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
    private void FitnessUpdate()
    {
        isGoingWrongWay = 1 - nextCheckpoint.magnitude / (nextCheckpoint -  transform.position).magnitude;
        float velocityValue = 0f;
        if (rb.velocity.magnitude < 15)
        {
            velocityValue -= 150;
        }
        else
        {
            velocityValue += 10;
        }
        //if (fitness < distanceTraveled) fitness = distanceTraveled;
        //RemainingTime -= (Time.fixedDeltaTime % 60) * 10;
        
        fitness = -isGoingWrongWay * 10 + bonus + velocityValue; // (dotProduct * 3) + crossMagnitude  + 
    }

    [SerializeField] float isTouched;
    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.GetComponent<MyObj>() != null)
        {
            //IsTouchingObj(true);
        }
    }
    
    private void OnCollisionExit(Collision other)
    {
        //IsTouchingObj(false);
    }

    private void IsTouchingObj(bool touching)
    {
        if(touching) isTouched = 50;
        else isTouched = -50;
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
}
