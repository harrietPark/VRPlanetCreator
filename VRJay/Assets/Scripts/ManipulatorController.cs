using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManipulatorController : MonoBehaviour
{
    public Camera _camera;
    public float radius = 1f;
    [Range(0, 1)] public float hardness = 0.1f;

    Manipulator _manipulator;
    GameObject _manipulatorAnchor;
    GameObject _manipulatorHandle;

    Vector3 _prevMousePos;

    bool _dragging;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            bool hit = Physics.Raycast(ray, out RaycastHit hitInfo, 100f);
           
            //if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            if(hitInfo.collider.transform != null)
            {
                Debug.Log("raycast works");
                var hitRenderer = hitInfo.collider.GetComponentInChildren<Renderer>();
                if (hitRenderer != null)
                {
                    _manipulator = gameObject.AddComponent<Manipulator>();
                    _manipulatorAnchor = new GameObject("MouseAnchor");
                    _manipulatorAnchor.transform.position = hitInfo.point;
                    _manipulatorHandle = new GameObject("MouseHandle");
                    _manipulatorHandle.transform.position = _manipulatorAnchor.transform.position;

                    _manipulator.Anchor = _manipulatorAnchor.transform;
                    _manipulator.Handle = _manipulatorHandle.transform;
                    _manipulator._renderer = hitRenderer;
                    _manipulator.hardness = hardness;
                    _manipulator.radius = radius;

                    _dragging = true;
                    _prevMousePos = Input.mousePosition;
                }
            }
        }
        else if(_dragging && Input.GetMouseButton(0))
        {
            var mouseDelta = Input.mousePosition - _prevMousePos;
            _manipulatorHandle.transform.Translate(mouseDelta * 0.01f);
            _prevMousePos = Input.mousePosition;
        }
        else if(Input.GetMouseButtonUp(0))
        {
            /*
            if (_dragging)
            {
                Destroy(_manipulator);
                Destroy(_manipulatorAnchor);
                Destroy(_manipulatorHandle);
            }
            */
           // _dragging = false;
        }
    }
}
