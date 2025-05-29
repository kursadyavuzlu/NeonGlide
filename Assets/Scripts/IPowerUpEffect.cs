using UnityEngine;

// PowerUpType enum'�n�n tan�mland��� dosya
// E�er PowerUpType globalde de�ilse, bu using sat�r� gerekebilir.
// using YourGameNamespace;

public interface IPowerUpEffect
{
	// Power-Up aktif edildi�inde �a�r�lacak metod.
	// player: Power-Up'� alan PlayerController referans�.
	// powerUpManager: Power-Up'� y�neten PlayerPowerUpManager referans�.
	void Activate(PlayerController player, PlayerPowerUpManager powerUpManager);

	// Power-Up manuel olarak devre d��� b�rak�ld���nda veya s�resi doldu�unda �a�r�lacak metod.
	void Deactivate();

	// Power-Up'�n �u an aktif olup olmad���n� sorgulamak i�in.
	bool IsActive();
}