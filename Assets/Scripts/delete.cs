using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class delete : MonoBehaviour
{
    //public big_bubble_cr emmitor;
    public GameObject flower;
    public float y;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        y = transform.position.y;
        if (y <= -16)
        {

            Destroy(flower);
            //emmitor.crateB();
           

        }
        
    }
}
