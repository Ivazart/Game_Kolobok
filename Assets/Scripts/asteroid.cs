using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class asteroid : MonoBehaviour
{
    
    public float speedx;
    public float speedy;
   

    // Start is called before the first frame update
    void Start()
    {
     

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(speedy, speedx, 0 * Time.deltaTime);
      
    }
}
