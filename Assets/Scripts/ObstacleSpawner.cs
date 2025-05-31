using System.Collections;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
	[SerializeField] private float _minSpawnY = -2f;
	[SerializeField] private float _maxSpawnY = 2f;
	[SerializeField] private float _spawnXOffsetFromCamera = 10f;

	[SerializeField] private float _minSpawnTime = 1f;
	[SerializeField] private float _maxSpawnTime = 2f;

	[Header("Difficulty Progression")]
	[SerializeField] private float _spawnTimeDecreaseRate = 0.05f;
	[SerializeField] private float _minAllowedSpawnTime = 0.5f;
	[SerializeField] private float _difficultyIncreaseInterval = 10f;

	[SerializeField] private float _obstacleSpeedIncreaseRate = 0.5f;
	[SerializeField] private float _maxAllowedObstacleSpeed = 15f;

	private float _initialObstacleMoveSpeed;

	private float _initialMinSpawnTime;
	private float _initialMaxSpawnTime;
	private float _nextDifficultyIncreaseTime;

	[SerializeField] private float _obstacleMoveSpeed = 5f;
	[SerializeField] private Vector2 _crossMoveSpeedRange = new Vector2(-1f, 1f);

	private float _nextSpawnTime;
	private string[] _obstacleTags = { "Obstacle0", "Obstacle1", "Obstacle2", "Obstacle3", "Obstacle4", "Obstacle5" };

	void Start()
	{
		if (ObjectPoolManager.Instance == null)
		{
			Debug.LogError("ObjectPoolManager Instance not found! Make sure an ObjectPoolManager GameObject is in the scene with the script attached.");
			return;
		}

		StartCoroutine(SpawnObstaclesCoroutine());

		_initialMinSpawnTime = _minSpawnTime;
		_initialMaxSpawnTime = _maxSpawnTime;
		_nextDifficultyIncreaseTime = Time.time + _difficultyIncreaseInterval;

		_initialObstacleMoveSpeed = _obstacleMoveSpeed;
	}

	void Update()
	{
		if (Time.time > _nextDifficultyIncreaseTime)
		{
			IncreaseDifficulty();
			_nextDifficultyIncreaseTime = Time.time + _difficultyIncreaseInterval;
		}
	}

	IEnumerator SpawnObstaclesCoroutine()
	{
		while (true)
		{
			yield return new WaitForSeconds(Random.Range(_minSpawnTime, _maxSpawnTime));

			string randomObstacleTag = _obstacleTags[Random.Range(0, _obstacleTags.Length)];

			float randomY = Random.Range(_minSpawnY, _maxSpawnY);

			float cameraX = Camera.main.transform.position.x;
			Vector3 spawnPosition = new Vector3(cameraX + _spawnXOffsetFromCamera, randomY, 0f);

			GameObject newObstacle = ObjectPoolManager.Instance.SpawnFromPool(randomObstacleTag, spawnPosition, Quaternion.identity);

			if (newObstacle == null)
			{
				Debug.LogWarning("Failed to spawn " + randomObstacleTag + " from pool. Check ObjectPoolManager setup.");
				continue;
			}

			PoolObject pooledObject = newObstacle.GetComponent<PoolObject>();
			if (pooledObject != null)
			{
				pooledObject.SetTag(randomObstacleTag);
			}
			else
			{
				Debug.LogWarning("PoolObject script not found on spawned obstacle: " + newObstacle.name + ". Make sure it's attached to the prefab.");
			}

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

	private void IncreaseDifficulty()
	{
		_minSpawnTime = Mathf.Max(_minAllowedSpawnTime, _minSpawnTime - _spawnTimeDecreaseRate);
		_maxSpawnTime = Mathf.Max(_minAllowedSpawnTime, _maxSpawnTime - _spawnTimeDecreaseRate);

		_obstacleMoveSpeed = Mathf.Min(_maxAllowedObstacleSpeed, _obstacleMoveSpeed + _obstacleSpeedIncreaseRate);

		Debug.Log($"Difficulty Increased! New Spawn Time Range: {_minSpawnTime:F2}s - {_maxSpawnTime:F2}s, New Obstacle Speed: {_obstacleMoveSpeed:F2}");
	}
}