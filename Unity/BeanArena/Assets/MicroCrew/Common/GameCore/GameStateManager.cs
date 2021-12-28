using MicroCrew.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : Singleton<GameStateManager>, IStateMachine<GameContext, GameState> {

	public static GameState current {
		get {
			return (GameState)inst.m_Context.GetState();
		}
	}

	private GameContext m_Context;

	public override void Init() {
		Game.InitIfNeeded(null);

		m_Context = new GameContext() {
			game = Game.inst
		};

		this.Wait(() => {
			GameScene scene = MSceneManager.currentScene;

			if (scene.name == "menu") {
				SwitchTo(new GameState_Menu());
			} else {
				SwitchTo(new GameState_Gameplay());
			}
		}, 1);
	}

	protected override void Shutdown() {

	}

	public bool SwitchTo(GameState nextState) {
		return m_Context.GetState().SwitchTo_Internal(nextState, m_Context);
	}

	public GameContext GetContext() {
		return m_Context;
	}

	public GameState GetState() {
		return (GameState)m_Context.GetState();
	}

}

public class GameContext : IStateMachineContext {

	public Game game;

	private GameState m_State;

	public GameContext() {
		m_State = new GameState_None();
	}

	public IStateMachineState GetState() {
		return m_State;
	}

	public void SetState(IStateMachineState state) {
		m_State = (GameState)state;
	}
}

public abstract class GameState : IStateMachineState {
	public GameStateType type;

	protected abstract bool SwitchTo(GameState nextState, GameContext gameContext);
	protected abstract void OnSwitchedToMe(GameState prevState, GameContext context);

	public bool SwitchTo_Internal(IStateMachineState nextState, IStateMachineContext context) {
		GameState next = (GameState)nextState;

		if (type == next.type) {
			Debug.Log("Not switching to the same state: " + type);
			return false;
		} else {
			Debug.Log(type + " is switching to " + next.type);
			bool switchSuccess = SwitchTo(next, (GameContext)context);
			if (switchSuccess) {
				next.OnSwitchedToMe_Internal(this, context);
				return true;
			} else {
				return false;
			}
		}
	}

	public void OnSwitchedToMe_Internal(IStateMachineState prevState, IStateMachineContext context) {
		GameState prev = (GameState)prevState;
		Debug.Log(type + " is game state now!");
		OnSwitchedToMe(prev, (GameContext)context);
	}

	public GameState(GameStateType _type) {
		type = _type;
	}
}

public enum GameStateType {
	None,
	Menu,
	Tutorial,
	Gameplay
}

public class GameState_None : GameState {

	public GameState_None() : base(GameStateType.None) {

	}

	protected override void OnSwitchedToMe(GameState prevState, GameContext context) {

	}

	protected override bool SwitchTo(GameState state, GameContext context) {

		switch (state.type) {
			case GameStateType.Menu:
				context.SetState(state);
				return true;
			case GameStateType.Gameplay:
				context.SetState(state);
				return true;
		}

		return false;
	}

}

public class GameState_Menu : GameState {

	public GameState_Menu() : base(GameStateType.Menu) {

	}

	protected override void OnSwitchedToMe(GameState prevState, GameContext context) {
		Debug.Log("OnSwitchedToMe: menu");		
	}

	protected override bool SwitchTo(GameState state, GameContext context) {
		switch (state.type) {
			case GameStateType.Menu:
				return false;
			case GameStateType.Gameplay:
				context.SetState(state);
				return true;
		}

		return false;
	}

}

public class GameState_Gameplay : GameState {

	public GameState_Gameplay() : base(GameStateType.Gameplay) {

	}

	protected override void OnSwitchedToMe(GameState prevState, GameContext context) {
		context.game.SetupGame();
	}

	protected override bool SwitchTo(GameState state, GameContext context) {
		switch (state.type) {
			case GameStateType.Menu:
				context.SetState(state);
				return true;
			case GameStateType.Gameplay:
				return false;
		}

		return false;
	}

}
