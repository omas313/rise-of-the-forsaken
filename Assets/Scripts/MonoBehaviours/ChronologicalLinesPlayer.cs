using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChronologicalLinesPlayer : MonoBehaviour
{
    [SerializeField] ChronologicalTextLines _linesDefinition;
    [SerializeField] float _delayBetweenLines = 2f;
    [SerializeField] GameEvent _storyFinished;
    [SerializeField] GameObject _cursor;

    TextMeshProUGUI _text;
    string[] _lines;

    public void Init(ChronologicalTextLines textLines)
    {
        _linesDefinition = textLines;

        _text = GetComponentInChildren<TextMeshProUGUI>();
        _lines = textLines.TextLines;
        _cursor.SetActive(false);
    }

    public void PlayLines()
    {
        StartCoroutine(LinePlayer());
    }

    public IEnumerator LinePlayer()
    {
        var canvas = GetComponent<CanvasGroup>();

        while (canvas.alpha < 1f)
        {
            canvas.alpha += Time.deltaTime * 2f;
            yield return null;
        }

        var index = 0;
        _text.SetText("");

        yield return new WaitForSeconds(_delayBetweenLines * 0.5f);

        while (index < _lines.Length)
        {
            var text = _text.text + "\n\n" + _lines[index++];
            _text.SetText(text.Trim());
            yield return new WaitForSeconds(_delayBetweenLines);
        }

        _cursor.SetActive(true);

        while (true)
        {
            if (Input.GetButtonDown("Confirm"))
                break;

            yield return null;
        }

        _cursor.SetActive(false);
        _storyFinished.Raise();


        while (canvas.alpha > 0f)
        {
            canvas.alpha -= Time.deltaTime * 2f;
            yield return null;
        }
    }

    void Awake()
    {
        if (_linesDefinition != null)
            Init(_linesDefinition);
            
    }
}
