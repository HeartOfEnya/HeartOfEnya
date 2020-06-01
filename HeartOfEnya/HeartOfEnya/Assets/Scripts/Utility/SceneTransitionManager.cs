﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager main;
    public Animator fade;
    public float fadeTime = 1f;
    private bool click = false;
    
    /// <summary>
    /// Implements the singleton pattern
    /// </summary>
    private void Awake()
    {
        if (main == null)
        {
            main = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    /// <summary>
    /// Changes to the scene specified by given scene name
    /// </summary>
    public void TransitionScenes(string sceneName)
    {
        // If scene is Battle, do the special to battle transition
        // If scene is camp, campIntro, or campOutro, do the special to camp transition
        // else do the normal fade
        StartCoroutine(StartFade(sceneName));
        
    }

    IEnumerator StartFade(string name)
    {
        if (!(click))
        {
            click = true;
            fade.SetTrigger("Start");
            yield return new WaitForSeconds(fadeTime);
            SceneManager.LoadScene(name);
            fade.SetTrigger("End");
            click = false;
        }
        
    }
    /// <summary>
    /// Exit Functionality for Exit Button
    /// </summary>
    public void doExitGame()
    {
        Application.Quit();
    }
}
