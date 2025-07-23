using UnityEngine;
using DG.Tweening;

public class AnimationHandler : MonoBehaviour
{
    [SerializeField] private Camera _mainCam;

    private void CamShake()
    {
        _mainCam.transform.DOShakePosition(0.3f, strength: new Vector3(0.1f, 0.1f, 0), vibrato: 10, randomness: 90);
    }
    private void OnEnable()
    {
        RegisterEvens();
    }

    private void OnDisable()
    {
        UnRegisterEvents();
    }
    private void RegisterEvens()
    {
        AnimEvents.CamEvents += CamShake;
    }

    private void UnRegisterEvents()
    {
        AnimEvents.CamEvents -= CamShake;
    }
}
  