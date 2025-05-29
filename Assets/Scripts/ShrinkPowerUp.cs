using UnityEngine;
using System.Collections;

public class ShrinkPowerUp : MonoBehaviour, IPowerUpEffect
{
	// PlayerPowerUpManager'dan ta��nan de�i�kenler
	[SerializeField] private float _shrinkScale = 0.5f; // Oyuncunun k���lece�i oran
	[SerializeField] private float _shrinkDuration = 5f; // K���kl���n s�resi
	[SerializeField] private float _scaleChangeSpeed = 5f; // Boyut de�i�tirme h�z� (Lerp i�in)

	private PlayerController _playerController;
	private PlayerPowerUpManager _powerUpManager; // Gerekti�inde PlayerPowerUpManager'dan bilgi almak i�in

	private Vector3 _playerOriginalScale; // Oyuncunun orijinal boyutunu saklamak i�in
	private Coroutine _activeShrinkRoutine; // Mevcut Coroutine'i durdurmak i�in
	private bool _isShrinkingActive = false; // K���lme Power-Up'�n�n aktif olup olmad���n� g�sterir

	// Update metodunda s�rekli k���lme kontrol� i�in (PlayerPowerUpManager'dan ta��nd�)
	private Vector3 _targetScale;
	private bool _returningToNormal = false;


	public void Activate(PlayerController player, PlayerPowerUpManager powerUpManager)
	{
		_playerController = player;
		_powerUpManager = powerUpManager;

		// Player'�n orijinal boyutunu bir kez al (e�er zaten al�nmad�ysa)
		if (_playerOriginalScale == Vector3.zero) // Ba�lang��ta s�f�r olabilir
		{
			_playerOriginalScale = _playerController.transform.localScale;
		}

		Debug.Log("ShrinkPowerUp: Activate �a�r�ld�.");

		if (_activeShrinkRoutine != null)
		{
			StopCoroutine(_activeShrinkRoutine);
			Debug.Log("ShrinkPowerUp: Mevcut Shrink rutini durduruldu.");
		}
		_activeShrinkRoutine = StartCoroutine(ShrinkRoutine());
	}

	public void Deactivate()
	{
		Debug.Log("ShrinkPowerUp: Deactivate �a�r�ld�.");
		if (_activeShrinkRoutine != null)
		{
			StopCoroutine(_activeShrinkRoutine);
			_activeShrinkRoutine = null;
		}
		// E�er d��ar�dan devre d��� b�rak�l�rsa, boyutu hemen normale d�nd�r
		if (_playerController != null)
		{
			_playerController.transform.localScale = _playerOriginalScale;
		}
		_isShrinkingActive = false;
		_returningToNormal = false;
	}

	public bool IsActive()
	{
		return _isShrinkingActive; // _activeShrinkRoutine null olsa bile, normalle�me s�recinde aktif say�labiliriz.
	}

	// Shrink Power-Up'�n�n ana �al��ma rutini
	IEnumerator ShrinkRoutine()
	{
		_isShrinkingActive = true;
		_returningToNormal = false;
		Debug.Log("ShrinkPowerUp: Shrink Power-Up Rutini BA�LADI! Oyuncu k���l�yor.");

		// Hedef k���lme boyutu
		_targetScale = _playerOriginalScale * _shrinkScale;
		float timer = 0f;
		float duration = 0.5f; // K���lme animasyonunun s�resi

		// K���lme animasyonu
		while (timer < duration)
		{
			if (_playerController == null) yield break; // Oyuncu yoksa Coroutine'i durdur
			_playerController.transform.localScale = Vector3.Lerp(_playerOriginalScale, _targetScale, timer / duration);
			timer += Time.deltaTime * _scaleChangeSpeed; // H�zland�rma i�in _scaleChangeSpeed kullan�yoruz
			yield return null;
		}
		if (_playerController != null)
		{
			_playerController.transform.localScale = _targetScale; // Animasyon bitince tam k���lm�� boyuta ayarla
		}

		Debug.Log("ShrinkPowerUp: Oyuncu k���ld�. K���lme s�resi bekleniyor...");
		yield return new WaitForSeconds(_shrinkDuration); // K���lm�� halde kalma s�resi

		Debug.Log("ShrinkPowerUp: K���lme s�resi bitti. Oyuncu normale d�n�yor.");
		_returningToNormal = true; // Normale d�n�� ba�lad�

		// Normale d�n�� animasyonu
		timer = 0f;
		while (timer < duration)
		{
			if (_playerController == null) yield break; // Oyuncu yoksa Coroutine'i durdur
			_playerController.transform.localScale = Vector3.Lerp(_targetScale, _playerOriginalScale, timer / duration);
			timer += Time.deltaTime * _scaleChangeSpeed; // H�zland�rma i�in _scaleChangeSpeed kullan�yoruz
			yield return null;
		}
		if (_playerController != null)
		{
			_playerController.transform.localScale = _playerOriginalScale; // Animasyon bitince orijinal boyuta ayarla
		}

		_activeShrinkRoutine = null;
		_isShrinkingActive = false;
		_returningToNormal = false;
		Debug.Log("ShrinkPowerUp: Shrink Power-Up Rutini B�TT�!");
	}

	// PlayerPowerUpManager'�n Update metodundan ta��nan s�rekli boyut kontrol�
	void Update()
	{
		// E�er Shrink Power-Up aktif de�ilse ve oyuncu orijinal boyutunda de�ilse, yava��a normale d�nd�r
		// NOT: Bu blok sadece Coroutine �al��m�yorsa ve _isShrinkingActive false ise devreye girmeli.
		// Ancak Coroutine i�indeki Lerp zaten bunu yap�yor, bu d��ar�daki kontrol art�k gereksiz olabilir.
		// E�er Coroutine bir sebepten erken durur ve oyuncu ortada bir boyutta kal�rsa diye tutulabilir.
		// Ancak bu yap�sal de�i�iklikle Coroutine'ler i�i bitirene kadar devam edece�i i�in bu kontrol� kald�rmak daha temiz olabilir.
		// �imdilik tutal�m ve test edelim. E�er sorun ��kar�rsa kald�r�r�z.
		if (!_isShrinkingActive && _playerController != null && _playerController.transform.localScale != _playerOriginalScale)
		{
			// E�er oyuncunun boyutu hedef (orijinal) boyuttan sapm��sa, yava��a geri d�nd�r.
			// Bu, Lerp'in sonland�r�lamad��� veya kesintiye u�rad��� durumlar i�in bir g�venlik �nlemi.
			if (Vector3.Distance(_playerController.transform.localScale, _playerOriginalScale) > 0.01f) // Epsilon de�eri
			{
				_playerController.transform.localScale = Vector3.Lerp(_playerController.transform.localScale, _playerOriginalScale, Time.deltaTime * _scaleChangeSpeed);
			}
			else
			{
				_playerController.transform.localScale = _playerOriginalScale; // �ok yak�nsa direkt ata
			}
		}
	}


	void OnDestroy()
	{
		if (_activeShrinkRoutine != null)
		{
			StopCoroutine(_activeShrinkRoutine);
		}
		// GameObject yok edildi�inde oyuncunun boyutunu s�f�rla
		if (_playerController != null && _playerController.transform.localScale != _playerOriginalScale)
		{
			_playerController.transform.localScale = _playerOriginalScale;
		}
	}
}