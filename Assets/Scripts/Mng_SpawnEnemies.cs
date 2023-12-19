using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Mng_SpawnEnemies : MonoBehaviour
{
    private GameManager manager;

    public List<Transform> direction;
    public List<int> patronASeguir;

    public List<GameObject> enemyPrefabs; public GameObject fakeEnemy;

    public int currentEnemy = 0;
    public int currentEnemyDirection;
    public int currentLevelEnemies = 10;

    public float enemySpeed = 5f;
    public float enemySpeedSave;
    public float spawnTimeMultiplier = 1f;

    public GameObject a1;
    public bool a1Active = false;

    private void Awake()
    {
        manager = FindObjectOfType<GameManager>();

        a1.SetActive(false);

        GenerateList();
    }

    private void Start()
    {
        StartLevel();
    }

    public void SpawnEnemy()
    {
        if (currentEnemy >= patronASeguir.Count) { currentEnemy = 0; }

        currentEnemyDirection = patronASeguir[currentEnemy % patronASeguir.Count];
        Vector3 spawnPosition = direction[currentEnemyDirection].position;
        Vector3 targetPosition = Vector3.zero;

        Quaternion rotation = Quaternion.LookRotation(targetPosition - spawnPosition);

        GameObject selectedEnemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];

        Instantiate(selectedEnemyPrefab, spawnPosition, rotation);
        currentEnemy++;
    }

    public void GenerateList()
    {
        for (int i = 0; i < currentLevelEnemies; i++)
        {
            int randomNumber = Random.Range(0, direction.Count);
            patronASeguir.Add(randomNumber);
        }
    } //Genera la lista de direcciones que seguirá el nivel.

    private IEnumerator StartLevelCorroutine()
    {
        while (currentEnemy < currentLevelEnemies)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(1 / spawnTimeMultiplier);
        }
        NextLevel();
    }

    public void NextLevel()
    {
        manager.currentLevel++;
        currentLevelEnemies += 1;
        enemySpeed += 0.5f;
        spawnTimeMultiplier += 0.1f;

        manager.ShowText("SIGUIENTE NIVEL!!!");

        if (manager.currentLevel > 5 && !a1Active)
        {
            a1.SetActive(true);
            a1Active = true;
        }

        StartLevel();
    }

    public void RewindLevel()
    {
        StopAllCoroutines();
        StartCoroutine(RewindLevelCorroutine());
    }

    public void FinishRewind()
    {
        StopAllCoroutines();
        manager.StopRewind();
        StartCoroutine(StartLevelCorroutine());
    }

    public void StartLevel()
    {
        currentEnemy = 0;
        manager.StopRewind(); //regresa la pantalla a color y recupera movimientos

        patronASeguir.Clear();
        GenerateList();
        StartCoroutine(StartLevelCorroutine());
    }

    public void Pause()
    {
        enemySpeedSave = enemySpeed;
        enemySpeed = 0;
        StopAllCoroutines();
    }

    public void UnPause()
    {
        enemySpeed = enemySpeedSave;
        StartCoroutine(StartLevelCorroutine());
    }

    private IEnumerator RewindLevelCorroutine() //genera los falsos enemigos devuelta a las posiciónes originales
    {
        while (currentEnemy > 0)
        {
            SpawnFakeEnemy();
            yield return new WaitForSeconds((1 / spawnTimeMultiplier) / 10);
        }

        FinishRewind();
    }

    public void SpawnFakeEnemy()
    {
        if (currentEnemy >= patronASeguir.Count) { currentEnemy = 0; }

        currentEnemyDirection = patronASeguir[currentEnemy % patronASeguir.Count];
        Vector3 spawnPosition = Vector3.zero;
        Vector3 targetPosition = direction[currentEnemyDirection].position;

        Quaternion rotation = Quaternion.LookRotation(targetPosition - spawnPosition);

        Instantiate(fakeEnemy, spawnPosition, rotation);
        currentEnemy--;
    }
}
