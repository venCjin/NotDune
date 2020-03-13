using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class CameraController : MonoBehaviour
{
    public enum CameraMode { AboveGround, UnderGround };

    [SerializeField] private CinemachineVirtualCamera _aboveGroundCamera = null;
    [SerializeField] private CinemachineVirtualCamera _underGroundCamera = null;

    private CharacterController _characterController;

    private void Awake()
    {
        _characterController = FindObjectOfType<CharacterController>();
        _characterController.OnStateChanged += OnStateChanged;
    }

    private void OnDestroy()
    {
        _characterController.OnStateChanged -= OnStateChanged;
    }

    private void OnStateChanged(Type state)
    {
        if (state == typeof(AboveGroundMovementState))
        {
            SetCameraMode(CameraMode.AboveGround);
        }
        else if (state == typeof(UnderGroundMovementState))
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
