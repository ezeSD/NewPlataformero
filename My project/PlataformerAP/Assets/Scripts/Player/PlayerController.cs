using System;
using UnityEngine;

public class PlayerController
{
    float xInput, yInput;
    public Action<float, float> OnMove;
    public Action<bool> OnJump;
    public Action<bool> OnRun;
    public Action OnAttack;
    bool isjumping;
    public PlayerController()
    {
        Debug.Log("PlayerController created");
    }


    public void ArtificialUpdate()
    {
        WalkInputs();
        HandleJump();
        HandleAttack();
    }


    void WalkInputs()
    {
        xInput = Input.GetAxis("Vertical"); 
        yInput = Input.GetAxis("Horizontal");     
        OnMove(xInput, yInput);
    }


    public bool getJumping()
    {
        isjumping = Input.GetKey(KeyCode.Space);
        OnJump(isjumping);
        return isjumping;
    }

    public bool setJumping(bool jump)
    {
        isjumping = jump;
        return isjumping;
    }

    void HandleJump()
    {
        isjumping = Input.GetKey(KeyCode.Space);
        if(isjumping)
        {
            OnJump?.Invoke(isjumping);

        }
    }


    void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnAttack?.Invoke();
        }
    }


}