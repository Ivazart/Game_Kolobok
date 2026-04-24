using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private float damping = 1.5f;
    [SerializeField] private Vector2 offset = new (0f, 0f);
    [SerializeField] private float offsetYForGroundVision = 1f;
    
    private Transform player;

    public void SetPlayer(Transform player)
    {
       this.player = player;
    }
    
    public void InstantMove()
    {
        MoveToPosition(instantMove:true);
    }
    
    private void Update()
    {
        MoveToPosition();
    }

    private void MoveToPosition(bool instantMove = false)
    {
        Vector3 position = transform.position;
        var target = new Vector3(player.position.x - offset.x, offset.y - offsetYForGroundVision, position.z);
        Vector3 currentPosition = instantMove ? target :
            Vector3.Lerp(position, target, damping * Time.deltaTime);
        transform.position = currentPosition;
    }
}
