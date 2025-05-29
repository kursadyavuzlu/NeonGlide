using UnityEngine;
using System.Collections;

public class ScoreMultiplierPowerUp : MonoBehaviour, IPowerUpEffect
{
	// PlayerPowerUpManager'dan taþýnan deðiþkenler
	[SerializeField] private int _scoreMultiplierAmount = 2;
	[SerializeField] private float _scoreMultiplierDuration = 7f;

	private PlayerController _playerController; // Þu an doðrudan kullanýlmasa da, Activate metodunun imzasýna uygunluk için tutuluyor
	private PlayerPowerUpManager _powerUpManager;

	private Coroutine _activeScoreMultiplierRoutine; // Mevcut Coroutine'i durdurmak için

	public void Activate(PlayerController player, PlayerPowerUpManager powerUpManager)
	{
		_playerController = player;
		_powerUpManager = powerUpManager;

		Debug.Log("ScoreMultiplierPowerUp: Activate çaðrýldý.");

		if (_activeScoreMultiplierRoutine != null)
		{
			StopCoroutine(_activeScoreMultiplierRoutine);
			Debug.Log("ScoreMultiplierPowerUp: Mevcut Score Multiplier rutini durduruldu.");
		}
		_activeScoreMultiplierRoutine = StartCoroutine(ScoreMultiplierRoutine());
	}

	public void Deactivate()
	{
		Debug.Log("ScoreMultiplierPowerUp: Deactivate çaðrýldý.");
		if (_activeScoreMultiplierRoutine != null)
		{
			StopCoroutine(_activeScoreMultiplierRoutine);
			_activeScoreMultiplierRoutine = null;
		}

		// Deactive edildiðinde çarpaný sýfýrla
		if (GameManager.Instance != null)
		{
			GameManager.Instance.ResetScoreMultiplier();
		}
	}

	public bool IsActive()
	{
		return _activeScoreMultiplierRoutine != null;
	}

	IEnumerator ScoreMultiplierRoutine()
	{
		Debug.Log("ScoreMultiplierPowerUp: Score Multiplier Rutini BAÞLADI! Çarpan: " + _scoreMultiplierAmount + "x");

		if (GameManager.Instance != null)
		{
			GameManager.Instance.SetScoreMultiplier(_scoreMultiplierAmount);
		}
		else
		{
			Debug.LogWarning("ScoreMultiplierPowerUp: GameManager.Instance bulunamadý! Puan çarpaný uygulanamadý.");
		}

		yield return new WaitForSeconds(_scoreMultiplierDuration);

		_activeScoreMultiplierRoutine = null;
		Debug.Log("ScoreMultiplierPowerUp: Score Multiplier Rutini BÝTTÝ! Çarpan sýfýrlandý.");

		if (GameManager.Instance != null)
		{
			GameManager.Instance.ResetScoreMultiplier();
		}
	}

	void OnDestroy()
	{
		if (_activeScoreMultiplierRoutine != null)
		{
			StopCoroutine(_activeScoreMultiplierRoutine);
		}
		// GameObject yok edildiðinde çarpanýn sýfýrlandýðýndan emin ol
		if (GameManager.Instance != null && IsActive())
		{
			GameManager.Instance.ResetScoreMultiplier();
		}
	}
}