using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class big_bubble_cr : MonoBehaviour
{
    public GameObject bubble;
    public GameObject emmitor;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void crateB()
    {
        var bubbleC = Instantiate(bubble);
        bubbleC.transform.position = emmitor.transform.position;


    }



}
