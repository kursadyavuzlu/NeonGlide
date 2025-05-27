using UnityEngine;
using System.Collections;

public class ObstacleSpawner : MonoBehaviour
{

	[SerializeField] private GameObject[] _asteroidPrefabs;

	[SerializeField] private float _minSpawnY = -2f;
	[SerializeField] private float _maxSpawnY = 2f;
	[SerializeField] private float _spawnXOffsetFromCamera = 10f;

	[SerializeField] private float _minSpawnTime = 1f;
	[SerializeField] private float _maxSpawnTime = 2f;

	[SerializeField] private float _obstacleMoveSpeed = 5f;

	[SerializeField] private Vector2 _crossMoveSpeedRange = new Vector2(-1f, 1f);

	private float _nextSpawnTime;

	void Start()
	{

		if (_asteroidPrefabs == null || _asteroidPrefabs.Length == 0)
		{
			Debug.LogError("Asteroid Prefabs array is empty! Please assign asteroid prefabs in the Inspector.");

			return;
		}
		_nextSpawnTime = Time.time + Random.Range(_minSpawnTime, _maxSpawnTime);

		StartCoroutine(SpawnObstaclesCoroutine());
	}


	void Update()
	{

	}

	IEnumerator SpawnObstaclesCoroutine()
	{
		while (true)
		{

			yield return new WaitForSeconds(Random.Range(_minSpawnTime, _maxSpawnTime));

			GameObject randomAsteroidPrefab = _asteroidPrefabs[Random.Range(0, _asteroidPrefabs.Length)];

			float randomY = Random.Range(_minSpawnY, _maxSpawnY);

			float cameraX = Camera.main.transform.position.x;
			Vector3 spawnPosition = new Vector3(cameraX + _spawnXOffsetFromCamera, randomY, 0f);

			GameObject newObstacle = Instantiate(randomAsteroidPrefab, spawnPosition, Quaternion.identity);

			ObstacleController obstacleController = newObstacle.GetComponent<ObstacleController>();

			if (obstacleController != null)
			{
				obstacleController.SetSpeed(_obstacleMoveSpeed);

				obstacleController.SetCrossSpeed(Random.Range(_crossMoveSpeedRange.x, _crossMoveSpeedRange.y));
			}
			else
			{

				Debug.LogWarning("ObstacleController not found on spawned obstacle: " + newObstacle.name + ". Make sure it's attached to the prefab.");
			}
		}
	}
}