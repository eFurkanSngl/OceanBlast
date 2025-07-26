using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _clickSound;
    [Inject] private SignalBus _signalBus;

    private void OpenBoxClick()
    {
        _clickSound.Play();
    }

    private void Start()
    {
        _signalBus.Subscribe<ClickSoundSignals>(OpenBoxClick);
    }

    private void OnDestroy()
    {
        _signalBus.Unsubscribe<ClickSoundSignals>(OpenBoxClick);   
    }
}
