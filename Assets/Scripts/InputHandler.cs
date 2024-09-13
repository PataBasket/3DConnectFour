using System;
using UnityEngine;

public class InputHandler
{
    private Vector3 previousMousePosition;
    private Camera camera_object;
    private GameObject _baseObject;

    public event Action<GameObject, Vector2Int> OnPoleClicked;

    public InputHandler()
    {
        camera_object = Camera.main;
        _baseObject = GameObject.Find("Base");
    }

    public void Update()
    {
        RotateCamera();
        RotateBase();
    }

    private void RotateCamera()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = camera_object.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GameObject clickedPole = hit.collider.gameObject;
                if (clickedPole.CompareTag("Pole"))
                {
                    Vector2Int gridIndex = GridManager.Instance.GetGridIndex(clickedPole);  // ここで GridManager.Instance を使用
                    OnPoleClicked?.Invoke(clickedPole, gridIndex);
                }
            }
        }
    }
    
    private void RotateBase()
    {
        if (Input.GetMouseButtonDown(1)) // 右クリックを押した瞬間
        {
            previousMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(1)) // 右クリックを押し続けている間
        {
            Vector3 delta = Input.mousePosition - previousMousePosition;
            
            float rotationY = -delta.x * 0.2f;
            _baseObject.transform.Rotate(Vector3.up, rotationY, Space.World);

            previousMousePosition = Input.mousePosition;
        }
    }
}