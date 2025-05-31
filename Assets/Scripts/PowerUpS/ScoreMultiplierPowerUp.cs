using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreMultiplierPowerUp : MonoBehaviour, IPowerUpEffect
{
	[Header("Score Multiplier Settings")]
	[SerializeField] private int _scoreMultiplierAmount = 2;
	[SerializeField] private float _scoreMultiplierDuration = 7f;
	[SerializeField] private AudioClip _multiplierActivateSound;

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
			Debug.Log("Score Multiplier Power-Up zaten aktif, süre sýfýrlandý.");
		}
		else
		{
			Debug.Log("Score Multiplier Power-Up aktifleþti! Çarpan: " + _scoreMultiplierAmount + "x");
			_isActive = true;

			if (GameManager.Instance != null)
			{
				GameManager.Instance.SetScoreMultiplier(_scoreMultiplierAmount);
			}
			else
			{
				Debug.LogWarning("ScoreMultiplierPowerUp: GameManager.Instance bulunamadý! Puan çarpaný uygulanamadý.");
			}

			if (_audioSource != null && _multiplierActivateSound != null)
			{
				_audioSource.PlayOneShot(_multiplierActivateSound);
			}
		}

		if (_powerUpIconUI != null)
		{
			_powerUpIconUI.gameObject.SetActive(true);
			_powerUpIconUI.fillAmount = 1f;
		}

		_powerUpDurationCoroutine = StartCoroutine(PowerUpDurationRoutine(_scoreMultiplierDuration));
	}

	public void Deactivate()
	{
		if (!_isActive) return;

		Debug.Log("Score Multiplier Power-Up devre dýþý býrakýldý! Çarpan sýfýrlandý.");
		_isActive = false;

		if (_powerUpDurationCoroutine != null)
		{
			StopCoroutine(_powerUpDurationCoroutine);
			_powerUpDurationCoroutine = null;
		}

		if (GameManager.Instance != null)
		{
			GameManager.Instance.ResetScoreMultiplier();
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