using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShrinkPowerUp : MonoBehaviour, IPowerUpEffect
{
	[Header("Shrink Settings")]
	[SerializeField] private float _shrinkScale = 0.5f;
	[SerializeField] private float _shrinkDuration = 5f;
	[SerializeField] private float _scaleChangeSpeed = 5f;
	[SerializeField] private AudioClip _shrinkActivateSound;

	[Header("UI Settings")]
	[SerializeField] private Image _powerUpIconUI;

	private PlayerController _playerController;
	private PlayerPowerUpManager _powerUpManager;
	private AudioSource _audioSource;

	private Vector3 _playerOriginalScale;
	private Coroutine _powerUpDurationCoroutine;
	private Coroutine _scaleChangeCoroutine;
	private bool _isActive = false;

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

	void Start()
	{
		if (_playerController == null)
		{
			_playerController = FindAnyObjectByType<PlayerController>();
		}

		if (_playerController != null)
		{
			_playerOriginalScale = _playerController.transform.localScale;
		}
		else
		{
			Debug.LogError("ShrinkPowerUp: PlayerController bulunamadý!");
		}
	}

	public void Activate(PlayerController player, PlayerPowerUpManager powerUpManager)
	{
		_playerController = player;
		_powerUpManager = powerUpManager;

		if (_playerOriginalScale == Vector3.zero && _playerController != null)
		{
			_playerOriginalScale = _playerController.transform.localScale;
		}

		if (_isActive)
		{
			if (_powerUpDurationCoroutine != null)
			{
				StopCoroutine(_powerUpDurationCoroutine);
			}
			if (_scaleChangeCoroutine != null)
			{
				StopCoroutine(_scaleChangeCoroutine);
				_playerController.transform.localScale = _playerOriginalScale * _shrinkScale;
			}
			Debug.Log("Shrink Power-Up zaten aktif, süre sýfýrlandý.");
		}
		else
		{
			Debug.Log("Shrink Power-Up aktifleþti! Oyuncu küçülüyor.");
			_isActive = true;

			if (_audioSource != null && _shrinkActivateSound != null)
			{
				_audioSource.PlayOneShot(_shrinkActivateSound);
			}
		}

		if (_powerUpIconUI != null)
		{
			_powerUpIconUI.gameObject.SetActive(true);
			_powerUpIconUI.fillAmount = 1f;
		}

		_scaleChangeCoroutine = StartCoroutine(ScalePlayer(_playerOriginalScale * _shrinkScale, _playerOriginalScale, true, _shrinkDuration));
	}

	public void Deactivate()
	{
		if (!_isActive) return;

		Debug.Log("Shrink Power-Up devre dýþý býrakýldý! Oyuncu normale dönüyor.");
		_isActive = false;

		if (_powerUpDurationCoroutine != null)
		{
			StopCoroutine(_powerUpDurationCoroutine);
			_powerUpDurationCoroutine = null;
		}
		if (_scaleChangeCoroutine != null)
		{
			StopCoroutine(_scaleChangeCoroutine);
			_scaleChangeCoroutine = null;
		}

		if (_playerController != null)
		{
			_playerController.transform.localScale = _playerOriginalScale;
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

	private IEnumerator ScalePlayer(Vector3 targetScale, Vector3 originalScale, bool isShrinking, float durationRemaining)
	{
		Vector3 currentScale = _playerController.transform.localScale;
		float elapsedLerpTime = 0f;
		float lerpDuration = 1f / _scaleChangeSpeed;

		while (elapsedLerpTime < lerpDuration)
		{
			if (_playerController == null) yield break;
			_playerController.transform.localScale = Vector3.Lerp(currentScale, targetScale, elapsedLerpTime / lerpDuration);
			elapsedLerpTime += Time.deltaTime;
			yield return null;
		}
		if (_playerController != null)
		{
			_playerController.transform.localScale = targetScale;
		}

		if (isShrinking)
		{
			Debug.Log("ShrinkPowerUp: Oyuncu küçüldü. Küçülme süresi bekleniyor...");
			_powerUpDurationCoroutine = StartCoroutine(PowerUpDurationRoutine(durationRemaining));
		}
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
		if (_playerController != null && _isActive)
		{
			_playerController.transform.localScale = _playerOriginalScale;
		}

		if (_powerUpDurationCoroutine != null)
		{
			StopCoroutine(_powerUpDurationCoroutine);
		}
		if (_scaleChangeCoroutine != null)
		{
			StopCoroutine(_scaleChangeCoroutine);
		}
	}
}