using UnityEngine;

public enum PowerUpType
{
	Shield,
	JumpBoost,
	ControlGlitch
}

public class PowerUp : MonoBehaviour
{
	[Header("Power-Up Distribution")]
	[Range(0, 100)][SerializeField] private int _shieldChance = 30;
	[Range(0, 100)][SerializeField] private int _jumpBoostChance = 45;
	[Range(0, 100)][SerializeField] private int _controlGlitchChance = 25;

	[SerializeField] private float _destroyDelay = 0.1f;

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			PlayerController player = other.GetComponent<PlayerController>();
			if (player != null)
			{
				ApplyRandomPowerUp(player);
			}

			Destroy(gameObject, _destroyDelay);
		}
	}

	private void ApplyRandomPowerUp(PlayerController player)
	{
		int totalChance = _shieldChance + _jumpBoostChance + _controlGlitchChance;
		if (totalChance != 100)
		{
			Debug.LogWarning("PowerUp chances do not add up to 100! Please adjust them in the Inspector. Current total: " + totalChance);
		}

		int randomNumber = Random.Range(0, totalChance);

		if (randomNumber < _shieldChance)
		{
			Debug.Log("Power-Up: Shield Activated!");
			player.ActivatePowerUp(PowerUpType.Shield);
		}
		else if (randomNumber < _shieldChance + _jumpBoostChance)
		{
			Debug.Log("Power-Up: Jump Boost Activated!");
			player.ActivatePowerUp(PowerUpType.JumpBoost);
		}
		else
		{
			Debug.LogWarning("Power-Up: Control Glitch Activated!");
			player.ActivatePowerUp(PowerUpType.ControlGlitch);
		}
	}
}