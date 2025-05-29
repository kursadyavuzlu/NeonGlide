using UnityEngine;
using System.Collections; // Coroutine kullan�m� i�in gerekli

public class PlayerController : MonoBehaviour
{
	[SerializeField] private float _moveSpeed = 5f;
	[SerializeField] private float _jumpForce = 10f;

	[Header("Audio Settings")]
	[SerializeField] private AudioClip _jumpSound;
	[SerializeField] private AudioClip _collectPowerUpSound;
	[SerializeField] private AudioClip _explosionSound;

	private AudioSource _jumpAudioSource;
	private AudioSource _collectAudioSource;
	private AudioSource _explosionAudioSource;

	[Header("Effects")]
	[SerializeField] private GameObject _explosionAnimationPrefab;
	[SerializeField] private GameObject _powerUpCollectEffectPrefab;

	private Rigidbody2D _rb;
	private SpriteRenderer _spriteRenderer; // UI veya di�er g�rsel kontroller i�in referans�n� tutmaya devam ediyoruz
	private Collider2D _playerCollider;
	private Vector3 _originalScale; // Player'�n orijinal boyutunu saklayacak

	private bool _jumpInputReceived = false;
	private bool _isGameOver = false;

	private float _screenHeight;

	private PlayerPowerUpManager _playerPowerUpManager; // PlayerPowerUpManager referans�

	void Awake()
	{
		_rb = GetComponent<Rigidbody2D>();
		_spriteRenderer = GetComponent<SpriteRenderer>();
		_playerCollider = GetComponent<Collider2D>();
		_originalScale = transform.localScale;

		// AudioSource'lar� programatik olarak ekliyoruz ve klipleri at�yoruz
		_jumpAudioSource = gameObject.AddComponent<AudioSource>();
		_jumpAudioSource.playOnAwake = false;
		_jumpAudioSource.loop = false;
		if (_jumpSound != null) _jumpAudioSource.clip = _jumpSound;

		_collectAudioSource = gameObject.AddComponent<AudioSource>();
		_collectAudioSource.playOnAwake = false;
		_collectAudioSource.loop = false;
		if (_collectPowerUpSound != null) _collectAudioSource.clip = _collectPowerUpSound;

		_explosionAudioSource = gameObject.AddComponent<AudioSource>();
		_explosionAudioSource.playOnAwake = false;
		_explosionAudioSource.loop = false;
		if (_explosionSound != null) _explosionAudioSource.clip = _explosionSound;

		_playerPowerUpManager = GetComponent<PlayerPowerUpManager>();
		if (_playerPowerUpManager != null)
		{
			_playerPowerUpManager.Setup(this); // Sadece 'this' (PlayerController referans�) g�nderiliyor
		}
		else
		{
			Debug.LogError("PlayerController: PlayerPowerUpManager bile�eni bulunamad�! L�tfen Player GameObject'ine ekleyin.");
		}

		Debug.Log("PlayerController: Awake tamamland�.");
	}

	void Start()
	{
		_isGameOver = false;
		_screenHeight = Camera.main.orthographicSize * 2f;
		_rb.gravityScale = 1f;
	}

	void Update()
	{
		if (!_isGameOver && Input.GetMouseButtonDown(0))
		{
			_jumpInputReceived = true;
		}
	}

	void FixedUpdate()
	{
		if (!_isGameOver)
		{
			// H�zland�rma Power-Up'�n�n etkisi PlayerPowerUpManager'dan al�nacak
			float currentMoveSpeed = _moveSpeed;
			if (_playerPowerUpManager != null && _playerPowerUpManager.GetIsSpeedBoostActive())
			{
				currentMoveSpeed *= _playerPowerUpManager.GetSpeedBoostMultiplier();
			}
			_rb.linearVelocity = new Vector2(currentMoveSpeed, _rb.linearVelocity.y);

			if (_jumpInputReceived)
			{
				// Z�plama G��lendirme Power-Up'�n�n etkisi PlayerPowerUpManager'dan al�nacak
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

			// Ekran d���na ��kma kontrol� (oyuncu boyutunu dinamik al�yoruz)
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

	public void ActivatePowerUp(PowerUpType type)
	{
		Debug.Log("PlayerController.ActivatePowerUp �a�r�ld�. Al�nan PowerUpType: " + type.ToString());

		// Power-Up'� aktif etme g�revini tamamen PlayerPowerUpManager'a devret
		if (_playerPowerUpManager != null)
		{
			_playerPowerUpManager.ActivatePowerUp(type);
		}
		else
		{
			Debug.LogError("PlayerPowerUpManager referans� bulunamad�! Power-Up aktif edilemedi.");
		}

		if (_collectAudioSource != null && _collectAudioSource.clip != null)
		{
			_collectAudioSource.PlayOneShot(_collectAudioSource.clip);
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
				Destroy(effectInstance, 1.0f); // Varsay�lan s�re
			}
		}
		else
		{
			Debug.LogError("PlayerController: _powerUpCollectEffectPrefab atanmam��! L�tfen Inspector'dan atay�n.");
		}
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Obstacle"))
		{
			if (_explosionAudioSource != null && _explosionAudioSource.clip != null)
			{
				_explosionAudioSource.PlayOneShot(_explosionAudioSource.clip);
			}

			if (_explosionAnimationPrefab != null)
			{
				Debug.Log("Animasyonlu patlama efekti instantiate ediliyor!");
				GameObject effectInstance = Instantiate(_explosionAnimationPrefab, transform.position, Quaternion.identity);

				float animationDuration = 0.5f; // Animasyonun yakla��k s�resi
				Destroy(effectInstance, animationDuration);
			}
			else
			{
				Debug.LogError("PlayerController: _explosionAnimationPrefab atanmam��! L�tfen Inspector'dan atay�n.");
			}

			// Kalkan kontrol� art�k PlayerPowerUpManager �zerinden yap�lacak
			if (_playerPowerUpManager != null && _playerPowerUpManager.GetIsShieldActive())
			{
				Debug.Log("Shield protected you from an obstacle!");
				Destroy(collision.gameObject);
			}
			else
			{
				Debug.Log("Game Over! Bir engele �arpt�n!");
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
		_rb.linearVelocity = Vector2.zero;
	}
}