using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetTrack : MonoBehaviour
{

    public Transform trackee;
    public float wiggleRange;
    public float wiggleSpeed;
    public Vector3 posOffset;
    private float randSeed;
    

    // Start is called before the first frame update
    void Start()
    {
        randSeed = gameObject.GetInstanceID();
        
    }

    // Update is called once per frame
    void Update()
    {

        transform.position = trackee.position + posOffset;
        transform.rotation = trackee.rotation;

        

        transform.Translate(Vector3.up * Mathf.Sin(Time.time*wiggleSpeed + randSeed) * wiggleRange);
    }
}
