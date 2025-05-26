using UnityEngine;

public class ObstacleMover : MonoBehaviour
{

	[SerializeField] private float _moveSpeed = 5f;
	[SerializeField] private float _destroyXPosition = -10f;

	private Rigidbody2D _rb;

	void Awake()
	{
		_rb = GetComponent<Rigidbody2D>();
	}

	void FixedUpdate()
	{

		_rb.linearVelocity = new Vector2(-_moveSpeed, _rb.linearVelocity.y);

		if (transform.position.x < _destroyXPosition)
		{
			Destroy(gameObject);
		}
	}
}