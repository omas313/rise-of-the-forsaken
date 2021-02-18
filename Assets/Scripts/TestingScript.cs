using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingScript : MonoBehaviour
{
    [SerializeField] MagicAttacksStore _store;

    [SerializeField] Element fire;
    [SerializeField] Element ice;
    [SerializeField] Element earth;
    [SerializeField] Element wind;


    void Start()
    {
        Debug.Log(_store.GetMagicAttacksWithElements(fire, ice).Length);
        Debug.Log(_store.GetMagicAttacksWithElements(fire, wind).Length);
        Debug.Log(_store.GetMagicAttacksWithElements(fire, earth).Length);

        Debug.Log(_store.GetMagicAttacksWithElements(ice, wind).Length);
        Debug.Log(_store.GetMagicAttacksWithElements(ice, earth).Length);

        Debug.Log(_store.GetMagicAttacksWithElements(wind, earth).Length);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
