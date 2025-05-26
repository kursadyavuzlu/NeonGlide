using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	[Header("UI References")]
	[SerializeField] private GameObject _gameOverPanel;



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

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		if (_gameOverPanel != null)
		{
			_gameOverPanel.SetActive(false);
		}

		Time.timeScale = 1f;
	}

	// Update is called once per frame
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

			Time.timeScale = 0f;
		}
	}

	public void RestartGame()
	{


		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}