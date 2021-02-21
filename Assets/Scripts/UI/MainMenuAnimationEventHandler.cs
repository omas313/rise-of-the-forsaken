using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuAnimationEventHandler : MonoBehaviour
{
    [SerializeField] GameEvent _mainMenuAnimationFinished;

    public void InvokeEvent()
    {
        _mainMenuAnimationFinished.Raise();
    }
}
