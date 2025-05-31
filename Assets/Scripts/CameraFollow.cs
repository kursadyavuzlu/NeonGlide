using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	[SerializeField] private Transform _target;
	[SerializeField] private Vector3 _offset = new Vector3(0f, 0f, -10f);

	[Header("Speed Progression")]
	[SerializeField] private float _baseFollowSpeed = 5f;
	[SerializeField] private float _speedIncreaseRate = 0.1f;
	[SerializeField] private float _maxFollowSpeed = 20f;
	[SerializeField] private float _speedIncreaseInterval = 10f;

	private float _currentFollowSpeed;
	private float _nextSpeedIncreaseTime;

	void Start()
	{
		_currentFollowSpeed = _baseFollowSpeed;
		_nextSpeedIncreaseTime = Time.time + _speedIncreaseInterval;
	}

	void LateUpdate()
	{
		Vector3 targetPosition = new Vector3(_target.position.x + _offset.x, _offset.y, _offset.z);
		transform.position = Vector3.MoveTowards(transform.position, targetPosition, _currentFollowSpeed * Time.deltaTime);

		if (Time.time > _nextSpeedIncreaseTime)
		{
			IncreaseCameraSpeed();
			_nextSpeedIncreaseTime = Time.time + _speedIncreaseInterval;
		}
	}

	private void IncreaseCameraSpeed()
	{
		_currentFollowSpeed = Mathf.Min(_maxFollowSpeed, _currentFollowSpeed + _speedIncreaseRate);
		Debug.Log($"Camera Speed Increased! New Follow Speed: {_currentFollowSpeed:F2}");
	}
}