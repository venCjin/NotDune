using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public enum CameraMode { AboveGround, UnderGround };

    [SerializeField] private CinemachineVirtualCamera _aboveGroundCamera = null;
    [SerializeField] private CinemachineVirtualCamera _underGroundCamera = null;

    public CharacterController characterController;

    private void Start()
    {
        characterController = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
        SetCameraMode(CameraMode.AboveGround);
    }

    private void Update()
    {
        if (characterController.isAboveGround)
        {
            SetCameraMode(CameraMode.AboveGround);
        }
        else
        {
            SetCameraMode(CameraMode.UnderGround);
        }
    }

    public void SetCameraMode(CameraMode mode)
    {
        if (mode == CameraMode.AboveGround)
        {
            _aboveGroundCamera.Priority = 100;
            _underGroundCamera.Priority = 0;
        }
        else
        {
            _aboveGroundCamera.Priority = 0;
            _underGroundCamera.Priority = 100;
        }
    }
}
