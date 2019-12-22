using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartBeat : MonoBehaviour
{
    AudioSource heartBeat;
    UIControl ui;
    PlayerData data;

    private void Start()
    {
        ui = FindObjectOfType<UIControl>();
        heartBeat = GetComponent<AudioSource>();

        heartBeat.ignoreListenerPause = true;
        data = FindObjectOfType<PlayerData>();
    }
    public void HeartBeatSoundPlay()
    {
        if (ui.heartPulseActive)
        {
            if (data.hp > 70 && data.hp <= 100)
                heartBeat.pitch = 1f;
            if (data.hp > 40 && data.hp <= 70)
                heartBeat.pitch = 1.25f;
            if (data.hp > 0 && data.hp <= 40)
                heartBeat.pitch = 1.5f;

            heartBeat.Play();
        }
    }
}
