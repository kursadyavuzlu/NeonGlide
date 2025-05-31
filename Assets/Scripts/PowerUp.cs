// PowerUp.cs
using UnityEngine;

public enum PowerUpType
{
	Shield,
	JumpBoost,
	SpeedBoost,
	ScoreMultiplier,
	Shrink,
	TimeSlowdown,
	ClearAllObstacles
}

public class PowerUp : MonoBehaviour
{
	[Header("Power-Up Distribution")]
	[Range(0, 100)][SerializeField] private int _shieldChance = 15;
	[Range(0, 100)][SerializeField] private int _jumpBoostChance = 15;
	[Range(0, 100)][SerializeField] private int _speedBoostChance = 15;
	[Range(0, 100)][SerializeField] private int _scoreMultiplierChance = 15;
	[Range(0, 100)][SerializeField] private int _shrinkChance = 20;
	[Range(0, 100)][SerializeField] private int _timeSlowdownChance = 10;
	[Range(0, 100)][SerializeField] private int _clearAllObstaclesChance = 10;

	[SerializeField] private float _destroyDelay = 0.1f;

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			Destroy(gameObject, _destroyDelay);
		}
	}

	public PowerUpType GetRandomPowerUpType()
	{
		int totalChance = _shieldChance + _jumpBoostChance + _speedBoostChance + _scoreMultiplierChance + _shrinkChance + _timeSlowdownChance + _clearAllObstaclesChance;

		if (totalChance != 100)
		{
			Debug.LogWarning("PowerUp chances do not add up to 100! Please adjust them in the Inspector. Current total: " + totalChance);
		}

		int randomNumber = Random.Range(0, totalChance);

		if (randomNumber < _shieldChance)
		{
			return PowerUpType.Shield;
		}
		else if (randomNumber < _shieldChance + _jumpBoostChance)
		{
			return PowerUpType.JumpBoost;
		}
		else if (randomNumber < _shieldChance + _jumpBoostChance + _speedBoostChance)
		{
			return PowerUpType.SpeedBoost;
		}
		else if (randomNumber < _shieldChance + _jumpBoostChance + _speedBoostChance + _scoreMultiplierChance)
		{
			return PowerUpType.ScoreMultiplier;
		}
		else if (randomNumber < _shieldChance + _jumpBoostChance + _speedBoostChance + _scoreMultiplierChance + _shrinkChance)
		{
			return PowerUpType.Shrink;
		}
		else if (randomNumber < _shieldChance + _jumpBoostChance + _speedBoostChance + _scoreMultiplierChance + _shrinkChance + _timeSlowdownChance)
		{
			return PowerUpType.TimeSlowdown;
		}
		else
		{
			return PowerUpType.ClearAllObstacles;
		}
	}
}