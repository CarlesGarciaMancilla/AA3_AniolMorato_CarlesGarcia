using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderStrenght : MonoBehaviour
{

    public float strenght;
    public Slider strenghtSlider;



    // Start is called before the first frame update
    void Start()
    {
        strenghtSlider.value = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space)) 
        {
            strenght++;
            strenghtSlider.value = strenght;
        }

       


    }
}
