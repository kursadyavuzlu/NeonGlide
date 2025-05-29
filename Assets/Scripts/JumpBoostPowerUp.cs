using UnityEngine;
using System.Collections;

public class JumpBoostPowerUp : MonoBehaviour, IPowerUpEffect
{
	// PlayerPowerUpManager'dan ta��nan de�i�kenler
	[SerializeField] private float _jumpBoostMultiplier = 1.5f;
	[SerializeField] private float _jumpBoostDuration = 7f;

	private PlayerController _playerController;
	private PlayerPowerUpManager _powerUpManager; // Gerekti�inde PlayerPowerUpManager'dan bilgi almak i�in

	private Coroutine _activeJumpBoostRoutine; // Mevcut Coroutine'i durdurmak i�in

	public void Activate(PlayerController player, PlayerPowerUpManager powerUpManager)
	{
		_playerController = player;
		_powerUpManager = powerUpManager;

		Debug.Log("JumpBoostPowerUp: Activate �a�r�ld�.");

		if (_activeJumpBoostRoutine != null)
		{
			StopCoroutine(_activeJumpBoostRoutine);
			Debug.Log("JumpBoostPowerUp: Mevcut Jump Boost rutini durduruldu.");
		}
		_activeJumpBoostRoutine = StartCoroutine(JumpBoostRoutine());
	}

	public void Deactivate()
	{
		Debug.Log("JumpBoostPowerUp: Deactivate �a�r�ld�.");
		if (_activeJumpBoostRoutine != null)
		{
			StopCoroutine(_activeJumpBoostRoutine);
			_activeJumpBoostRoutine = null;
		}
		// E�er burada �zel bir g�rsel/i�itsel efektin devre d��� b�rak�lmas� gerekiyorsa buraya ekle.
		// �imdilik sadece Debug.Log yeterli.
	}

	public bool IsActive()
	{
		return _activeJumpBoostRoutine != null;
	}

	IEnumerator JumpBoostRoutine()
	{
		Debug.Log("JumpBoostPowerUp: Jump Boost Rutini BA�LADI!");
		// Bu Power-Up'�n etkisi do�rudan PlayerController'�n FixedUpdate'indeki GetJumpBoostActive()
		// ve GetJumpBoostMultiplier() metodlar� �zerinden al�nacak.
		// Burada g�rsel/i�itsel bir geri bildirim ba�lat�labilir.

		yield return new WaitForSeconds(_jumpBoostDuration);

		_activeJumpBoostRoutine = null;
		Debug.Log("JumpBoostPowerUp: Jump Boost Rutini B�TT�!");
		// G�rsel/i�itsel geri bildirim burada sonland�r�labilir.
	}

	void OnDestroy()
	{
		if (_activeJumpBoostRoutine != null)
		{
			StopCoroutine(_activeJumpBoostRoutine);
		}
	}

	// PlayerController'�n kullanabilmesi i�in Getter metodlar�
	public float GetJumpBoostMultiplier()
	{
		return _jumpBoostMultiplier;
	}
}