using UnityEngine;
using System.Collections;

public class JumpBoostPowerUp : MonoBehaviour, IPowerUpEffect
{
	// PlayerPowerUpManager'dan taþýnan deðiþkenler
	[SerializeField] private float _jumpBoostMultiplier = 1.5f;
	[SerializeField] private float _jumpBoostDuration = 7f;

	private PlayerController _playerController;
	private PlayerPowerUpManager _powerUpManager; // Gerektiðinde PlayerPowerUpManager'dan bilgi almak için

	private Coroutine _activeJumpBoostRoutine; // Mevcut Coroutine'i durdurmak için

	public void Activate(PlayerController player, PlayerPowerUpManager powerUpManager)
	{
		_playerController = player;
		_powerUpManager = powerUpManager;

		Debug.Log("JumpBoostPowerUp: Activate çaðrýldý.");

		if (_activeJumpBoostRoutine != null)
		{
			StopCoroutine(_activeJumpBoostRoutine);
			Debug.Log("JumpBoostPowerUp: Mevcut Jump Boost rutini durduruldu.");
		}
		_activeJumpBoostRoutine = StartCoroutine(JumpBoostRoutine());
	}

	public void Deactivate()
	{
		Debug.Log("JumpBoostPowerUp: Deactivate çaðrýldý.");
		if (_activeJumpBoostRoutine != null)
		{
			StopCoroutine(_activeJumpBoostRoutine);
			_activeJumpBoostRoutine = null;
		}
		// Eðer burada özel bir görsel/iþitsel efektin devre dýþý býrakýlmasý gerekiyorsa buraya ekle.
		// Þimdilik sadece Debug.Log yeterli.
	}

	public bool IsActive()
	{
		return _activeJumpBoostRoutine != null;
	}

	IEnumerator JumpBoostRoutine()
	{
		Debug.Log("JumpBoostPowerUp: Jump Boost Rutini BAÞLADI!");
		// Bu Power-Up'ýn etkisi doðrudan PlayerController'ýn FixedUpdate'indeki GetJumpBoostActive()
		// ve GetJumpBoostMultiplier() metodlarý üzerinden alýnacak.
		// Burada görsel/iþitsel bir geri bildirim baþlatýlabilir.

		yield return new WaitForSeconds(_jumpBoostDuration);

		_activeJumpBoostRoutine = null;
		Debug.Log("JumpBoostPowerUp: Jump Boost Rutini BÝTTÝ!");
		// Görsel/iþitsel geri bildirim burada sonlandýrýlabilir.
	}

	void OnDestroy()
	{
		if (_activeJumpBoostRoutine != null)
		{
			StopCoroutine(_activeJumpBoostRoutine);
		}
	}

	// PlayerController'ýn kullanabilmesi için Getter metodlarý
	public float GetJumpBoostMultiplier()
	{
		return _jumpBoostMultiplier;
	}
}