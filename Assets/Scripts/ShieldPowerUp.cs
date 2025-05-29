using UnityEngine;
using System.Collections; // Coroutine i�in

// IPowerUpEffect aray�z� ayn� namespace'te veya globalde tan�ml�ysa using eklemeye gerek olmayabilir.
// Ancak emin olmak i�in ekleyebiliriz:
// using YourGameNamespace; // E�er IPowerUpEffect bir namespace i�indeyse (emin de�ilsen ekleyebilirsin, sorun olmaz)

public class ShieldPowerUp : MonoBehaviour, IPowerUpEffect
{
	// PlayerController'dan PlayerPowerUpManager'a, oradan buraya ta��nan de�i�kenler
	[SerializeField] private float _shieldDuration = 5f;
	[SerializeField] private GameObject _shieldEffectObject; // Kalkan�n g�rsel efekti

	// PlayerPowerUpManager'dan al�nacak referanslar
	private PlayerController _playerController; // PlayerController'�n kendisini do�rudan de�il, etkilerini kontrol etmesi i�in
	private PlayerPowerUpManager _powerUpManager; // Durum g�ncellemesi i�in

	private Coroutine _activeShieldRoutine; // Mevcut Coroutine'i durdurmak i�in

	// IPowerUpEffect aray�z�n�n gerektirdi�i metodlar
	public void Activate(PlayerController player, PlayerPowerUpManager powerUpManager)
	{
		_playerController = player;
		_powerUpManager = powerUpManager;

		// E�er halihaz�rda bir kalkan aktifse, s�resini s�f�rla ve yeniden ba�lat
		if (_activeShieldRoutine != null)
		{
			StopCoroutine(_activeShieldRoutine);
		}
		_activeShieldRoutine = StartCoroutine(ShieldRoutine());
	}

	// ShieldPowerUp'�n d��ar�dan devre d��� b�rak�lmas� i�in
	public void Deactivate()
	{
		if (_activeShieldRoutine != null)
		{
			StopCoroutine(_activeShieldRoutine);
			_activeShieldRoutine = null; // Coroutine referans�n� temizle
		}
		// Kalkan efekti objesini devre d��� b�rak
		if (_shieldEffectObject != null)
		{
			_shieldEffectObject.SetActive(false);
		}
		Debug.Log("ShieldPowerUp: Kalkan d��ar�dan devre d��� b�rak�ld�.");
	}

	IEnumerator ShieldRoutine()
	{
		Debug.Log("ShieldPowerUp: ShieldRoutine BA�LADI! Player'�n kalkan� aktifle�iyor.");

		// Kalkan efekti objesini aktif et
		if (_shieldEffectObject != null)
		{
			// ShieldEffectObject'i oyuncunun transformunun �ocu�u yaparak onunla birlikte hareket etmesini sa�la
			// Bu, kalkan prefab'inin sahneye eklendi�i yerde de�il, oyuncunun �zerinde g�r�nmesini sa�lar.
			// E�er _shieldEffectObject bir UI Canvas'� veya �zel bir render hedefi kullan�yorsa,
			// bu sat�r� dikkatli kullanmak veya alternatif bir yakla��m d���nmek gerekebilir.
			// Ancak, genel olarak oyuncunun etraf�nda bir efektse bu uygundur.
			_shieldEffectObject.transform.SetParent(_playerController.transform);
			_shieldEffectObject.transform.localPosition = Vector3.zero; // Oyuncunun merkezinde durmas� i�in
			_shieldEffectObject.SetActive(true);
		}
		else
		{
			Debug.LogWarning("ShieldPowerUp: _shieldEffectObject atanmam��! Kalkan efekti g�rseli �al��mayabilir.");
		}

		yield return new WaitForSeconds(_shieldDuration);

		// Kalkan s�resi bitti�inde
		if (_shieldEffectObject != null)
		{
			_shieldEffectObject.SetActive(false);
			// Parent'� temizlemeyi unutma, aksi halde GameObject yok olsa bile efekt kalabilir.
			// Ancak bu GameObject'in kendisi kal�c� oldu�u i�in, sadece SetActive(false) yeterli olabilir.
			// �leride efekt GameObject'i de yok edilecekse, parent'� null yapmak gerekebilir.
		}
		Debug.Log("ShieldPowerUp: ShieldRoutine B�TT�! Player'�n kalkan� devre d��� b�rak�l�yor.");
	}

	// PlayerPowerUpManager'�n kalkan�n aktif olup olmad���n� sorgulamas� i�in
	// Bu metod PlayerPowerUpManager'daki GetIsShieldActive() metodunun yerini alacak.
	public bool IsActive()
	{
		return _activeShieldRoutine != null; // Coroutine �al���yorsa kalkan aktif demektir
	}

	void OnDestroy()
	{
		// GameObject yok edildi�inde �al��an coroutine'i durdur
		if (_activeShieldRoutine != null)
		{
			StopCoroutine(_activeShieldRoutine);
		}
	}
}