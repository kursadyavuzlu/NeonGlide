using UnityEngine;

public enum PowerUpType
{
	Shield,
	JumpBoost,
	SpeedBoost,
	ScoreMultiplier,
	Shrink,
	TimeSlowdown,
	ClearAllObstacles // <-- Yeni Power-Up: Ekranda o an bulunan tüm engelleri yok etme
}

public class PowerUp : MonoBehaviour
{
	[Header("Power-Up Distribution")]
	[Range(0, 100)][SerializeField] private int _shieldChance = 15;
	[Range(0, 100)][SerializeField] private int _jumpBoostChance = 15;
	[Range(0, 100)][SerializeField] private int _speedBoostChance = 15;
	[Range(0, 100)][SerializeField] private int _scoreMultiplierChance = 15; // Þanslarý ayarladým
	[Range(0, 100)][SerializeField] private int _shrinkChance = 20;
	[Range(0, 100)][SerializeField] private int _timeSlowdownChance = 10; // Þanslarý ayarladým
	[Range(0, 100)][SerializeField] private int _clearAllObstaclesChance = 10; // <-- Yeni Þans Deðiþkeni (Baþlangýç deðeri)

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
		// Tüm Power-Up þanslarýnýn toplamýný al
		int totalChance = _shieldChance + _jumpBoostChance + _speedBoostChance + _scoreMultiplierChance + _shrinkChance + _timeSlowdownChance + _clearAllObstaclesChance;

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
		else if (randomNumber < _shieldChance + _jumpBoostChance + _speedBoostChance)
		{
			Debug.Log("Power-Up: Speed Boost Activated!");
			player.ActivatePowerUp(PowerUpType.SpeedBoost);
		}
		else if (randomNumber < _shieldChance + _jumpBoostChance + _speedBoostChance + _scoreMultiplierChance)
		{
			Debug.Log("Power-Up: Score Multiplier Activated!");
			player.ActivatePowerUp(PowerUpType.ScoreMultiplier);
		}
		else if (randomNumber < _shieldChance + _jumpBoostChance + _speedBoostChance + _scoreMultiplierChance + _shrinkChance)
		{
			Debug.Log("Power-Up: Shrink Activated!");
			player.ActivatePowerUp(PowerUpType.Shrink);
		}
		else if (randomNumber < _shieldChance + _jumpBoostChance + _speedBoostChance + _scoreMultiplierChance + _shrinkChance + _timeSlowdownChance)
		{
			Debug.Log("Power-Up: Time Slowdown Activated!");
			player.ActivatePowerUp(PowerUpType.TimeSlowdown);
		}
		else // <-- Yeni koþul, sonuncusu bu olacaðý için 'else' yeterli
		{
			Debug.Log("Power-Up: Clear All Obstacles Activated!");
			player.ActivatePowerUp(PowerUpType.ClearAllObstacles);
		}
	}
}