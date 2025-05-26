using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[SerializeField] private float _moveSpeed = 5f;
	[SerializeField] private float _jumpForce = 10f;

	private Rigidbody2D _rb;
	private bool _jumpInputReceived = false;

	private bool _isGameOver = false;

	void Awake()
	{
		_rb = GetComponent<Rigidbody2D>();
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		_isGameOver = false;
	}

	// Update is called once per frame
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
			_rb.gravityScale = 1f;

			if (GameManager.Instance != null)
			{
				GameManager.Instance.GameOver();
			}
		}
	}
}