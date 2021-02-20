using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuCursor : MonoBehaviour
{
    public static MenuCursor Instance { get; private set; }

    Image[] _images;


    public void PlaceAt(Vector3 position)
    {
        foreach (var image in _images)
            image.enabled = true;

        transform.position = position;
    }

    public void HideCursor()
    {
        // Debug.Log("hiding");
        foreach (var image in _images)
            image.enabled = false;
    }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);    

        _images = GetComponentsInChildren<Image>();

        HideCursor();
    }
}
