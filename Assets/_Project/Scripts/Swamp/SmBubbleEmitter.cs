using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SmBubbleEmitter : MonoBehaviour
{
    [SerializeField] private GameObject bubblePrefab;
    [SerializeField] private float time;
    [SerializeField] private int n;
    [SerializeField] private float scale;
    
    private float randomXp;
    private float randomSc;
    private float x;
    private float y;
    private GameObject parent;
    
    private void Awake()
    {
        parent = GameObject.FindWithTag("Temporal");
    }
    
    private void Start()
    {
        InvokeRepeating(nameof(Create), 2.0f, time);
    }

    private void Update()
    {
        int random = Random.Range(1, n);
        if (random == 1)
        {
            Create();
        }
    }

    private void Create()
    {
        randomSc = Random.Range(0.3f, 1f);
        randomXp = Random.Range(-10f, 10f); 
        var babbleC = Instantiate(bubblePrefab, parent.transform);

        babbleC.transform.localScale = new Vector2(scale * randomSc, scale * randomSc);
        var position = transform.position;
        y = position.y;
        x = position.x + randomXp;
        babbleC.transform.position = new Vector2(x, y);
        Destroy(babbleC, 5f);
    }
}
