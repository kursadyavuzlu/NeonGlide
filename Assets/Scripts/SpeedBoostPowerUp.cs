using UnityEngine;
using System.Collections;

public class SpeedBoostPowerUp : MonoBehaviour, IPowerUpEffect
{
	// PlayerPowerUpManager'dan taþýnan deðiþkenler
	[SerializeField] private float _speedBoostMultiplier = 1.5f;
	[SerializeField] private float _speedBoostDuration = 5f;

	private PlayerController _playerController;
	private PlayerPowerUpManager _powerUpManager;

	private Coroutine _activeSpeedBoostRoutine; // Mevcut Coroutine'i durdurmak için

	public void Activate(PlayerController player, PlayerPowerUpManager powerUpManager)
	{
		_playerController = player;
		_powerUpManager = powerUpManager;

		Debug.Log("SpeedBoostPowerUp: Activate çaðrýldý.");

		if (_activeSpeedBoostRoutine != null)
		{
			StopCoroutine(_activeSpeedBoostRoutine);
			Debug.Log("SpeedBoostPowerUp: Mevcut Speed Boost rutini durduruldu.");
		}
		_activeSpeedBoostRoutine = StartCoroutine(SpeedBoostRoutine());
	}

	public void Deactivate()
	{
		Debug.Log("SpeedBoostPowerUp: Deactivate çaðrýldý.");
		if (_activeSpeedBoostRoutine != null)
		{
			StopCoroutine(_activeSpeedBoostRoutine);
			_activeSpeedBoostRoutine = null;
		}
		// Görsel/iþitsel efektler burada devre dýþý býrakýlabilir.
	}

	public bool IsActive()
	{
		return _activeSpeedBoostRoutine != null;
	}

	IEnumerator SpeedBoostRoutine()
	{
		Debug.Log("SpeedBoostPowerUp: Speed Boost Rutini BAÞLADI!");
		// Bu Power-Up'ýn etkisi doðrudan PlayerController'ýn FixedUpdate'indeki GetIsSpeedBoostActive()
		// ve GetSpeedBoostMultiplier() metodlarý üzerinden alýnacak.
		// Burada görsel/iþitsel bir geri bildirim baþlatýlabilir.

		yield return new WaitForSeconds(_speedBoostDuration);

		_activeSpeedBoostRoutine = null;
		Debug.Log("SpeedBoostPowerUp: Speed Boost Rutini BÝTTÝ!");
		// Görsel/iþitsel geri bildirim burada sonlandýrýlabilir.
	}

	void OnDestroy()
	{
		if (_activeSpeedBoostRoutine != null)
		{
			StopCoroutine(_activeSpeedBoostRoutine);
		}
	}

	// PlayerController'ýn kullanabilmesi için Getter metodlarý
	public float GetSpeedBoostMultiplier()
	{
		return _speedBoostMultiplier;
	}
}