using UnityEngine;

public class ObstacleController : MonoBehaviour
{
	[Header("Hareket Ayarlarý")]
	[SerializeField] private float _baseMoveSpeed = 5f;
	[SerializeField] private float _destroyXPosition = -10f;
	private Rigidbody2D _rb;
	private float _currentMoveSpeed;
	private float _currentCrossSpeed;

	void Awake()
	{
		_rb = GetComponent<Rigidbody2D>();
		if (_rb == null)
		{
			Debug.LogError("ObstacleController'a baðlý GameObject'te Rigidbody2D bulunamadý!");
			enabled = false;
		}

		_currentMoveSpeed = _baseMoveSpeed;
	}

	void FixedUpdate()
	{
		_rb.linearVelocity = new Vector2(-_currentMoveSpeed, _currentCrossSpeed);

		if (transform.position.x < _destroyXPosition)
		{
			Destroy(gameObject);
		}
	}

	public void SetSpeed(float speed)
	{
		_currentMoveSpeed = speed;
	}

	public void SetCrossSpeed(float crossSpeed)
	{
		_currentCrossSpeed = crossSpeed;
	}
}