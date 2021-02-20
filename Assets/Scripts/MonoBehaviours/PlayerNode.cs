using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNode : MonoBehaviour
{
    const string ISMOVING_BOOL = "IsMoving";

    public bool HasDestination => _currentDestination != null;
    public bool HasCurrentNode => _currentNode != null;

    [SerializeField] Animation _fadeInAnimation;
    [SerializeField] GameEvent _loadBattleRequestedEvent;
    [SerializeField] GameEvent _pressedSpaceToAdvanceEvent;
    [SerializeField] GameEvent _idleFor10SecondsEvent;

    [SerializeField] float _moveSpeed = 1f;
    [SerializeField] WorldMapNode _startingNode;

    WorldMapNode _currentNode;
    WorldMapNode _currentDestination;
    Animator _animator;
    bool _shouldMove;
    bool _isIdle = true;
    bool _isLoadingLevel;
    private float _idleTimer;

    // let game manager call this with the first or saved node
    public void EnterIdleAtNode(WorldMapNode node)
    {
        _currentNode = node;
        transform.position = node.transform.position;
        _isIdle = true;
    }

    public void MoveToDestination(WorldMapNode node)
    {
        if (node == null)
        {
            Debug.Log("Game over");
        }

        _shouldMove = true;
        _currentNode = null;
        _currentDestination = node;
    }

    public void Stop()
    {
        _shouldMove = false;
        _isIdle = true;
        _animator.SetBool(ISMOVING_BOOL, false);
    }

    void Update()
    {
        if (_isLoadingLevel)
            return;

        if (_isIdle)
        {
            HandleIdle();
            return;
        }

        if (_shouldMove && HasDestination)
            Move();        
    }

    void HandleIdle()
    {
        _idleTimer += Time.deltaTime;

        if (_idleTimer > 10f)
        {
            _idleTimer = 0f;
            _idleFor10SecondsEvent.Raise();
        }

        if (Input.GetButtonDown("Confirm"))
        {
            _idleTimer = 0f;
            _isIdle = false;

            _pressedSpaceToAdvanceEvent.Raise();

            if (HasDestination)
                MoveToDestination(_currentDestination);
            else if (HasCurrentNode)
                MoveToDestination(_currentNode.NextNode);
        }
    }

    void Move()
    {
        _animator.SetBool(ISMOVING_BOOL, true);
        transform.position = Vector2.MoveTowards(transform.position, _currentDestination.transform.position, _moveSpeed * Time.deltaTime);

        if (HasReachedDestination())
            StopAtDestination();
    }

    void StopAtDestination()
    {
        transform.position = _currentDestination.transform.position;
        _currentNode = _currentDestination;
        _currentDestination = null;
        Stop();

        StartCoroutine(LoadNodeBattle());
    }

    IEnumerator LoadNodeBattle()
    {
        _isLoadingLevel = true;

        _loadBattleRequestedEvent.Raise(); // not using for now...
        _fadeInAnimation.Play(); // here for now.....
        yield return new WaitForSeconds(1f);

        GameManager.Instance.LoadBattleScene(_currentNode.BattleDefinition);
    }

    bool HasReachedDestination() => Vector2.Distance(_currentDestination.transform.position, transform.position) < 0.1f;

    void Start()
    {
        if (_startingNode != null)
            EnterIdleAtNode(_startingNode);    
    }
    void Awake()
    {
        _animator = GetComponent<Animator>();
    }
}
