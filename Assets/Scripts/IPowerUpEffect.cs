using UnityEngine;

public interface IPowerUpEffect
{
	void Activate(PlayerController player, PlayerPowerUpManager powerUpManager);

	void Deactivate();

	bool IsActive();
}