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

	private float _scoreFloat = 0f;
	private int _displayScore = 0;
	private const string HighScoreKey = "HighScore";

	private PlayerController _playerController;

	[Header("Audio Settings")]
	[SerializeField] private AudioClip _gameOverSound;

	private AudioSource _gameManagerAudioSource;

	private int _currentScoreMultiplier = 1;
	// --- Yeni EKLENEN KOD BAÞLANGICI ---
	private float _originalTimeScale = 1f; // Oyunun orijinal zaman ölçeði
										   // --- Yeni EKLENEN KOD SONU ---


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

		// --- Burasý GÜNCELLENDÝ: Time.timeScale baþlangýçta 1f'e ayarlanmalý ---
		Time.timeScale = 1f;
		_originalTimeScale = 1f; // Orijinal zaman ölçeðini kaydet
								 // --- GÜNCELLEME SONU ---


		_playerController = FindAnyObjectByType<PlayerController>();
		if (_playerController == null)
		{
			Debug.LogError("PlayerController script not found in the scene! Make sure your Player object has the PlayerController component.");
		}

		_scoreFloat = 0f;
		_displayScore = 0;
		_currentScoreMultiplier = 1;
		UpdateScoreUI();
		LoadAndSetHighScores();
	}

	void Update()
	{
		if (Time.timeScale > 0)
		{
			_scoreFloat += Time.deltaTime * 10f * _currentScoreMultiplier;

			int newDisplayScore = Mathf.FloorToInt(_scoreFloat);

			if (newDisplayScore != _displayScore)
			{
				_displayScore = newDisplayScore;
				UpdateScoreUI();
			}
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
		if (Time.timeScale > 0)
		{
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

			// Oyun bittiðinde zamaný sýfýrlayalým, ancak sesin çalmasýna izin vermek için sona býrakalým
			Time.timeScale = 0f;
		}
	}

	public void RestartGame()
	{
		Time.timeScale = 1f; // Sahneyi yeniden yüklemeden önce zaman ölçeðini sýfýrla
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

	// --- Yeni EKLENEN KOD BAÞLANGICI ---

	/// <summary>
	/// Oyunun global zaman ölçeðini ayarlar.
	/// </summary>
	/// <param name="newTimeScale">Yeni zaman ölçeði deðeri (örn: 0.5f için %50 hýz).</param>
	public void SetTimeScale(float newTimeScale)
	{
		// Mevcut zaman ölçeðini kaydet (eðer zaten yavaþlatýlmýþsa üst üste binmesin)
		// Veya orijinal _originalTimeScale'i koru ve sadece yavaþlat
		_originalTimeScale = Time.timeScale; // Mevcut Time.timeScale'i kaydet (eðer baþka bir þey deðiþtirdiyse)
		Time.timeScale = newTimeScale;
		Debug.Log("GameManager: Time scale set to " + newTimeScale);
	}

	/// <summary>
	/// Oyunun zaman ölçeðini orijinal deðerine (1f) sýfýrlar.
	/// </summary>
	public void ResetTimeScale()
	{
		Time.timeScale = 1f; // Orijinal zaman ölçeðine döndür
		Debug.Log("GameManager: Time scale reset to 1f.");
	}
	// --- Yeni EKLENEN KOD SONU ---
}