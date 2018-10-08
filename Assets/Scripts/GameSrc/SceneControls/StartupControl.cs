using RoachGame.Widgets;
using System;
using UniRx;
using UnityEngine;

public class StartupControl : MonoBehaviour {

    public float startupDelay = 1f;
    private SyncSceneFadeLoader sceneFadeLoader;

    private float delayAmount = 0f;
    private IDisposable delayUpdateSubscribe;

    private void Start() {
        sceneFadeLoader = GetComponent<SyncSceneFadeLoader>();
        delayAmount = startupDelay + sceneFadeLoader.fadeDuration;
        delayUpdateSubscribe = Observable.EveryUpdate().Subscribe(DelayUpdate);
    }

    private void DelayUpdate(long ut) {
        if (delayAmount > 0f) {
            delayAmount -= Time.deltaTime;
        } else {
            sceneFadeLoader.LoadScene(GameConfigs.NAME_SCENE_MENU);
            delayUpdateSubscribe.Dispose();
        }
    }
}
