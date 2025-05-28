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

	private float _scoreFloat = 0f; // Skoru ondal�kl� olarak tutmak i�in yeni de�i�ken
	private int _displayScore = 0; // UI'da g�sterilecek tam say� skor
	private const string HighScoreKey = "HighScore";

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

		// Skorlar� s�f�rla ve UI'� g�ncelle
		_scoreFloat = 0f;
		_displayScore = 0;
		UpdateScoreUI();
		LoadAndSetHighScores();
	}

	void Update()
	{
		// Debug.Log("GameManager Update() �al���yor."); // Test i�in ekledi�imiz bu debug sat�r�n� kald�r�yorum

		if (Time.timeScale > 0)
		{
			// Skoru float olarak art�r (saniyede 10 birim)
			_scoreFloat += Time.deltaTime * 10f;

			// Float skordan tam say� k�sm�n� alarak g�sterilecek skoru belirle
			int newDisplayScore = Mathf.FloorToInt(_scoreFloat);

			// E�er g�sterilecek skor de�i�tiyse, UI'� g�ncelle
			if (newDisplayScore != _displayScore)
			{
				_displayScore = newDisplayScore;
				UpdateScoreUI();
			}
		}
		// else
		// {
		//     Debug.Log("Time.timeScale is 0 or less: " + Time.timeScale); // Test i�in ekledi�imiz bu debug sat�r�n� kald�r�yorum
		// }
	}

	private void UpdateScoreUI()
	{
		if (_scoreText != null)
		{
			_scoreText.text = "Score: " + _displayScore.ToString(); // _displayScore kullan
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
		if (Time.timeScale > 0)
		{
			Debug.Log("Game over! Final Score: " + _displayScore); // _displayScore kullan

			// Y�ksek skoru kontrol et ve kaydet
			int currentHighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
			if (_displayScore > currentHighScore) // _displayScore kullan
			{
				PlayerPrefs.SetInt(HighScoreKey, _displayScore); // _displayScore kullan
				Debug.Log("New High Score: " + _displayScore); // _displayScore kullan
			}
			PlayerPrefs.Save(); // De�i�iklikleri kaydet

			if (_gameOverPanel != null)
			{
				_gameOverPanel.SetActive(true);
				if (_finalScoreText != null)
				{
					_finalScoreText.text = "Your Score: " + _displayScore.ToString(); // _displayScore kullan
				}
				LoadAndSetHighScores(); // Oyun sonunda y�ksek skoru tekrar g�ster (yeni y�ksek skor g�ncellenmi� olabilir)
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