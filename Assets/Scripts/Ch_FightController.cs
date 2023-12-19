using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ch_FightController : MonoBehaviour
{
    public Transform player;
    private Quaternion targetRotation;
    public float rotateSpeed = 360f; // Velocidad de rotación para alinear al jugador
    public float rotationSpeed = 720f; // Velocidad de giro del personaje

    private GameManager manager;

    private void Awake()
    {
        manager = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        targetRotation = transform.rotation;
    }

    private void Update()
    {
        if (!manager.ignoreInputs) // Revisa si el juego está en pausa o rewind
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            if (h != 0 || v != 0)
            {
                Vector3 direction = new Vector3(h, 0, v).normalized;
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                targetRotation = Quaternion.Euler(0, targetAngle, 0);
            }

            if (player.rotation != targetRotation)
            {
                // Suaviza la rotación del jugador hacia la dirección objetivo
                player.rotation = Quaternion.RotateTowards(player.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }

}