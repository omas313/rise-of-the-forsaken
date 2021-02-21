using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleConclusionPanel : MonoBehaviour
{
    [SerializeField] RandomTextLineStore _textStore;

    public IEnumerator Play()
    {
        GetComponentInChildren<TextMeshProUGUI>().SetText(_textStore.GetRandomLine());

        var animation = GetComponent<Animation>();
        animation.Play();

        yield return new WaitForSeconds(0.2f);
        yield return new WaitUntil(() => !animation.isPlaying);
    }
}
