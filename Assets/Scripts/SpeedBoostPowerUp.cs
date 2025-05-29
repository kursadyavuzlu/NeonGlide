using UnityEngine;
using System.Collections;

public class SpeedBoostPowerUp : MonoBehaviour, IPowerUpEffect
{
	// PlayerPowerUpManager'dan ta��nan de�i�kenler
	[SerializeField] private float _speedBoostMultiplier = 1.5f;
	[SerializeField] private float _speedBoostDuration = 5f;

	private PlayerController _playerController;
	private PlayerPowerUpManager _powerUpManager;

	private Coroutine _activeSpeedBoostRoutine; // Mevcut Coroutine'i durdurmak i�in

	public void Activate(PlayerController player, PlayerPowerUpManager powerUpManager)
	{
		_playerController = player;
		_powerUpManager = powerUpManager;

		Debug.Log("SpeedBoostPowerUp: Activate �a�r�ld�.");

		if (_activeSpeedBoostRoutine != null)
		{
			StopCoroutine(_activeSpeedBoostRoutine);
			Debug.Log("SpeedBoostPowerUp: Mevcut Speed Boost rutini durduruldu.");
		}
		_activeSpeedBoostRoutine = StartCoroutine(SpeedBoostRoutine());
	}

	public void Deactivate()
	{
		Debug.Log("SpeedBoostPowerUp: Deactivate �a�r�ld�.");
		if (_activeSpeedBoostRoutine != null)
		{
			StopCoroutine(_activeSpeedBoostRoutine);
			_activeSpeedBoostRoutine = null;
		}
		// G�rsel/i�itsel efektler burada devre d��� b�rak�labilir.
	}

	public bool IsActive()
	{
		return _activeSpeedBoostRoutine != null;
	}

	IEnumerator SpeedBoostRoutine()
	{
		Debug.Log("SpeedBoostPowerUp: Speed Boost Rutini BA�LADI!");
		// Bu Power-Up'�n etkisi do�rudan PlayerController'�n FixedUpdate'indeki GetIsSpeedBoostActive()
		// ve GetSpeedBoostMultiplier() metodlar� �zerinden al�nacak.
		// Burada g�rsel/i�itsel bir geri bildirim ba�lat�labilir.

		yield return new WaitForSeconds(_speedBoostDuration);

		_activeSpeedBoostRoutine = null;
		Debug.Log("SpeedBoostPowerUp: Speed Boost Rutini B�TT�!");
		// G�rsel/i�itsel geri bildirim burada sonland�r�labilir.
	}

	void OnDestroy()
	{
		if (_activeSpeedBoostRoutine != null)
		{
			StopCoroutine(_activeSpeedBoostRoutine);
		}
	}

	// PlayerController'�n kullanabilmesi i�in Getter metodlar�
	public float GetSpeedBoostMultiplier()
	{
		return _speedBoostMultiplier;
	}
}