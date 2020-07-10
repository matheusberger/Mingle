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
        print("moving npc");
        Vector3 target3D = new Vector3(target.x, transform.position.y, target.y);
        characterController.Move(target3D - transform.position);
    }

    public void RotatePlayer(float rotation)
    {
        throw new System.NotImplementedException();
    }
}
