using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnIntroLoaded : MonoBehaviour
{
    public float VideoTime;
    void Start()
    {
        StartCoroutine(IntroPause());
    }

    private IEnumerator IntroPause()
    {
        yield return new WaitForSeconds(VideoTime);
        SceneManager.LoadScene(1);
    }
}
