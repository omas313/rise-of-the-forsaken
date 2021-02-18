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

    [SerializeField] TextMeshProUGUI _battleText;
    [SerializeField] List<PartyMember> _playerParty;
    [SerializeField] List<Enemy> _enemies;

    List<PartyMember> _activePlayerParty = new List<PartyMember>();
    List<Enemy> _activeEnemies = new List<Enemy>();
    List<BattleParticipant> _battleParticipants = new List<BattleParticipant>();
    BattleParticipant _currentParticipant;
    int _currentIndex;

    public void StartBattle(List<PartyMember> playerParty, List<Enemy> enemies)
    {
        StartCoroutine(TurnBasedBattle(playerParty, enemies));
    }

    void Start()
    {
        // Gaemmanager will call this?
        StartBattle(_playerParty, _enemies);
    }

    void Update()
    {
        SetBattleText();
    }

    IEnumerator TurnBasedBattle(List<PartyMember> playerParty, List<Enemy> enemies)
    {
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
                PlayerPartyUpdated?.Invoke(_playerParty, _currentParticipant as PartyMember);

            yield return new WaitForSeconds(0.25f);
            yield return _battleParticipants[_currentIndex].PerformAction(_activePlayerParty, _activeEnemies);
            yield return new WaitForSeconds(0.25f);

            _currentIndex = (_currentIndex + 1) % _battleParticipants.Count;

            yield return CheckDeadParticipants();

            PlayerPartyUpdated?.Invoke(_playerParty, null);
            
            if (AllEnemiesAreDead())
            {
                yield return BattleVictory();
                break;
            }
        }

        Debug.Log("battle ended");
    }

    void InitBattleParticipants(List<PartyMember> playerParty, List<Enemy> enemies)
    {
        // todo: get player party from somewhere
        // todo: who gives BattleManager the enemies? maybe battle data SO?
        _battleParticipants.Clear();
        
        foreach (var partyMemeber in _playerParty)
            _battleParticipants.Add(partyMemeber); 
        
        foreach (var enemy in _enemies)
            _battleParticipants.Add(enemy); 

        _battleParticipants = _battleParticipants.OrderByDescending(bp => bp.CharacterStats.CurrentSpeed).ToList();
    }

    bool AllEnemiesAreDead()
    {
        foreach (var participant in _battleParticipants)
            if (participant is Enemy)
                return false;

        return true;
    }

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

    void SetBattleText()
    {
        if (_battleParticipants == null)
            return;

        var stringBuilder = new StringBuilder();
        foreach (var participant in _battleParticipants)
        {
            var turn = _currentParticipant == participant ? " [turn]" : "";
            stringBuilder.AppendLine($"{participant.Name}: {participant.CharacterStats.CurrentHP} {turn}");
        }
            
        _battleText.SetText(stringBuilder.ToString());
    }

    [ContextMenu("kill all enemies")]
    public void CM_KillAllEnemies()
    {
        foreach (var enemy in _enemies)
            enemy.CharacterStats.CurrentHP = 0;
    }
}

