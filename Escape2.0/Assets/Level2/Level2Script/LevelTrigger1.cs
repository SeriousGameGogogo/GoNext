using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTrigger1 : MonoBehaviour
{
    public Waves waves;
    public WaterBoat boatController;
    public float tranSpeed;
    public float timer = 0.0f;
    public bool timerSwitch =false;
    public ParticleSystem sunParticle;
    public ParticleSystem rainParticle;
    public Light directLight;
    public Light pointLight_1;
    public Light pointLight_2;
    public Light pointLight_3;
    // Start is called before the first frame update
    void Start()
    {
        waves = FindObjectOfType<Waves>();
        boatController = FindObjectOfType<WaterBoat>();
        sunParticle.Play();
        rainParticle.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if(timerSwitch)
        {
            timer += Time.deltaTime;
        }
        if (timer >= 5)
        {
            waves.Octaves[1].speed = new Vector2(0.0f, -70);
            waves.Octaves[1].height = 0.5f;
            //directLight.color = new Color(0.1f,0.1f,0.1f,1.0f);
            directLight.color = new Color(1.0f,1.0f,1.0f,1.0f);
            pointLight_1.color = new Color(0.2f,0.2f,1.0f,1.0f);
            pointLight_2.color = new Color(0.2f,0.2f,1.0f,1.0f);
            pointLight_3.color = new Color(0.2f,0.2f,1.0f,1.0f);
            boatController.forwardPower = 0f;
            boatController.back = true;
            sunParticle.Stop();
            rainParticle.Play();

        }
        if (timer >= 15)
        {
            waves.Octaves[1].speed = new Vector2(0.0f, 30);
            waves.Octaves[1].height = 0.5f;
            directLight.color = new Color(1.0f,1.0f,1.0f,1.0f);
            pointLight_1.color = new Color(1.0f,0.2f,0.2f,1.0f);
            pointLight_2.color = new Color(1.0f,0.2f,0.2f,1.0f);
            pointLight_3.color = new Color(1.0f,0.2f,0.2f,1.0f);
            boatController.forwardPower += 1.0f;
            boatController.back = false;
            timer = -5.0f;
            sunParticle.Play();
            rainParticle.Stop();
        }
    }
    void OnTriggerEnter(Collider collision) 
    {
        if(collision.tag == "Boat")
        timerSwitch = true;
        {
            timerSwitch = true;
            Debug.Log("Level 1 Trigger");
        }
    }
}
