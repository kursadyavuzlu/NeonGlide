using UnityEngine;
using System.Collections;

// PowerUpType enum'ý PowerUp.cs dosyasýnda tanýmlý olduðu için burada tekrar tanýmlanmamalýdýr.

public class PlayerController : MonoBehaviour
{
	[SerializeField] private float _moveSpeed = 5f;
	[SerializeField] private float _jumpForce = 10f;

	[Header("Power-Up Settings")]
	[SerializeField] private float _jumpBoostMultiplier = 1.5f;
	[SerializeField] private float _jumpBoostDuration = 7f;
	[SerializeField] private float _shieldDuration = 5f;
	[SerializeField] private float _speedBoostMultiplier = 1.5f;
	[SerializeField] private float _speedBoostDuration = 5f;
	[SerializeField] private int _scoreMultiplierAmount = 2;
	[SerializeField] private float _scoreMultiplierDuration = 7f;
	[SerializeField] private float _shrinkScale = 0.5f;
	[SerializeField] private float _shrinkDuration = 5f;
	[SerializeField] private float _scaleChangeSpeed = 5f;
	[SerializeField] private float _timeSlowdownFactor = 0.5f;
	[SerializeField] private float _timeSlowdownDuration = 6f;

	[Header("Audio Settings")]
	[SerializeField] private AudioClip _jumpSound;
	[SerializeField] private AudioClip _collectPowerUpSound;
	[SerializeField] private AudioClip _explosionSound;
	[SerializeField] private AudioClip _clearObstaclesSound; // <-- Yeni: Engelleri temizleme sesi

	private AudioSource _jumpAudioSource;
	private AudioSource _collectAudioSource;
	private AudioSource _explosionAudioSource;
	// Ses kaynaklarý listesine bir eleman daha eklenecek
	private AudioSource _clearObstaclesAudioSource; // <-- Yeni: Engelleri temizleme sesi kaynaðý

	[Header("Effects")]
	[SerializeField] private GameObject _explosionAnimationPrefab;
	[SerializeField] private GameObject _powerUpCollectEffectPrefab;
	[SerializeField] private GameObject _shieldEffectObject;
	[SerializeField] private GameObject _clearObstaclesEffectPrefab; // <-- Yeni: Engelleri temizleme efekti


	private Rigidbody2D _rb;
	private SpriteRenderer _spriteRenderer;
	private Collider2D _playerCollider;

	private bool _jumpInputReceived = false;

	private bool _isGameOver = false;

	private float _screenHeight;
	private float _playerOriginalHeight;
	private Vector3 _playerOriginalScale;

	private bool _isShieldActive = false;
	private bool _isJumpBoostActive = false;
	private bool _isSpeedBoostActive = false;
	private bool _isScoreMultiplierActive = false;
	private bool _isShrinkActive = false;
	private bool _isTimeSlowdownActive = false;
	// ClearAllObstacles için aktiflik deðiþkenine ihtiyaç yok, anlýk etki.

	void Awake()
	{
		_rb = GetComponent<Rigidbody2D>();
		_spriteRenderer = GetComponent<SpriteRenderer>();
		_playerCollider = GetComponent<Collider2D>();

		AudioSource[] sources = GetComponents<AudioSource>();
		// Ses kaynaklarýný kontrol et ve ata
		if (sources.Length >= 4) // Þimdi 4 ses kaynaðýna ihtiyacýmýz var (Jump, Collect, Explosion, ClearObstacles)
		{
			_jumpAudioSource = sources[0];
			_collectAudioSource = sources[1];
			_explosionAudioSource = sources[2];
			_clearObstaclesAudioSource = sources[3]; // <-- Yeni ses kaynaðýný ata
		}
		else
		{
			Debug.LogError("Player GameObject'inde yeterli sayýda AudioSource bileþeni bulunamadý! Lütfen 4 adet eklediðinizden emin olun.");
		}
	}

	void Start()
	{
		_isGameOver = false;

		_playerOriginalScale = transform.localScale;
		_playerOriginalHeight = _playerCollider.bounds.size.y / 2f;

		_screenHeight = Camera.main.orthographicSize * 2f;
		_rb.gravityScale = 1f;

		if (_shieldEffectObject != null)
		{
			_shieldEffectObject.SetActive(false);
			Debug.Log("PlayerController: ShieldEffectObject Start'ta devre dýþý býrakýldý.");
		}
		else
		{
			Debug.LogWarning("PlayerController: _shieldEffectObject atanmamýþ! Kalkan efekti görseli çalýþmayabilir.");
		}
	}

	void Update()
	{
		if (!_isGameOver && Input.GetMouseButtonDown(0))
		{
			_jumpInputReceived = true;
		}

		if (!_isShrinkActive && transform.localScale != _playerOriginalScale)
		{
			transform.localScale = Vector3.Lerp(transform.localScale, _playerOriginalScale, Time.deltaTime * _scaleChangeSpeed);
		}
	}

	void FixedUpdate()
	{
		if (!_isGameOver)
		{
			float currentMoveSpeed = _moveSpeed * (_isSpeedBoostActive ? _speedBoostMultiplier : 1f);
			_rb.linearVelocity = new Vector2(currentMoveSpeed, _rb.linearVelocity.y);


			if (_jumpInputReceived)
			{
				float currentJumpForce = _isJumpBoostActive ? _jumpForce * _jumpBoostMultiplier : _jumpForce;
				_rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0);
				_rb.AddForce(Vector2.up * currentJumpForce, ForceMode2D.Impulse);

				if (_jumpAudioSource != null && _jumpSound != null)
				{
					_jumpAudioSource.PlayOneShot(_jumpSound);
				}
				_jumpInputReceived = false;
			}

			float currentPlayerHeight = _playerCollider.bounds.size.y / 2f;
			if (transform.position.y > (_screenHeight / 2f) + currentPlayerHeight)
			{
				transform.position = new Vector3(transform.position.x, -(_screenHeight / 2f) - currentPlayerHeight, transform.position.z);
			}
			else if (transform.position.y < -(_screenHeight / 2f) - currentPlayerHeight)
			{
				transform.position = new Vector3(transform.position.x, (_screenHeight / 2f) + currentPlayerHeight, transform.position.z);
			}
		}
		else
		{
			_rb.linearVelocity = Vector2.zero;
		}
	}

	public void ActivatePowerUp(PowerUpType type)
	{
		Debug.Log("PlayerController.ActivatePowerUp çaðrýldý. Alýnan PowerUpType: " + type.ToString());

		switch (type)
		{
			case PowerUpType.Shield:
				if (!_isShieldActive)
				{
					StartCoroutine(ShieldPowerUpRoutine());
				}
				break;
			case PowerUpType.JumpBoost:
				if (!_isJumpBoostActive)
				{
					StartCoroutine(JumpBoostPowerUpRoutine());
				}
				break;
			case PowerUpType.SpeedBoost:
				if (!_isSpeedBoostActive)
				{
					StartCoroutine(SpeedBoostPowerUpRoutine());
				}
				break;
			case PowerUpType.ScoreMultiplier:
				if (!_isScoreMultiplierActive)
				{
					StartCoroutine(ScoreMultiplierPowerUpRoutine());
				}
				break;
			case PowerUpType.Shrink:
				if (!_isShrinkActive)
				{
					StartCoroutine(ShrinkPowerUpRoutine());
				}
				break;
			case PowerUpType.TimeSlowdown:
				if (!_isTimeSlowdownActive)
				{
					StartCoroutine(TimeSlowdownPowerUpRoutine());
				}
				break;
			case PowerUpType.ClearAllObstacles: // <-- Yeni case bloðu
				ClearAllObstaclesRoutine(); // Anlýk etki, Coroutine'e gerek yok
				break;
		}

		if (_collectAudioSource != null && _collectPowerUpSound != null)
		{
			_collectAudioSource.PlayOneShot(_collectPowerUpSound);
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
				Debug.LogWarning("PlayerController: _powerUpCollectEffectPrefab atanmamýþ! Lütfen Inspector'dan atayýn.");
				Destroy(effectInstance, 1.0f);
			}
		}
		else
		{
			Debug.LogError("PlayerController: _powerUpCollectEffectPrefab atanmamýþ! Lütfen Inspector'dan atayýn.");
		}
	}

	IEnumerator ShieldPowerUpRoutine()
	{
		_isShieldActive = true;
		Debug.Log("ShieldPowerUpRoutine BAÞLADI! Player'ýn kalkaný aktifleþiyor.");
		if (_shieldEffectObject != null)
		{
			_shieldEffectObject.SetActive(true);
		}

		yield return new WaitForSeconds(_shieldDuration);

		_isShieldActive = false;
		Debug.Log("ShieldPowerUpRoutine BÝTTÝ! Player'ýn kalkaný devre dýþý býrakýlýyor.");
		if (_shieldEffectObject != null)
		{
			_shieldEffectObject.SetActive(false);
		}
	}

	IEnumerator JumpBoostPowerUpRoutine()
	{
		_isJumpBoostActive = true;
		Debug.Log("Jump Boost Active!");
		// TODO: Zýplama gücü aktifken görsel geri bildirim ekleme yeri.

		yield return new WaitForSeconds(_jumpBoostDuration);

		_isJumpBoostActive = false;
		Debug.Log("Jump Boost Deactivated!");
		// TODO: Zýplama gücü etkisi sona erdiðinde görsel geri bildirimi kaldýrma yeri.
	}

	IEnumerator SpeedBoostPowerUpRoutine()
	{
		_isSpeedBoostActive = true;
		Debug.Log("Speed Boost Active!");
		// TODO: Hýzlandýrma aktifken görsel geri bildirim ekleme yeri.

		yield return new WaitForSeconds(_speedBoostDuration);

		_isSpeedBoostActive = false;
		Debug.Log("Speed Boost Deactivated!");
		// TODO: Hýzlandýrma etkisi sona erdiðinde görsel geri bildirimi kaldýrma yeri.
	}

	IEnumerator ScoreMultiplierPowerUpRoutine()
	{
		_isScoreMultiplierActive = true;
		Debug.Log("Score Multiplier Active! Multiplier: " + _scoreMultiplierAmount + "x");
		// TODO: Puan çarpaný aktifken görsel geri bildirim ekleme yeri.

		if (GameManager.Instance != null)
		{
			GameManager.Instance.SetScoreMultiplier(_scoreMultiplierAmount);
		}

		yield return new WaitForSeconds(_scoreMultiplierDuration);

		_isScoreMultiplierActive = false;
		Debug.Log("Score Multiplier Deactivated! Multiplier Reset.");
		// TODO: Puan çarpaný etkisi sona erdiðinde görsel geri bildirimi kaldýrma yeri.

		if (GameManager.Instance != null)
		{
			GameManager.Instance.ResetScoreMultiplier();
		}
	}

	IEnumerator ShrinkPowerUpRoutine()
	{
		_isShrinkActive = true;
		Debug.Log("Shrink Power-Up Active! Player is shrinking.");

		Vector3 targetScale = _playerOriginalScale * _shrinkScale;
		float timer = 0f;
		float duration = 0.5f;

		while (timer < duration)
		{
			transform.localScale = Vector3.Lerp(_playerOriginalScale, targetScale, timer / duration);
			timer += Time.deltaTime * _scaleChangeSpeed;
			yield return null;
		}
		transform.localScale = targetScale;

		// TODO: Küçülme aktifken görsel geri bildirim ekleme yeri.

		yield return new WaitForSeconds(_shrinkDuration);

		_isShrinkActive = false;
		Debug.Log("Shrink Power-Up Deactivated! Player is returning to normal size.");

		timer = 0f;
		while (timer < duration)
		{
			transform.localScale = Vector3.Lerp(targetScale, _playerOriginalScale, timer / duration);
			timer += Time.deltaTime * _scaleChangeSpeed;
			yield return null;
		}
		transform.localScale = _playerOriginalScale;

		// TODO: Küçülme etkisi sona erdiðinde görsel geri bildirimi kaldýrma yeri.
	}

	IEnumerator TimeSlowdownPowerUpRoutine()
	{
		_isTimeSlowdownActive = true;
		Debug.Log("Time Slowdown Active! Time Scale: " + _timeSlowdownFactor);
		// TODO: Zaman yavaþlatma aktifken görsel geri bildirim ekleme yeri.

		if (GameManager.Instance != null)
		{
			GameManager.Instance.SetTimeScale(_timeSlowdownFactor);
		}

		yield return new WaitForSeconds(_timeSlowdownDuration * _timeSlowdownFactor);

		_isTimeSlowdownActive = false;
		Debug.Log("Time Slowdown Deactivated! Time Scale Reset.");
		// TODO: Zaman yavaþlatma etkisi sona erdiðinde görsel geri bildirimi kaldýrma yeri.

		if (GameManager.Instance != null)
		{
			GameManager.Instance.ResetTimeScale();
		}
	}

	private void ClearAllObstaclesRoutine() // <-- Yeni metot
	{
		Debug.Log("Clear All Obstacles Power-Up Active! Destroying all obstacles.");

		if (_clearObstaclesAudioSource != null && _clearObstaclesSound != null)
		{
			_clearObstaclesAudioSource.PlayOneShot(_clearObstaclesSound); // Sesi çal
		}

		// Engelleri bul ve yok et
		GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
		foreach (GameObject obstacle in obstacles)
		{
			// Yok etme efekti varsa burada Instantiate edilebilir
			if (_clearObstaclesEffectPrefab != null)
			{
				Instantiate(_clearObstaclesEffectPrefab, obstacle.transform.position, Quaternion.identity);
			}
			Destroy(obstacle);
		}
		// TODO: Engeller yok edilirken görsel bir efekt veya animasyon ekleme yeri (örneðin bir patlama, bir ýþýk parlamasý vb.).
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Obstacle"))
		{
			if (_explosionAudioSource != null && _explosionSound != null)
			{
				_explosionAudioSource.PlayOneShot(_explosionSound);
			}

			if (_explosionAnimationPrefab != null)
			{
				Debug.Log("Animasyonlu patlama efekti instantiate ediliyor!");
				GameObject effectInstance = Instantiate(_explosionAnimationPrefab, transform.position, Quaternion.identity);

				float animationDuration = 0.5f;
				Destroy(effectInstance, animationDuration);
			}
			else
			{
				Debug.LogError("PlayerController: _explosionAnimationPrefab atanmamýþ! Lütfen Inspector'dan atayýn.");
			}

			if (_isShieldActive)
			{
				Debug.Log("Shield protected you from an obstacle!");
				Destroy(collision.gameObject);
				_isShieldActive = false;
				if (_shieldEffectObject != null)
				{
					_shieldEffectObject.SetActive(false);
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
		_rb.linearVelocity = Vector2.zero;
	}
}