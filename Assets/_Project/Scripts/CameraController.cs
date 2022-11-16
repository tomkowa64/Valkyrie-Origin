using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Do not touch")]
    [SerializeField] private Transform player;

    // Update is called once per frame
    void Update()
    {
        if(GameObject.FindGameObjectWithTag("Player"))
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
        }
    }
}
