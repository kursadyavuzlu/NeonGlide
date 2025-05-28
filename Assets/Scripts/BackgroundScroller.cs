using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
	[SerializeField] private float _scrollSpeed = 1f; // Arka planýn kaydýrma hýzý

	private float _spriteWorldWidth; // Sprite'ýn dünya birimlerindeki gerçek geniþliði
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
			Debug.LogError("BackgroundScroller'ýn baðlý olduðu GameObject'te SpriteRenderer bulunamadý!");
			enabled = false;
			return;
		}

		_mainCamera = Camera.main; // Ana kamerayý al
		if (_mainCamera == null)
		{
			Debug.LogError("Ana Kamera bulunamadý! 'MainCamera' tag'ine sahip bir kamera olduðundan emin olun.");
			enabled = false;
			return;
		}
	}

	void Update()
	{
		// Arka planý sola doðru kaydýr
		transform.Translate(Vector3.left * _scrollSpeed * Time.deltaTime);

		// Kameranýn sol kenarýnýn dünya koordinatýndaki X pozisyonunu hesapla
		float cameraLeftEdgeX = _mainCamera.transform.position.x - (_mainCamera.orthographicSize * _mainCamera.aspect);

		// Bu arka planýn sað kenarýnýn dünya koordinatýndaki X pozisyonunu hesapla
		float backgroundRightEdgeX = transform.position.x + (_spriteWorldWidth / 2f);

		// Eðer bu arka planýn sað kenarý, kameranýn sol kenarýndan daha sola geçtiyse
		// (yani ekranýn dýþýna tamamen çýktýysa)
		if (backgroundRightEdgeX < cameraLeftEdgeX)
		{
			// Bu arka planý, diðer arka planýn bittiði yerin saðýna taþý
			// Yani, mevcut pozisyonundan iki sprite geniþliði kadar ileri sar
			transform.position = new Vector3(transform.position.x + (_spriteWorldWidth * 2f), transform.position.y, transform.position.z);
		}
	}
}