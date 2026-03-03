using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerLife : MonoBehaviour, IDamageable
{
    [Header("Configuración de Vida")]
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private int vidaPorCorazon = 100;

    [Header("Sistema de Vidas")]
    [SerializeField] private int maxLives = 3;
    [SerializeField] private int currentLives;

    [Header("UI References")]
    [SerializeField] private Image healImage;
    [SerializeField] private TextMeshProUGUI healText;
    [SerializeField] private Image[] heartImages;

    private bool isInmune = false;
    private Rigidbody _rb;
    PlayerView playerView;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        playerView = new PlayerView();
    }

    private void Start()
    {
        currentHealth = _maxHealth;
        currentLives = maxLives;

        if (CheckpointManager.Instance != null)
        {
            CheckpointManager.Instance.SetInitialPosition(transform.position);
        }

        playerView.setHealFeedback(healText, healImage, currentHealth);
    }

    private void LoseHeart()
    {
        currentLives--;
        heartImages[currentLives].enabled = false;

        if (currentLives <= 0)
        {
            GameOver();
        }
        else
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        Vector3 respawnPos = Vector3.zero;

        if (CheckpointManager.Instance != null)
        {
            respawnPos = CheckpointManager.Instance.GetRespawnPosition();
        }

        transform.position = respawnPos;

        if (_rb != null)
        {
            _rb.velocity = Vector3.zero;
        }

        currentHealth = _maxHealth;
        playerView.setHealFeedback(healText, healImage, currentHealth);
    }

    private void GameOver()
    {
        SceneManager.LoadScene("Lose");
        Destroy(gameObject);
    }

    #region IDamageable Implementation
    public void TakeDamage(int amount)
    {
        if (isInmune) return;

        if (currentHealth > 0)
        {
            currentHealth -= amount;
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            LoseHeart();
        }
        playerView.setHealFeedback(healText, healImage, currentHealth);
    }
    #endregion

    #region SetFunctions
    public void SetInmune(bool value)
    {
        isInmune = value;
    }
    #endregion

    #region GetFunctions
    public int GetCurrentLives()
    {
        return currentLives;
    }
    #endregion

    public void HandleFall()
    {
        LoseHeart();
    }
}