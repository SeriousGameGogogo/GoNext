using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatMove : MonoBehaviour
{
    public GameObject Boat;
    public Rigidbody boatRigidbody;
    public float moveSpeed = 20.0f;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HorizontalMove(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        VerticalMove(Input.GetAxis("Vertical"));
    }
    void HorizontalMove(float horizontal, float vertical)
    {
        if(horizontal != 0)
        {
            boatRigidbody.velocity = new Vector3(boatRigidbody.velocity.x, boatRigidbody.velocity.y, horizontal * moveSpeed);
            Debug.Log("平移");
        }
        else if(horizontal == 0 && vertical == 0)
        {
            boatRigidbody.velocity = Vector3.zero;
        }
    }
    void VerticalMove(float vertical)
    {
        if(vertical != 0)
        {
            boatRigidbody.velocity = new Vector3(-vertical * moveSpeed, boatRigidbody.velocity.y, boatRigidbody.velocity.z);
            Debug.Log("进退");
        }
    }
    private void OnTriggerEnter(Collider collision) 
    {
        if(collision.tag == "PlliarShadows")
        {
            moveSpeed = 50.0f;
        }
    }
    private void OnTriggerExit(Collider collision) 
    {
        if(collision.tag == "PlliarShadows")
        {
            moveSpeed = 20.0f;
        }
    }
}
