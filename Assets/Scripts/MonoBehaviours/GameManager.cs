using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameState { Battle, Map };

    public static GameManager Instance { get; private set; }

    BattleController _battleController;
    BattleDataDefinition _currentBattleDataDefinition;

    public void LoadBattleScene(BattleDataDefinition battleDataDefinition)
    {
        _currentBattleDataDefinition = battleDataDefinition;

        var operation = SceneManager.LoadSceneAsync("Battle");
        operation.completed += OnBattleSceneLoaded;
    }

    void OnBattleSceneLoaded(AsyncOperation operation)
    {
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
