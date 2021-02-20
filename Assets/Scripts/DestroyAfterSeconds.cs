using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
    [SerializeField] float _seconds = 3f;
    void Awake() => Destroy(gameObject, _seconds);
}
