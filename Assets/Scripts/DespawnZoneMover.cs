using UnityEngine;

public class DespawnZoneMover : MonoBehaviour
{
	[SerializeField] private Camera _mainCamera;
	[SerializeField] private float _xOffsetFromCameraEdge = -10f;

	void Awake()
	{
		if (_mainCamera == null)
		{
			_mainCamera = Camera.main;
		}

		if (_mainCamera == null)
		{
			Debug.LogError("DespawnZoneMover: Main Camera not found! Please assign it in the Inspector or ensure a camera is tagged 'MainCamera'.");
			enabled = false;
		}
	}

	void Update()
	{
		if (_mainCamera != null)
		{
			float cameraLeftEdgeX = _mainCamera.ScreenToWorldPoint(new Vector3(0, _mainCamera.pixelHeight / 2, _mainCamera.transform.position.z - _mainCamera.nearClipPlane)).x;

			float newXPosition = cameraLeftEdgeX + _xOffsetFromCameraEdge;

			transform.position = new Vector3(newXPosition, transform.position.y, transform.position.z);
		}
	}
}