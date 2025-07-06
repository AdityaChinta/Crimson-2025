using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    private float sceneLoadDelay = 1f;
    //private static bool hasEnteredNextScene = false;
    public static bool hasEnteredNextScene = false;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
            StartCoroutine(LoadNextScene());
        
    }

    IEnumerator LoadNextScene()
    {
        yield return new WaitForSecondsRealtime(sceneLoadDelay);
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex;
        if (SceneManager.GetActiveScene().buildIndex == 11 || SceneManager.GetActiveScene().buildIndex == 13)
        {
            nextSceneIndex = currentSceneIndex - 1;
            hasEnteredNextScene = true;
            Debug.Log("-1");
        }
        else
        {
            if (hasEnteredNextScene)
            {
                nextSceneIndex = currentSceneIndex + 2;
                Debug.Log("+2");
                hasEnteredNextScene = false;
            }
            else
            {
                nextSceneIndex = currentSceneIndex + 1;
                Debug.Log("+1");
            }
        }
        //if(nextSceneIndex == SceneManager.sceneCountInBuildSettings)
            //nextSceneIndex = 0;
        SceneManager.LoadScene(nextSceneIndex);
    }

}
