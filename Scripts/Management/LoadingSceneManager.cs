using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoBehaviour
{
    public static LoadingSceneManager instance = null;
    
    [Header("UI Elements")]
    [SerializeField] private CanvasGroup fader;
    private float duration = 1f;
    
    [SerializeField] private GameObject loadingUI;

    #region Singleton
    void Init()
    {
        if (instance == null)
        {
            instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Awake()
    {
        Init();
    }
    #endregion

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        fader.DOFade(0f, duration)
        .OnStart(() =>
        {
            loadingUI.SetActive(false);
        })
        .OnComplete(() =>
        {
            fader.blocksRaycasts = false;
        });
    }

    public void ChangeScene(string sceneName)
    {
        fader.DOFade(1f, duration)
        .OnStart(() =>
        {
            fader.blocksRaycasts = true;
        })
        .OnComplete( async() =>
        {
            await LoadSceneAsync(sceneName);
        });
    }

    async Task LoadSceneAsync(string target)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(target);
        
        float progress = 0f;
        float timePassed = 0f;

        while (!operation.isDone)
        {
            operation.allowSceneActivation = false;
            loadingUI.SetActive(true);

            timePassed += Time.deltaTime;
            progress = Mathf.Lerp(progress, operation.progress * 100f, timePassed);

            if (progress >= 90f)
            {
                progress = Mathf.Lerp(progress, 100f, timePassed);
                if (progress >= 100f)
                {
                    operation.allowSceneActivation = true;
                }
            }
            
            await Task.Yield();
        }
    }
}
