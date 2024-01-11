using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovingBall : MonoBehaviour
{
    [SerializeField]
    IK_tentacles _myOctopus;

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

    Vector3 _dir;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            strenght++;
            strenghtSlider.value = strenght;
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {

            Shoot();
        }
        else if (Input.GetKey(KeyCode.Z)) 
        {
            magnusStrenght -=0.1f;
            magnusSlider.value = magnusStrenght;
        }
        else if (Input.GetKey(KeyCode.X))
        {
            magnusStrenght += 0.1f;
            magnusSlider.value = magnusStrenght;
        }

        transform.rotation = Quaternion.identity;

        //get the Input from Horizontal axis
        float horizontalInput = Input.GetAxis("Horizontal");
        //get the Input from Vertical axis
        float verticalInput = Input.GetAxis("Vertical");

        
        //update the position
        //transform.position = transform.position + new Vector3(-horizontalInput * _movementSpeed * Time.deltaTime, verticalInput * _movementSpeed * Time.deltaTime, 0);

    }

    void Shoot() 
    {

        Vector3 shoot = (target.transform.position - this.transform.position).normalized;
        rb.AddForce(strenght * shoot, ForceMode.Impulse);
        var direction = Vector3.Cross(rb.angularVelocity, rb.velocity) ;
        var magnitude = 4 / 3f * Mathf.PI * magnusStrenght * Mathf.Pow(radius, 3);
        rb.AddForce(magnitude * direction);


        
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
    }

    private void OnCollisionEnter(Collision collision)
    {
        _myOctopus.NotifyShoot();
    }
}
