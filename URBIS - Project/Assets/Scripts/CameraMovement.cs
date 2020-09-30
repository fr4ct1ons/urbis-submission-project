﻿using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform objectToMove;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Camera camera;
    [SerializeField] private float movementSpeed = 3.0f;
    [SerializeField] private CinemachineFreeLook freeLookCamera;

    private Vector3 bufferForward = Vector3.zero;
    private Vector3 bufferRight = Vector3.zero;
    private float cameraRotationValue;
    private CameraInputs inputs;

    public delegate void ClickDelegate(RaycastHit hitObject);

    public static event ClickDelegate OnMouseClick;

    private void Awake()
    {
        if (!camera)
        {
            camera = Camera.main;
        }
        inputs = new CameraInputs();
        inputs.Gameplay.MoveCamera.performed += MoveCamera;
        inputs.Gameplay.MoveCamera.canceled += ctx =>
        {
            bufferForward = Vector3.zero;
            bufferRight = Vector3.zero;
        };
        
        inputs.Gameplay.RotateCamera.canceled += ctx => cameraRotationValue = 0.0f;
        inputs.Gameplay.RotateCamera.performed += RotateCamera;
        inputs.Gameplay.LeftClick.performed += Select;
    }

    private void Select(InputAction.CallbackContext obj)
    {
        Ray screenRay = camera.ScreenPointToRay(Mouse.current.position.ReadValue());

        Debug.DrawRay(screenRay.origin, screenRay.direction * 100.0f, Color.green, 3.0f);

        RaycastHit result;
        Physics.Raycast(screenRay, out result);

        OnMouseClick?.Invoke(result);
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
        bufferForward = cameraTransform.forward;
        bufferForward.y = 0.0f;
        bufferForward *= obj.ReadValue<Vector2>().y;
        
        bufferRight = cameraTransform.right;
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