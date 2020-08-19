using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountDownTimer 
{
    private float length;
    private float currentTime;
    private bool isCountDown;

    public CountDownTimer(float length=1, bool isCountDown = false)
    {
        this.currentTime = 0;
        this.length = length;
        this.isCountDown = isCountDown;
    }
    public void CountDownUpdate()
    {
        if(isCountDown){
            currentTime += Time.deltaTime;
            if (currentTime >= length)
            {
                currentTime = 0;
                isCountDown = false;
            }
        }
    }

    public bool IsCountDown() {
        return this.isCountDown;
    }

    public void StartCountDownTimer()
    {
        isCountDown = true;
        currentTime = 0;
    }
   
}
