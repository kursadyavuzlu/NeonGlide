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

	private float _scoreFloat = 0f; // Skoru ondalýklý olarak tutmak için yeni deðiþken
	private int _displayScore = 0; // UI'da gösterilecek tam sayý skor
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

		// Skorlarý sýfýrla ve UI'ý güncelle
		_scoreFloat = 0f;
		_displayScore = 0;
		UpdateScoreUI();
		LoadAndSetHighScores();
	}

	void Update()
	{
		// Debug.Log("GameManager Update() çalýþýyor."); // Test için eklediðimiz bu debug satýrýný kaldýrýyorum

		if (Time.timeScale > 0)
		{
			// Skoru float olarak artýr (saniyede 10 birim)
			_scoreFloat += Time.deltaTime * 10f;

			// Float skordan tam sayý kýsmýný alarak gösterilecek skoru belirle
			int newDisplayScore = Mathf.FloorToInt(_scoreFloat);

			// Eðer gösterilecek skor deðiþtiyse, UI'ý güncelle
			if (newDisplayScore != _displayScore)
			{
				_displayScore = newDisplayScore;
				UpdateScoreUI();
			}
		}
		// else
		// {
		//     Debug.Log("Time.timeScale is 0 or less: " + Time.timeScale); // Test için eklediðimiz bu debug satýrýný kaldýrýyorum
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

			// Yüksek skoru kontrol et ve kaydet
			int currentHighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
			if (_displayScore > currentHighScore) // _displayScore kullan
			{
				PlayerPrefs.SetInt(HighScoreKey, _displayScore); // _displayScore kullan
				Debug.Log("New High Score: " + _displayScore); // _displayScore kullan
			}
			PlayerPrefs.Save(); // Deðiþiklikleri kaydet

			if (_gameOverPanel != null)
			{
				_gameOverPanel.SetActive(true);
				if (_finalScoreText != null)
				{
					_finalScoreText.text = "Your Score: " + _displayScore.ToString(); // _displayScore kullan
				}
				LoadAndSetHighScores(); // Oyun sonunda yüksek skoru tekrar göster (yeni yüksek skor güncellenmiþ olabilir)
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