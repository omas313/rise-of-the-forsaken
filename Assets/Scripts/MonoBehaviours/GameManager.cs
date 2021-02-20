using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameState { MainMenu, Battle, WorldMap };

    public static GameManager Instance { get; private set; }
    public GameState CurrentGameState { get; private set; }


    [SerializeField] int _mainMenuBuildIndex = 0;
    [SerializeField] int _worldMapBuildIndex = 1;

    BattleController _battleController;
    BattleDataDefinition _currentBattleDataDefinition;

    public void LoadWorldMap()
    {
        var operation = SceneManager.LoadSceneAsync(_worldMapBuildIndex);
        operation.completed += op => CurrentGameState = GameState.WorldMap;
    }

    public void LoadMainMenu()
    {
        var operation = SceneManager.LoadSceneAsync(_mainMenuBuildIndex);
        operation.completed += op => CurrentGameState = GameState.MainMenu;
    }

    public void LoadBattleScene(BattleDataDefinition battleDataDefinition)
    {
        _currentBattleDataDefinition = battleDataDefinition;

        var operation = SceneManager.LoadSceneAsync("Battle");
        operation.completed += OnBattleSceneLoaded;
    }

    void OnBattleSceneLoaded(AsyncOperation operation)
    {
        CurrentGameState = GameState.Battle;
        _battleController = FindObjectOfType<BattleController>();
        _battleController.InitBattle(_currentBattleDataDefinition);
    }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);    

        DontDestroyOnLoad(gameObject);
    }
}
