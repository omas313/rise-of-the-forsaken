using System.Collections;
using UnityEngine;

public class EndScreen : MonoBehaviour
{
    [SerializeField] Animation _textAnimation;
    [SerializeField] Animation _fadeIn;
    private void Awake()
    {
        StartCoroutine(LoadMainMenu());
    }

    IEnumerator LoadMainMenu()
    {
        yield return new WaitForSeconds(0.25f);
        yield return new WaitUntil(() => !_textAnimation.isPlaying);

        yield return new WaitUntil(() => Input.GetButtonDown("Confirm"));

        _fadeIn.Play();
        yield return new WaitForSeconds(0.15f);
        yield return new WaitUntil(() => !_fadeIn.isPlaying);

        GameManager.Instance.LoadMainMenu();
    }
}
