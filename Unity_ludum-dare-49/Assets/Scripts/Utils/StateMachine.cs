using System;
using System.Collections.Generic;

namespace Utils
{
    public struct StateChange<T> where T : struct
    {
        public T newState;
        public T previousState;
    }

    public interface IStateMachine
    {
        bool TriggerEvents { get; set; }
    }

    public class StateMachine<T> : IStateMachine where T : struct
    {
        public Action<StateChange<T>> OnStateChange;
        public bool TriggerEvents { get; set; }
        public T CurrentState { get; protected set; }
        public T PreviousState { get; protected set; }

        public StateMachine()
        {
            //A basic StateMachine that holds no extra information
        }

        public StateMachine(bool triggerEvents = false)
        {
            TriggerEvents = triggerEvents;
        }

        public virtual void ChangeState(T newState)
        {
            // if the "new state" is the current one, we do nothing and exit
            if (EqualityComparer<T>.Default.Equals(newState, CurrentState)) return;

            // we store our previous state incase it is need for events or checking
            PreviousState = CurrentState;
            CurrentState = newState;

            if (TriggerEvents)
            {
                var stateChange = new StateChange<T>() { newState = CurrentState, previousState = PreviousState };
                OnStateChange?.Invoke(stateChange);
            }
        }

        public virtual void RestorePreviousState()
        {
            // we restore our previous state
            CurrentState = PreviousState;

            if (TriggerEvents)
            {
                var stateChange = new StateChange<T>() { newState = CurrentState, previousState = PreviousState };
                OnStateChange?.Invoke(stateChange);
            }
        }
    }
}