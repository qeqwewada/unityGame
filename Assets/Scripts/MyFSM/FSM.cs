using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public enum StateType
{
    Idle,
    walk,
}

public interface IState
{
    
    public void Enter();
    public void Update();
    public void Exit();
}

[Serializable]
public class Parameter {


}

public class FSM 
{
    public IState currentState;
    public Dictionary<StateType, IState> stateDict=new Dictionary<StateType, IState>();
    public Parameter parameter;
    public FSM(Parameter parameter)
    {
        this.parameter = parameter;
    }
    public void AddState(StateType stateType,IState state)
    {
        if (stateDict.ContainsKey(stateType))
        {
            Debug.Log(stateType + "is contained");
            return;
        }
        stateDict.Add(stateType, state);
    }

    public void ChangeState(StateType state)
    {
        if (!stateDict.ContainsKey(state))
        {
            Debug.Log(state + "not found");
            return;
        }
        if (currentState != null)
        {
            currentState.Exit();
        }
        
        if (stateDict.ContainsKey(state))
        {
            currentState = stateDict[state];
            currentState.Enter();
        }
    }
    public void Update()
    {
        if (currentState != null)
        {
            currentState.Update();
        }
        else
        {
            Debug.Log(currentState + "=null");
        }
       
    }
}
