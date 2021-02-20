using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
using System.Linq;
using System.Collections;

public class BattleController : MonoBehaviour
{
    public event Action<List<PartyMember>, List<Enemy>> BattleStarted;
    public event Action<List<PartyMember>, PartyMember> PlayerPartyUpdated;
    public event Action<Enemy> EnemyDied;
    public event Action<PartyMember> PartyMemberDied;

    public PartyMember CurrentActivePartyMember { get; private set; }

    [SerializeField] bool _setParticipantsManually;

    List<Enemy> _enemies;
    List<PartyMember> _playerParty;
    List<BattleParticipant> _battleParticipants = new List<BattleParticipant>();
    List<PartyMember> _activePlayerParty = new List<PartyMember>();
    List<Enemy> _activeEnemies = new List<Enemy>();
    BattleParticipant _currentParticipant;
    int _currentIndex;
    bool _hasBattleStarted;

    public void InitBattle(List<PartyMember> playerParty, List<EnemyDefinition> enemiesDefinitions)
    {
        // create enemies

        // StartBattle();
    }

    public bool IsCurrentActivePartyMember(PartyMember member) => CurrentActivePartyMember == member;

    void StartBattle(List<PartyMember> playerParty, List<Enemy> enemies) => StartCoroutine(TurnBasedBattle(playerParty, enemies));

    IEnumerator TurnBasedBattle(List<PartyMember> playerParty, List<Enemy> enemies)
    {
        _hasBattleStarted = true;

        yield return new WaitForSeconds(0.25f); // for UI to sub to events

        _playerParty = playerParty;
        _enemies = enemies;

        _activePlayerParty = new List<PartyMember>(_playerParty);
        _activeEnemies = new List<Enemy>(_enemies);

        PlayerPartyUpdated?.Invoke(_playerParty, null);
        InitBattleParticipants(_playerParty, _enemies);
        BattleStarted?.Invoke(_playerParty, _enemies);

        _currentIndex = 0;

        while (true)
        {
            Debug.Log("battle loop");

            _currentParticipant = _battleParticipants[_currentIndex];

            if (_currentParticipant is PartyMember)
            {
                var partyMember = _currentParticipant as PartyMember;
                CurrentActivePartyMember = partyMember;   
                yield return partyMember.PreTurnAction(_activePlayerParty, _activeEnemies);
                PlayerPartyUpdated?.Invoke(_playerParty, partyMember);
            }
            else
                CurrentActivePartyMember = null;   

            yield return new WaitForSeconds(0.25f);
            yield return _battleParticipants[_currentIndex].PerformAction(_activePlayerParty, _activeEnemies);
            yield return new WaitForSeconds(0.25f);

             if (_currentParticipant is PartyMember)
                PlayerPartyUpdated?.Invoke(_playerParty, null);

            _currentIndex = (_currentIndex + 1) % _battleParticipants.Count;

            yield return CheckDeadParticipants();

            PlayerPartyUpdated?.Invoke(_playerParty, null);
            
            if (AllEnemiesAreDead())
            {
                yield return BattleVictory();
                break;
            }

            if (AllPartyMembersAreDead())
            {
                yield return BattleLoss();
                break;
            }
        }

        Debug.Log("battle ended");
    }

    void InitBattleParticipants(List<PartyMember> playerParty, List<Enemy> enemies)
    {
        _battleParticipants.Clear();
        
        foreach (var partyMember in _playerParty)
            _battleParticipants.Add(partyMember); 
        
        foreach (var enemy in _enemies)
            _battleParticipants.Add(enemy); 

        _battleParticipants = _battleParticipants.OrderByDescending(bp => bp.CharacterStats.CurrentSpeed).ToList();
    }

    IEnumerator CheckDeadParticipants()
    {
        var deadParticipants = new List<BattleParticipant>();
        foreach (var participant in _battleParticipants)
            if (participant.IsDead)
                deadParticipants.Add(participant);

        if (deadParticipants.Count != 0)
        {
            var nextParticipant = _battleParticipants[_currentIndex];
            while (nextParticipant.IsDead)
            {
                _currentIndex = (_currentIndex + 1) % _battleParticipants.Count;
                nextParticipant = _battleParticipants[_currentIndex];
            }

            yield return KillAndRemoveParticipants(deadParticipants);

            _currentIndex = _battleParticipants.IndexOf(nextParticipant);
        }
    }

    IEnumerator KillAndRemoveParticipants(List<BattleParticipant> deadParticipants)
    {
        foreach (var deadParticipant in deadParticipants)
        {
            yield return deadParticipant.Die();
            _battleParticipants.Remove(deadParticipant);

            if (deadParticipant is Enemy)
            {
                var enemy = deadParticipant as Enemy;
                _activeEnemies.Remove(enemy);
                EnemyDied?.Invoke(enemy);
            }
            else
            {
                var partyMember = deadParticipant as PartyMember;
                _activePlayerParty.Remove(partyMember);
                PartyMemberDied?.Invoke(partyMember);
            }
        }
    }

    bool AllEnemiesAreDead() => _activeEnemies.Count == 0;
    // {
    //     foreach (var participant in _battleParticipants)
    //         if (participant is Enemy)
    //             return false;

    //     return true;
    // }

    bool AllPartyMembersAreDead() => _activePlayerParty.Count == 0;
    // {
    //     foreach (var participant in _battleParticipants)
    //         if (participant is PartyMember)
    //             return false;

    //     return true;
    // }

    IEnumerator BattleVictory()
    {
        while (true)
        {
            Debug.Log($"victory");
            yield return null;

            if (Input.GetKeyDown(KeyCode.Space))
                break;
        }
    }

    IEnumerator BattleLoss()
    {
        while (true)
        {
            Debug.Log($"lost battle");
            yield return null;

            if (Input.GetKeyDown(KeyCode.Space))
                break;
        }
    }

    void Update()
    {
        if (!_hasBattleStarted)
            Debug.Log("battle was not started");
    }

    void Awake()
    {
        if (_setParticipantsManually)
        {
            GetParticipants();
            StartBattle(_playerParty, _enemies);
        }
    }

    void GetParticipants()
    {
        _enemies = FindObjectsOfType<Enemy>().ToList();
        _playerParty = FindObjectsOfType<PartyMember>().ToList();
    }
}
