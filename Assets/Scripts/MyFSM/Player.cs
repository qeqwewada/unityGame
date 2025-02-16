using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
 namespace MyFSM{
    [Serializable]
    public class PlayerParameter : Parameter
    {
        public float walkSpeed=1f;
        public Animator animator;
        public CharacterController characterController;
        public float rotateSpeed=5;
        public float gravity=-9.8f;
    }
    public class Player : MonoBehaviour
    {
        public FSM fSM;
        public PlayerParameter playerParameter;
        void Start()
        {
            fSM = new FSM(playerParameter);
            fSM.AddState(StateType.Idle, new IdleState(this));
            fSM.AddState(StateType.walk, new MoveState(this));

            fSM.ChangeState(StateType.Idle);
        }

        // Update is called once per frame
        void Update()
        {
            fSM.Update();
        }
    }
   
}

