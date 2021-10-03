using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class GameManager : SingletonBehaviour<GameManager>
{
    public static GameState State => Instance._gameState.CurrentState;
    private readonly StateMachine<GameState> _gameState = new StateMachine<GameState>(true);


    protected override void Initialize()
    {

    }

    protected override void Shutdown()
    {

    }

    public void Reset()
    {
        PrefabManager.Instance.Reset();
    }

    public enum GameState
    {
        Splash,
        Menu,
        Tutorial,
        Game,
        GameOver,
    }
}