﻿using System.Collections;
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
            updateBodyPosition();
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

    private void updateBodyPosition()
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
}
