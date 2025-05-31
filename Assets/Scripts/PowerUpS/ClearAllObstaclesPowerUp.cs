using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ClearAllObstaclesPowerUp : MonoBehaviour, IPowerUpEffect
{
	[Header("Clear Obstacles Settings")]
	[SerializeField] private AudioClip _clearObstaclesSound;
	[SerializeField] private GameObject _clearObstaclesEffectPrefab;

	[Header("UI Settings")]
	[SerializeField] private Image _powerUpIconUI;

	private AudioSource _audioSource;

	void Awake()
	{
		_audioSource = GetComponent<AudioSource>();
		if (_audioSource == null)
		{
			_audioSource = gameObject.AddComponent<AudioSource>();
		}
		_audioSource.playOnAwake = false;
		_audioSource.loop = false;
	}

	public void Activate(PlayerController player, PlayerPowerUpManager powerUpManager)
	{
		Debug.Log("ClearAllObstaclesPowerUp: Activate çaðrýldý. Tüm engeller temizleniyor.");

		if (_audioSource != null && _clearObstaclesSound != null)
		{
			_audioSource.PlayOneShot(_clearObstaclesSound);
		}
		else if (_clearObstaclesSound == null)
		{
			Debug.LogWarning("ClearAllObstaclesPowerUp: _clearObstaclesSound atanmamýþ. Sesi çalmak için atama yapýn.");
		}

		GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
		foreach (GameObject obstacle in obstacles)
		{
			if (_clearObstaclesEffectPrefab != null)
			{
				GameObject effectInstance = Instantiate(_clearObstaclesEffectPrefab, obstacle.transform.position, Quaternion.identity);
				ParticleSystem ps = effectInstance.GetComponent<ParticleSystem>();
				if (ps != null)
				{
					Destroy(effectInstance, ps.main.duration);
				}
				else
				{
					Destroy(effectInstance, 1.0f);
				}
			}
			else
			{
				Debug.LogWarning("ClearAllObstaclesPowerUp: _clearObstaclesEffectPrefab atanmamýþ. Efekti görmek için atama yapýn.");
			}
			Destroy(obstacle);
		}

		if (_powerUpIconUI != null)
		{
			_powerUpIconUI.gameObject.SetActive(true);
			StartCoroutine(HideIconAfterDelay(_powerUpIconUI, 1.5f));
		}
	}

	public void Deactivate()
	{
		Debug.Log("ClearAllObstaclesPowerUp: Deactivate çaðrýldý (tek seferlik efekttir, normalde kullanýlmaz).");
	}

	public bool IsActive()
	{
		return false;
	}

	private IEnumerator HideIconAfterDelay(Image icon, float delay)
	{
		yield return new WaitForSeconds(delay);
		if (icon != null)
		{
			icon.gameObject.SetActive(false);
		}
	}
}