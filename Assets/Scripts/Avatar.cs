using UnityEngine;
using System.Collections;

public class Avatar : MonoBehaviour {

	public NavNode firstNode;
	private NavNode targetNode;
	public float LevelTimerStart = 0;

	private GameState.Power? lastPower;
	private tk2dSprite sprite;

	void Start()
	{
		sprite = GetComponent<tk2dSprite>();
		targetNode = firstNode;
		GameState.LevelTimer = LevelTimerStart;
		GameState.CurrentMode = GameState.PlayMode.NotStarted;
		GameState.ResetPlayer();
		lastPower = GameState.ActivePower;
	}

	Color ColorNormal = new Color(1, 1, 1);
	Color ColorStun = new Color(0.9f, 0.6f, 0.6f);
	Color ColorAltPath = new Color(0.2f, 0.5f, 0.9f);
	Color ColorCoins2x = new Color(1, 0.95f, 0.7f);
	Color ColorSpeed2x = new Color(0.6f, 255, 0.6f);
	Color ColorHalveLosses = new Color(0.9f, 0.6f, 0.3f);
	Color ColorInvincible = new Color(1, 1, 1, 0.5f);

	void SetColorByStatus()
	{
		if (GameState.StunTimer > 0)
		{
			sprite.color = ColorStun;
		}
		else if (GameState.ActivePower.HasValue)
		{
			switch (GameState.ActivePower.Value)
			{
				case GameState.Power.AlternatePath:
				sprite.color = ColorAltPath;
				break;

				case GameState.Power.Coin2x:
				sprite.color = ColorCoins2x;
				break;

				case GameState.Power.HalvePenalty:
				sprite.color = ColorHalveLosses;
				break;

				case GameState.Power.Invincible:
				sprite.color = ColorInvincible;
				break;

				case GameState.Power.Speed2x:
				sprite.color = ColorSpeed2x;
				break;
			}
		}
		else
		{
			sprite.color = ColorNormal;
		}

		sprite.ForceBuild();
		lastPower = GameState.ActivePower;
	}

	private Vector3 targetDirection;
	private Vector3 remainingDistance;
	void Update () {
		if (lastPower != GameState.ActivePower)
		{
			SetColorByStatus();
		}

		if (GameState.CurrentMode == GameState.PlayMode.Started)
		{
			// Update the timers
			GameState.LevelTimer -= Time.deltaTime;
			if (GameState.PowerTimer > 0)
			{
				GameState.PowerTimer -= Time.deltaTime;
				if (GameState.PowerTimer < 0)
				{
					Debug.Log ("Power deactivated");
					GameState.ResetPlayer ();
				}
			}

			// Move if not stunned
			if (GameState.StunTimer < 0)
			{
				// Figure out the next move
				remainingDistance = targetNode.transform.position - transform.position;
				targetDirection = remainingDistance.normalized;
				targetDirection *= (GameState.PlayerSpeed * Time.deltaTime);
				if (targetDirection.magnitude > remainingDistance.magnitude)
				{
					targetDirection = remainingDistance;
				}

				// Move the player
				transform.position += targetDirection;
			}
			// Otherwise, decrement the stun timer
			else
			{
				GameState.StunTimer -= Time.deltaTime;
				if (GameState.StunTimer < 0)
				{
					SetColorByStatus();
				}
			}
		}
	}

	private NavNode tmpNode;
	void OnTriggerEnter(Collider collider)
	{
		switch (collider.gameObject.tag)
		{
			case "NavNode":
			Debug.Log (string.Format ("Intersected with NavNode {0}", collider.name));
			tmpNode = collider.GetComponent<NavNode>();
			if (GameState.FollowAlternatePath && tmpNode.AltNavNode != null)
			{
				targetNode = tmpNode.AltNavNode;
			}
			else if (tmpNode.NextNavNode != null)
			{
				targetNode = tmpNode.NextNavNode;
			}
			else
			{
				GameState.EndLevel();
			}
			break;

			case "Coin":
			GameState.Score += GameState.CoinPoints;
			DestroyObject(collider.gameObject);
			Debug.Log (string.Format("Scored {0} for a total of {1}", GameState.CoinPoints, GameState.Score));
			break;

			case "Enemy":
			if (!GameState.PlayerInvincible)
			{
				GameState.Score -= GameState.EnemyPointLoss;
				if (GameState.Score < 0) { GameState.Score = 0; }
				DestroyObject(collider.gameObject);
				GameState.StunTimer = GameState.StunTimerMax;
				SetColorByStatus();
				Debug.Log (string.Format("Lost {0} for a total of {1}", GameState.EnemyPointLoss, GameState.Score));
			}
			break;
		}
	}
}
