using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;


namespace OctopusController
{
  
    public class MyScorpionController
    {
        //TAIL
        Transform tailTarget;
        Transform tailEndEffector;
        MyTentacleController _tail;
        float animationRange = 3.46f;
        float growthRate = 150.0f;

        //LEGS
        Transform[] legTargets = new Transform[6];
        Transform[] legFutureBases = new Transform[6];
        MyTentacleController[] _legs = new MyTentacleController[6];
        bool playingAnimation = false;
        float futureBaseDistance = 1f;
        Vector3[] virtualLegs;
        float[] legDistanceCheck;

        #region public
        public void InitLegs(Transform[] LegRoots,Transform[] LegFutureBases, Transform[] LegTargets)
        {
            _legs = new MyTentacleController[LegRoots.Length];
            //Legs init
            for(int i = 0; i < LegRoots.Length; i++)
            {
                _legs[i] = new MyTentacleController();
                _legs[i].LoadTentacleJoints(LegRoots[i], TentacleMode.LEG);
                //Initialize anything needed for the FABRIK implementation
                legFutureBases[i] = LegFutureBases[i];
                legTargets[i] = LegTargets[i];
            }
            virtualLegs = new Vector3[_legs[0].Bones.Length];
            legDistanceCheck = new float[_legs[0].Bones.Length - 1];

        }

        public void InitTail(Transform TailBase)
        {
            
            _tail = new MyTentacleController();
            _tail.LoadTentacleJoints(TailBase, TentacleMode.TAIL);
            //Initialize anything needed for the Gradient Descent implementation
            tailEndEffector = _tail.Bones[_tail.Bones.Length - 1];
        }

        //Check when to start the animation towards target and implement Gradient Descent method to move the joints.
        public void NotifyTailTarget(Transform target)
        {
            tailTarget = target;
        }

        //Notifies the start of the walking animation
        public void NotifyStartWalk()
        {
            playingAnimation = true;

            Debug.Log("Starting Leg Animation");
        }

        //Create the apropiate animations and update the IK from the legs and tail
        public void UpdateIK()
        {
            updateTail();

            if(playingAnimation)
            {
                updateLegPos();

                if(Vector3.Distance(tailEndEffector.transform.position, tailTarget.transform.position) < animationRange)
                {
                    Debug.Log("Stopping Leg Animation");

                    playingAnimation = false;
                }
            }
        }
        #endregion


        #region private
        //TODO: Implement the leg base animations and logic
        private void updateLegPos()
        {
            //check for the distance to the futureBase, then if it's too far away start moving the leg towards the future base position
            for (int i = 0; i < 6; i++)
            {
                Debug.Log("Starting Leg Base Distance Check");



                if (Vector3.Distance(_legs[i].Bones[0].position, legFutureBases[i].position) > futureBaseDistance)
                {
                    Debug.Log($"Checking bone {i}");
                    _legs[i].Bones[0].position = Vector3.Lerp(_legs[i].Bones[0].position, legFutureBases[i].position, 1.4f);
                }
                updateLegs(i);
            }
        }

        private void updateTail()
        {
            if (tailEndEffector != null && tailTarget != null)
            {
                if (Vector3.Distance(tailEndEffector.transform.position, tailTarget.transform.position) < animationRange)
                {
                    for (int i = 0; i < _tail.Bones.Length - 1; i++)
                    {
                        float descent = CalculateGradient(_tail.Bones[i]);
                        _tail.Bones[i].transform.Rotate((Vector3.forward * -descent) * growthRate);
                    }
                }
            }
        }

        private float CalculateGradient(Transform joint)
        {
            float distanceEffectorTarget1 = Vector3.Distance(tailEndEffector.transform.position, tailTarget.transform.position);
            joint.transform.Rotate(Vector3.forward * 0.01f);
            float distanceEffectorTarget2 = Vector3.Distance(tailEndEffector.transform.position, tailTarget.transform.position);
            joint.transform.Rotate(Vector3.forward * -0.01f);
            float result = (distanceEffectorTarget2 - distanceEffectorTarget1) / 0.01f;
            return result;
        }

        //TODO: implement fabrik method to move legs 
        private void updateLegs(int legIndex)
        {
            Debug.Log($"Updating bone {legIndex}");

            // Save the position of the bones in copy
            for (int i = 0; i <= _legs[0].Bones.Length - 1; i++)
            {
                virtualLegs[i] = _legs[legIndex].Bones[i].position;
            }

            // Calculate the distance between the bones
            for (int i = 0; i <= _legs[legIndex].Bones.Length - 2; i++)
            {
                legDistanceCheck[i] = Vector3.Distance(_legs[legIndex].Bones[i].position, _legs[legIndex].Bones[i + 1].position);
            }

            float targetDistance = Vector3.Distance(virtualLegs[0], legTargets[legIndex].position);

            if (targetDistance < legDistanceCheck.Sum())
            {
                while (Vector3.Distance(virtualLegs[virtualLegs.Length - 1], legTargets[legIndex].position) != 0 || Vector3.Distance(virtualLegs[0], _legs[legIndex].Bones[0].position) != 0)
                {
                    virtualLegs[virtualLegs.Length - 1] = legTargets[legIndex].position;

                    for (int i = _legs[legIndex].Bones.Length - 2; i >= 0; i--)
                    {
                        Vector3 directonVector1 = (virtualLegs[i + 1] - virtualLegs[i]).normalized;
                        Vector3 movementVector1 = directonVector1 * legDistanceCheck[i];
                        virtualLegs[i] = virtualLegs[i + 1] - movementVector1;
                    }

                    virtualLegs[0] = _legs[legIndex].Bones[0].position;

                    for (int i = 1; i < _legs[legIndex].Bones.Length - 1; i++)
                    {
                        Vector3 directonVector2 = (virtualLegs[i - 1] - virtualLegs[i]).normalized;
                        Vector3 movementVector2 = directonVector2 * legDistanceCheck[i - 1];
                        virtualLegs[i] = virtualLegs[i - 1] - movementVector2;

                    }
                }

                // Update real legs
                for (int i = 0; i <= _legs[legIndex].Bones.Length - 2; i++)
                {
                    Vector3 directionVector = (virtualLegs[i + 1] - virtualLegs[i]).normalized;
                    Vector3 antMovement = (_legs[legIndex].Bones[i + 1].position - _legs[legIndex].Bones[i].position).normalized;
                    Quaternion legRotation = Quaternion.FromToRotation(antMovement, directionVector);


                    _legs[legIndex].Bones[i].rotation = legRotation * _legs[legIndex].Bones[i].rotation;
                }
            }
        }
        #endregion
    }
}
