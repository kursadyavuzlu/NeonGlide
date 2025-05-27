using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[SerializeField] private float _moveSpeed = 5f;
	[SerializeField] private float _jumpForce = 10f;

	private Rigidbody2D _rb;
	private bool _jumpInputReceived = false;

	private bool _isGameOver = false;

	private float _screenHeight;
	private float _playerHeight;

	void Awake()
	{
		_rb = GetComponent<Rigidbody2D>();
	}

	void Start()
	{
		_isGameOver = false;

		_playerHeight = GetComponent<Collider2D>().bounds.size.y / 2f;

		_screenHeight = Camera.main.orthographicSize * 2f;
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
				_rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0);
				_rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
				_jumpInputReceived = false;
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

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Obstacle"))
		{
			Debug.Log("Game Over! Bir engele çarptýn!");
			_isGameOver = true;


			if (GameManager.Instance != null)
			{
				GameManager.Instance.GameOver();
			}
		}
	}

	public void StopPlayerMovement()
	{
		_isGameOver = true;
		_rb.linearVelocity = Vector2.zero;
		_rb.gravityScale = 0f;
	}
}