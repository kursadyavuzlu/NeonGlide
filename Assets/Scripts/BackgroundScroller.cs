using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
	[SerializeField] private float _scrollSpeed = 1f; // Arka plan�n kayd�rma h�z�

	private float _spriteWorldWidth; // Sprite'�n d�nya birimlerindeki ger�ek geni�li�i
	private Camera _mainCamera; // Ana kameraya referans

	void Start()
	{
		SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
		if (spriteRenderer != null)
		{
			_spriteWorldWidth = spriteRenderer.bounds.size.x;
		}
		else
		{
			Debug.LogError("BackgroundScroller'�n ba�l� oldu�u GameObject'te SpriteRenderer bulunamad�!");
			enabled = false;
			return;
		}

		_mainCamera = Camera.main; // Ana kameray� al
		if (_mainCamera == null)
		{
			Debug.LogError("Ana Kamera bulunamad�! 'MainCamera' tag'ine sahip bir kamera oldu�undan emin olun.");
			enabled = false;
			return;
		}
	}

	void Update()
	{
		// Arka plan� sola do�ru kayd�r
		transform.Translate(Vector3.left * _scrollSpeed * Time.deltaTime);

		// Kameran�n sol kenar�n�n d�nya koordinat�ndaki X pozisyonunu hesapla
		float cameraLeftEdgeX = _mainCamera.transform.position.x - (_mainCamera.orthographicSize * _mainCamera.aspect);

		// Bu arka plan�n sa� kenar�n�n d�nya koordinat�ndaki X pozisyonunu hesapla
		float backgroundRightEdgeX = transform.position.x + (_spriteWorldWidth / 2f);

		// E�er bu arka plan�n sa� kenar�, kameran�n sol kenar�ndan daha sola ge�tiyse
		// (yani ekran�n d���na tamamen ��kt�ysa)
		if (backgroundRightEdgeX < cameraLeftEdgeX)
		{
			// Bu arka plan�, di�er arka plan�n bitti�i yerin sa��na ta��
			// Yani, mevcut pozisyonundan iki sprite geni�li�i kadar ileri sar
			transform.position = new Vector3(transform.position.x + (_spriteWorldWidth * 2f), transform.position.y, transform.position.z);
		}
	}
}