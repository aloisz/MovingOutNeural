using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform holdingPoint;
    [SerializeField] private AnimationCurve forceBySpeed;
    [SerializeField] private Agent agent;

    public bool isMoving;
    public float horizontalInput;
    public float verticalInput;
    public float holdingInput;
    
    [SerializeField] private Transform centerOfMass;

    private float steering;
    [SerializeField] private float maxSteering = 45;
    [SerializeField] private float steeringLerp = 20;

    [SerializeField] private float torqueStrengh = 0.001f;
    [SerializeField] private float torqueStrenghDrifting = 0.001f;
    [SerializeField] private AnimationCurve antiTorque;
    [SerializeField] private AnimationCurve torqueByVelocity;
    [SerializeField] private AnimationCurve driftBySpeed;
    
    private Vector3 localvelocity;
    private Vector3 steeringDirection;
    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centerOfMass.localPosition;
    }
    
    public void Reset()
    {
        horizontalInput = 0;
        verticalInput = 0;
        holdingInput = 0;
    }

    void FixedUpdate()
    {
        GetLocalVelocity();
        SetDirection();
        RotateVehicule();
        GoForward();
        CatchObj();

        if (horizontalInput > .5f || verticalInput > .5f) isMoving = true;
        else isMoving = false;
        
        ShowDirection();
    }

    private void CatchObj()
    {
        if(!agent.obj) return;
        agent.obj.GetComponent<Rigidbody>().isKinematic = true;
        agent.obj.GetComponent<Collider>().enabled = false;
        agent.obj.transform.position = holdingPoint.position;
        
        agent.nextCheckpoint = agent.basePosition;
    }

    private void GetLocalVelocity()
    {
        localvelocity = transform.InverseTransformVector(rb.velocity);
    }
    
    private void SetDirection()
    {
        steering = math.lerp(steering, horizontalInput * maxSteering, steeringLerp * Time.deltaTime);
        steeringDirection = Quaternion.AngleAxis(steering, Vector3.up) * transform.forward;
    }
    
    private void RotateVehicule()
    {
        rb.AddTorque(0,steering * torqueStrengh * torqueByVelocity.Evaluate(localvelocity.z),0);
        rb.AddTorque(0,-rb.angularVelocity.y * antiTorque.Evaluate(math.abs(steering)),0);
    }
    
    private void GoForward()
    {
        rb.AddForce(transform.forward * (verticalInput * forceBySpeed.Evaluate(localvelocity.z)*10));
    }

    private void ShowDirection()
    {
        /*Debug.DrawRay(transform.position, steeringDirection * 10, Color.magenta);
        Debug.DrawRay(transform.position, rb.velocity * 10, Color.blue);*/
    }
}
