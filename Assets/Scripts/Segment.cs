using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Segment : MonoBehaviour
{
    public float AboveGroundSpeed;
    public float UnderGroundSpeed;
    private float _speed;
    public Transform target;
    private CharacterController _characterController;
    public float _distanceBeetweenSegments;



    // Start is called before the first frame update
    void Start()
    {
        _speed = AboveGroundSpeed;
        _characterController = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
        _characterController.OnStateChanged += OnStateChange;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float step = _speed * Time.deltaTime;
        if(Vector3.Distance(target.position, transform.position) > _distanceBeetweenSegments)
        {
            transform.Translate(Vector3.forward * _speed * Time.fixedDeltaTime);
        }
        transform.LookAt(target.position);
    }

    private void OnStateChange(CharacterController.State state)
    {
        
        if(state == CharacterController.State.AboveGround)
        {
            _speed = AboveGroundSpeed;
           
        }
        else
        {
            _speed = UnderGroundSpeed;
        }
    }

    private void OnDestroy()
    {
        _characterController.OnStateChanged -= OnStateChange;
    }
}
