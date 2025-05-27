using UnityEngine;

public class ObstacleController : MonoBehaviour
{

	private float _speed;
	private float _crossSpeed;

	public void SetSpeed(float speed)
	{
		_speed = speed;
	}

	public void SetCrossSpeed(float crossSpeed)
	{
		_crossSpeed = crossSpeed;
	}

	void Update()
	{

		transform.Translate(new Vector3(-_speed, _crossSpeed, 0) * Time.deltaTime);

		if (transform.position.x < -10f)
		{
			Destroy(gameObject);
		}
	}
}