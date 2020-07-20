using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteController : MonoBehaviour, IPlayerListener
{
    private CharacterController characterController;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    public void MovePlayer(Vector2 target)
    {
        Vector3 target3D = new Vector3(target.x, transform.position.y, target.y);
        if(characterController != null)
        {
            characterController.Move((target3D - transform.position) * Time.deltaTime);
        }
    }

    public void RotatePlayer(float rotation)
    {
        transform.rotation = new Quaternion(0.0f, rotation, 0.0f, transform.rotation.w);
    }
}
