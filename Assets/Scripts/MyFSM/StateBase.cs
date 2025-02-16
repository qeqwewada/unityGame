using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyFSM
{
    public abstract class StateBase :IState
    {
        protected Player kalie;

        public StateBase(Player kalie)
        {
            this.kalie = kalie;
        }

        public abstract void Enter();

        public abstract void Update();
        public abstract void Exit();

        
      
    }
}
