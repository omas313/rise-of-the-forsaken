using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameState { MainMenu, Battle, WorldMap,
        Final
    }
    public static GameManager Instance { get; private set; }
    public GameState CurrentGameState { get; private set; }

    [SerializeField] int _mainMenuBuildIndex = 0;
    [SerializeField] int _worldMapBuildIndex = 1;
    [SerializeField] int _finalSceneBuildIndex = 3;
    [SerializeField] int _finalBattleNumber = 8;

    [SerializeField] BattleResult _battleResult;

    BattleController _battleController;
    BattleDataDefinition _currentBattleDataDefinition;
    private bool _wonLastBattle;

    public void BattleComplete(BattleDataDefinition battleDataDefinition, bool won)
    {

        if (won && battleDataDefinition.Order == _finalBattleNumber)
        {
            PlayerPrefs.DeleteAll();
            LoadFinalScene();
            return;
        }

        if (won)
        {
            _battleResult.SetResult(battleDataDefinition, won);
            PlayerPrefs.SetInt("MAIN_BATTLES_COMPLETED_COUNT", battleDataDefinition.Order);
        }

        LoadWorldMap();
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

    void LoadFinalScene()
    {
        var operation = SceneManager.LoadSceneAsync(_finalSceneBuildIndex);
        operation.completed += op => CurrentGameState = GameState.Final;
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
