using Ditzelgames;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WaterFloat))]
public class WaterBoat : MonoBehaviour
{
    //visible Properties
    public Transform Motor;
    public float SteerPower = 500f;
    public float forwardPower = 5f;
    public float leftRightPower = 5f;
    public float backPower = 5f;
    public float MaxSpeed = 10f;
    public float Drag = 0.1f;

    //used Components
    protected Rigidbody Rigidbody;
    protected Quaternion StartRotation;
    //protected ParticleSystem ParticleSystem;
    protected Camera Camera;

    //internal Properties
    protected Vector3 CamVel;
    public float rotationOffsetY;
    public GameObject freeCamera;
    public bool back = false;


    public void Awake()
    {
        //ParticleSystem = GetComponentInChildren<ParticleSystem>();
        Rigidbody = GetComponent<Rigidbody>();
        StartRotation = Motor.localRotation;
        Camera = Camera.main;
    }

    public void FixedUpdate()
    {
        rotationOffsetY =  268.686f - freeCamera.transform.eulerAngles.y;
        if (Input.GetAxis("Mouse X") != 0)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, freeCamera.transform.eulerAngles.y, transform.eulerAngles.z);
        }
        //default direction
        var forceDirection = transform.forward;
        //var steer = 0;
        
         /*if (Input.GetAxis("Mouse X") != 0)
        {
            //Debug.Log(Input.GetAxis("Mouse X"));
            if (Input.GetAxis("Mouse X") < 0.1f && Input.GetAxis("Mouse X") > -0.1f)
            {
                // return;
            }
            transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X") * Time.fixedDeltaTime * 200, 0));
            //clearArrow(false);
        }*/

        //steer direction [-1,0,1]
        if (Input.GetKey(KeyCode.A))
            //steer = 1;
            PhysicsHelper.ApplyForceToReachVelocity(Rigidbody, -transform.right * MaxSpeed, leftRightPower);
        if (Input.GetKey(KeyCode.D))
            //steer = -1;
            PhysicsHelper.ApplyForceToReachVelocity(Rigidbody, transform.right * MaxSpeed, leftRightPower);


        //Rotational Force
        //Rigidbody.AddForceAtPosition(steer * transform.right * SteerPower / 100f, Motor.position);

        //compute vectors
        var forward = Vector3.Scale(new Vector3(1,0,1), transform.forward);
        var targetVel = Vector3.zero;

        //forward/backward poewr
        if (Input.GetKey(KeyCode.W))
            PhysicsHelper.ApplyForceToReachVelocity(Rigidbody, forward * MaxSpeed, forwardPower);
        if (Input.GetKey(KeyCode.S))
            PhysicsHelper.ApplyForceToReachVelocity(Rigidbody, forward * -MaxSpeed, forwardPower);

        if (back)
            PhysicsHelper.ApplyForceToReachVelocity(Rigidbody, forward * -MaxSpeed, backPower);

        //Motor Animation // Particle system
        //Motor.SetPositionAndRotation(Motor.position, transform.rotation * StartRotation * Quaternion.Euler(0, 30f * steer, 0));
        /*if (ParticleSystem != null)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
                ParticleSystem.Play();
            else
                ParticleSystem.Pause();
        }*/

        //moving forward
        var movingForward = Vector3.Cross(transform.forward, Rigidbody.velocity).y < 0;

        //move in direction
        Rigidbody.velocity = Quaternion.AngleAxis(Vector3.SignedAngle(Rigidbody.velocity, (movingForward ? 1f : 0f) * transform.forward, Vector3.up) * Drag, Vector3.up) * Rigidbody.velocity;

        //camera position
        //Camera.transform.LookAt(transform.position + transform.forward * 6f + transform.up * 2f);
        //Camera.transform.position = Vector3.SmoothDamp(Camera.transform.position, transform.position + transform.forward * -8f + transform.up * 2f, ref CamVel, 0.05f);
    }

    void OnTriggerEnter(Collider collision) 
    {
        if(collision.tag == "PillarShadows")
        {
            collision.gameObject.AddComponent<PillarShadow>();
            backPower = 0.0f;
        }
    }
    void OnTriggerExit(Collider collision) 
    {
        if(collision.tag == "PillarShadows")
        {
            Destroy(collision.gameObject.GetComponent<PillarShadow>());
            backPower = 1.0f;
        }
    }

}