using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MGameEvent<T> {
    static event System.Action<T> onEvent;

    public static void Register(System.Action<T> listener) {
        onEvent += listener;
    }

    public static void Unregister(System.Action<T> listener) {
        onEvent -= listener;
    }

    public static void Invoke(T eventData) {
        onEvent?.Invoke(eventData);
    }
}

public class GameStartEvent : MGameEvent<GameStartEvent> {

    public GameMode gameMode;

    public GameStartEvent(GameMode _gameMode) {
        gameMode = _gameMode;
    }

    public enum GameMode {
        PVP,
        PVE,
        Sandbox
    }
}