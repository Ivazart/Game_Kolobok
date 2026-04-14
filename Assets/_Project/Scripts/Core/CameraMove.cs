using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private float damping = 1.5f;
    [SerializeField] private Vector2 offset = new (0f, 0f);
    [SerializeField] private GameManager gm;
    
    private Transform player;
    
    private void Start()
    {
        FindPlayer();
    }

    private void Update()
    {
        if (gm.isDragging) 
            return;
        
        Vector3 position = transform.position;
        var target = new Vector3(player.position.x - offset.x, offset.y, position.z);
        Vector3 currentPosition = Vector3.Lerp(position, target, damping * Time.deltaTime);
        transform.position = currentPosition;
    }
    
    private void FindPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        transform.position = new Vector3(player.position.x - offset.x, offset.y, transform.position.z);
    }

}
