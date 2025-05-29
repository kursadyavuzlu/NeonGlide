using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerPowerUpManager : MonoBehaviour
{
	private PlayerController _playerController;

	// --- YEN�: IPowerUpEffect referanslar� ---
	private IPowerUpEffect _shieldPowerUpEffect;
	private JumpBoostPowerUp _jumpBoostPowerUpEffect;
	private SpeedBoostPowerUp _speedBoostPowerUpEffect;
	private IPowerUpEffect _scoreMultiplierPowerUpEffect;
	private ShrinkPowerUp _shrinkPowerUpEffect;
	private IPowerUpEffect _timeSlowdownPowerUpEffect;
	private IPowerUpEffect _clearAllObstaclesPowerUpEffect;

	void Awake()
	{
		_shieldPowerUpEffect = GetComponentInChildren<ShieldPowerUp>();
		if (_shieldPowerUpEffect == null)
		{
			Debug.LogError("PlayerPowerUpManager: ShieldPowerUp bile�eni bulunamad�! L�tfen Player alt�nda '_ShieldPowerUpLogic' GameObject'ine ekleyin.");
		}

		_jumpBoostPowerUpEffect = GetComponentInChildren<JumpBoostPowerUp>();
		if (_jumpBoostPowerUpEffect == null)
		{
			Debug.LogError("PlayerPowerUpManager: JumpBoostPowerUp bile�eni bulunamad�! L�tfen Player alt�nda '_JumpBoostPowerUpLogic' GameObject'ine ekleyin.");
		}

		_speedBoostPowerUpEffect = GetComponentInChildren<SpeedBoostPowerUp>();
		if (_speedBoostPowerUpEffect == null)
		{
			Debug.LogError("PlayerPowerUpManager: SpeedBoostPowerUp bile�eni bulunamad�! L�tfen Player alt�nda '_SpeedBoostPowerUpLogic' GameObject'ine ekleyin.");
		}

		_scoreMultiplierPowerUpEffect = GetComponentInChildren<ScoreMultiplierPowerUp>();
		if (_scoreMultiplierPowerUpEffect == null)
		{
			Debug.LogError("PlayerPowerUpManager: ScoreMultiplierPowerUp bile�eni bulunamad�! L�tfen Player alt�nda '_ScoreMultiplierPowerUpLogic' GameObject'ine ekleyin.");
		}

		_shrinkPowerUpEffect = GetComponentInChildren<ShrinkPowerUp>();
		if (_shrinkPowerUpEffect == null)
		{
			Debug.LogError("PlayerPowerUpManager: ShrinkPowerUp bile�eni bulunamad�! L�tfen Player alt�nda '_ShrinkPowerUpLogic' GameObject'ine ekleyin.");
		}

		_timeSlowdownPowerUpEffect = GetComponentInChildren<TimeSlowdownPowerUp>();
		if (_timeSlowdownPowerUpEffect == null)
		{
			Debug.LogError("PlayerPowerUpManager: TimeSlowdownPowerUp bile�eni bulunamad�! L�tfen Player alt�nda '_TimeSlowdownPowerUpLogic' GameObject'ine ekleyin.");
		}

		_clearAllObstaclesPowerUpEffect = GetComponentInChildren<ClearAllObstaclesPowerUp>();
		if (_clearAllObstaclesPowerUpEffect == null)
		{
			Debug.LogError("PlayerPowerUpManager: ClearAllObstaclesPowerUp bile�eni bulunamad�! L�tfen Player alt�nda '_ClearAllObstaclesPowerUpLogic' GameObject'ine ekleyin.");
		}
	}

	public void Setup(PlayerController player)
	{
		_playerController = player;
		Debug.Log("PlayerPowerUpManager: Setup tamamland�.");
	}

	void Update()
	{
		// T�m Power-Up mant��� kendi scriptlerine ta��nd��� i�in buras� bo�.
	}

	public void ActivatePowerUp(PowerUpType type)
	{
		Debug.Log($"PlayerPowerUpManager: {type} Power-Up'� aktif ediliyor.");

		switch (type)
		{
			case PowerUpType.Shield:
				if (_shieldPowerUpEffect != null)
				{
					_shieldPowerUpEffect.Activate(_playerController, this);
				}
				break;
			case PowerUpType.JumpBoost:
				if (_jumpBoostPowerUpEffect != null)
				{
					_jumpBoostPowerUpEffect.Activate(_playerController, this);
				}
				break;
			case PowerUpType.SpeedBoost:
				if (_speedBoostPowerUpEffect != null)
				{
					_speedBoostPowerUpEffect.Activate(_playerController, this);
				}
				break;
			case PowerUpType.ScoreMultiplier:
				if (_scoreMultiplierPowerUpEffect != null)
				{
					_scoreMultiplierPowerUpEffect.Activate(_playerController, this);
				}
				break;
			case PowerUpType.Shrink:
				if (_shrinkPowerUpEffect != null)
				{
					_shrinkPowerUpEffect.Activate(_playerController, this);
				}
				break;
			case PowerUpType.TimeSlowdown:
				if (_timeSlowdownPowerUpEffect != null)
				{
					_timeSlowdownPowerUpEffect.Activate(_playerController, this);
				}
				break;
			case PowerUpType.ClearAllObstacles:
				if (_clearAllObstaclesPowerUpEffect != null)
				{
					_clearAllObstaclesPowerUpEffect.Activate(_playerController, this);
				}
				break;
		}
	}

	// --- PlayerController'�n sorgulayabilmesi i�in getter metodlar� ---
	public bool GetIsShieldActive()
	{
		if (_shieldPowerUpEffect != null)
		{
			return _shieldPowerUpEffect.IsActive();
		}
		return false;
	}

	public bool GetIsJumpBoostActive()
	{
		if (_jumpBoostPowerUpEffect != null)
		{
			return _jumpBoostPowerUpEffect.IsActive();
		}
		return false;
	}

	public float GetJumpBoostMultiplier()
	{
		if (_jumpBoostPowerUpEffect != null)
		{
			return _jumpBoostPowerUpEffect.GetJumpBoostMultiplier();
		}
		return 1f;
	}

	public bool GetIsSpeedBoostActive()
	{
		if (_speedBoostPowerUpEffect != null)
		{
			return _speedBoostPowerUpEffect.IsActive();
		}
		return false;
	}
	public float GetSpeedBoostMultiplier()
	{
		if (_speedBoostPowerUpEffect != null)
		{
			return _speedBoostPowerUpEffect.GetSpeedBoostMultiplier();
		}
		return 1f;
	}

	public bool GetIsScoreMultiplierActive()
	{
		if (_scoreMultiplierPowerUpEffect != null)
		{
			return _scoreMultiplierPowerUpEffect.IsActive();
		}
		return false;
	}

	public bool GetIsShrinkActive()
	{
		if (_shrinkPowerUpEffect != null)
		{
			return _shrinkPowerUpEffect.IsActive();
		}
		return false;
	}

	public bool GetIsTimeSlowdownActive()
	{
		if (_timeSlowdownPowerUpEffect != null)
		{
			return _timeSlowdownPowerUpEffect.IsActive();
		}
		return false;
	}

	public bool GetIsClearAllObstaclesActive()
	{
		if (_clearAllObstaclesPowerUpEffect != null)
		{
			return _clearAllObstaclesPowerUpEffect.IsActive();
		}
		return false;
	}
}