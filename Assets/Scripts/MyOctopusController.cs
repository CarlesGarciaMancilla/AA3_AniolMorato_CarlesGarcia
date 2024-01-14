using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


namespace OctopusController
{
    public enum TentacleMode { LEG, TAIL, TENTACLE };

    public class MyOctopusController
    {

        MyTentacleController[] _tentacles = new MyTentacleController[4];

        Transform _currentRegion;
        Transform _target;


        Transform[] _randomTargets;// = new Transform[4];



        Vector3 targetPosition;
        Vector3 targetRegion;
        float sqrDistance;
        bool random = false;
        float time = 3f;

        float _twistMin, _twistMax;
        float _swingMin, _swingMax;

        #region public methods
        //DO NOT CHANGE THE PUBLIC METHODS!!

        public float TwistMin { set => _twistMin = value; }
        public float TwistMax { set => _twistMax = value; }
        public float SwingMin { set => _swingMin = 0; }
        public float SwingMax { set => _swingMax = 5; }


        public void TestLogging(string objectName)
        {


            UnityEngine.Debug.Log("hello, I am initializing my Octopus Controller in object " + objectName);


        }

        public void Init(Transform[] tentacleRoots, Transform[] randomTargets)
        {
            _tentacles = new MyTentacleController[tentacleRoots.Length];


            // foreach (Transform t in tentacleRoots)
            for (int i = 0; i < tentacleRoots.Length; i++)
            {

                _tentacles[i] = new MyTentacleController();
                _tentacles[i].LoadTentacleJoints(tentacleRoots[i], TentacleMode.TENTACLE);
                //TODO: initialize any variables needed in ccd
            }

            _randomTargets = randomTargets;
            //TODO: use the regions however you need to make sure each tentacle stays in its region
            for (int i = 0; i < tentacleRoots.Length; i++)
            {
                // _tentacles[i].Bones[1].position
            }


        }


        public void NotifyTarget(Transform target, Transform region)
        {
            _currentRegion = region;
            _target = target;
            targetPosition = new Vector3(target.position.x, target.position.y, target.position.z);
            targetRegion = new Vector3(region.position.x, region.position.y, region.position.z);
            UnityEngine.Debug.Log("notify target");

        }

        public void NotifyShoot()
        {
            //TODO. what happens here?
            random = true;

            UnityEngine.Debug.Log("Shoot");
        }


        public void UpdateTentacles()
        {
            //TODO: implement logic for the correct tentacle arm to stop the ball and implement CCD method

            if (random)
            {
                for (int i = 0; i < _tentacles.Length; i++)
                {
                    NotifyTarget(_target, _randomTargets[i].parent);
                    NotifyTarget(_randomTargets[i], _randomTargets[i].parent);
                }

                if (time - Time.deltaTime <= 0)
                {
                    random = false;
                    time = 3;

                }

            }
            else
            {
                for (int i = 0; i < _tentacles.Length; i++)
                {

                    NotifyTarget(_randomTargets[i], _randomTargets[i].parent);
                }
            }




            update_ccd();
        }




        #endregion


        #region private and internal methods
        //todo: add here anything that you need

        public static void RotateJoint(Transform bone, float angle, Vector3 axis)
        {


            bone.transform.Rotate(axis, angle);


        }

        void update_ccd()
        {

            for (int i = 0; i < _tentacles.Length; i++)
            {
                MyTentacleController tentacle = _tentacles[i];
                Vector3 currentPosition = tentacle.GetEffector.position;
                Vector3 targetDirection = _target.position - currentPosition;




                for (int j = tentacle.Bones.Length - 1; j >= 0; j--)
                {

                   

                    float currentSwing = _tentacles[i].GetSwingAngle(j);
                    float clampedSwing = Mathf.Clamp(currentSwing, _swingMin, _swingMax);
                    _tentacles[i].SetSwingAngle(clampedSwing, j);

                    //Vector3 jointPosition = tentacle.Bones[j].position;
                    //Vector3 jointToTarget = _target.position - jointPosition;
                    //Vector3 axis = Vector3.Cross(targetDirection, Vector3.up).normalized;

                    //float angle = Vector3.Angle(jointToTarget, targetDirection);


                    //RotateJoint(tentacle.Bones[j], angle, axis);
                }
            }
        }

        #endregion

    }
}
