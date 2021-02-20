using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyTargetPositionManager : MonoBehaviour
{
    [SerializeField] Transform[] _positions;
    [SerializeField] GameEvent _backEvent;

    Dictionary<Transform, Enemy> _enemyPositions;
    List<Transform> _activePositions;

    int _currentIndex;
    Camera _camera;
    bool _isControllingCursor;

    public void StartChoosingTarget()
    {
        _isControllingCursor = true;
        _currentIndex = 0;
        SetCurrentPosition();
    }

    void Start()
    {
        _camera = Camera.main;

        var battleController = FindObjectOfType<BattleController>();
        battleController.BattleStarted += OnBattleStarted;
        battleController.EnemyDied += OnEnemyDied;
    }

    void OnBattleStarted(List<PartyMember> playerParty, List<Enemy> enemies)
    {
        _enemyPositions = new Dictionary<Transform, Enemy>();
        _activePositions = new List<Transform>();

        for (var i = 0; i < enemies.Count; i++)
        {
            var position = _positions[i];
            _enemyPositions[position] = enemies[i];
            _activePositions.Add(position);
            enemies[i].transform.position = _activePositions[i].position;
        }

        _activePositions = _activePositions.OrderBy(p => p.name).ToList();

        SetEnemySortingOrders();
    }

    void SetEnemySortingOrders()
    {
        int i = 0;
        foreach (var position in _activePositions)
            _enemyPositions[position].GetComponentInChildren<SpriteRenderer>().sortingOrder = i++;
    }

    void OnEnemyDied(Enemy enemy)
    {
        Transform positionToRemove = null;

        foreach (var pair in _enemyPositions)
            if (pair.Value == enemy)
                positionToRemove = pair.Key;
        
        _activePositions.Remove(positionToRemove);
        _enemyPositions.Remove(positionToRemove);
    }

    void Update()
    {
        if (!_isControllingCursor)    
            return;

        if (Input.GetButtonDown("Up"))
            GoToPreviousPosition();
        else if (Input.GetButtonDown("Down"))
            GoToNextPosition();
        else if (Input.GetButtonDown("Confirm"))
            ConfirmCurrentSelection();    
        else if (Input.GetButtonDown("Back"))
            GoBack();
    }
    
    void GoBack()
    {
        if (_backEvent != null)
            _backEvent.Raise();
            
        MenuCursor.Instance.HideCursor();
        _isControllingCursor = false;
    }

    void GoToPreviousPosition()
    {
        _currentIndex = Mathf.Max(_currentIndex - 1, 0);        
        SetCurrentPosition();
    }

    void GoToNextPosition()
    {
        _currentIndex = Mathf.Min(_currentIndex + 1, _activePositions.Count - 1);              
        SetCurrentPosition();
    }

    void ConfirmCurrentSelection()
    {
        MenuCursor.Instance.HideCursor();
        _isControllingCursor = false;
        var selectedEnemy = _enemyPositions[_activePositions[_currentIndex]];
        BattleEvents.InvokeEnemyTargetSelected(selectedEnemy);
    }

    void SetCurrentPosition()
    {
        var position = _activePositions[_currentIndex].position;
        MenuCursor.Instance.PlaceAt(_camera.WorldToScreenPoint(position));
    }
}