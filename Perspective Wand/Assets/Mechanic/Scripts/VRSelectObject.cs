using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRSelectObject : MonoBehaviour
{
    public Transform leftHandTransform;
    public Renderer leftHandRenderer;
    public LineRenderer line;

    public Transform CameraTransform;
    public float hitDistance = 1000f;
    public LayerMask layerMask;

    public LockPerspectiveScaling lockPerspectiveScaling;
    public LayerMask blockPushBackLayers;

    public AudioSource AudioSourceGlobal;
    public AudioClip PowerUp;
    public AudioClip PowerDown;


    UnityEngine.XR.InputDevice leftHand;
    UnityEngine.XR.InputDevice head;

    private bool isPressingTriggerButton;

    private GameObject selectedObject;

    private LayerMask canScaleLayer;

    private void Awake()
    {
        canScaleLayer = LayerMask.NameToLayer("CanScale");
        lockPerspectiveScaling = GameObject.FindObjectOfType<LockPerspectiveScaling>();
    }

    private void Start()
    {
        SetHead();
        SetLeftHand();
    }

    private void SetHead()
    {
        var devices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.CenterEye, devices);

        if (devices.Count == 1)
        {
            head = devices[0];
            Debug.Log(string.Format("Head found! Device name '{0}' with role '{1}'", head.name, head.role.ToString()));
        }
        else if (devices.Count > 1)
        {
            Debug.Log("Found more than one center eye!");
        }
    }

    private void SetLeftHand()
    {
        var devices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.LeftHand, devices);

        if (devices.Count == 1)
        {
            leftHand = devices[0];
            Debug.Log(string.Format("Left Hand Found! Device name '{0}' with role '{1}'", leftHand.name, leftHand.role.ToString()));
        }
        else if (devices.Count > 1)
        {
            Debug.Log("Found more than one left hand!");
        }
    }

    void Update()
    {
        if(leftHand != null && head != null)
        {
            // check if we are pressing a button
            leftHand.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out isPressingTriggerButton);
            var temp = GetAimedObject();

            if (selectedObject == null)
            {
                leftHandRenderer.material.color = temp != null ? Color.magenta : Color.white;

                if (isPressingTriggerButton && temp != null)
                {
                    selectedObject = GetAimedObject();
                    if(selectedObject != null)
                    {
                        lockPerspectiveScaling.StartScaling(selectedObject, CameraTransform, hitDistance, blockPushBackLayers, leftHandTransform);

                        AudioSourceGlobal.pitch = Random.Range(1.0f, 2.0f);
                        AudioSourceGlobal.PlayOneShot(PowerUp);
                    }
                }
            }

            if(selectedObject != null && isPressingTriggerButton == false)
            {
                lockPerspectiveScaling.StopScaling();
                selectedObject = null;
            }
        }
    }

    public GameObject GetAimedObject()
    {
        //Vector3 leftHandPosition = Vector3.zero;
        //Quaternion leftHandRotation = Quaternion.identity;
        //if (false == leftHand.TryGetFeatureValue(UnityEngine.XR.CommonUsages.devicePosition, out leftHandPosition))
        //{
        //    Debug.LogError("No Left Hand Position");
        //}

        //if (false == leftHand.TryGetFeatureValue(UnityEngine.XR.CommonUsages.deviceRotation, out leftHandRotation))
        //{
        //    Debug.LogError("No Left Hand Rotation");
        //}


        RaycastHit hit;
        if (Physics.Raycast(leftHandTransform.position, leftHandTransform.TransformDirection(Vector3.forward), out hit, hitDistance, layerMask))
        {
            if (canScaleLayer == hit.transform.gameObject.layer)
            {
                return hit.transform.gameObject;
            }
        }

        //line.SetPositions(new Vector3[] { leftHandTransform.position, leftHandTransform.TransformDirection(Vector3.forward) * hitDistance });
        return null;
    }
}
