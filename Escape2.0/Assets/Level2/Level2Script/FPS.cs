using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS : MonoBehaviour
{
   public Transform camPivot;
   public Transform cam;
   float heading=0;
   Vector2  input;
   Animator run;

   void Update() 
   {
       heading += Input.GetAxis("Mouse X")*Time.deltaTime*45;
       camPivot.rotation=Quaternion.Euler(0,heading,0);

       input=new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
       input = Vector2.ClampMagnitude(input,1);

       Vector3 camF =cam.forward;
       Vector3 camR= cam.right;

       camF.y=0;
       camR.y=0;
       camF = camF.normalized;
       camR=camR.normalized;

       transform.position += (camF*input.y + camR*input.x)*Time.deltaTime*5;
   }



//    float translation;
//    float strafe;
//    public float rotationSpeed;

   
//      void FixedUpdate() 
//        {
//         Vector3 movement = new Vector3 (strafe, 0f, translation);

//         transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (movement), rotationSpeed);    

//         transform.Translate (cam.TransformDirection(movement));  
//        }

       

      


//    public float movementSpeed;
//    public float distance;
//    float vertical;
//    float horizontal;

//    void ControllPlayer()
//      {
//          float moveHorizontal = Input.GetAxisRaw ("Horizontal");
//          float moveVertical = Input.GetAxisRaw ("Vertical");
        
 
//          Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
         
//         if (movement != Vector3.zero)
//         {
//          transform.rotation = Quaternion.LookRotation(movement);
//         }
 
//          transform.Translate (movement * movementSpeed * Time.deltaTime, Space.World);
//          transform.position = transform.position + Camera.main.transform.forward * distance * Time.deltaTime;
     
//           Vector3 targetDirection = new Vector3(horizontal, 0f, vertical);
//           targetDirection = Camera.main.transform.TransformDirection(targetDirection);
//           targetDirection.y = 0.0f;
//      }
   
   
   
}
