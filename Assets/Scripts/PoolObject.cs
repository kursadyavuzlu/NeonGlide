using UnityEngine;

public class PoolObject : MonoBehaviour
{
	private string _poolTag;

	public void SetTag(string tag)
	{
		_poolTag = tag;
	}

	public void ReturnToPool()
	{
		if (!string.IsNullOrEmpty(_poolTag) && ObjectPoolManager.Instance != null)
		{
			ObjectPoolManager.Instance.ReturnToPool(_poolTag, this.gameObject);
		}
		else
		{
			Debug.LogWarning("PoolObject: Tag atanmamış veya ObjectPoolManager bulunamadı. Obje yok ediliyor: " + this.name);
			Destroy(this.gameObject);
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("DespawnZone"))
		{
			ReturnToPool();
		}
	}

	void OnEnable()
	{
	}

	void OnDisable()
	{
		if (GetComponent<Rigidbody2D>() != null)
		{
			GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
			GetComponent<Rigidbody2D>().angularVelocity = 0f;
		}
	}
}