using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmmiterSt : MonoBehaviour
{
    public GameObject bable;
    public GameObject emmitor;
    public float time;
    public int n;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("create", 2.0f, time);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void create()
    {
        var babbleC = Instantiate(bable);
        babbleC.transform.position = emmitor.transform.position;
        Destroy(babbleC, 17);
       

    }
}
