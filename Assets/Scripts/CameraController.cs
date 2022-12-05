using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField]
    private float edgeSize;

    [SerializeField]
    private Transform cameraTransform;

    [SerializeField]
    private float maxZoom;
    [SerializeField]
    private float minZoom;

    [SerializeField]
    private float movementSpeed;
    [SerializeField]
    private float movementTime;
    [SerializeField]
    private float rotationAmount;
    [SerializeField]
    private Vector3 zoomAmount;

    private Vector3 newPosition;
    private Quaternion newRotation;
    private Vector3 newZoom;

    private Vector3 mouseDragStartPos;
    private Vector3 mouseDragCurrPos;

    private Vector3 mouseRotateStartPos;
    private Vector3 mouseRotateCurrPos;

    [SerializeField]
    private bool lockEdgeScrolling = false;
    [SerializeField]
    private bool lockOnDragMove = false;
    [SerializeField]
    private bool playerAnchored = false;
    public bool PlayerAnchored
    {
        get
        {
            return playerAnchored;
        }
    }
    public GameObject player;

    private void Start()
    {
        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = cameraTransform.localPosition;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Update()
    {
        HandleKeyboardInput();
        HandleMouseInput();
        UpdateCameraPosition();
    }

    private void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            LockCameraOnPlayer();
        }
    }

    public void LockCameraOnPlayer()
    {
        if (!playerAnchored)
        {
            transform.SetParent(player.transform);
            lockEdgeScrolling = true;
            lockOnDragMove = true;
            playerAnchored = true;
        }
        else
        {
            transform.SetParent(null);
            lockEdgeScrolling = false;
            lockOnDragMove = false;
            playerAnchored = false;
        }
    }

    private void HandleMouseInput()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            newZoom += Input.mouseScrollDelta.y * zoomAmount;
        }

        // Move Camera on edge scrolling
        if (!lockEdgeScrolling)
        {
            if (Input.mousePosition.x > Screen.width - edgeSize)
            {
                newPosition += (transform.right * movementSpeed);
            }
            if (Input.mousePosition.x < edgeSize)
            {
                newPosition -= (transform.right * movementSpeed);
            }
            if (Input.mousePosition.y > Screen.height - edgeSize)
            {
                newPosition += (transform.forward * movementSpeed);
            }
            if (Input.mousePosition.y < edgeSize)
            {
                newPosition -= (transform.forward * movementSpeed);
            }
        }

        // Move camera on mouse drag
        if (!lockOnDragMove)
        {
            if (Input.GetMouseButtonDown(0))
            {
                lockEdgeScrolling = true;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                Plane ground = new Plane(Vector3.up, Vector3.zero);

                float rayLength;

                if (ground.Raycast(ray, out rayLength))
                {
                    mouseDragStartPos = ray.GetPoint(rayLength);
                }
            }
            if (Input.GetMouseButton(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                Plane ground = new Plane(Vector3.up, Vector3.zero);

                float rayLength;

                if (ground.Raycast(ray, out rayLength))
                {
                    mouseDragCurrPos = ray.GetPoint(rayLength);

                    newPosition = (transform.position + mouseDragStartPos - mouseDragCurrPos);
                }
            }
        }

        // Rotation
        if (Input.GetMouseButtonDown(2))
        {
            if (!playerAnchored) lockEdgeScrolling = true;
            mouseRotateStartPos = Input.mousePosition;
        }
        if (Input.GetMouseButton(2))
        {
            mouseRotateCurrPos = Input.mousePosition;

            Vector3 difference = mouseRotateStartPos - mouseRotateCurrPos;

            mouseRotateStartPos = mouseRotateCurrPos;

            newRotation *= Quaternion.Euler(Vector3.up * (-difference.x / 5f));
        }
        if (!Input.GetMouseButton(0) || !Input.GetMouseButton(2))
        {
            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(2))
            {
                if (!playerAnchored) lockEdgeScrolling = false;
            }
        }
    }
    void UpdateCameraPosition()
    {
        newZoom.y = Mathf.Clamp(newZoom.y, minZoom, maxZoom);
        newZoom.z = Mathf.Clamp(newZoom.z, -maxZoom, -minZoom);

        if (playerAnchored) newPosition = player.transform.position;

        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);
    }
}
