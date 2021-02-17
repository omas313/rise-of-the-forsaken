using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCursor : MonoBehaviour
{
    public static MenuCursor Instance { get; private set; }


    public void PlaceAt(Vector3 position)
    {
        gameObject.SetActive(true);
        transform.position = position;
    }

    public void HideCursor() => gameObject.SetActive(true);

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);    

        HideCursor();
    }
}
