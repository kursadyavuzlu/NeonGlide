using UnityEngine;

public class ObstacleController : MonoBehaviour
{
	[Header("Hareket Ayarlar�")]
	[SerializeField] private float _baseMoveSpeed = 5f; // Engelin varsay�lan ileri (sola) hareket h�z�
	[SerializeField] private float _destroyXPosition = -10f; // Engelin yok edilece�i X konumu

	private Rigidbody2D _rb;
	private float _currentMoveSpeed; // D��ar�dan ayarlanabilecek anl�k hareket h�z�
	private float _currentCrossSpeed; // D��ar�dan ayarlanabilecek anl�k �apraz hareket h�z�

	// ObstacleMover'dan gelen h�z ve Rigidbody referans�
	void Awake()
	{
		_rb = GetComponent<Rigidbody2D>();
		if (_rb == null)
		{
			Debug.LogError("ObstacleController'a ba�l� GameObject'te Rigidbody2D bulunamad�!");
			enabled = false;
		}
		// Ba�lang��ta ana h�z� ayarla, bu daha sonra Spawner taraf�ndan de�i�tirilebilir
		_currentMoveSpeed = _baseMoveSpeed;
	}

	// FixedUpdate fizik hesaplamalar� i�in en uygun yerdir.
	void FixedUpdate()
	{
		// Engeli sola ve �apraz hareket ettir
		// Y ekseni i�in _rb.linearVelocity.y kullanmak, varolan yer �ekimi veya �arpma etkilerini korur.
		_rb.linearVelocity = new Vector2(-_currentMoveSpeed, _currentCrossSpeed);

		// E�er engel belirlenen yok etme konumunun soluna ge�tiyse yok et
		if (transform.position.x < _destroyXPosition)
		{
			Destroy(gameObject);
		}
	}

	// ObstacleSpawner'dan h�zlar� ayarlamak i�in kullan�lan metotlar
	public void SetSpeed(float speed)
	{
		_currentMoveSpeed = speed;
	}

	public void SetCrossSpeed(float crossSpeed)
	{
		_currentCrossSpeed = crossSpeed;
	}
}