using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[Header("Effects")]
	[SerializeField] private GameObject _explosionAnimationPrefab;
	[SerializeField] private GameObject _powerUpCollectEffectPrefab;

	[Header("Audio Settings")]
	[SerializeField] private AudioClip _jumpSound;
	[SerializeField] private AudioClip _collectPowerUpSound;
	[SerializeField] private AudioClip _explosionSound;

	[SerializeField] private float _moveSpeed = 5f;
	[SerializeField] private float _jumpForce = 10f;

	private AudioSource _jumpAudioSource;
	private AudioSource _collectAudioSource;
	private AudioSource _explosionAudioSource;

	private Rigidbody2D _rb;
	private SpriteRenderer _spriteRenderer;
	private Collider2D _playerCollider;
	private Vector3 _originalScale;

	private bool _jumpInputReceived = false;
	private bool _isGameOver = false;

	private float _screenHeight;

	private PlayerPowerUpManager _playerPowerUpManager;

	private float _jumpInputBufferTime = 0.15f;
	private float _lastUnpausedTime;

	void Awake()
	{
		_rb = GetComponent<Rigidbody2D>();
		_spriteRenderer = GetComponent<SpriteRenderer>();
		_playerCollider = GetComponent<Collider2D>();
		_originalScale = transform.localScale;

		_jumpAudioSource = SetupAudioSource(_jumpSound);
		_collectAudioSource = SetupAudioSource(_collectPowerUpSound);
		_explosionAudioSource = SetupAudioSource(_explosionSound);

		_playerPowerUpManager = GetComponent<PlayerPowerUpManager>();

		if (_playerPowerUpManager != null)
		{
			_playerPowerUpManager.Setup(this);
		}
		else
		{
			Debug.LogError("PlayerController: PlayerPowerUpManager bileþeni bulunamadý! Lütfen Player GameObject'ine ekleyin.");
		}

		Debug.Log("PlayerController: Awake tamamlandý.");
	}

	private AudioSource SetupAudioSource(AudioClip clip)
	{
		AudioSource audioSource = gameObject.AddComponent<AudioSource>();
		audioSource.playOnAwake = false;
		audioSource.loop = false;
		if (clip != null)
		{
			audioSource.clip = clip;
		}
		return audioSource;
	}

	void Start()
	{
		_isGameOver = false;
		_screenHeight = Camera.main.orthographicSize * 2f;
		_rb.gravityScale = 1f;
		_lastUnpausedTime = Time.timeSinceLevelLoad;
	}

	void Update()
	{
		if (Time.timeScale > 0f && !_isGameOver && Input.GetMouseButtonDown(0))
		{
			if (Time.timeSinceLevelLoad - _lastUnpausedTime < _jumpInputBufferTime)
			{
				return;
			}
			_jumpInputReceived = true;
		}
	}

	void FixedUpdate()
	{
		if (!_isGameOver)
		{
			float currentMoveSpeed = _moveSpeed;
			if (_playerPowerUpManager != null && _playerPowerUpManager.GetIsSpeedBoostActive())
			{
				currentMoveSpeed *= _playerPowerUpManager.GetSpeedBoostMultiplier();
			}
			_rb.linearVelocity = new Vector2(currentMoveSpeed, _rb.linearVelocity.y);

			if (_jumpInputReceived)
			{
				float currentJumpForce = _jumpForce;
				if (_playerPowerUpManager != null && _playerPowerUpManager.GetIsJumpBoostActive())
				{
					currentJumpForce *= _playerPowerUpManager.GetJumpBoostMultiplier();
				}
				_rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0);
				_rb.AddForce(Vector2.up * currentJumpForce, ForceMode2D.Impulse);

				if (_jumpAudioSource != null && _jumpAudioSource.clip != null)
				{
					_jumpAudioSource.PlayOneShot(_jumpAudioSource.clip);
				}
				_jumpInputReceived = false;
			}

			float currentPlayerHalfHeight = _playerCollider.bounds.size.y / 2f;
			if (transform.position.y > (_screenHeight / 2f) + currentPlayerHalfHeight)
			{
				transform.position = new Vector3(transform.position.x, -(_screenHeight / 2f) - currentPlayerHalfHeight, transform.position.z);
			}
			else if (transform.position.y < -(_screenHeight / 2f) - currentPlayerHalfHeight)
			{
				transform.position = new Vector3(transform.position.x, (_screenHeight / 2f) + currentPlayerHalfHeight, transform.position.z);
			}
		}
		else
		{
			_rb.linearVelocity = Vector2.zero;
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("PowerUp"))
		{
			Debug.Log("PlayerController: Power-Up toplandý!");

			PowerUp powerUp = other.GetComponent<PowerUp>();

			if (powerUp != null)
			{
				PowerUpType randomType = powerUp.GetRandomPowerUpType();
				Debug.Log($"PlayerController: Rastgele seçilen Power-Up türü: {randomType}");
				ActivatePowerUp(randomType);

				PoolObject collectedPowerUpPoolObject = other.gameObject.GetComponent<PoolObject>();
				if (collectedPowerUpPoolObject != null)
				{
					collectedPowerUpPoolObject.ReturnToPool();
				}
				else
				{
					Destroy(other.gameObject);
				}
			}
			else
			{
				Debug.LogWarning("PlayerController: Toplanan objede PowerUp bileþeni bulunamadý! Tag: PowerUp olan objeler PowerUp script'i içermeli.");
			}
		}
	}

	public void ActivatePowerUp(PowerUpType type)
	{
		Debug.Log("PlayerController.ActivatePowerUp çaðrýldý. Alýnan PowerUpType: " + type.ToString());

		if (_playerPowerUpManager != null)
		{
			_playerPowerUpManager.ActivatePowerUp(type);
		}
		else
		{
			Debug.LogError("PlayerPowerUpManager referansý bulunamadý! Power-Up aktif edilemedi.");
		}

		if (_collectAudioSource != null && _collectAudioSource.clip != null)
		{
			_collectAudioSource.PlayOneShot(_collectAudioSource.clip);
		}
		else
		{
			Debug.LogError("PlayerController: _collectAudioSource veya klibi atanmamýþ! Ses çalýnamýyor.");
		}

		if (_powerUpCollectEffectPrefab != null)
		{
			Debug.Log("Power-Up toplama efekti Instantiate ediliyor: " + _powerUpCollectEffectPrefab.name);
			GameObject effectInstance = Instantiate(_powerUpCollectEffectPrefab, transform.position, Quaternion.identity);

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
			Debug.LogError("PlayerController: _powerUpCollectEffectPrefab atanmamýþ! Lütfen Inspector'dan atayýn.");
		}
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (Time.timeScale > 0f && !_isGameOver && collision.gameObject.CompareTag("Obstacle"))
		{
			if (_explosionAudioSource != null && _explosionAudioSource.clip != null)
			{
				_explosionAudioSource.PlayOneShot(_explosionAudioSource.clip);
			}
			else
			{
				Debug.LogError("PlayerController: _explosionAudioSource veya klibi atanmamýþ! Patlama sesi çalýnamýyor.");
			}

			if (_explosionAnimationPrefab != null)
			{
				Debug.Log("Animasyonlu patlama efekti instantiate ediliyor!");
				GameObject effectInstance = Instantiate(_explosionAnimationPrefab, transform.position, Quaternion.identity);

				Animator anim = effectInstance.GetComponent<Animator>();
				if (anim != null && anim.runtimeAnimatorController != null)
				{
					float animationDuration = 0f;
					foreach (AnimationClip clip in anim.runtimeAnimatorController.animationClips)
					{
						if (clip.name == "PlayerExplosionAnimationName")
						{
							animationDuration = clip.length;
							break;
						}
					}
					if (animationDuration > 0)
					{
						Destroy(effectInstance, animationDuration);
					}
					else
					{
						Destroy(effectInstance, 1.0f);
					}
				}
				else
				{
					Destroy(effectInstance, 1.0f);
				}
			}
			else
			{
				Debug.LogError("PlayerController: _explosionAnimationPrefab atanmamýþ! Lütfen Inspector'dan atayýn.");
			}

			if (_playerPowerUpManager != null && _playerPowerUpManager.GetIsShieldActive())
			{
				Debug.Log("Shield protected you from an obstacle!");
				PoolObject collidedObstaclePoolObject = collision.gameObject.GetComponent<PoolObject>();
				if (collidedObstaclePoolObject != null)
				{
					collidedObstaclePoolObject.ReturnToPool();
				}
				else
				{
					Destroy(collision.gameObject);
				}
			}
			else
			{
				Debug.Log("Game Over! Bir engele çarptýn!");
				_isGameOver = true;

				if (GameManager.Instance != null)
				{
					GameManager.Instance.GameOver();
				}
			}
		}
	}

	public void StopPlayerMovement()
	{
		_isGameOver = true;
		if (_rb != null)
		{
			_rb.linearVelocity = Vector2.zero;
			_rb.angularVelocity = 0f;
		}
	}

	public void SetLastUnpausedTime(float time)
	{
		_lastUnpausedTime = time;
	}
}