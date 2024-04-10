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
    
    private float distanceTraveled;
    [Space]
    [SerializeField] private float totalCheckpointDist;
    public Transform nextCheckpoint;
    [SerializeField] private float nextCheckpointDist;
    
    [SerializeField] float[] inputs;

    [SerializeField]private MeshRenderer _meshRenderer;
    [Space]
    [SerializeField] private Material firstMat;
    [SerializeField] private Material defaulttMat;
    [SerializeField] private Material mutatedMat;

    
    [Space]
    [SerializeField] private float dotProduct;
    [SerializeField] private float crossMagnitude;
    private Vector3 cross;
    private Vector3 dirToTarget;
    private void Start()
    {
        IsTouchingObj(false);
    }

    public void ResetAgent(Vector3 offSet)
    {
        inputs = new float[net.layers[0]]; // Init input 
        transform.position = offSet;
        transform.rotation = Quaternion.identity;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
        playerController.Reset();
        fitness = 0;
        totalCheckpointDist = 0;
        nextCheckpoint = CheckpointManager.Instance.firstCheckpoint;
        nextCheckpointDist = (nextCheckpoint.position - transform.position).magnitude;

        isGoingWrongWay = 0;
        isTouched = 0;
        checkPoints = 0;
        IsTouchingObj(false);
        RemainingTime = baseTime;

        dotProduct = 0;
        dirToTarget = Vector3.zero;
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

        inputs[6] = IsInteracting(pos + setUpPos, transform.forward, 2f);
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

    [HideInInspector]public MyObj obj;
    private float IsInteracting(Vector3 origin, Vector3 dir, float lenght)
    {
        float returnValue = 0f;
        if (Physics.Raycast(origin, dir, out hit, rayRange * lenght, layerMask))
        {
            float value = 1 - hit.distance / (rayRange * lenght);

            if (hit.transform.GetComponent<IInteractable>() != null)
            { 
                hit.transform.GetComponent<IInteractable>().Interact(this);
                returnValue = 1f;
            }
            else
            {
                returnValue = 0f;
            }
            Debug.DrawRay(origin, dir * hit.distance, Color.Lerp(Color.red, Color.green, value));
        }
        return returnValue;
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
        //distanceTraveled = totalCheckpointDist + (nextCheckpointDist - (nextCheckpoint.position - transform.position).magnitude);
        //isGoingWrongWay = nextCheckpointDist - (nextCheckpoint.position - transform.position).magnitude;
        //if (fitness < distanceTraveled) fitness = distanceTraveled;
        //RemainingTime -= (Time.fixedDeltaTime % 60) * 10;

        
        dirToTarget = Vector3.Normalize(nextCheckpoint.position - transform.position);
        dotProduct = Vector3.Dot(transform.right, dirToTarget);
        cross = Vector3.Cross(transform.right, dirToTarget);
        crossMagnitude = cross.magnitude;
        
        fitness = dotProduct + crossMagnitude; // + RemainingTime isGoingWrongWay + isTouched + checkPoints + 
    }

    [SerializeField] float isTouched;
    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.GetComponent<MyObj>() != null)
        {
            IsTouchingObj(true);
        }
    }
    
    private void OnCollisionExit(Collision other)
    {
        IsTouchingObj(false);
    }

    private void IsTouchingObj(bool touching)
    {
        if(touching) isTouched = 50;
        else isTouched = -50;
    }

    public void CheckpointReached(Transform nextCheckpoint)
    {
        totalCheckpointDist += nextCheckpointDist;
        this.nextCheckpoint = nextCheckpoint;
        nextCheckpointDist = (nextCheckpoint.position - transform.position).magnitude;
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

    [SerializeField] private float checkPoints;
    public float AddScoreByPassingCheckpoint(float points)
    {
        return this.checkPoints += points;
    }

    [SerializeField] private float baseTime = 5;
    [SerializeField] private float RemainingTime = 5;
    public void ResetTimer()
    {
        RemainingTime = baseTime;
    }
}
