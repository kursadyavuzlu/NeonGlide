using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
	public static ObjectPoolManager Instance { get; private set; }

	[System.Serializable]
	public class Pool
	{
		public string tag;
		public GameObject prefab;
		public int size;
	}

	public List<Pool> pools;
	private Dictionary<string, Queue<GameObject>> poolDictionary;

	void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		else
		{
			Instance = this;
		}

		poolDictionary = new Dictionary<string, Queue<GameObject>>();

		foreach (Pool pool in pools)
		{
			Queue<GameObject> objectPool = new Queue<GameObject>();

			for (int i = 0; i < pool.size; i++)
			{
				GameObject obj = Instantiate(pool.prefab);
				obj.SetActive(false);
				objectPool.Enqueue(obj);
			}
			poolDictionary.Add(pool.tag, objectPool);
		}
	}

	public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
	{
		if (!poolDictionary.ContainsKey(tag))
		{
			Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
			return null;
		}

		GameObject objectToSpawn = poolDictionary[tag].Dequeue();

		objectToSpawn.SetActive(true);
		objectToSpawn.transform.position = position;
		objectToSpawn.transform.rotation = rotation;

		return objectToSpawn;
	}

	public void ReturnToPool(string tag, GameObject objectToReturn)
	{
		if (!poolDictionary.ContainsKey(tag))
		{
			Debug.LogWarning("Pool with tag " + tag + " doesn't exist. Object " + objectToReturn.name + " destroyed.");
			Destroy(objectToReturn);
			return;
		}

		objectToReturn.SetActive(false);
		poolDictionary[tag].Enqueue(objectToReturn);
	}
}