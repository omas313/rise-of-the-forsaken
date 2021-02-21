using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] Animation _fadeInImageAnimation;
    [SerializeField] RectTransform _mainMenuPanel;
    [SerializeField] UIMenuItem _startGameItem;
    [SerializeField] UIMenuItem _loadGameItem;
    [SerializeField] MenuItemDefinition _startGameItemDefinition;
    [SerializeField] MenuItemDefinition _loadGameItemDefinition;

    [SerializeField] BattleResult _battleResult;
    [SerializeField] BattleDataStore _battleDataStore;
    
    List<UIMenuItem> _menuItems = new List<UIMenuItem>();
    CanvasGroup _canvasGroup;

    int _currentItemIndex;
    bool _hasRequestedStart;

    public void OnStartGameConfirmed()
    {
        Debug.Log("start");
        
        // StartCoroutine(ShowStory());
    }

    public void OnLoadGameConfirmed()
    {

        // StartCoroutine(StartGame());
        Debug.Log("load");
    }

    public void OnMainMenuAnimationFinished()
    {
        GoToPreviousItem();
    }

    void Update()
    {
        if (_hasRequestedStart)
            return;

        if (Input.GetButtonDown("Up"))
            GoToPreviousItem();
        else if (Input.GetButtonDown("Down"))
            GoToNextItem();
        else if (Input.GetButtonDown("Confirm"))
            ConfirmCurrentSelection();
    }

    void GoToNextItem()
    {
        _currentItemIndex = Mathf.Min(_currentItemIndex + 1, _menuItems.Count - 1);
        MenuCursor.Instance.PlaceAt(_menuItems[_currentItemIndex].CursorPosition);
    }

    void GoToPreviousItem()
    {
        _currentItemIndex = Mathf.Max(_currentItemIndex - 1, 0);
        MenuCursor.Instance.PlaceAt(_menuItems[_currentItemIndex].CursorPosition);
    }

    private void ConfirmCurrentSelection()
    {
        _hasRequestedStart = true;
        _menuItems[_currentItemIndex].PerformAction();
    }

    IEnumerator StartGame()
    {
        _fadeInImageAnimation.Play();
        yield return new WaitForSeconds(0.2f);
        yield return new WaitUntil(() => !_fadeInImageAnimation.isPlaying);
        GameManager.Instance.LoadWorldMap();
    }

    private void Awake()
    {
        _startGameItem.gameObject.SetActive(true);
        _startGameItem.Init(_startGameItemDefinition);
        _menuItems.Add(_startGameItem);

        if (HasSaveGame())
        {
            _loadGameItem.gameObject.SetActive(true);
            _loadGameItem.Init(_loadGameItemDefinition);
            _mainMenuPanel.sizeDelta = new Vector2(_mainMenuPanel.sizeDelta.x, _mainMenuPanel.sizeDelta.y + 90f);
            _menuItems.Add(_loadGameItem);

            _battleResult.SetResult(_battleDataStore.GetBattle(PlayerPrefs.GetInt("MAIN_BATTLES_COMPLETED_COUNT")), won: true);
        }
        else
            _loadGameItem.gameObject.SetActive(false);        
    }

    private bool HasSaveGame() => PlayerPrefs.GetInt("MAIN_BATTLES_COMPLETED_COUNT") != 0;

    [ContextMenu("set pp")]
    public void SetPPInt() => PlayerPrefs.SetInt("MAIN_BATTLES_COMPLETED_COUNT", 1);

    [ContextMenu("clear pp")]
    public void ClearPPInt() => PlayerPrefs.DeleteAll();
}
