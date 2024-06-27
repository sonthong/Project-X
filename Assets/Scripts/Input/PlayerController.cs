using System;
using System.Collections;
using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;

public class PlayerController : ValidatedMonoBehaviour
{
    [Header("Reference")]
    [SerializeField, Anywhere] InputReader input;

    [Header("Settings")] 
    [SerializeField] float moveSpeed;
    [SerializeField] Vector2 moveDirection;
    public float acceleration = 1.0f;
    public float maxSpeed = 10.0f;
    public float deceleration = 2.0f;
    
    private float _currentVelocity = 0.0f;

    void Start()
    {
        input.EnablePlayerActions();
    } 

    private void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        Debug.Log(input.mousePosition.x * 2048f);
        Debug.Log(Screen.width / 2f);
        if (input.mousePosition.x * 2048f >= Screen.width / 2f)
        {
            moveDirection = Vector2.right;
            _currentVelocity = Mathf.Clamp(_currentVelocity + acceleration * Time.deltaTime, -maxSpeed, maxSpeed);

        }
        else
        {
            moveDirection = Vector2.left;
            _currentVelocity = Mathf.Clamp(_currentVelocity - acceleration * Time.deltaTime, -maxSpeed, maxSpeed);

        };
    }
}
