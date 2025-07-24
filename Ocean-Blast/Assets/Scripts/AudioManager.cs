using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _clickSound;

    private void OpenBoxClick()
    {
        _clickSound.Play();
    }

    private void OnEnable()
    {
        OpenBoxClickEvent.ClickSoundEvent += OpenBoxClick;
    }

    private void OnDisable()
    {
        OpenBoxClickEvent.ClickSoundEvent -= OpenBoxClick;
    }
}
