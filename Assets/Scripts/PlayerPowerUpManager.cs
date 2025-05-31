using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerPowerUpManager : MonoBehaviour
{
	private PlayerController _playerController;

	private Dictionary<PowerUpType, IPowerUpEffect> _powerUpEffects = new Dictionary<PowerUpType, IPowerUpEffect>();

	private JumpBoostPowerUp _jumpBoostPowerUpRef;
	private SpeedBoostPowerUp _speedBoostPowerUpRef;
	private ShrinkPowerUp _shrinkPowerUpRef;


	void Awake()
	{
		InitializePowerUp<ShieldPowerUp>(PowerUpType.Shield);
		InitializePowerUp<JumpBoostPowerUp>(PowerUpType.JumpBoost);
		InitializePowerUp<SpeedBoostPowerUp>(PowerUpType.SpeedBoost);
		InitializePowerUp<ScoreMultiplierPowerUp>(PowerUpType.ScoreMultiplier);
		InitializePowerUp<ShrinkPowerUp>(PowerUpType.Shrink);
		InitializePowerUp<TimeSlowdownPowerUp>(PowerUpType.TimeSlowdown);
		InitializePowerUp<ClearAllObstaclesPowerUp>(PowerUpType.ClearAllObstacles);

		_jumpBoostPowerUpRef = _powerUpEffects[PowerUpType.JumpBoost] as JumpBoostPowerUp;
		_speedBoostPowerUpRef = _powerUpEffects[PowerUpType.SpeedBoost] as SpeedBoostPowerUp;
		_shrinkPowerUpRef = _powerUpEffects[PowerUpType.Shrink] as ShrinkPowerUp;
	}

	private void InitializePowerUp<T>(PowerUpType type) where T : MonoBehaviour, IPowerUpEffect
	{
		T powerUp = GetComponentInChildren<T>();
		if (powerUp != null)
		{
			_powerUpEffects.Add(type, powerUp);
		}
		else
		{
			Debug.LogError($"PlayerPowerUpManager: {typeof(T).Name} bileþeni bulunamadý! Lütfen Player altýnda ilgili GameObject'ine ekleyin.");
		}
	}

	public void Setup(PlayerController player)
	{
		_playerController = player;
		Debug.Log("PlayerPowerUpManager: Setup tamamlandý.");
	}

	void Update()
	{

	}

	public void ActivatePowerUp(PowerUpType type)
	{
		Debug.Log($"PlayerPowerUpManager: {type} Power-Up'ý aktif ediliyor.");

		if (_powerUpEffects.TryGetValue(type, out IPowerUpEffect powerUpEffect))
		{
			powerUpEffect.Activate(_playerController, this);
		}
		else
		{
			Debug.LogWarning($"PlayerPowerUpManager: {type} türünde Power-Up efekti bulunamadý veya atanmadý.");
		}
	}

	public bool IsPowerUpActive(PowerUpType type)
	{
		if (_powerUpEffects.TryGetValue(type, out IPowerUpEffect powerUpEffect))
		{
			return powerUpEffect.IsActive();
		}
		return false;
	}

	//public T GetPowerUpValue<T>(PowerUpType type) where T : struct
	//{
	//	Debug.LogWarning("PlayerPowerUpManager: GetPowerUpValue generik metodu þu anki Power-Up arayüzü ile uyumlu deðildir. " +
	//					 "Lütfen GetJumpBoostMultiplier gibi spesifik metotlarý kullanýn veya IPowerUpEffect'e GetValue() ekleyin.");
	//	return default(T); // Varsayýlan deðeri döndür
	//}

	public bool GetIsShieldActive()
	{
		return IsPowerUpActive(PowerUpType.Shield);
	}

	public bool GetIsJumpBoostActive()
	{
		return IsPowerUpActive(PowerUpType.JumpBoost);
	}

	public float GetJumpBoostMultiplier()
	{
		if (_jumpBoostPowerUpRef != null)
		{
			return _jumpBoostPowerUpRef.GetJumpBoostMultiplier();
		}
		return 1f;
	}

	public bool GetIsSpeedBoostActive()
	{
		return IsPowerUpActive(PowerUpType.SpeedBoost);
	}

	public float GetSpeedBoostMultiplier()
	{
		if (_speedBoostPowerUpRef != null)
		{
			return _speedBoostPowerUpRef.GetSpeedBoostMultiplier();
		}
		return 1f;
	}

	public bool GetIsScoreMultiplierActive()
	{
		return IsPowerUpActive(PowerUpType.ScoreMultiplier);
	}

	public bool GetIsShrinkActive()
	{
		return IsPowerUpActive(PowerUpType.Shrink);
	}

	public bool GetIsTimeSlowdownActive()
	{
		return IsPowerUpActive(PowerUpType.TimeSlowdown);
	}

	public bool GetIsClearAllObstaclesActive()
	{
		return IsPowerUpActive(PowerUpType.ClearAllObstacles);
	}
}