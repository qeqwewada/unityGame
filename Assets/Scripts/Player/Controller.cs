using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public CharacterController characterController;
    public ModelBase player;
    public CinemachineVirtualCamera virtualCamera;
    public PlayerBaseState playerBase;
    IdleState idleState;
    public enum State
    {
        Walk,
        Run,
        Turn,
        Idle,
        Evade,
    }

    public State currentState = State.Idle;
    void Start()
    {
         idleState = new IdleState() { controller = this};
    }

    // Update is called once per frame
    void Update()
    {
        idleState.Update();
        switchPlayerState();
        
    }

    void switchPlayerState()
    {
        switch (currentState)
        {
            case State.Idle:
                player.Animator.CrossFade("Idle", 0.1f);
                break;
            case State.Walk:
                player.Animator.CrossFade("Move",0.1f);
                break;
            case State.Evade:
                player.Animator.CrossFade("Evade",0.1f);
                break;
        }
        
    }

    void checkGrounded()
    {

    }
}
