using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class levelProgress : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform endLineTransform;
    [SerializeField] private Transform startLineTransform;

    public Slider slider;

    private Vector3 endLinePossition;
 
    private float fullDistance;


    // Start is called before the first frame update
    void Start()
    {
        endLinePossition = endLineTransform.position;
        fullDistance = Vector3.Distance(startLineTransform.position, endLinePossition);



    }

    // Update is called once per frame
    void Update()
    {
        float newDistance = GetDistance();
        slider.value = Mathf.InverseLerp( fullDistance, 0f, newDistance);
        
    }

    private float GetDistance ()
    {
        return Vector3.Distance(playerTransform.position, endLinePossition);

    }
}
