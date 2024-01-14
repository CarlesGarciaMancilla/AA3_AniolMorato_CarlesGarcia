using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OctopusController;

public class IK_Scorpion : MonoBehaviour
{
    MyScorpionController _myController= new MyScorpionController();

    public IK_tentacles _myOctopus;

    [Header("Body")]
    float animTime;
    public float animDuration = 5;
    bool animPlaying = false;
    public Transform Body;
    public Transform StartPos;
    public Transform EndPos;

    [Header("Tail")]
    public Transform tailTarget;
    public Transform tail;

    [Header("Legs")]
    public Transform[] legs;
    public Transform[] legTargets;
    public Transform[] futureLegBases;

    [Header("Update Body Position")]
    public Transform[] joints;
    public Vector3 startingPosition;
    public float startingYPosition;
    public Vector3 temporaryPositions;
    public Vector3 updatedPosition;

    [Header("Update Body Rotation")]
    public int a;

    // Start is called before the first frame update
    void Start()
    {
        _myController.InitLegs(legs,futureLegBases,legTargets);
        _myController.InitTail(tail);

        startingYPosition = joints[0].transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if(animPlaying)
            animTime += Time.deltaTime;

        NotifyTailTarget();
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NotifyStartWalk();
            animTime = 0;
            animPlaying = true;
        }

        if (animTime < animDuration)
        {
            Body.position = Vector3.Lerp(StartPos.position, EndPos.position, animTime / animDuration);
            UpdateBodyPosition();
            //UpdateBodyRotation();
        }
        else if (animTime >= animDuration && animPlaying)
        {
            Body.position = EndPos.position;
            animPlaying = false;
        }

        _myController.UpdateIK();
    }
    
    //Function to send the tail target transform to the dll
    public void NotifyTailTarget()
    {
        _myController.NotifyTailTarget(tailTarget);
    }

    //Trigger Function to start the walk animation
    public void NotifyStartWalk()
    {

        _myController.NotifyStartWalk();
    }

    private void UpdateBodyPosition()
    {
        if (startingPosition.y != joints[0].transform.localPosition.y)
        {
            temporaryPositions.y = joints[0].transform.position.y - startingYPosition;
            startingPosition = joints[0].transform.localPosition;
        }
        updatedPosition = Body.localPosition;
        updatedPosition.y = temporaryPositions.y;
        Body.localPosition = updatedPosition;
    }

    private void UpdateBodyRotation()
    {

        //Initialize vectors for leg pos
        Vector3 rightLegs = new Vector3(0, 0, 0);
        Vector3 leftLegs = new Vector3(0, 0, 0);


        //Add legs to joints
        for (int i = 0; i < 6; i++)
        {
            //Right legs only
            if (i == 0 || i == 2 || i == 4)
            {
                rightLegs += joints[i].transform.position;
            }
            //Left legs only
            else if (i == 1 || i == 3 || i == 5)
            {
                leftLegs += joints[i].transform.position;
            }
        }

        if (rightLegs.y == leftLegs.y)
        {
            Body.transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, 0);
        }
        else if (leftLegs.y > rightLegs.y)
        {
            float legHeight = leftLegs.y - rightLegs.y;
            Body.transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, transform.localRotation.z + legHeight * 5);
        }
        else if (rightLegs.y > leftLegs.y)
        {
            float legHeight = rightLegs.y - leftLegs.y;
            Body.transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, transform.localRotation.z - legHeight * 5);
        }
    }
}
