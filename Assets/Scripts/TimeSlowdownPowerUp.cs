using UnityEngine;
using System.Collections;

public class TimeSlowdownPowerUp : MonoBehaviour, IPowerUpEffect
{
	// PlayerPowerUpManager'dan ta��nan de�i�kenler
	[SerializeField] private float _timeSlowdownFactor = 0.5f; // Zaman�n yava�lama oran� (0.5f = %50 h�z)
	[SerializeField] private float _timeSlowdownDuration = 6f; // Yava�lama s�resi

	private PlayerController _playerController; // �u an do�rudan kullan�lmasa da, Activate metodunun imzas�na uygunluk i�in tutuluyor
	private PlayerPowerUpManager _powerUpManager;

	private Coroutine _activeTimeSlowdownRoutine; // Mevcut Coroutine'i durdurmak i�in

	public void Activate(PlayerController player, PlayerPowerUpManager powerUpManager)
	{
		_playerController = player;
		_powerUpManager = powerUpManager;

		Debug.Log("TimeSlowdownPowerUp: Activate �a�r�ld�.");

		if (_activeTimeSlowdownRoutine != null)
		{
			StopCoroutine(_activeTimeSlowdownRoutine);
			Debug.Log("TimeSlowdownPowerUp: Mevcut Time Slowdown rutini durduruldu.");
		}
		_activeTimeSlowdownRoutine = StartCoroutine(TimeSlowdownRoutine());
	}

	public void Deactivate()
	{
		Debug.Log("TimeSlowdownPowerUp: Deactivate �a�r�ld�.");
		if (_activeTimeSlowdownRoutine != null)
		{
			StopCoroutine(_activeTimeSlowdownRoutine);
			_activeTimeSlowdownRoutine = null;
		}

		// Deactive edildi�inde zaman �l�e�ini s�f�rla
		if (GameManager.Instance != null)
		{
			GameManager.Instance.ResetTimeScale();
		}
	}

	public bool IsActive()
	{
		return _activeTimeSlowdownRoutine != null;
	}

	IEnumerator TimeSlowdownRoutine()
	{
		Debug.Log("TimeSlowdownPowerUp: Time Slowdown Rutini BA�LADI! Zaman �l�e�i: " + _timeSlowdownFactor);

		if (GameManager.Instance != null)
		{
			GameManager.Instance.SetTimeScale(_timeSlowdownFactor);
		}
		else
		{
			Debug.LogWarning("TimeSlowdownPowerUp: GameManager.Instance bulunamad�! Zaman yava�latma uygulanamad�.");
		}

		// Zaman yava�lad��� i�in bekleme s�resi de yava�layacak, bu y�zden normal s�resini �arpana b�l�yoruz.
		yield return new WaitForSeconds(_timeSlowdownDuration * _timeSlowdownFactor);

		_activeTimeSlowdownRoutine = null;
		Debug.Log("TimeSlowdownPowerUp: Time Slowdown Rutini B�TT�! Zaman �l�e�i s�f�rland�.");

		if (GameManager.Instance != null)
		{
			GameManager.Instance.ResetTimeScale();
		}
	}

	void OnDestroy()
	{
		if (_activeTimeSlowdownRoutine != null)
		{
			StopCoroutine(_activeTimeSlowdownRoutine);
		}
		// GameObject yok edildi�inde zaman �l�e�inin s�f�rland���ndan emin ol
		if (GameManager.Instance != null && IsActive())
		{
			GameManager.Instance.ResetTimeScale();
		}
	}
}