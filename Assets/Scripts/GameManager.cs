using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	[Header("UI References")]
	[SerializeField] private GameObject _gameOverPanel;

	private PlayerController _playerController;

	private void Awake()
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
	}

	void Start()
	{
		if (_gameOverPanel != null)
		{
			_gameOverPanel.SetActive(false);
		}

		Time.timeScale = 1f;

		_playerController = FindAnyObjectByType<PlayerController>();
		if (_playerController == null)
		{
			Debug.LogError("PlayerController script not found in the scene! Make sure your Player object has the PlayerController component.");
		}
	}

	void Update()
	{
		
	}

	public void GameOver()
	{
		if (Time.timeScale > 0)
		{
			Debug.Log("Game over!");

			if (_gameOverPanel != null)
			{
				_gameOverPanel.SetActive(true);
			}

			if (_playerController != null)
			{
				_playerController.StopPlayerMovement();
			}

			Time.timeScale = 0f;
		}
	}

	public void RestartGame()
	{

		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}