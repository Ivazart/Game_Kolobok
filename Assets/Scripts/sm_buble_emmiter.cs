using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sm_buble_emmiter : MonoBehaviour
{
   
    public GameObject bable;
    public GameObject emmitor;
    public float time;
    public int n;
    public float scale;
    float randomXp;
    float randomSc;
    float x;
    float y;
   
    void Start()
    {
        InvokeRepeating("create", 2.0f, time);

    }

    // Update is called once per frame
    void Update()
    {
        int random = UnityEngine.Random.Range(1, n);
        if (random == 1)
        {
            create();
        }
        

    }

    void create()
    {
        randomSc = Random.Range(0.3f, 1f);
        randomXp = Random.Range(-10f, 10f); 
        var babbleC = Instantiate(bable);

        babbleC.transform.localScale = new Vector2(scale * randomSc, scale * randomSc);
        y = emmitor.transform.position.y;
        x = emmitor.transform.position.x + randomXp;
        babbleC.transform.position = new Vector2(x, y);
        Destroy(babbleC, 5);


    }
}
