using UnityEngine;
using System.Collections;

public class PowerUpSpawner : MonoBehaviour
{
	[Header("Spawner Settings")]
	[SerializeField] private GameObject _powerUpPrefab;
	[SerializeField] private float _minSpawnInterval = 10f;
	[SerializeField] private float _maxSpawnInterval = 20f;
	[SerializeField] private float _spawnOffsetFromCameraEdge = 3f;

	private Camera _mainCamera;
	private float _screenHeight;
	private float _powerUpHeight;

	void Start()
	{
		_mainCamera = Camera.main;
		if (_mainCamera == null)
		{
			Debug.LogError("Ana Kamera bulunamadý! Lütfen sahnede 'MainCamera' tag'ine sahip bir kamera olduðundan emin olun.");
			enabled = false;
			return;
		}

		_screenHeight = _mainCamera.orthographicSize * 2f;

		if (_powerUpPrefab != null)
		{
			Collider2D powerUpCollider = _powerUpPrefab.GetComponent<Collider2D>();
			if (powerUpCollider != null)
			{
				_powerUpHeight = powerUpCollider.bounds.size.y / 2f;
			}
			else
			{
				Debug.LogWarning("PowerUp prefab'inizde Collider2D bulunmuyor. Yükseklik hesaplanamayacak.");
			}
		}
		else
		{
			Debug.LogError("PowerUp Prefab atanmamýþ! Power-Up üretilemeyecek.");
			return;
		}

		StartCoroutine(SpawnPowerUpsRoutine());
	}

	IEnumerator SpawnPowerUpsRoutine()
	{
		while (true)
		{
			float randomInterval = Random.Range(_minSpawnInterval, _maxSpawnInterval);
			yield return new WaitForSeconds(randomInterval);

			float spawnXPosition = _mainCamera.transform.position.x + (_mainCamera.orthographicSize * _mainCamera.aspect) + _spawnOffsetFromCameraEdge;

			float randomY = Random.Range(-(_screenHeight / 2f) + _powerUpHeight, (_screenHeight / 2f) - _powerUpHeight);

			Vector3 spawnPosition = new Vector3(spawnXPosition, randomY, 0f);
			Instantiate(_powerUpPrefab, spawnPosition, Quaternion.identity);

			Debug.Log("Power-Up üretildi!");
		}
	}
}