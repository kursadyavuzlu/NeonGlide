using UnityEngine;
using System.Collections;

public class TimeSlowdownPowerUp : MonoBehaviour, IPowerUpEffect
{
	// PlayerPowerUpManager'dan taþýnan deðiþkenler
	[SerializeField] private float _timeSlowdownFactor = 0.5f; // Zamanýn yavaþlama oraný (0.5f = %50 hýz)
	[SerializeField] private float _timeSlowdownDuration = 6f; // Yavaþlama süresi

	private PlayerController _playerController; // Þu an doðrudan kullanýlmasa da, Activate metodunun imzasýna uygunluk için tutuluyor
	private PlayerPowerUpManager _powerUpManager;

	private Coroutine _activeTimeSlowdownRoutine; // Mevcut Coroutine'i durdurmak için

	public void Activate(PlayerController player, PlayerPowerUpManager powerUpManager)
	{
		_playerController = player;
		_powerUpManager = powerUpManager;

		Debug.Log("TimeSlowdownPowerUp: Activate çaðrýldý.");

		if (_activeTimeSlowdownRoutine != null)
		{
			StopCoroutine(_activeTimeSlowdownRoutine);
			Debug.Log("TimeSlowdownPowerUp: Mevcut Time Slowdown rutini durduruldu.");
		}
		_activeTimeSlowdownRoutine = StartCoroutine(TimeSlowdownRoutine());
	}

	public void Deactivate()
	{
		Debug.Log("TimeSlowdownPowerUp: Deactivate çaðrýldý.");
		if (_activeTimeSlowdownRoutine != null)
		{
			StopCoroutine(_activeTimeSlowdownRoutine);
			_activeTimeSlowdownRoutine = null;
		}

		// Deactive edildiðinde zaman ölçeðini sýfýrla
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
		Debug.Log("TimeSlowdownPowerUp: Time Slowdown Rutini BAÞLADI! Zaman Ölçeði: " + _timeSlowdownFactor);

		if (GameManager.Instance != null)
		{
			GameManager.Instance.SetTimeScale(_timeSlowdownFactor);
		}
		else
		{
			Debug.LogWarning("TimeSlowdownPowerUp: GameManager.Instance bulunamadý! Zaman yavaþlatma uygulanamadý.");
		}

		// Zaman yavaþladýðý için bekleme süresi de yavaþlayacak, bu yüzden normal süresini çarpana bölüyoruz.
		yield return new WaitForSeconds(_timeSlowdownDuration * _timeSlowdownFactor);

		_activeTimeSlowdownRoutine = null;
		Debug.Log("TimeSlowdownPowerUp: Time Slowdown Rutini BÝTTÝ! Zaman Ölçeði sýfýrlandý.");

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
		// GameObject yok edildiðinde zaman ölçeðinin sýfýrlandýðýndan emin ol
		if (GameManager.Instance != null && IsActive())
		{
			GameManager.Instance.ResetTimeScale();
		}
	}
}