using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHide : MonoBehaviour
{
    public Light PointLight;
    public float gapTime; //闪烁的间隔时间，在Unity中修改
    public float temp = 0;
    bool IsDisplay = true;
    public float t = 0.0f;
    public float moveSpeed = 40.0f;
    void Start()
    {


    }


    // Update每帧调用一次
    void Update()
    {
        Flicker();
    }
    public void Effect()
    {
        temp += Time.deltaTime;
        if(temp >= gapTime)
        {
            if(IsDisplay)
            {
                PointLight.intensity = Mathf.Lerp(10.0f, 20.0f, Time.deltaTime);
                IsDisplay = false;
                temp = 0;
            }
            else
            {
                PointLight.intensity = Mathf.Lerp(20.0f, 10.0f, Time.deltaTime);;
                IsDisplay = true;
                temp = 0;
            }
        }
    }
    public void Flicker()
    {
        PointLight.intensity = Mathf.Lerp(10.0f, 20.0f, temp);
        if(temp < 0.0f)
        {
            temp += Time.deltaTime * moveSpeed;
            Debug.Log("+");
            Debug.Log(Time.deltaTime);
        }
        else if(temp >= 1.0f)
        {
            temp -= Time.deltaTime * moveSpeed;
            Debug.Log("-");
        }

    }
}
