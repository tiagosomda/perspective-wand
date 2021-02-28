using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnZone : MonoBehaviour
{
    public Transform respawnPoint;


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Crossed Respawn Zone : " + other.gameObject);

        var respawnObj = other.gameObject;

        var controller = respawnObj.GetComponent<CharacterController>();
        if (controller != null)
        {
            RespawnCharacterController(respawnObj, controller);
        }
        else
        {
            Respawn(respawnObj);
        }
    }


    private void Respawn(GameObject obj)
    {
        obj.transform.localScale = Vector3.one;
        obj.transform.position = respawnPoint.position;
    }   
    
    private void RespawnCharacterController(GameObject obj, CharacterController controller)
    {
        controller.enabled = false;
        obj.transform.position = respawnPoint.position;
        controller.enabled = true;
    }

}
