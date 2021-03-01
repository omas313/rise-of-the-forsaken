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
    bool _canSelect;

    public void StartChoosingTarget()
    {
        StartCoroutine(StartSelectionInSeconds(0.1f));
    }

    IEnumerator StartSelectionInSeconds(float delay)
    {
        yield return new WaitForSeconds(delay);
        _currentIndex = 0;
        SetCurrentPosition();
        _isControllingCursor = true;
        _canSelect = true;
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
        if (!_isControllingCursor || !_canSelect)    
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

        _canSelect = true;
    }

    void SetCurrentPosition()
    {
        var position = _activePositions[_currentIndex].position;
        position = _camera.WorldToScreenPoint(position) - new Vector3(60f, 0f, 0f);
        MenuCursor.Instance.PlaceAt(position);
    }
    
    void OnDestroy()
    {
        BattleEvents.BattleStarted -= OnBattleStarted;
        BattleEvents.EnemyDied -= OnEnemyDied;
    }

    void Start()
    {
        _camera = Camera.main;
        BattleEvents.BattleStarted += OnBattleStarted;
        BattleEvents.EnemyDied += OnEnemyDied;
    }
}
