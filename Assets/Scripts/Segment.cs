using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Segment : MonoBehaviour
{
    public float speed;
    public Transform Target;

    [SerializeField]
    private float _distanceBeetweenSegments = 2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float step = speed * Time.deltaTime;
        Vector3 v = Target.position - transform.position;
        v.Normalize();
        v *= _distanceBeetweenSegments;
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(Target.position.x - v.x, Target.position.y - v.y, Target.position.z - v.z), step);
    }
}
