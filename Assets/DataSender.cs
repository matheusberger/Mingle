using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSender : MonoBehaviour
{
    public SocketManager socketManager;
    public GameObject jammo;

    private PlayerData playerData;

    // Start is called before the first frame update
    void Start()
    {
        playerData = new PlayerData(0, "", jammo.transform.position.x, jammo.transform.position.z, jammo.transform.rotation.y, null);
    }

    // Update is called once per frame
    void Update()
    {
        if (socketManager.ConfirmConnection())
        {
            if (playerData.socketID == "")
            {
                playerData.socketID = socketManager.GetSocketID();
            }

            playerData.SetPosition(jammo.transform.position.x, jammo.transform.position.z);
            playerData.SetRotation(jammo.transform.rotation.y);

            socketManager.SendPlayerData(playerData);
        }
    }
}
