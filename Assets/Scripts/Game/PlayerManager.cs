using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;

    GameObject player;

    public int kills = 0;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        if (PV.IsMine)
        {
            CreateController();
        }
    }

    void CreateController()
    {
        Transform spawnPoint = SpawnManager.instance.GetSpawnPoint();
        player = PhotonNetwork.Instantiate(Path.Combine("Photon Prefabs", "Player"), spawnPoint.position, spawnPoint.rotation, 0, new object[] { PV.ViewID });
    }

    public void Die()
    {
        PhotonNetwork.Destroy(player);
        Invoke("CreateController", 3f);
    }
}
