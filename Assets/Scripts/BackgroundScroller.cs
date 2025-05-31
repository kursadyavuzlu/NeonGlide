using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
	[SerializeField] private float _scrollSpeed = 1f;

	private float _spriteWorldWidth;
	private Camera _mainCamera;

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

		_mainCamera = Camera.main;
		if (_mainCamera == null)
		{
			Debug.LogError("Ana Kamera bulunamad�! 'MainCamera' tag'ine sahip bir kamera oldu�undan emin olun.");
			enabled = false;
			return;
		}
	}

	void Update()
	{
		transform.Translate(Vector3.left * _scrollSpeed * Time.deltaTime);

		float cameraLeftEdgeX = _mainCamera.transform.position.x - (_mainCamera.orthographicSize * _mainCamera.aspect);

		float backgroundRightEdgeX = transform.position.x + (_spriteWorldWidth / 2f);

		if (backgroundRightEdgeX < cameraLeftEdgeX)
		{

			transform.position = new Vector3(transform.position.x + (_spriteWorldWidth * 2f), transform.position.y, transform.position.z);
		}
	}
}