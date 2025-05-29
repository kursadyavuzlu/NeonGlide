using UnityEngine;
using System.Collections; // Coroutine için

// IPowerUpEffect arayüzü ayný namespace'te veya globalde tanýmlýysa using eklemeye gerek olmayabilir.
// Ancak emin olmak için ekleyebiliriz:
// using YourGameNamespace; // Eðer IPowerUpEffect bir namespace içindeyse (emin deðilsen ekleyebilirsin, sorun olmaz)

public class ShieldPowerUp : MonoBehaviour, IPowerUpEffect
{
	// PlayerController'dan PlayerPowerUpManager'a, oradan buraya taþýnan deðiþkenler
	[SerializeField] private float _shieldDuration = 5f;
	[SerializeField] private GameObject _shieldEffectObject; // Kalkanýn görsel efekti

	// PlayerPowerUpManager'dan alýnacak referanslar
	private PlayerController _playerController; // PlayerController'ýn kendisini doðrudan deðil, etkilerini kontrol etmesi için
	private PlayerPowerUpManager _powerUpManager; // Durum güncellemesi için

	private Coroutine _activeShieldRoutine; // Mevcut Coroutine'i durdurmak için

	// IPowerUpEffect arayüzünün gerektirdiði metodlar
	public void Activate(PlayerController player, PlayerPowerUpManager powerUpManager)
	{
		_playerController = player;
		_powerUpManager = powerUpManager;

		// Eðer halihazýrda bir kalkan aktifse, süresini sýfýrla ve yeniden baþlat
		if (_activeShieldRoutine != null)
		{
			StopCoroutine(_activeShieldRoutine);
		}
		_activeShieldRoutine = StartCoroutine(ShieldRoutine());
	}

	// ShieldPowerUp'ýn dýþarýdan devre dýþý býrakýlmasý için
	public void Deactivate()
	{
		if (_activeShieldRoutine != null)
		{
			StopCoroutine(_activeShieldRoutine);
			_activeShieldRoutine = null; // Coroutine referansýný temizle
		}
		// Kalkan efekti objesini devre dýþý býrak
		if (_shieldEffectObject != null)
		{
			_shieldEffectObject.SetActive(false);
		}
		Debug.Log("ShieldPowerUp: Kalkan dýþarýdan devre dýþý býrakýldý.");
	}

	IEnumerator ShieldRoutine()
	{
		Debug.Log("ShieldPowerUp: ShieldRoutine BAÞLADI! Player'ýn kalkaný aktifleþiyor.");

		// Kalkan efekti objesini aktif et
		if (_shieldEffectObject != null)
		{
			// ShieldEffectObject'i oyuncunun transformunun çocuðu yaparak onunla birlikte hareket etmesini saðla
			// Bu, kalkan prefab'inin sahneye eklendiði yerde deðil, oyuncunun üzerinde görünmesini saðlar.
			// Eðer _shieldEffectObject bir UI Canvas'ý veya özel bir render hedefi kullanýyorsa,
			// bu satýrý dikkatli kullanmak veya alternatif bir yaklaþým düþünmek gerekebilir.
			// Ancak, genel olarak oyuncunun etrafýnda bir efektse bu uygundur.
			_shieldEffectObject.transform.SetParent(_playerController.transform);
			_shieldEffectObject.transform.localPosition = Vector3.zero; // Oyuncunun merkezinde durmasý için
			_shieldEffectObject.SetActive(true);
		}
		else
		{
			Debug.LogWarning("ShieldPowerUp: _shieldEffectObject atanmamýþ! Kalkan efekti görseli çalýþmayabilir.");
		}

		yield return new WaitForSeconds(_shieldDuration);

		// Kalkan süresi bittiðinde
		if (_shieldEffectObject != null)
		{
			_shieldEffectObject.SetActive(false);
			// Parent'ý temizlemeyi unutma, aksi halde GameObject yok olsa bile efekt kalabilir.
			// Ancak bu GameObject'in kendisi kalýcý olduðu için, sadece SetActive(false) yeterli olabilir.
			// Ýleride efekt GameObject'i de yok edilecekse, parent'ý null yapmak gerekebilir.
		}
		Debug.Log("ShieldPowerUp: ShieldRoutine BÝTTÝ! Player'ýn kalkaný devre dýþý býrakýlýyor.");
	}

	// PlayerPowerUpManager'ýn kalkanýn aktif olup olmadýðýný sorgulamasý için
	// Bu metod PlayerPowerUpManager'daki GetIsShieldActive() metodunun yerini alacak.
	public bool IsActive()
	{
		return _activeShieldRoutine != null; // Coroutine çalýþýyorsa kalkan aktif demektir
	}

	void OnDestroy()
	{
		// GameObject yok edildiðinde çalýþan coroutine'i durdur
		if (_activeShieldRoutine != null)
		{
			StopCoroutine(_activeShieldRoutine);
		}
	}
}