using UnityEngine;
using System.Collections;

public class ScoreMultiplierPowerUp : MonoBehaviour, IPowerUpEffect
{
	// PlayerPowerUpManager'dan ta��nan de�i�kenler
	[SerializeField] private int _scoreMultiplierAmount = 2;
	[SerializeField] private float _scoreMultiplierDuration = 7f;

	private PlayerController _playerController; // �u an do�rudan kullan�lmasa da, Activate metodunun imzas�na uygunluk i�in tutuluyor
	private PlayerPowerUpManager _powerUpManager;

	private Coroutine _activeScoreMultiplierRoutine; // Mevcut Coroutine'i durdurmak i�in

	public void Activate(PlayerController player, PlayerPowerUpManager powerUpManager)
	{
		_playerController = player;
		_powerUpManager = powerUpManager;

		Debug.Log("ScoreMultiplierPowerUp: Activate �a�r�ld�.");

		if (_activeScoreMultiplierRoutine != null)
		{
			StopCoroutine(_activeScoreMultiplierRoutine);
			Debug.Log("ScoreMultiplierPowerUp: Mevcut Score Multiplier rutini durduruldu.");
		}
		_activeScoreMultiplierRoutine = StartCoroutine(ScoreMultiplierRoutine());
	}

	public void Deactivate()
	{
		Debug.Log("ScoreMultiplierPowerUp: Deactivate �a�r�ld�.");
		if (_activeScoreMultiplierRoutine != null)
		{
			StopCoroutine(_activeScoreMultiplierRoutine);
			_activeScoreMultiplierRoutine = null;
		}

		// Deactive edildi�inde �arpan� s�f�rla
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
		Debug.Log("ScoreMultiplierPowerUp: Score Multiplier Rutini BA�LADI! �arpan: " + _scoreMultiplierAmount + "x");

		if (GameManager.Instance != null)
		{
			GameManager.Instance.SetScoreMultiplier(_scoreMultiplierAmount);
		}
		else
		{
			Debug.LogWarning("ScoreMultiplierPowerUp: GameManager.Instance bulunamad�! Puan �arpan� uygulanamad�.");
		}

		yield return new WaitForSeconds(_scoreMultiplierDuration);

		_activeScoreMultiplierRoutine = null;
		Debug.Log("ScoreMultiplierPowerUp: Score Multiplier Rutini B�TT�! �arpan s�f�rland�.");

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
		// GameObject yok edildi�inde �arpan�n s�f�rland���ndan emin ol
		if (GameManager.Instance != null && IsActive())
		{
			GameManager.Instance.ResetScoreMultiplier();
		}
	}
}