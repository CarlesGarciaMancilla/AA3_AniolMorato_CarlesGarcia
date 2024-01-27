using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;




namespace OctopusController
{


    internal class MyTentacleController

    //MAINTAIN THIS CLASS AS INTERNAL
    {

        TentacleMode tentacleMode;
        Transform[] _bones;
        Transform[] _endEffectorSphere;

        public Transform[] Bones { get => _bones; }
        public Transform[] EndEffector { get => _endEffectorSphere; }

        //Exercise 1.
        public Transform[] LoadTentacleJoints(Transform root, TentacleMode mode)
        {

            Transform _bonesDupe = root;
            List<Transform> joints = new List<Transform>();
            //TODO: add here whatever is needed to find the bones forming the tentacle for all modes
            //you may want to use a list, and then convert it to an array and save it into _bones
            tentacleMode = mode;

            switch (tentacleMode)
            {
                case TentacleMode.LEG:
                    //TODO: in _endEffectorsphere you keep a reference to the base of the leg
                    _bonesDupe = root.GetChild(0);
                    joints.Add(_bonesDupe);
                    do
                    {
                        _bonesDupe = _bonesDupe.GetChild(1);
                        joints.Add(_bonesDupe);
                    } while (_bonesDupe.name != "Joint2");

                    _endEffectorSphere = new Transform[1];
                    _endEffectorSphere[0] = _bonesDupe;
                    _bonesDupe = _bonesDupe.GetChild(1);
                    joints.Add(_bonesDupe);
                    break;
                case TentacleMode.TAIL:
                    //TODO: in _endEffectorsphere you keep a reference to the red sphere 
                    joints.Add(_bonesDupe);
                    do
                    {
                        _bonesDupe = _bonesDupe.GetChild(1);
                        joints.Add(_bonesDupe);

                    } while (_bonesDupe.name != "EndEffector");

                    _endEffectorSphere = new Transform[1];
                    _endEffectorSphere[0] = _bonesDupe;
                    break;
                case TentacleMode.TENTACLE:
                    //TODO: in _endEffectorphere you  keep a reference to the sphere with a collider attached to the endEffector
                    _bonesDupe = _bonesDupe.GetChild(0);
                    _bonesDupe = _bonesDupe.GetChild(0);

                    do
                    {
                        _bonesDupe = _bonesDupe.GetChild(0);
                        joints.Add(_bonesDupe);

                    } while (_bonesDupe.name != "Bone.001_end");
                    //gameobject

                    _endEffectorSphere = new Transform[1];
                    _endEffectorSphere[0] = _bonesDupe;
                    break;
            }
            _bones = joints.ToArray();
            joints.Clear();
            return Bones;
        }


    }
}