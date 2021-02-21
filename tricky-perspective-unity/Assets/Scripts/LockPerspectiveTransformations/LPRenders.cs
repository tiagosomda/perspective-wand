using System.Collections.Generic;
using UnityEngine;

public class LPRenders : LockPerspectiveTransformer
{
    private List<Renderer> renderers = new List<Renderer>();

    public void Start(GameObject selectedObject)
    {
        renderers.Clear();
        GetRenderers(selectedObject.transform);
        foreach (var r in renderers)
        {
            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }

    public void Stop()
    {
        foreach (var r in renderers)
        {
            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        }
    }

    private void GetRenderers(Transform parent)
    {
        foreach (Transform child in parent)
        {
            GetRenderers(child);
        }

        var render = parent.gameObject.GetComponent<Renderer>();
        renderers.Add(render);
    }
}
