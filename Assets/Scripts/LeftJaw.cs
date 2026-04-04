using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LeftJaw : MonoBehaviour
{
    public float speed;
    public float degrees = 90;
    public bool rot;
    // Start is called before the first frame update
    void Start()
    {
        rot = false;
    }

    // Update is called once per frame
    void Update()
    {
        


        if (Input.GetKeyDown(KeyCode.A))
        {
            rot = true;
       

        }

        if (rot == true)
        {
            rote();

        }
      



    }

    private void rote()
    {
        Vector3 to = new Vector3(0, 0, degrees);
        transform.eulerAngles = Vector3.Lerp(transform.rotation.eulerAngles, to, Time.deltaTime * speed);


    }
}
