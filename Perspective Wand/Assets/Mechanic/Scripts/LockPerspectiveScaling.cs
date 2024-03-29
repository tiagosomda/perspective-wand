﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockPerspectiveScaling : MonoBehaviour
{
    private Transform pointerTransform;
    private Transform cameraTransform;
    private LayerMask layerMask;
    private float maxPushBackDistance = 1000;

    private GameObject selectedObj;
    private float initialDistance;
    private Vector3 initialScale;

    private Vector3 DebugCube;


    private List<LockPerspectiveTransformer> perspectiveTransformers = new List<LockPerspectiveTransformer>();

    public void Awake()
    {
        perspectiveTransformers.Add(new LPRigidbody());
        perspectiveTransformers.Add(new LPLayerMask());
        perspectiveTransformers.Add(new LPRenders());
    }
    public void StartScaling(
        GameObject obj,
        Transform cameraTransform,
        float maxPushBackDistance,
        LayerMask blockPushBackLayers,
        Transform pointerTransform = null)
    {
        selectedObj = obj;
        this.cameraTransform = cameraTransform;
        this.pointerTransform = pointerTransform == null ? cameraTransform : pointerTransform;
        this.maxPushBackDistance = maxPushBackDistance;
        this.layerMask = blockPushBackLayers;

        initialDistance = GetDistanceFrom(cameraTransform.position);
        initialScale = selectedObj.transform.localScale;

        foreach (var t in perspectiveTransformers)
        {
            t.Start(selectedObj);
        }

        StartCoroutine(UpdateScale());
    }

    public void StopScaling()
    {
        StopCoroutine(UpdateScale());
        
        foreach (var t in perspectiveTransformers)
        {
            t.Stop();
        }

        selectedObj = null;
    }
    
    private IEnumerator UpdateScale()
    {
        while(true)
        {
            yield return new WaitForEndOfFrame();

            if (selectedObj != null)
            {
                AdjustPosition();
                AdjustScaling();
            }
        }
    }

    private void AdjustPosition()
    {
        selectedObj.transform.position = GetFarthestPosition(
            pointerTransform.forward,
            pointerTransform.position,
            selectedObj.transform.rotation,
            maxPushBackDistance);
    }

    private void AdjustScaling()
    {
        var currentDistance = GetDistanceFrom(cameraTransform.position);
        float scalingFactor = GetScalingFactor(currentDistance);
        Vector3 newScale = GetCurrentScale(scalingFactor);
        selectedObj.transform.localScale = newScale;
    }

    private float GetDistanceFrom(Vector3 from)
        => Vector3.Distance(from, selectedObj.transform.position);

    private float GetScalingFactor(float currentDistance)
        => currentDistance/initialDistance;
    

    private Vector3 GetCurrentScale(float scale)
        => new Vector3(initialScale.x*scale,initialScale.y*scale,initialScale.z*scale);

    private Vector3 GetFarthestPosition(
        Vector3 pointerDirection,
        Vector3 pointerPosition,
        Quaternion rotation,
        float maxDistance)
    {
        var nextPosition = pointerPosition + pointerDirection;
        var previousPosition = nextPosition;

        Collider[] colliders = null;

        float distance = 0.0f;
        int count = 0;
        Vector3 scale = Vector3.zero;

        while(colliders == null || colliders.Length == 0)
        {
            previousPosition = nextPosition; 
            nextPosition += pointerDirection*0.1f;

            distance = Vector3.Distance(pointerPosition, nextPosition);
            scale = GetCurrentScale(GetScalingFactor(distance));
            var boxBounds = scale / 2.0f;
            DebugCube = boxBounds;
            colliders = Physics.OverlapBox(nextPosition, boxBounds, rotation, layerMask);

            if(distance > maxDistance)
            {
                break;
            }
        }

        return previousPosition;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(this.transform.position, DebugCube);
    }

}
