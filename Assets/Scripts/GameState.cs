using UnityEngine;
using System.Collections;

public static class GameState {

	#region Mode
	public enum PlayMode
	{
		NotStarted,
		Started,
		Finished
	}
	public static PlayMode CurrentMode = PlayMode.NotStarted;
	#endregion

	#region Timers
	public static float LevelTimer = 0;

	public static float StunTimerMax = 1;
	public static float StunTimer = 0;

	public const float PowerTimerMax = 2;
	public static float PowerTimer = 0;

	public const float DiscardTimerMax = 0.25f;
	public static float DiscardTimer = 0;
	#endregion

	#region Movement
	public const float DefaultPlayerSpeed = 0.5f;
	public static float PlayerSpeed = 0.5f;

	public static bool FollowAlternatePath = true;
	public static bool PlayerInvincible = false;
	#endregion

	#region Scoring
	public static int Score = 0;
	public const int DefaultCoinPoints = 100;
	public static int CoinPoints = 100;

	public const int DefaultEnemyLoss = 300;
	public static int EnemyPointLoss = 300;
	#endregion

	public static void GameReset()
	{
		Score = 0;
	}

	public static void ResetPlayer()
	{
		PlayerSpeed = DefaultPlayerSpeed;
		CoinPoints = DefaultCoinPoints;
		EnemyPointLoss = DefaultEnemyLoss;
		PlayerInvincible = false;
		FollowAlternatePath = false;
		ActivePower = null;
		PickNewPower();
	}

	static Power[] PowerPicker = new Power[] { Power.AlternatePath, Power.Speed2x, Power.Coin2x, Power.HalvePenalty, Power.Invincible };
	static Power? lastPower = null;
	public static void PickNewPower()
	{
		lastPower = CurrentPower;
		while (CurrentPower == lastPower)
		{
			CurrentPower = PowerPicker[Random.Range(0, PowerPicker.Length)];
		}
	}

	public static void EndLevel()
	{
		if (CurrentMode != PlayMode.Finished)
		{
			Score += Mathf.CeilToInt (LevelTimer);
		}
		CurrentMode = PlayMode.Finished;
		Debug.Log ("End Level");
	}

	public enum Power
	{
		AlternatePath,
		Speed2x,
		Coin2x,
		HalvePenalty,
		Invincible
	}

	public static Power CurrentPower;
	public static Power? ActivePower;

	public static void ActivatePower(Power power)
	{
		Debug.Log (string.Format ("Activated power {0}", power));
		PowerTimer = PowerTimerMax;
		ActivePower = power;
		switch (power)
		{
			case Power.AlternatePath:
			FollowAlternatePath = true;
			break;

			case Power.Coin2x:
			CoinPoints = DefaultCoinPoints * 2;
			break;

			case Power.HalvePenalty:
			EnemyPointLoss = DefaultEnemyLoss / 2;
			break;

			case Power.Invincible:
			PlayerInvincible = true;
			break;

			case Power.Speed2x:
			PlayerSpeed = DefaultPlayerSpeed * 2;
			break;
		}
	}
}
