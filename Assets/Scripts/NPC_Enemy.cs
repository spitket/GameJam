using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Enemy : MonoBehaviour
{
    public GameManager manager;
    public Mng_SpawnEnemies spawnManager;
    public GameObject explosionPrefab1;
    public GameObject explosionPrefab2;
    
    
    public float speed = 5;
    public float defaultSpeed;

    private void Awake()
    {
        spawnManager = FindObjectOfType<Mng_SpawnEnemies>();
        manager = FindObjectOfType<GameManager>();
        speed = spawnManager.enemySpeed;
        defaultSpeed = speed;
    }

    public void Kboom()
    {
        Instantiate(explosionPrefab1, transform.position, Quaternion.identity);
        Instantiate(explosionPrefab2, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void Start()
    {
        Destroy(gameObject, 5f);
    }

    void FixedUpdate()   
    {
        if (manager.pause)  { speed = 0; }
        else  {  speed = defaultSpeed;  }

        if (manager.rewind) { Kboom(); }
    
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bloqueo"))
        {
            manager.CameraShake(0.5f);
            manager.chargeRewind();
            Instantiate(explosionPrefab1, transform.position, Quaternion.identity);
            Instantiate(explosionPrefab2, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        if (other.CompareTag("Player"))
        {
            manager.CameraShake(4);
            manager.StartRewind();
            Destroy(gameObject);
        }
    }
}
