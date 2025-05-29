using UnityEngine;
using System.Collections;

public class ClearAllObstaclesPowerUp : MonoBehaviour, IPowerUpEffect
{
	// PlayerPowerUpManager'dan taþýnan deðiþkenler
	[SerializeField] private AudioClip _clearObstaclesSound;
	[SerializeField] private GameObject _clearObstaclesEffectPrefab;

	private AudioSource _clearObstaclesAudioSource;

	// Tek seferlik bir Power-Up olduðu için aktiflik durumu geçici olacak
	private bool _isActiveInstantEffect = false;


	void Awake()
	{
		// Kendi AudioSource'unu ekle
		_clearObstaclesAudioSource = gameObject.AddComponent<AudioSource>();
		_clearObstaclesAudioSource.playOnAwake = false;
		_clearObstaclesAudioSource.loop = false;
		if (_clearObstaclesSound != null)
		{
			_clearObstaclesAudioSource.clip = _clearObstaclesSound;
		}
		else
		{
			Debug.LogWarning("ClearAllObstaclesPowerUp: _clearObstaclesSound atanmamýþ! Ses çalmayabilir.");
		}
	}

	public void Activate(PlayerController player, PlayerPowerUpManager powerUpManager)
	{
		Debug.Log("ClearAllObstaclesPowerUp: Activate çaðrýldý. Tüm engeller temizleniyor.");
		_isActiveInstantEffect = true; // Efektin aktif olduðu kýsa aný iþaretle

		// Ses çal
		if (_clearObstaclesAudioSource != null && _clearObstaclesSound != null)
		{
			_clearObstaclesAudioSource.PlayOneShot(_clearObstaclesSound);
		}
		else if (_clearObstaclesSound == null)
		{
			Debug.LogWarning("ClearAllObstaclesPowerUp: _clearObstaclesSound atanmamýþ. Sesi çalmak için atama yapýn.");
		}

		// Tüm engelleri bul ve yok et
		GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
		foreach (GameObject obstacle in obstacles)
		{
			// Yok etme efekti oluþtur
			if (_clearObstaclesEffectPrefab != null)
			{
				// Efektin yok olmasý için bir süre belirle (partikül sisteminin süresi gibi)
				GameObject effectInstance = Instantiate(_clearObstaclesEffectPrefab, obstacle.transform.position, Quaternion.identity);
				ParticleSystem ps = effectInstance.GetComponent<ParticleSystem>();
				if (ps != null)
				{
					Destroy(effectInstance, ps.main.duration);
				}
				else
				{
					Destroy(effectInstance, 1.0f); // Varsayýlan süre
				}
			}
			else
			{
				Debug.LogWarning("ClearAllObstaclesPowerUp: _clearObstaclesEffectPrefab atanmamýþ. Efekti görmek için atama yapýn.");
			}
			Destroy(obstacle);
		}

		// Tek seferlik bir efekt olduðu için hemen devre dýþý býrak
		_isActiveInstantEffect = false;
	}

	// Bu metod tek seferlik efektler için doðrudan çaðrýlmayabilir, ancak arayüz gereði burada
	public void Deactivate()
	{
		// Tek seferlik bir efekt olduðu için genellikle bu metod doðrudan çaðrýlmaz
		Debug.Log("ClearAllObstaclesPowerUp: Deactivate çaðrýldý (tek seferlik efekttir, normalde kullanýlmaz).");
	}

	// Tek seferlik efektler için her zaman false döner
	public bool IsActive()
	{
		return _isActiveInstantEffect; // Efektin çalýþtýðý aný kontrol etmek için kullanabiliriz
	}
}