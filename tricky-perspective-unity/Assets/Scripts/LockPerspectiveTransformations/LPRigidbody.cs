using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface LockPerspectiveTransformer
{
    void Start(GameObject selectedObject);
    void Stop();
}


public class LPRigidbody : LockPerspectiveTransformer
{
    private Rigidbody initialRigidBody;
    private bool initialUseGravity;
    private bool initialFreezeRotation;
    private RigidbodyConstraints rbConstraints;

    public void Start(GameObject selectedObject)
    {
        initialRigidBody = selectedObject.GetComponent<Rigidbody>();
        if (initialRigidBody != null)
        {
            initialUseGravity = initialRigidBody.useGravity;
            initialFreezeRotation = initialRigidBody.freezeRotation;
            rbConstraints = initialRigidBody.constraints;

            initialRigidBody.useGravity = false;
            initialRigidBody.freezeRotation = false;
            initialRigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    public void Stop()
    {
        if (initialRigidBody != null)
        {
            initialRigidBody.useGravity = initialUseGravity;
            initialRigidBody.freezeRotation = initialFreezeRotation;
            initialRigidBody.constraints = rbConstraints;
            initialRigidBody.velocity = Vector3.zero;
            initialRigidBody = null;
        }
    }
}
