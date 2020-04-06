using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesMove : MonoBehaviour
{
    public int moveSpeed = 10;
    public Rigidbody enemyRig;
    public MeshFilter mesh;
    // Start is called before the first frame update
    void Start()
    {
        //transform.position = Vector3.Lerp(new Vector3(0.0f, 2.2f, -89.5f), new Vector3(0.0f, 2.2f, 88.5f), moveSpeed * Time.deltaTime);
    }

    // Update is called once per frame
    void Update()
    {
        enemyRig.velocity = Vector3.forward * moveSpeed;
    }
}
