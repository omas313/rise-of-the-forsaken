using System.Collections;
using UnityEngine;

public class EndScreen : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine(LoadMainMenu());
    }

    IEnumerator LoadMainMenu()
    {
        yield return new WaitForSeconds(8f);
        GameManager.Instance.LoadMainMenu();
    }
}
