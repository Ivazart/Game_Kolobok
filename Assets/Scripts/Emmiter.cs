using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emmiter : MonoBehaviour
{
    public GameObject bable;
    public GameObject emmitor;
    public int n;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        create();
        
    }

    void create()
    {
        int random = UnityEngine.Random.Range(1, n);
        if (random == 1)
        {
            var babbleC = Instantiate(bable);
            babbleC.transform.position = emmitor.transform.position;
            Destroy(babbleC, 17);
            
            
            
            
        }

    }


}   
