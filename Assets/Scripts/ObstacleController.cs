using UnityEngine;

public class ObstacleController : MonoBehaviour
{
	[Header("Hareket Ayarlarý")]
	[SerializeField] private float _baseMoveSpeed = 5f; // Engelin varsayýlan ileri (sola) hareket hýzý
	[SerializeField] private float _destroyXPosition = -10f; // Engelin yok edileceði X konumu

	private Rigidbody2D _rb;
	private float _currentMoveSpeed; // Dýþarýdan ayarlanabilecek anlýk hareket hýzý
	private float _currentCrossSpeed; // Dýþarýdan ayarlanabilecek anlýk çapraz hareket hýzý

	// ObstacleMover'dan gelen hýz ve Rigidbody referansý
	void Awake()
	{
		_rb = GetComponent<Rigidbody2D>();
		if (_rb == null)
		{
			Debug.LogError("ObstacleController'a baðlý GameObject'te Rigidbody2D bulunamadý!");
			enabled = false;
		}
		// Baþlangýçta ana hýzý ayarla, bu daha sonra Spawner tarafýndan deðiþtirilebilir
		_currentMoveSpeed = _baseMoveSpeed;
	}

	// FixedUpdate fizik hesaplamalarý için en uygun yerdir.
	void FixedUpdate()
	{
		// Engeli sola ve çapraz hareket ettir
		// Y ekseni için _rb.linearVelocity.y kullanmak, varolan yer çekimi veya çarpma etkilerini korur.
		_rb.linearVelocity = new Vector2(-_currentMoveSpeed, _currentCrossSpeed);

		// Eðer engel belirlenen yok etme konumunun soluna geçtiyse yok et
		if (transform.position.x < _destroyXPosition)
		{
			Destroy(gameObject);
		}
	}

	// ObstacleSpawner'dan hýzlarý ayarlamak için kullanýlan metotlar
	public void SetSpeed(float speed)
	{
		_currentMoveSpeed = speed;
	}

	public void SetCrossSpeed(float crossSpeed)
	{
		_currentCrossSpeed = crossSpeed;
	}
}