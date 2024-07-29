using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Game.Test
{
    public class FMODTest : MonoBehaviour
    {
        public Button btnPlay;
        public Button btnStop;

        private void Awake()
        {
            btnPlay.onClick.AddListener(OnBtnPlay);
            btnStop.onClick.AddListener(OnBtnStop);
        }

        private void OnBtnPlay()
        {
            Global.Get<AudioSystem>().Create(FMODName.Event.first_step, out var instance, true);
            instance.start();
        }

        private void OnBtnStop()
        {
            var instance = Global.Get<AudioSystem>().Get(FMODName.Event.first_step);
            instance.stop(STOP_MODE.IMMEDIATE);
        }
    }
}