using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform objectToMove;
    [SerializeField] private Transform camera;
    [SerializeField] private float movementSpeed = 3.0f;
    [SerializeField] private CinemachineFreeLook freeLookCamera;

    private Vector3 bufferForward = Vector3.zero;
    private Vector3 bufferRight = Vector3.zero;
    private float cameraRotationValue;
    private CameraInputs inputs;

    private void Awake()
    {
        inputs = new CameraInputs();
        inputs.Gameplay.MoveCamera.performed += MoveCamera;
        inputs.Gameplay.MoveCamera.canceled += ctx =>
        {
            bufferForward = Vector3.zero;
            bufferRight = Vector3.zero;
        };
        
        inputs.Gameplay.RotateCamera.canceled += ctx => cameraRotationValue = 0.0f;
        inputs.Gameplay.RotateCamera.performed += RotateCamera;
    }

    private void RotateCamera(InputAction.CallbackContext obj)
    {
        cameraRotationValue = obj.ReadValue<float>() * -1;
    }

    private void Update()
    {
        objectToMove.position += bufferForward * Time.deltaTime * movementSpeed;
        objectToMove.position += bufferRight * Time.deltaTime * movementSpeed;

        freeLookCamera.m_XAxis.m_InputAxisValue = cameraRotationValue;
    }

    private void MoveCamera(InputAction.CallbackContext obj)
    {
        bufferForward = camera.forward;
        bufferForward.y = 0.0f;
        bufferForward *= obj.ReadValue<Vector2>().y;
        
        bufferRight = camera.right;
        bufferRight *= obj.ReadValue<Vector2>().x;
    }

    private void OnEnable()
    {
        inputs.Enable();
    }

    private void OnDisable()
    {
        inputs.Disable();
    }
}
