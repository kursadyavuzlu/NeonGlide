using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class JumpBoostPowerUp : MonoBehaviour, IPowerUpEffect
{
	[Header("Jump Boost Settings")]
	[SerializeField] private float _jumpBoostMultiplier = 1.5f;
	[SerializeField] private float _jumpBoostDuration = 7f;
	[SerializeField] private AudioClip _jumpBoostActivateSound;

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
			Debug.Log("Jump Boost Power-Up zaten aktif, süre sýfýrlandý.");
		}
		else
		{
			Debug.Log("Jump Boost Power-Up aktifleþti!");
			_isActive = true;

			if (_audioSource != null && _jumpBoostActivateSound != null)
			{
				_audioSource.PlayOneShot(_jumpBoostActivateSound);
			}
		}

		if (_powerUpIconUI != null)
		{
			_powerUpIconUI.gameObject.SetActive(true);
			_powerUpIconUI.fillAmount = 1f;
		}

		_powerUpDurationCoroutine = StartCoroutine(PowerUpDurationRoutine(_jumpBoostDuration));
	}

	public void Deactivate()
	{
		if (!_isActive) return;

		Debug.Log("Jump Boost Power-Up devre dýþý býrakýldý!");
		_isActive = false;

		if (_powerUpDurationCoroutine != null)
		{
			StopCoroutine(_powerUpDurationCoroutine);
			_powerUpDurationCoroutine = null;
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
	public float GetJumpBoostMultiplier()
	{
		return _jumpBoostMultiplier;
	}
}