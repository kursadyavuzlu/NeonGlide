using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TimeSlowdownPowerUp : MonoBehaviour, IPowerUpEffect
{
	[Header("Time Slowdown Settings")]
	[SerializeField] private float _timeSlowdownFactor = 0.5f;
	[SerializeField] private float _timeSlowdownDuration = 6f;
	[SerializeField] private AudioClip _slowdownActivateSound;

	[Header("UI Settings")]
	[SerializeField] private Image _powerUpIconUI;

	private PlayerController _playerController;
	private PlayerPowerUpManager _powerUpManager;
	private AudioSource _audioSource;

	private bool _isActive = false;
	private Coroutine _powerUpDurationCoroutine;

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
		_playerController = player;
		_powerUpManager = powerUpManager;

		if (_isActive)
		{
			if (_powerUpDurationCoroutine != null)
			{
				StopCoroutine(_powerUpDurationCoroutine);
			}
			Debug.Log("Time Slowdown Power-Up zaten aktif, süre sýfýrlandý.");
		}
		else
		{
			Debug.Log("Time Slowdown Power-Up aktifleþti! Zaman Ölçeði: " + _timeSlowdownFactor);
			_isActive = true;

			if (GameManager.Instance != null)
			{
				GameManager.Instance.SetTimeScale(_timeSlowdownFactor);
			}
			else
			{
				Debug.LogWarning("TimeSlowdownPowerUp: GameManager.Instance bulunamadý! Zaman yavaþlatma uygulanamadý.");
			}

			if (_audioSource != null && _slowdownActivateSound != null)
			{
				_audioSource.PlayOneShot(_slowdownActivateSound);
			}
		}

		if (_powerUpIconUI != null)
		{
			_powerUpIconUI.gameObject.SetActive(true);
			_powerUpIconUI.fillAmount = 1f;
		}

		_powerUpDurationCoroutine = StartCoroutine(PowerUpDurationRoutine(_timeSlowdownDuration));
	}

	public void Deactivate()
	{
		if (!_isActive) return;

		Debug.Log("Time Slowdown Power-Up devre dýþý býrakýldý! Zaman Ölçeði sýfýrlandý.");
		_isActive = false;

		if (_powerUpDurationCoroutine != null)
		{
			StopCoroutine(_powerUpDurationCoroutine);
			_powerUpDurationCoroutine = null;
		}

		if (GameManager.Instance != null)
		{
			GameManager.Instance.ResetTimeScale();
		}

		if (_powerUpIconUI != null)
		{
			_powerUpIconUI.gameObject.SetActive(false);
		}
	}

	public bool IsActive()
	{
		return _isActive;
	}

	private IEnumerator PowerUpDurationRoutine(float duration)
	{
		float timer = duration;

		while (timer > 0)
		{
			timer -= Time.deltaTime;

			if (_powerUpIconUI != null)
			{
				_powerUpIconUI.fillAmount = timer / duration;
			}
			yield return null;
		}
		Deactivate();
	}

	void OnDestroy()
	{
		if (_powerUpDurationCoroutine != null)
		{
			StopCoroutine(_powerUpDurationCoroutine);
		}
	}
}