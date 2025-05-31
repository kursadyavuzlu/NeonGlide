using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShieldPowerUp : MonoBehaviour, IPowerUpEffect
{
	[Header("Shield Settings")]
	[SerializeField] private float _shieldDuration = 5f;
	[SerializeField] private GameObject _shieldEffectPrefab;
	[SerializeField] private AudioClip _shieldActivateSound;

	[Header("UI Settings")]
	[SerializeField] private Image _powerUpIconUI;

	private PlayerController _playerController;
	private PlayerPowerUpManager _powerUpManager;
	private AudioSource _audioSource;
	private GameObject _activeShieldEffectInstance;

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

		if (_shieldEffectPrefab != null)
		{
			_activeShieldEffectInstance = Instantiate(_shieldEffectPrefab, transform);
			_activeShieldEffectInstance.transform.localPosition = Vector3.zero;
			_activeShieldEffectInstance.SetActive(false);
		}
		else
		{
			Debug.LogWarning("ShieldPowerUp: _shieldEffectPrefab atanmamýþ! Kalkan efekti çalýþmayacak.");
		}
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
			Debug.Log("Shield Power-Up zaten aktif, süre sýfýrlandý.");
		}
		else
		{
			Debug.Log("Shield Power-Up aktifleþti!");
			_isActive = true;

			if (_activeShieldEffectInstance != null)
			{
				_activeShieldEffectInstance.transform.SetParent(_playerController.transform);
				_activeShieldEffectInstance.transform.localPosition = Vector3.zero;
				_activeShieldEffectInstance.SetActive(true);
			}

			if (_audioSource != null && _shieldActivateSound != null)
			{
				_audioSource.PlayOneShot(_shieldActivateSound);
			}
		}

		if (_powerUpIconUI != null)
		{
			_powerUpIconUI.gameObject.SetActive(true);
			_powerUpIconUI.fillAmount = 1f;
		}

		_powerUpDurationCoroutine = StartCoroutine(PowerUpDurationRoutine(_shieldDuration));
	}

	public void Deactivate()
	{
		if (!_isActive) return;

		Debug.Log("Shield Power-Up devre dýþý býrakýldý!");
		_isActive = false;

		if (_powerUpDurationCoroutine != null)
		{
			StopCoroutine(_powerUpDurationCoroutine);
			_powerUpDurationCoroutine = null;
		}

		if (_activeShieldEffectInstance != null)
		{
			_activeShieldEffectInstance.SetActive(false);
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