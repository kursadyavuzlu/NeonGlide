using UnityEngine;

// PowerUpType enum'ýnýn tanýmlandýðý dosya
// Eðer PowerUpType globalde deðilse, bu using satýrý gerekebilir.
// using YourGameNamespace;

public interface IPowerUpEffect
{
	// Power-Up aktif edildiðinde çaðrýlacak metod.
	// player: Power-Up'ý alan PlayerController referansý.
	// powerUpManager: Power-Up'ý yöneten PlayerPowerUpManager referansý.
	void Activate(PlayerController player, PlayerPowerUpManager powerUpManager);

	// Power-Up manuel olarak devre dýþý býrakýldýðýnda veya süresi dolduðunda çaðrýlacak metod.
	void Deactivate();

	// Power-Up'ýn þu an aktif olup olmadýðýný sorgulamak için.
	bool IsActive();
}