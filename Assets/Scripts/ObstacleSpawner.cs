using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{

	[SerializeField] private GameObject _obstaclePrefab;

	[SerializeField] private float _minSpawnY = -2f;
	[SerializeField] private float _maxSpawnY = 2f; 
	[SerializeField] private float _spawnXOffsetFromCamera = 10f;

	[SerializeField] private float _minSpawnTime = 1f;
	[SerializeField] private float _maxSpawnTime = 2f;

	private float _nextSpawnTime;

	void Start()
	{
		_nextSpawnTime = Time.time + Random.Range(_minSpawnTime, _maxSpawnTime);
	}

	void Update()
	{

		if (Time.time >= _nextSpawnTime)
		{
			SpawnObstacle();
			_nextSpawnTime = Time.time + Random.Range(_minSpawnTime, _maxSpawnTime);
		}
	}

	void SpawnObstacle()
	{
		
		float randomY = Random.Range(_minSpawnY, _maxSpawnY);
		float cameraX = Camera.main.transform.position.x;
		Vector3 spawnPosition = new Vector3(cameraX + _spawnXOffsetFromCamera, randomY, 0f);

		Instantiate(_obstaclePrefab, spawnPosition, Quaternion.identity);
	}
}