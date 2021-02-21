using UnityEngine;

public class LPLayerMask : LockPerspectiveTransformer
{
    private int initialLayer;
    private GameObject selectedObject;

    public void Start(GameObject selectedObject)
    {
        this.selectedObject = selectedObject;
        initialLayer = selectedObject.layer;
        selectedObject.layer = 0;
    }

    public void Stop()
    {
        selectedObject.layer = initialLayer;
    }
}
