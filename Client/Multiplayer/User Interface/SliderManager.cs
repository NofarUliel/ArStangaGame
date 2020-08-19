using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderManager : MonoBehaviour
{
    [SerializeField]
    private Slider slider;
    // Start is called before the first frame update
    void Start()
    {
        slider.value = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (slider.value < 1)
        {
            slider.value += 0.2f;
        }
        else
        {
            slider.value = 0;
        }

    }
}