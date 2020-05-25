﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Experimental;
using System;
using UnityEngine.InputSystem;

public class DialogBox : MonoBehaviour, IPausable
{
    public enum State
    {
        Inactive,
        Scrolling,
        Cancel,
        Waiting,
    }
    public PauseHandle PauseHandle { get; set; }
    public TextMeshProUGUI text;
    public Image portrait;
    public string VoiceEvent { get => voiceEmitter.Event; set => voiceEmitter = GameObject.Find(value).GetComponent<FMODUnity.StudioEventEmitter>(); }

    private State state = State.Inactive;
    private bool finishedWithStartAnimation = true;
    private FMODUnity.StudioEventEmitter voiceEmitter;

    private void Awake()
    {
        PauseHandle = new PauseHandle(OnPause);
    }

    private void OnPause(bool b)
    {
        if(b)
            voiceEmitter?.SetParameter("Space", 1);
    }

    private void OnConfirm()
    {
        if (PauseHandle.Paused)
            return;
        if(state != State.Inactive)
            state = state.Next();
    }

    public IEnumerator PlayLine(string line, float scrollDelay, float spaceDelay, Sprite portrait = null, string voiceEvent = null)
    {
        // Set Portrait if desired
        if (portrait != null)
            this.portrait.sprite = portrait;
        if (voiceEvent != null)
            VoiceEvent = voiceEvent;
        yield return new WaitWhile(() => PauseHandle.Paused);
        // If we are starting up, wait until finished starting
        yield return new WaitUntil(() => finishedWithStartAnimation);
        // Scroll if there is a scroll delay
        if (scrollDelay > 0)
        {
            // Start the text scroll effect
            if (!voiceEmitter.IsPlaying())
                voiceEmitter.Play();
            state = State.Scrolling;
            text.text = string.Empty;
            for (int i = 0; state != State.Cancel && i < line.Length - 1; ++i)
            {
                yield return new WaitWhile(() => PauseHandle.Paused);
                text.text += line[i];
                if (char.IsWhiteSpace(line[i]))
                {
                    voiceEmitter.SetParameter("Space", 1);
                    yield return new WaitForSeconds(spaceDelay);
                }
                else
                {
                    voiceEmitter.SetParameter("Space", 0);
                    yield return new WaitForSeconds(scrollDelay);
                }                
            }
        }
        // Dump text
        text.text = line;
        // Wait until the player continues
        state = State.Waiting;
        voiceEmitter.SetParameter("Space", 1);
        yield return new WaitWhile(() => state == State.Waiting);
        state = State.Inactive;
    }

    public void Stop()
    {
        StopAllCoroutines();
        if (voiceEmitter != null)
            voiceEmitter.Stop();
        state = State.Inactive;         
    }
}
