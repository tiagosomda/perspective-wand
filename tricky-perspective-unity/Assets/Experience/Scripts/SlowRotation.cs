using UnityEngine;

public class SlowRotation : MonoBehaviour
{
    public Vector3 rotationSpeed;
   
    void Update()
    {
        this.transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
