using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovingBall : MonoBehaviour
{
    [SerializeField]
    IK_tentacles _myOctopus;

    [SerializeField]
    private LineRenderer LR;
    [SerializeField]
    private LineRenderer LRM;

    //movement speed in units per second
    [Range(-1.0f, 1.0f)]
    [SerializeField]
    private float _movementSpeed = 5f;
    private float radius = 0.5f;
    private Rigidbody rb;
    public float strenght;
    public float magnusStrenght;
    public GameObject target;
    public Slider strenghtSlider;
    public Slider magnusSlider;
    public Vector3 direction;
    public Vector3 shoot;
    public float magnitude;
    private int i = 0;
    private int lineIndex =200;
    private float timer =0.1f;
    private Vector3 startPosition;

    Vector3 _dir;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (strenght > strenghtSlider.maxValue)
            {
                strenghtSlider.value = strenghtSlider.maxValue;
            }
            else

                strenght++;
            strenghtSlider.value = strenght;
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {

            Shoot();
        }
        else if (Input.GetKey(KeyCode.Z))
        {

            if (magnusStrenght < magnusSlider.minValue)
            {
                magnusSlider.value = magnusSlider.minValue;
            }
            else
                magnusStrenght -= 0.1f;
            magnusSlider.value = magnusStrenght;
        }
        else if (Input.GetKey(KeyCode.X))
        {
            if (magnusStrenght > magnusSlider.maxValue)
            {
                magnusSlider.value = magnusSlider.maxValue;
            }
            else
                magnusStrenght += 0.1f;
            magnusSlider.value = magnusStrenght;
        }
        else if (Input.GetKeyDown(KeyCode.I)) 
        {
            if (LR.enabled == true)
            {
                LR.enabled = false;
                LRM.enabled = false;
            }
            else 
            {
                LR.enabled = true;
                LRM.enabled = true;
            }
        }

        transform.rotation = Quaternion.identity;

        //get the Input from Horizontal axis
        float horizontalInput = Input.GetAxis("Horizontal");
        //get the Input from Vertical axis
        float verticalInput = Input.GetAxis("Vertical");


        //update the position
        //transform.position = transform.position + new Vector3(-horizontalInput * _movementSpeed * Time.deltaTime, verticalInput * _movementSpeed * Time.deltaTime, 0);
         shoot = (target.transform.position - this.transform.position).normalized;
        
         direction = Vector3.Cross(rb.angularVelocity, rb.velocity);
         magnitude = 2 * Mathf.PI * magnusStrenght * Mathf.Pow(radius, 2) * 0.5f;


        if (LR.enabled == true)
        {
            drawLine();
            drawLineMagnuss();
        }
        
        

    }

    void Shoot() 
    {

        rb.AddForce(strenght * shoot,ForceMode.Impulse);
        rb.AddForce(magnitude * Vector3.up,ForceMode.Force);
        StartCoroutine(Wait());
        

        
    }

    void ResetGauge() 
    {
        strenght = 0;
        strenghtSlider.value = 0;
    }

    IEnumerator Wait() 
    {
        yield return new WaitForSeconds(1.5f);
        ResetGauge();
        transform.position = startPosition;
    }

    private void OnCollisionEnter(Collision collision)
    {
        _myOctopus.NotifyShoot();
    }

    private void drawLineMagnuss()
    {
        i = 0;
        startPosition = transform.position;
        LRM.positionCount = lineIndex;
        LRM.SetPosition(i, startPosition);
        for (float j = 0; i < LRM.positionCount; j += timer) 
        {
            i++;
            Vector3 linePosition = startPosition +j*(shoot * strenght) + (Vector3.up *magnitude);
            //linePosition.y = startPosition.y + Vector3.up.y * magnitude;
            LRM.SetPosition(i, linePosition);
        }
    }

    private void drawLine()
    {
        i = 0;
        startPosition = transform.position;
        LR.positionCount = lineIndex;
        LR.SetPosition(i, startPosition);
        for (float j = 0; i < LR.positionCount; j += timer)
        {
            i++;
            Vector3 linePosition = startPosition +j*(shoot * strenght) ;
            //linePosition.y = startPosition.y + Vector3.up.y * magnitude;
            LR.SetPosition(i, linePosition);
        }
    }
}
