using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
using System.Linq;
using System.Collections;

public class BattleController : MonoBehaviour
{
    public PartyMember CurrentActivePartyMember { get; private set; }

    // move this ui stuff out to battle ui handler
    [SerializeField] BattleConclusionPanel _victoryPanel;
    [SerializeField] BattleConclusionPanel _defeatPanel;
    [SerializeField] Animation _fadeInImage;

    // For debugging
    [SerializeField] bool _setParticipantsManually;
    [SerializeField] BattleDataDefinition _battleDataDefinition;

    List<Enemy> _enemies;
    List<PartyMember> _playerParty;
    List<BattleParticipant> _battleParticipants;
    List<PartyMember> _activePlayerParty;
    List<Enemy> _activeEnemies;
    BattleParticipant _currentParticipant;
    int _currentIndex;
    bool _hasBattleStarted;
    bool _hasBattleEnded;

    public bool IsCurrentActivePartyMember(PartyMember member) => CurrentActivePartyMember == member;

    public void InitBattle(BattleDataDefinition battleDataDefinition)
    {
        _battleDataDefinition = battleDataDefinition;

        FindObjectOfType<EnvironmentParticlesController>().SetEnvironmentParticles(battleDataDefinition);

        _playerParty = new List<PartyMember>();
        foreach (var prefab in battleDataDefinition.PlayerParty)
        {
            var partyMember = Instantiate(prefab, Vector3.zero, Quaternion.identity, GameObject.FindGameObjectWithTag("PlayerParty").transform);
            _playerParty.Add(partyMember);
        }

        _enemies = new List<Enemy>();
        foreach (var definition in battleDataDefinition.Enemies)
        {
            var enemy = Instantiate(definition.GameObjectprefab, Vector3.zero, Quaternion.identity, GameObject.FindGameObjectWithTag("Enemies").transform)
                .GetComponent<Enemy>();
            enemy.Initialize(definition);
            _enemies.Add(enemy);
        }
        
        StartBattle();
    }


    void StartBattle() => StartCoroutine(TurnBasedBattle());

    IEnumerator TurnBasedBattle()
    {
        _hasBattleStarted = true;

        yield return new WaitForSeconds(0.25f); // for UI to sub to events


        _activePlayerParty = new List<PartyMember>(_playerParty);
        _activeEnemies = new List<Enemy>(_enemies);

        BattleEvents.InvokePlayerPartyUpdated(_playerParty, null);
        InitBattleParticipants(_playerParty, _enemies);

        BattleEvents.InvokeBattleStarted(_playerParty, _enemies);

        _currentIndex = 0;

        // move out to ui
        var battleIntroAnimation = GameObject.FindWithTag("BattleIntro").GetComponent<Animation>();
        yield return new WaitUntil(() => !battleIntroAnimation.isPlaying);
        yield return new WaitForSeconds(0.5f);

        // move to ui
        if (_battleDataDefinition.HasPreturnDialogue)
            yield return PlayPreBattleDialogue();

        while (true)
        {
            // Debug.Log("battle loop");

            _currentParticipant = _battleParticipants[_currentIndex];

            if (_currentParticipant is PartyMember)
            {
                var partyMember = _currentParticipant as PartyMember;
                CurrentActivePartyMember = partyMember;   
                yield return partyMember.PreTurnAction(_activePlayerParty, _activeEnemies);
                BattleEvents.InvokePlayerPartyUpdated(_playerParty, partyMember);
            }
            else
                CurrentActivePartyMember = null;   

            yield return new WaitForSeconds(0.25f);
            yield return _battleParticipants[_currentIndex].PerformAction(_activePlayerParty, _activeEnemies);
            yield return new WaitForSeconds(0.25f);

             if (_currentParticipant is PartyMember)
                BattleEvents.InvokePlayerPartyUpdated(_playerParty, null);

            _currentIndex = (_currentIndex + 1) % _battleParticipants.Count;

            yield return CheckDeadParticipants();

            BattleEvents.InvokePlayerPartyUpdated(_playerParty, null);
            
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

        // Debug.Log("battle ended");
    }

    IEnumerator PlayPreBattleDialogue()
    {
        var textPlayer = FindObjectOfType<ChronologicalLinesPlayer>();
        textPlayer.Init(_battleDataDefinition.TextLines);
        yield return textPlayer.LinePlayer();
    }

    void InitBattleParticipants(List<PartyMember> playerParty, List<Enemy> enemies)
    {
        _battleParticipants = new List<BattleParticipant>();
        
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
                BattleEvents.InvokeEnemyDied(enemy);
            }
            else
            {
                var partyMember = deadParticipant as PartyMember;
                _activePlayerParty.Remove(partyMember);
                BattleEvents.InvokePartyMemberDied(partyMember);
            }
        }
    }

    bool AllEnemiesAreDead() => _activeEnemies.Count == 0;

    bool AllPartyMembersAreDead() => _activePlayerParty.Count == 0;

    IEnumerator BattleVictory()
    {
        if (!_hasBattleEnded)
        {
            BattleEvents.InvokeBattleEnded();
            _hasBattleEnded = true;
            yield return _victoryPanel.Play();

            _fadeInImage.Play();
            yield return new WaitForSeconds(0.2f); 
            yield return new WaitUntil(() => !_fadeInImage.isPlaying);

            GameManager.Instance.BattleComplete(_battleDataDefinition, won: true);
        }
    }

    IEnumerator BattleLoss()
    {
        if (!_hasBattleEnded)
        {
            BattleEvents.InvokeBattleEnded();

            _hasBattleEnded = true;

            yield return _defeatPanel.Play();

            _fadeInImage.Play();
            yield return new WaitForSeconds(0.2f); 
            yield return new WaitUntil(() => !_fadeInImage.isPlaying);

            GameManager.Instance.BattleComplete(_battleDataDefinition, won: false);
        }
    }

    void Update()
    {
        if (!_hasBattleStarted)
            Debug.Log("battle was not started");

        if (Input.GetKeyDown(KeyCode.PageUp))
            StartCoroutine(BattleVictory());
        else if (Input.GetKeyDown(KeyCode.PageDown))
            StartCoroutine(BattleLoss());
    }

    void Awake()
    {
        if (_setParticipantsManually && _battleDataDefinition != null)
        {
            GameManager.Instance.SetCurrentBattleData(this, _battleDataDefinition);
            _enemies = FindObjectsOfType<Enemy>().ToList();
            _playerParty = FindObjectsOfType<PartyMember>().ToList();
            InitBattle(_battleDataDefinition);
        }
    }
}
