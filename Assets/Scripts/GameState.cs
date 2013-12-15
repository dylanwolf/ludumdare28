using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
	public const int TimeBonusMultiplier = 100;

	public static float StunTimerMax = 1;
	public static float StunTimer = 0;

	public const float PowerTimerMax = 2;
	public static float PowerTimer = 0;

	public const float DiscardTimerMax = 0.25f;
	public static float DiscardTimer = 0;
	#endregion

	#region Movement
	public const float DefaultPlayerSpeed = 1.0f;
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
		DiscardPile.Clear ();
		DiscardPile.AddRange(new Power[] {
			Power.AlternatePath, Power.AlternatePath, Power.AlternatePath,
			Power.Coin2x, Power.Coin2x, Power.Coin2x,
			Power.HalvePenalty, Power.HalvePenalty, Power.HalvePenalty,
			Power.Invincible,
			Power.Speed2x, Power.Speed2x
		});
		PickNewPower(true);
	}
	
	static float levelTimerStart;
	static NavNode firstNode;
	static Vector3 avatarStart;
	public static void LevelSave()
	{
		levelTimerStart = Avatar.Current.LevelTimerStart;
		firstNode = Avatar.Current.firstNode;
		avatarStart = Avatar.Current.transform.position;
	}

	public static List<GameObject> Collectables = new List<GameObject>();

	public static void LevelReset()
	{
		while (Deck.Count > 0)
		{
			DiscardPile.Add(Deck.Pop ());
		}

		PowerTimer = 0;
		DiscardTimer = 0;
		Avatar.Current.transform.position = avatarStart;
		Avatar.Current.firstNode = firstNode;
		Avatar.Current.ResetLevel();
		SoundBoard.StartMusic();
		GameState.LevelTimer = levelTimerStart;

		foreach (GameObject obj in Collectables)
		{
			obj.SetActive(true);
		}
		Collectables.Clear ();
	}

	public static int GetBestScore(int newScore)
	{
		int bestScore = PlayerPrefs.GetInt("BestScore");
		if (newScore > bestScore)
		{
			PlayerPrefs.SetInt("BestScore", newScore);
			return newScore;
		}
		else
		{
			return bestScore;
		}
	}

	public static void ResetPlayer()
	{
		PlayerSpeed = DefaultPlayerSpeed;
		CoinPoints = DefaultCoinPoints;
		EnemyPointLoss = DefaultEnemyLoss;
		PlayerInvincible = false;
		FollowAlternatePath = false;
		ActivePower = null;
		PickNewPower(false);
	}

	static Stack<Power> Deck = new Stack<Power>();
	static List<Power> DiscardPile = new List<Power>();
	public static void PickNewPower(bool firstCard)
	{
		//CurrentPower = Power.AlternatePath;
		//return;

		if (!firstCard)
		{
			GameState.DiscardPile.Add(GameState.CurrentPower);
		}

		if (Deck.Count == 0)
		{
			Debug.Log ("Reshuffling discard pile with " + DiscardPile.Count.ToString() + " cards");
			foreach (Power p in DiscardPile.OrderBy(x => Random.value))
			{
				Debug.Log (string.Format ("Adding {0}", p));
				Deck.Push (p);
			}
			DiscardPile.Clear();
		}
		CurrentPower = Deck.Pop ();
		Debug.Log (string.Format("Drew {0}", CurrentPower));
	}

	public static void EndLevel()
	{
		if (CurrentMode != PlayMode.Finished)
		{
			Score += Mathf.CeilToInt (LevelTimer) * TimeBonusMultiplier;
		}
		CurrentMode = PlayMode.Finished;
		SoundBoard.StopMusic();
		SoundBoard.PlayFinish();
		GameOverScreen.Show();
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
		SoundBoard.PlayCard();
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
