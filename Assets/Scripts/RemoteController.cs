using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteController : MonoBehaviour, IPlayerListener
{
    private CharacterController characterController;
    private Animator anim;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    public void MovePlayer(Vector2 target)
    {
        Vector3 target3D = new Vector3(target.x, transform.position.y, target.y);
        if(characterController != null)
        {
            //Calculate the Input Magnitude
            var speed = target.sqrMagnitude;

            //Physically move player
            if (speed > 0.1f)
            {
                anim.SetFloat("Blend", speed, 0.3f, Time.deltaTime);
                characterController.Move((target3D - transform.position) * Time.deltaTime);
            }
            else if (speed < 0.1f)
            {
                anim.SetFloat("Blend", speed, 0.15f, Time.deltaTime);
            }
        }
    }

    public void RotatePlayer(float rotation)
    {
        transform.rotation = new Quaternion(0.0f, rotation, 0.0f, transform.rotation.w);
    }
}
