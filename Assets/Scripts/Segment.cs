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
    private Material _material;
    private Material _playerMaterial;




    // Start is called before the first frame update
    void Start()
    {
        _speed = AboveGroundSpeed;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        _characterController = player.GetComponent<CharacterController>();
        _characterController.OnStateChanged += OnStateChange;
        _material = GetComponentInChildren<MeshRenderer>().material;
        _playerMaterial = player.GetComponentInChildren<MeshRenderer>().material;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float step = _speed * Vector3.Distance(target.position, transform.position);
        if (Vector3.Distance(target.position, transform.position) > _distanceBeetweenSegments)
        {
            transform.Translate(Vector3.forward * step * Time.fixedDeltaTime);
        }
        transform.LookAt(target.position);
        _material.color = _playerMaterial.color;
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
