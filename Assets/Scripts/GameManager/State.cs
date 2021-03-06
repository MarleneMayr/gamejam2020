﻿using UnityEngine;

public abstract class State : MonoBehaviour
{
    [SerializeField] protected Menu menu;
    protected float menuFadeDuration = 0.2f;

    protected StateMachine stateMachine;
    protected static AudioManager audioManager;
    protected static LevelManager levelManager;

    public void Activate(StateMachine sm)
    {
        audioManager ??= FindObjectOfType<AudioManager>();
        levelManager ??= FindObjectOfType<LevelManager>();
        stateMachine = sm;
        menu?.Show(menuFadeDuration);
        gameObject.SetActive(true);

        AfterActivate();
    }

    public void Deactivate()
    {
        BeforeDeactivate();

        gameObject.SetActive(false);
        menu?.Hide(menuFadeDuration);
    }

    public abstract void AfterActivate();
    public abstract void BeforeDeactivate();

    public virtual void OnGeneralEvent() { }
}
