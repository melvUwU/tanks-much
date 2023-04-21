using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    public float startingHealth = 100f;
    public Slider slider;
    public Image fillImage;
    public Color fullHealthColor = Color.green;
    public Color zeroHealthColor = Color.red;
    public GameObject explosionPrefab;

    private AudioSource _explosionAudio;
    private ParticleSystem _explosionParticles;
    private float _currentHealth;
    private bool _isDead;

    private void Awake()
    {
        _explosionParticles = Instantiate(explosionPrefab).GetComponent<ParticleSystem>();
        _explosionAudio = _explosionParticles.GetComponent<AudioSource>();

        _explosionParticles.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _currentHealth = startingHealth;
        _isDead = false;

        SetHealthUI();
    }

    public void TakeDamage(float amount)
    {
        // Adjust the tank's current health, update the UI based on the new health and check whether or not the tank is dead.
        _currentHealth -= amount;

        SetHealthUI();

        if (_currentHealth <= 0f && !_isDead)
            OnDeath();
    }

    private void SetHealthUI()
    {
        // Adjust the value and colour of the slider.
        slider.value = _currentHealth;
        fillImage.color = Color.Lerp(zeroHealthColor, fullHealthColor, _currentHealth / startingHealth);
    }

    private void OnDeath()
    {
        // Play the effects for the death of the tank and deactivate it.
        _isDead = true;

        _explosionParticles.transform.position = this.transform.position;
        _explosionParticles.gameObject.SetActive(true);
        _explosionParticles.Play();

        _explosionAudio.Play();

        this.gameObject.SetActive(false);
    }
}