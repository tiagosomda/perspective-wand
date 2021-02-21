using UnityEngine;
using UnityEngine.UI;

public class SelectObject : MonoBehaviour
{
    public Transform CameraTransform;
    public Image crosshairs;
    public float hitDistance = 1000f;
    public LayerMask layerMask;
    public LayerMask blockPushBackLayers;
    public bool Selecting = true;
    public LockPerspectiveScaling lockPerspectiveScaling;

    private void Awake()
    {
        lockPerspectiveScaling = GameObject.FindObjectOfType<LockPerspectiveScaling>();
    }

    void Update()
    {
        if(Selecting == false)
        {
            if(Input.GetMouseButtonDown(0))
            {
                lockPerspectiveScaling.StopScaling();
                Selecting = true;
            }
            return;
        }

        GameObject selectedObject = GetAimedObject();
        crosshairs.color = selectedObject != null ? Color.red : Color.white;
        if(selectedObject != null && Input.GetMouseButtonDown(0))
        {
            Selecting = false;
            lockPerspectiveScaling.StartScaling(selectedObject, CameraTransform, hitDistance, blockPushBackLayers);
        }
    }

    public GameObject GetAimedObject()
    {
        RaycastHit hit;
        if (Physics.Raycast(CameraTransform.position, CameraTransform.TransformDirection(Vector3.forward), out hit, hitDistance, layerMask))
        {
            return hit.transform.gameObject;
        }

        return null;
    }
}
