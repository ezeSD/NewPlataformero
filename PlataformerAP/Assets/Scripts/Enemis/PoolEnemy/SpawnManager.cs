using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Factories")]
    [SerializeField] private FactoryMelee meleeFactory;
    [SerializeField] private FactoryRange rangeFactory;
    [SerializeField] private FactoryFly flyFactory;

    [Header("Spawn Points")]
    [SerializeField] private List<SpawnPoint> spawnPoints;

    [Header("Configuración Global")]
    [SerializeField] private Transform playerTarget; 

    private void Awake()
    {
        if (spawnPoints.Count == 0)
        {
            spawnPoints.AddRange(FindObjectsOfType<SpawnPoint>());
        }

        if (playerTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTarget = player.transform;
        }
    }

    private void Start()
    {
        SpawnAllEnemies();
    }


    public void SpawnAllEnemies()
    {
        foreach (SpawnPoint spawnPoint in spawnPoints)
        {
            if (spawnPoint.SpawnOnStart)
            {
                SpawnEnemiesAtPoint(spawnPoint);
            }
        }
    }

    public void SpawnEnemiesAtPoint(SpawnPoint spawnPoint)
    {
        Factory<Enemi> factory = GetFactoryByType(spawnPoint.EnemyType);

        if (factory == null)return;

        for (int i = 0; i < spawnPoint.MaxEnemies; i++)
        {
            SpawnSingleEnemy(factory, spawnPoint.transform.position, spawnPoint.EnemyType);
        }
    }

    public Enemi SpawnSingleEnemy(Factory<Enemi> factory, Vector3 position, EnemyTypeToSpawn type)
    {
        Enemi newEnemy = factory.Create();

        newEnemy.transform.position = position;
        newEnemy.transform.rotation = Quaternion.identity;
        newEnemy.Initialize(GetFactoryByTypeAsIA(type), playerTarget);
        return newEnemy;
    }


    public void ReturnEnemy(Enemi enemy, EnemyTypeToSpawn type)
    {
        Factory<Enemi> factory = GetFactoryByType(type);
        factory.Return(enemy);
    }

    private Factory<Enemi> GetFactoryByType(EnemyTypeToSpawn type)
    {
        return type switch
        {
            EnemyTypeToSpawn.Melee => meleeFactory,EnemyTypeToSpawn.Range => rangeFactory, EnemyTypeToSpawn.Fly => flyFactory,_ => null
        };
    }

    private Factory<IAEnemy> GetFactoryByTypeAsIA(EnemyTypeToSpawn type)
    {
        return null; 
    }

    public void AddSpawnPoint(SpawnPoint newPoint)
    {
        if (!spawnPoints.Contains(newPoint))
        {
            spawnPoints.Add(newPoint);
        }
    }

    public void RemoveSpawnPoint(SpawnPoint pointToRemove)
    {
        if (spawnPoints.Contains(pointToRemove))
        {
            spawnPoints.Remove(pointToRemove);
        }
    }
}