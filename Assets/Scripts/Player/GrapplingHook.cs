using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrapplingHook : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private DistanceJoint2D distJoint;

    private Vector3[] curvePoints = new Vector3[20];
    private Vector2 initEndPos;
    //Distance of the midpoint of the curve from the midpoint of the original line
    private float distFromLineMidpoint;

    // Start is called before the first frame update
    void Start()
    {
        PlayerInputMgr.instance.grapplingInput.action.started += OnGrapple;
        PlayerInputMgr.instance.grapplingInput.action.canceled += OnCancel;

        distJoint.enabled = false;
    }

    private void OnDestroy()
    {
        PlayerInputMgr.instance.grapplingInput.action.started -= OnGrapple;
        PlayerInputMgr.instance.grapplingInput.action.canceled -= OnCancel;
    }

    // Update is called once per frame
    void Update()
    {
        if (distJoint.enabled)
        {
            CalculateHookPoints();
            lineRenderer.SetPositions(curvePoints);
        }
    }

    #region Input Detection

    private void OnGrapple(InputAction.CallbackContext context)
    {
        initEndPos = mainCamera.ScreenToWorldPoint(Mouse.current.position.value);

        distJoint.connectedAnchor = initEndPos;
        distJoint.enabled = true;
        lineRenderer.enabled = true;
    }

    private void OnCancel(InputAction.CallbackContext context)
    {
        distJoint.enabled = false;
        lineRenderer.enabled = false;
    }

    #endregion

    //Manually set end point
    private void CalculateHookPoints()
    {
        //Get end pos
        Vector2 endPos = initEndPos;

        //If player is above the grapple point, make the line straight
        if (transform.position.y > endPos.y - 2)
        {
            distFromLineMidpoint = Mathf.Lerp(distFromLineMidpoint, 0, 0.1f);
        }
        else
        {
            distFromLineMidpoint = Mathf.Lerp(distFromLineMidpoint, -Mathf.Clamp(initEndPos.x - transform.position.x, -5, 5), 0.1f);
        }

        //Calculate quadratic curve
        Vector2 initVector = endPos - (Vector2)transform.position;
        Vector2 midPoint = endPos - (initVector / 2);
        Vector2 perpendicularVector = Vector2.Perpendicular(initVector).normalized;
        Vector2 curveMidpoint = midPoint + perpendicularVector * distFromLineMidpoint;
        for (int i = 0; i < 20; i++)
        {
            float t = (float)i / 20;
            Vector2 pos = ((1 - t) * (1 - t) * (Vector2)transform.position) + (2 * (1 - t) * t * curveMidpoint) + (t * t * endPos);

            curvePoints[i] = pos;
        }
    }
}
