using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    [Tooltip("Object to be moved with the movement keys.")]
    [SerializeField] private Transform objectToMove;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Camera camera;
    [SerializeField] private CinemachineFreeLook freeLookCamera;

    [Space]
    
    [SerializeField] private float maxZoom = 8.0f;
    [SerializeField] private float midRigMinHeight;
    [SerializeField] private float midRigMinRadius;
    
    [SerializeField] private float frontSpeed = 3.0f;
    [SerializeField] private float horizontalSpeed = 3.0f;
    [SerializeField] private float zoomSensibility = 1.0f;

    private Vector3 bufferForward = Vector3.zero;
    private Vector3 bufferRight = Vector3.zero;
    private float cameraRotationValue;
    private CameraInputs inputs;

    public delegate void ClickDelegate(RaycastHit hitObject);

    public static event ClickDelegate OnMouseClick;

    /// <summary>
    /// Creates an input object.
    /// </summary>
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
            //bufferForward = Vector3.zero;
            bufferRight = Vector3.zero;
        };
        
        inputs.Gameplay.RotateCamera.canceled += ctx => cameraRotationValue = 0.0f;
        inputs.Gameplay.RotateCamera.performed += RotateCamera;
        inputs.Gameplay.LeftClick.performed += Select;
        inputs.Gameplay.Zoom.performed += Zoom;

        PauseMenuAuxiliar.OnGamePause += () =>
        {
            inputs.Disable();
        };

        PauseMenuAuxiliar.OnGameResume += () =>
        {
            inputs.Enable();
        };
    }

    /// <summary>
    /// Zooms the camera in and out.
    /// </summary>
    /// <param name="obj"></param>
    private void Zoom(InputAction.CallbackContext obj)
    {
        freeLookCamera.m_Lens.OrthographicSize += obj.ReadValue<Vector2>().y * zoomSensibility * -1;
        freeLookCamera.m_Orbits[1].m_Height += obj.ReadValue<Vector2>().y * zoomSensibility * -1;
        freeLookCamera.m_Orbits[1].m_Radius += obj.ReadValue<Vector2>().y * zoomSensibility * -1;

        if (freeLookCamera.m_Lens.OrthographicSize < maxZoom)
        {
            freeLookCamera.m_Lens.OrthographicSize = maxZoom;
        }

        if (freeLookCamera.m_Orbits[1].m_Height < midRigMinHeight)
        {
            freeLookCamera.m_Orbits[1].m_Height = midRigMinHeight;
        }
        
        if (freeLookCamera.m_Orbits[1].m_Radius < midRigMinRadius)
        {
            freeLookCamera.m_Orbits[1].m_Radius = midRigMinRadius;
        }
    }

    /// <summary>
    /// Shoots a ray at the mouse position and if there is an object, invoke the OnMouseClick event.
    /// </summary>
    /// <param name="obj"></param>
    private void Select(InputAction.CallbackContext obj)
    {
        Ray screenRay = camera.ScreenPointToRay(Mouse.current.position.ReadValue());

        Debug.DrawRay(screenRay.origin, screenRay.direction * 100.0f, Color.green, 3.0f);

        RaycastHit result;
        if (Physics.Raycast(screenRay, out result))
        {
            OnMouseClick?.Invoke(result);
        }
    }

    /// <summary>
    /// Rotates the camera around.
    /// </summary>
    /// <param name="obj"></param>
    private void RotateCamera(InputAction.CallbackContext obj)
    {
        cameraRotationValue = obj.ReadValue<float>() * -1;
    }

    /// <summary>
    /// Moves the camera.
    /// </summary>
    private void Update()
    {
        bufferForward = cameraTransform.forward;
        bufferForward.y = 0.0f;
        bufferForward *= inputs.Gameplay.MoveCamera.ReadValue<Vector2>().y;

        objectToMove.position += bufferForward * Time.unscaledDeltaTime * frontSpeed * (1 + freeLookCamera.m_Lens.OrthographicSize - maxZoom);
        objectToMove.position += bufferRight * Time.unscaledDeltaTime * horizontalSpeed * (1 + freeLookCamera.m_Lens.OrthographicSize - maxZoom);

        freeLookCamera.m_XAxis.m_InputAxisValue = cameraRotationValue;
    }

    /// <summary>
    /// Sets the right movement direction.
    /// </summary>
    /// <param name="obj"></param>
    private void MoveCamera(InputAction.CallbackContext obj)
    {
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
