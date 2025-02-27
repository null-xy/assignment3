using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMotion : MonoBehaviour
{

    private Vector3 startPos;
    public float frequency, range = 1;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        float x = Mathf.Cos(Time.time * frequency) * range;
        float y = Mathf.Sin(Time.time * frequency) * range;
        float z = Mathf.Sin(Time.time * frequency) * range;

        x *= Mathf.Sin(Time.time);
        y *= Mathf.Sin(Time.time *3);
        z *= Mathf.Sin(Time.time * 2);

        transform.position = startPos + new Vector3(x, y, z);



        
    }
}
