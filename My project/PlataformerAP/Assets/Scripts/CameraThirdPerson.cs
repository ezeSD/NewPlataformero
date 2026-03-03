using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraThirdPerson : MonoBehaviour
{
    public Transform Player;
    float mouseX;
    float mouseY;
    public float mouseSensitivity = 100f;
    public float distanceFromPlayer = 2f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        cameraThirdPerson();
    }

    void cameraThirdPerson()
    {
        mouseX += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseY -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        mouseY = Mathf.Clamp(mouseY, -35, 60);
        Vector3 dir = new Vector3(0, 0, -distanceFromPlayer);
        Quaternion rotation = Quaternion.Euler(mouseY, mouseX, 0);
        transform.position = Player.position + rotation * dir;
        transform.LookAt(Player.position);

    }


}
