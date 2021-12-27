using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStateMachineState {
	bool SwitchTo_Internal(IStateMachineState nextState, IStateMachineContext context);
	void OnSwitchedToMe_Internal(IStateMachineState prevState, IStateMachineContext context);
}

public interface IStateMachineContext {
	IStateMachineState GetState();
	void SetState(IStateMachineState state);
}

public interface IStateMachine<TContext, TState> where TContext : IStateMachineContext where TState : IStateMachineState {
	TContext GetContext();
	TState GetState();
	bool SwitchTo(TState nextState);
}