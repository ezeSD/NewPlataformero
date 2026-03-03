using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWeapon : MonoBehaviour
{
   [SerializeField] private float rotationSpeed = 90f; 
    [SerializeField] private Vector3 rotationAxis = Vector3.up; 
    void Update()
    {
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
    }

}
