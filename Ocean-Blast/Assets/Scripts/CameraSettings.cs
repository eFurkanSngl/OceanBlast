using UnityEngine;
using Cinemachine;
using System;
public class BlastCameraSetup : MonoBehaviour
{
    [SerializeField] private float cameraDepth = -10f;
    [SerializeField] private float tiltAngle = 15f;
    private Camera _cam;
    private void Awake()
    {
        _cam = Camera.main;
    }
    private void SetupCamera(int gridX , int gridY)
    {
        if (_cam == null) return;

        _cam.orthographic = true;

        // Slight tilt on X to fake 3D depth
        _cam.transform.rotation = Quaternion.Euler(tiltAngle, 0, 0);

        // Position camera to center the grid
        _cam.transform.position = new Vector3(gridX / 2.7f, gridY / 4f, cameraDepth);

        // Auto adjust orthographic size
        float screenRatio = (float)Screen.width / Screen.height;
        float targetRatio = (float)gridX / gridY;

        if (screenRatio >= targetRatio)
            _cam.orthographicSize = gridY / 1.2f;
        else
            _cam.orthographicSize = (gridX / screenRatio) / 1.2f;

        Debug.Log("working");
    }
    private void OnEnable()
    {
        Debug.Log("RegisterEvents");
        RegisterEvents();
    }
    private void OnDisable()
    {
        Debug.Log("UnRegigsterEvetns");
        UnRegisterEvents();
    }
    private void RegisterEvents() => GridManager.GridManagerEvents += SetupCamera;
    private void UnRegisterEvents() => GridManager.GridManagerEvents -= SetupCamera;
}
