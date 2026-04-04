using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CameraMov : MonoBehaviour
{
    public Ball ball;
    public float damping = 1.5f;
    public Vector2 offset = new Vector2(0f, 0f);
    private Transform player;
    public GameManager gm;
    
   
    // Start is called before the first frame update

    public void FindPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        transform.position = new Vector3(player.position.x - offset.x, offset.y, transform.position.z);

    }   

    void Start()
    {
       
        FindPlayer();

    }

   

        // Update is called once per frame
    void Update()
    {

        int playerX = Mathf.RoundToInt(player.position.x);
        int cameraX = Mathf.RoundToInt(transform.position.x);

     
        if (gm.isDragging == false)
        {
            Vector3 target;
            target = new Vector3(player.position.x - offset.x, offset.y, transform.position.z);
            Vector3 currentPosition = Vector3.Lerp(transform.position, target, damping * Time.deltaTime);
            transform.position = currentPosition;

        }




       
    }
}
