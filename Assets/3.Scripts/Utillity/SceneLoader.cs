using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private Slider mLoadingSlider; // 로딩 바 UI 슬라이더
    
    public static string SceneToLoad { get; private set; }

    private void Start()
    {
        if (!string.IsNullOrEmpty(SceneToLoad))
        {
            StartCoroutine(LoadAsyncScene(SceneToLoad));
        }
        else
        {
             StartCoroutine(LoadAsyncScene("1_Lobby"));
        }
    }

    private IEnumerator LoadAsyncScene(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        
        operation.allowSceneActivation = false; 
        
        float targetProgress = 0;

        while (!operation.isDone)
        {
            targetProgress = Mathf.Clamp01(operation.progress / 0.9f);
            
            while (mLoadingSlider.value < targetProgress)
            {
                mLoadingSlider.value = Mathf.MoveTowards(mLoadingSlider.value, targetProgress, Time.deltaTime);
                yield return null;
            }
            
            if (operation.progress >= 0.9f)
            {
                yield return new WaitForSeconds(1.0f); 

                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
    
    public static void LoadScene(string sceneName)
    {
        SceneToLoad = sceneName;
        SceneManager.LoadScene("0_Loader"); 
    }
}
