using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderStrenght : MonoBehaviour
{

    public float strenght;
    public float timer = 1f;
    public Slider strenghtSlider;
    public bool up = true;
    public bool down = false;


    // Start is called before the first frame update
    void Start()
    {
        strenghtSlider.value = 0;
    }

    // Update is called once per frame
    void Update()
    {


        while (strenghtSlider.value < strenghtSlider.maxValue && up) 
        {
            strenghtSlider.value += 0.5f;           
        }

        while (strenghtSlider.value > strenghtSlider.minValue && down) 
        {
            strenghtSlider.value -= 0.5f;
        }

        if (strenghtSlider.value == strenghtSlider.maxValue) 
        {
         up = false;
         down = true;

        }
        else if (strenghtSlider.value == strenghtSlider.minValue)
        {
            up = true;
            down = false;

        }


    }
}
