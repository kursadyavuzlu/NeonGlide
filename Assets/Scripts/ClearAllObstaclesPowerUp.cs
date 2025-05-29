using UnityEngine;
using System.Collections;

public class ClearAllObstaclesPowerUp : MonoBehaviour, IPowerUpEffect
{
	// PlayerPowerUpManager'dan ta��nan de�i�kenler
	[SerializeField] private AudioClip _clearObstaclesSound;
	[SerializeField] private GameObject _clearObstaclesEffectPrefab;

	private AudioSource _clearObstaclesAudioSource;

	// Tek seferlik bir Power-Up oldu�u i�in aktiflik durumu ge�ici olacak
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
			Debug.LogWarning("ClearAllObstaclesPowerUp: _clearObstaclesSound atanmam��! Ses �almayabilir.");
		}
	}

	public void Activate(PlayerController player, PlayerPowerUpManager powerUpManager)
	{
		Debug.Log("ClearAllObstaclesPowerUp: Activate �a�r�ld�. T�m engeller temizleniyor.");
		_isActiveInstantEffect = true; // Efektin aktif oldu�u k�sa an� i�aretle

		// Ses �al
		if (_clearObstaclesAudioSource != null && _clearObstaclesSound != null)
		{
			_clearObstaclesAudioSource.PlayOneShot(_clearObstaclesSound);
		}
		else if (_clearObstaclesSound == null)
		{
			Debug.LogWarning("ClearAllObstaclesPowerUp: _clearObstaclesSound atanmam��. Sesi �almak i�in atama yap�n.");
		}

		// T�m engelleri bul ve yok et
		GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
		foreach (GameObject obstacle in obstacles)
		{
			// Yok etme efekti olu�tur
			if (_clearObstaclesEffectPrefab != null)
			{
				// Efektin yok olmas� i�in bir s�re belirle (partik�l sisteminin s�resi gibi)
				GameObject effectInstance = Instantiate(_clearObstaclesEffectPrefab, obstacle.transform.position, Quaternion.identity);
				ParticleSystem ps = effectInstance.GetComponent<ParticleSystem>();
				if (ps != null)
				{
					Destroy(effectInstance, ps.main.duration);
				}
				else
				{
					Destroy(effectInstance, 1.0f); // Varsay�lan s�re
				}
			}
			else
			{
				Debug.LogWarning("ClearAllObstaclesPowerUp: _clearObstaclesEffectPrefab atanmam��. Efekti g�rmek i�in atama yap�n.");
			}
			Destroy(obstacle);
		}

		// Tek seferlik bir efekt oldu�u i�in hemen devre d��� b�rak
		_isActiveInstantEffect = false;
	}

	// Bu metod tek seferlik efektler i�in do�rudan �a�r�lmayabilir, ancak aray�z gere�i burada
	public void Deactivate()
	{
		// Tek seferlik bir efekt oldu�u i�in genellikle bu metod do�rudan �a�r�lmaz
		Debug.Log("ClearAllObstaclesPowerUp: Deactivate �a�r�ld� (tek seferlik efekttir, normalde kullan�lmaz).");
	}

	// Tek seferlik efektler i�in her zaman false d�ner
	public bool IsActive()
	{
		return _isActiveInstantEffect; // Efektin �al��t��� an� kontrol etmek i�in kullanabiliriz
	}
}