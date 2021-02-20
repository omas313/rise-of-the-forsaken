using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public bool IsAvailable => !_animation.isPlaying;

    TextMeshProUGUI _text;
    Animation _animation;

    public void Play(string text, Vector3 position)
    {
        _text.SetText(text);
        transform.position = position;
        _animation.Play();
    }

    void Awake()
    {
        _text = GetComponentInChildren<TextMeshProUGUI>();
        _animation = GetComponent<Animation>();
    }
}
