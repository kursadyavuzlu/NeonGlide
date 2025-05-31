using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	[Header("UI References")]
	[SerializeField] private GameObject _gameOverPanel;
	[SerializeField] private TextMeshProUGUI _scoreText;
	[SerializeField] private TextMeshProUGUI _finalScoreText;
	[SerializeField] private TextMeshProUGUI _highScoreText;
	[SerializeField] private GameObject _pauseMenuPanel;

	private float _scoreFloat = 0f;
	private int _displayScore = 0;
	private const string HighScoreKey = "HighScore";

	private PlayerController _playerController;

	[Header("Game State")]
	private bool _isGamePaused = false;

	[Header("Audio Settings")]
	[SerializeField] private AudioClip _gameOverSound;
	private AudioSource _gameManagerAudioSource;

	private int _currentScoreMultiplier = 1;
	private float _originalTimeScale = 1f;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
		}
		else
		{
			Instance = this;
		}

		_gameManagerAudioSource = GetComponent<AudioSource>();
		if (_gameManagerAudioSource == null)
		{
			Debug.LogError("GameManager GameObject'inde AudioSource bileþeni bulunamadý! Lütfen bir tane eklediðinizden emin olun.");
		}
	}

	void Start()
	{
		if (_gameOverPanel != null)
		{
			_gameOverPanel.SetActive(false);
		}
		if (_pauseMenuPanel != null)
		{
			_pauseMenuPanel.SetActive(false);
		}

		Time.timeScale = 1f;
		_originalTimeScale = 1f;

		_playerController = FindAnyObjectByType<PlayerController>();
		if (_playerController == null)
		{
			Debug.LogError("GameManager: PlayerController script not found in the scene! Make sure your Player object has the PlayerController component.");
		}

		_scoreFloat = 0f;
		_displayScore = 0;
		_currentScoreMultiplier = 1;
		UpdateScoreUI();
		LoadAndSetHighScores();
	}

	void Update()
	{
		if (Time.timeScale > 0f && !_isGamePaused)
		{
			_scoreFloat += Time.deltaTime * 10f * _currentScoreMultiplier;

			int newDisplayScore = Mathf.FloorToInt(_scoreFloat);

			if (newDisplayScore != _displayScore)
			{
				_displayScore = newDisplayScore;
				UpdateScoreUI();
			}
		}

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			TogglePause();
		}
	}

	private void UpdateScoreUI()
	{
		if (_scoreText != null)
		{
			_scoreText.text = "Score: " + _displayScore.ToString() + (_currentScoreMultiplier > 1 ? " (x" + _currentScoreMultiplier + ")" : "");
		}
	}

	private void LoadAndSetHighScores()
	{
		int currentHighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
		if (_highScoreText != null)
		{
			_highScoreText.text = "High score: " + currentHighScore.ToString();
		}
	}

	public void GameOver()
	{
		if (Time.timeScale == 0f && _gameOverPanel != null && _gameOverPanel.activeSelf) return;

		Debug.Log("Game over! Final Score: " + _displayScore);

		int currentHighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
		if (_displayScore > currentHighScore)
		{
			PlayerPrefs.SetInt(HighScoreKey, _displayScore);
			Debug.Log("New High Score: " + _displayScore);
		}
		PlayerPrefs.Save();

		if (_gameOverPanel != null)
		{
			_gameOverPanel.SetActive(true);
			if (_finalScoreText != null)
			{
				_finalScoreText.text = "Your Score: " + _displayScore.ToString();
			}
			LoadAndSetHighScores();
		}

		if (_playerController != null)
		{
			_playerController.StopPlayerMovement();
		}

		if (_gameManagerAudioSource != null && _gameOverSound != null)
		{
			_gameManagerAudioSource.PlayOneShot(_gameOverSound);
		}
		Time.timeScale = 0f;
	}

	public void RestartGame()
	{
		_isGamePaused = false;
		Time.timeScale = 1f;
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void TogglePause()
	{
		if (Time.timeScale != 0f || _isGamePaused)
		{
			_isGamePaused = !_isGamePaused;

			if (_isGamePaused)
			{
				Time.timeScale = 0f;
				if (_pauseMenuPanel != null)
				{
					_pauseMenuPanel.SetActive(true);
				}
				Debug.Log("Oyun Duraklatýldý.");
			}
			else
			{
				Time.timeScale = 1f;
				if (_pauseMenuPanel != null)
				{
					_pauseMenuPanel.SetActive(false);
				}
				Debug.Log("Oyun Devam Ediyor.");

				if (_playerController != null)
				{
					_playerController.SetLastUnpausedTime(Time.timeSinceLevelLoad);
				}
			}
		}
	}

	public void ReturnToMainMenu()
	{
		_isGamePaused = false;
		Time.timeScale = 1f;
		SceneManager.LoadScene("MainMenu");
	}

	public void SetScoreMultiplier(int multiplier)
	{
		_currentScoreMultiplier = multiplier;
		Debug.Log("GameManager: Score multiplier set to x" + _currentScoreMultiplier);
		UpdateScoreUI();
	}

	public void ResetScoreMultiplier()
	{
		_currentScoreMultiplier = 1;
		Debug.Log("GameManager: Score multiplier reset to x1.");
		UpdateScoreUI();
	}

	public void SetTimeScale(float newTimeScale)
	{
		if (!_isGamePaused && Time.timeScale != 0f)
		{
			_originalTimeScale = Time.timeScale;
			Time.timeScale = newTimeScale;
			Debug.Log("GameManager: Time scale set to " + newTimeScale);
		}
	}

	public void ResetTimeScale()
	{
		if (!_isGamePaused && Time.timeScale != 0f)
		{
			Time.timeScale = _originalTimeScale;
			Debug.Log("GameManager: Time scale reset to original (" + _originalTimeScale + ").");
		}
	}
}