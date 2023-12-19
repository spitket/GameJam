using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_FakeEnemy : MonoBehaviour
{
    public GameManager manager;
    public Mng_SpawnEnemies spawnManager;
    
    public float speed = 5;
    public float defaultSpeed;

    private void Awake()
    {
        spawnManager = FindObjectOfType<Mng_SpawnEnemies>();
        manager = FindObjectOfType<GameManager>();
        speed = spawnManager.enemySpeed;
        defaultSpeed = speed;
    }

    private void Start()
    {
        Destroy(gameObject, 5f);
    }

    void FixedUpdate()   
    {
        if (manager.pause)
        {
            speed = 0;
        }
        else
        {
            speed = defaultSpeed;
        }
        
        transform.Translate(Vector3.forward * speed * 10 * Time.deltaTime);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Spawner"))
        {
            Destroy(gameObject);
        }
    }
}
