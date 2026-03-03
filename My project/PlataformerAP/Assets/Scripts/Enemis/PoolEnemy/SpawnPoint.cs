using UnityEngine;

public enum EnemyTypeToSpawn
{
    Melee,
    Range, 
    Fly
}

public class SpawnPoint : MonoBehaviour
{
    [Header("Tipo de Enemigo")]
    [SerializeField] private EnemyTypeToSpawn enemyType;

    [Header("Configuración")]
    [SerializeField] private int maxEnemies = 3;
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private bool spawnOnStart = true;

    public EnemyTypeToSpawn EnemyType => enemyType;
    public int MaxEnemies => maxEnemies;
    public float SpawnInterval => spawnInterval;
    public bool SpawnOnStart => spawnOnStart;

}