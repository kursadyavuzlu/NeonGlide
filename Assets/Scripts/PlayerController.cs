using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	[SerializeField] private float _moveSpeed = 5f;
	[SerializeField] private float _jumpForce = 10f;

	[Header("Power-Up Settings")]
	[SerializeField] private float _jumpBoostMultiplier = 1.5f;
	[SerializeField] private float _jumpBoostDuration = 7f;
	[SerializeField] private float _shieldDuration = 5f;
	[SerializeField] private float _controlGlitchDuration = 4f;
	[SerializeField] private float _glitchForce = 0.5f;

	private Rigidbody2D _rb;

	private bool _jumpInputReceived = false;

	private bool _isGameOver = false;

	private float _screenHeight;
	private float _playerHeight;

	private bool _isShieldActive = false;
	private bool _isJumpBoostActive = false;
	private bool _isControlGlitchActive = false;


	void Awake()
	{
		_rb = GetComponent<Rigidbody2D>();
	}

	void Start()
	{
		_isGameOver = false;

		_playerHeight = GetComponent<Collider2D>().bounds.size.y / 2f;

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
			_rb.linearVelocity = new Vector2(_moveSpeed, _rb.linearVelocity.y);

			if (_jumpInputReceived)
			{
				float currentJumpForce = _isJumpBoostActive ? _jumpForce * _jumpBoostMultiplier : _jumpForce;
				_rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0);
				_rb.AddForce(Vector2.up * currentJumpForce, ForceMode2D.Impulse);
				_jumpInputReceived = false;
			}

			if (_isControlGlitchActive)
			{
				_rb.AddForce(Vector2.up * Random.Range(-_glitchForce, _glitchForce), ForceMode2D.Force);
			}

			if (transform.position.y > (_screenHeight / 2f) + _playerHeight)
			{

				transform.position = new Vector3(transform.position.x, -(_screenHeight / 2f) - _playerHeight, transform.position.z);
			}

			else if (transform.position.y < -(_screenHeight / 2f) - _playerHeight)
			{
				transform.position = new Vector3(transform.position.x, (_screenHeight / 2f) + _playerHeight, transform.position.z);
			}
		}
		else
		{
			_rb.linearVelocity = Vector2.zero;
		}
	}

	public void ActivatePowerUp(PowerUpType type)
	{
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
			case PowerUpType.ControlGlitch:

				if (!_isControlGlitchActive)
				{
					StartCoroutine(ControlGlitchPowerUpRoutine());
				}
				break;
		}
	}

	IEnumerator ShieldPowerUpRoutine()
	{
		_isShieldActive = true;
		Debug.Log("Shield Active!");
									 // TODO: Kalkan aktifken g�rsel geri bildirim ekleme yeri (�rne�in Player etraf�nda bir kalkan sprite'� veya parlama).

		yield return new WaitForSeconds(_shieldDuration);

		_isShieldActive = false;
		Debug.Log("Shield Deactivated!");
										  // TODO: Kalkan etkisi sona erdi�inde g�rsel geri bildirimi kald�rma yeri.
	}

	IEnumerator JumpBoostPowerUpRoutine()
	{
		_isJumpBoostActive = true;
		Debug.Log("Jump Boost Active!");
										 // TODO: Z�plama g�c� aktifken g�rsel geri bildirim ekleme yeri (�rne�in Player'�n rengi de�i�sin veya partik�l efekti ��ks�n).


		yield return new WaitForSeconds(_jumpBoostDuration);

		_isJumpBoostActive = false;
		Debug.Log("Jump Boost Deactivated!");
											  // TODO: Z�plama g�c� etkisi sona erdi�inde g�rsel geri bildirimi kald�rma yeri.
	}

	IEnumerator ControlGlitchPowerUpRoutine()
	{
		_isControlGlitchActive = true;
		Debug.LogWarning("Control Glitch Active!");
													// TODO: Kontrol �a�mas� aktifken g�rsel geri bildirim ekleme yeri (�rne�in Player'�n rengi k�rm�z�ya d�ns�n, titreme efekti).


		yield return new WaitForSeconds(_controlGlitchDuration);

		_isControlGlitchActive = false;
		Debug.Log("Control Glitch Deactivated!");
												  // TODO: Kontrol �a�mas� etkisi sona erdi�inde g�rsel geri bildirimi kald�rma yeri.
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Obstacle"))
		{
			if (_isShieldActive)
			{
				Debug.Log("Shield protected you from an obstacle!");
				Destroy(collision.gameObject);
				_isShieldActive = false;
										 // TODO: Kalkan�n g�rsel geri bildirimini hemen kapatma yeri.
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