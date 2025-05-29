using UnityEngine;
using System.Collections;

public class ShrinkPowerUp : MonoBehaviour, IPowerUpEffect
{
	// PlayerPowerUpManager'dan taþýnan deðiþkenler
	[SerializeField] private float _shrinkScale = 0.5f; // Oyuncunun küçüleceði oran
	[SerializeField] private float _shrinkDuration = 5f; // Küçüklüðün süresi
	[SerializeField] private float _scaleChangeSpeed = 5f; // Boyut deðiþtirme hýzý (Lerp için)

	private PlayerController _playerController;
	private PlayerPowerUpManager _powerUpManager; // Gerektiðinde PlayerPowerUpManager'dan bilgi almak için

	private Vector3 _playerOriginalScale; // Oyuncunun orijinal boyutunu saklamak için
	private Coroutine _activeShrinkRoutine; // Mevcut Coroutine'i durdurmak için
	private bool _isShrinkingActive = false; // Küçülme Power-Up'ýnýn aktif olup olmadýðýný gösterir

	// Update metodunda sürekli küçülme kontrolü için (PlayerPowerUpManager'dan taþýndý)
	private Vector3 _targetScale;
	private bool _returningToNormal = false;


	public void Activate(PlayerController player, PlayerPowerUpManager powerUpManager)
	{
		_playerController = player;
		_powerUpManager = powerUpManager;

		// Player'ýn orijinal boyutunu bir kez al (eðer zaten alýnmadýysa)
		if (_playerOriginalScale == Vector3.zero) // Baþlangýçta sýfýr olabilir
		{
			_playerOriginalScale = _playerController.transform.localScale;
		}

		Debug.Log("ShrinkPowerUp: Activate çaðrýldý.");

		if (_activeShrinkRoutine != null)
		{
			StopCoroutine(_activeShrinkRoutine);
			Debug.Log("ShrinkPowerUp: Mevcut Shrink rutini durduruldu.");
		}
		_activeShrinkRoutine = StartCoroutine(ShrinkRoutine());
	}

	public void Deactivate()
	{
		Debug.Log("ShrinkPowerUp: Deactivate çaðrýldý.");
		if (_activeShrinkRoutine != null)
		{
			StopCoroutine(_activeShrinkRoutine);
			_activeShrinkRoutine = null;
		}
		// Eðer dýþarýdan devre dýþý býrakýlýrsa, boyutu hemen normale döndür
		if (_playerController != null)
		{
			_playerController.transform.localScale = _playerOriginalScale;
		}
		_isShrinkingActive = false;
		_returningToNormal = false;
	}

	public bool IsActive()
	{
		return _isShrinkingActive; // _activeShrinkRoutine null olsa bile, normalleþme sürecinde aktif sayýlabiliriz.
	}

	// Shrink Power-Up'ýnýn ana çalýþma rutini
	IEnumerator ShrinkRoutine()
	{
		_isShrinkingActive = true;
		_returningToNormal = false;
		Debug.Log("ShrinkPowerUp: Shrink Power-Up Rutini BAÞLADI! Oyuncu küçülüyor.");

		// Hedef küçülme boyutu
		_targetScale = _playerOriginalScale * _shrinkScale;
		float timer = 0f;
		float duration = 0.5f; // Küçülme animasyonunun süresi

		// Küçülme animasyonu
		while (timer < duration)
		{
			if (_playerController == null) yield break; // Oyuncu yoksa Coroutine'i durdur
			_playerController.transform.localScale = Vector3.Lerp(_playerOriginalScale, _targetScale, timer / duration);
			timer += Time.deltaTime * _scaleChangeSpeed; // Hýzlandýrma için _scaleChangeSpeed kullanýyoruz
			yield return null;
		}
		if (_playerController != null)
		{
			_playerController.transform.localScale = _targetScale; // Animasyon bitince tam küçülmüþ boyuta ayarla
		}

		Debug.Log("ShrinkPowerUp: Oyuncu küçüldü. Küçülme süresi bekleniyor...");
		yield return new WaitForSeconds(_shrinkDuration); // Küçülmüþ halde kalma süresi

		Debug.Log("ShrinkPowerUp: Küçülme süresi bitti. Oyuncu normale dönüyor.");
		_returningToNormal = true; // Normale dönüþ baþladý

		// Normale dönüþ animasyonu
		timer = 0f;
		while (timer < duration)
		{
			if (_playerController == null) yield break; // Oyuncu yoksa Coroutine'i durdur
			_playerController.transform.localScale = Vector3.Lerp(_targetScale, _playerOriginalScale, timer / duration);
			timer += Time.deltaTime * _scaleChangeSpeed; // Hýzlandýrma için _scaleChangeSpeed kullanýyoruz
			yield return null;
		}
		if (_playerController != null)
		{
			_playerController.transform.localScale = _playerOriginalScale; // Animasyon bitince orijinal boyuta ayarla
		}

		_activeShrinkRoutine = null;
		_isShrinkingActive = false;
		_returningToNormal = false;
		Debug.Log("ShrinkPowerUp: Shrink Power-Up Rutini BÝTTÝ!");
	}

	// PlayerPowerUpManager'ýn Update metodundan taþýnan sürekli boyut kontrolü
	void Update()
	{
		// Eðer Shrink Power-Up aktif deðilse ve oyuncu orijinal boyutunda deðilse, yavaþça normale döndür
		// NOT: Bu blok sadece Coroutine çalýþmýyorsa ve _isShrinkingActive false ise devreye girmeli.
		// Ancak Coroutine içindeki Lerp zaten bunu yapýyor, bu dýþarýdaki kontrol artýk gereksiz olabilir.
		// Eðer Coroutine bir sebepten erken durur ve oyuncu ortada bir boyutta kalýrsa diye tutulabilir.
		// Ancak bu yapýsal deðiþiklikle Coroutine'ler iþi bitirene kadar devam edeceði için bu kontrolü kaldýrmak daha temiz olabilir.
		// Þimdilik tutalým ve test edelim. Eðer sorun çýkarýrsa kaldýrýrýz.
		if (!_isShrinkingActive && _playerController != null && _playerController.transform.localScale != _playerOriginalScale)
		{
			// Eðer oyuncunun boyutu hedef (orijinal) boyuttan sapmýþsa, yavaþça geri döndür.
			// Bu, Lerp'in sonlandýrýlamadýðý veya kesintiye uðradýðý durumlar için bir güvenlik önlemi.
			if (Vector3.Distance(_playerController.transform.localScale, _playerOriginalScale) > 0.01f) // Epsilon deðeri
			{
				_playerController.transform.localScale = Vector3.Lerp(_playerController.transform.localScale, _playerOriginalScale, Time.deltaTime * _scaleChangeSpeed);
			}
			else
			{
				_playerController.transform.localScale = _playerOriginalScale; // Çok yakýnsa direkt ata
			}
		}
	}


	void OnDestroy()
	{
		if (_activeShrinkRoutine != null)
		{
			StopCoroutine(_activeShrinkRoutine);
		}
		// GameObject yok edildiðinde oyuncunun boyutunu sýfýrla
		if (_playerController != null && _playerController.transform.localScale != _playerOriginalScale)
		{
			_playerController.transform.localScale = _playerOriginalScale;
		}
	}
}