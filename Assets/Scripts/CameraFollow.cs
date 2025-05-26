using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	[SerializeField] private Transform _target;
	[SerializeField] private Vector3 _offset = new Vector3(0f, 0f, -10f);

	void LateUpdate()
	{
		Vector3 newPosition = new Vector3(_target.position.x + _offset.x, _offset.y, _offset.z);

		transform.position = newPosition;
	}
}