using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _clickSound;
    [SerializeField] private AudioSource _swapSound;
    [SerializeField] private AudioSource _mergeSound;
    [SerializeField] private AudioSource _fireSound;
    [Inject] private SignalBus _signalBus;

    private void OpenBoxClick()
    {
        _clickSound.Play();
    }
    private void SwapSound()
    {
        _swapSound.Play();
    }
    private void MergeSound()
    {
        _mergeSound.Play();
    }

    private void FireSound()
    {
        _fireSound.Play();
    }
    private void Start()
    {
        _signalBus.Subscribe<ClickSoundSignals>(OpenBoxClick);
        _signalBus.Subscribe<SwapSoundSignalbus>(SwapSound);
        _signalBus.Subscribe<MergeSoundBus>(MergeSound);
        _signalBus.Subscribe<FireSoundBus>(FireSound);
    }

    private void OnDestroy()
    {
        _signalBus.Unsubscribe<ClickSoundSignals>(OpenBoxClick);
        _signalBus.Unsubscribe<SwapSoundSignalbus>(SwapSound);
        _signalBus.Unsubscribe<MergeSoundBus>(MergeSound);
        _signalBus.Unsubscribe<FireSoundBus>(FireSound);
    }
}
    