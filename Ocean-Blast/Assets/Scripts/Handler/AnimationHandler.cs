using UnityEngine;
using DG.Tweening;
using Zenject;

public class AnimationHandler : MonoBehaviour
{
    [SerializeField] private Camera _mainCam;
    [Inject] private SignalBus _signalBus;
    private void CamShake()
    {
        _mainCam.transform.DOShakePosition(0.3f, strength: new Vector3(0.1f, 0.1f, 0), vibrato: 10, randomness: 90);
    }
    private void Awake()
    {
        _signalBus.Subscribe<AnimSignalBus>(CamShake);

    }
    private void OnDestroy()
    {
        _signalBus.Unsubscribe<AnimSignalBus>(CamShake);
    }
}
  