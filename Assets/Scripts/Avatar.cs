using UnityEngine;
using System.Collections;
using ThinksquirrelSoftware.Thinkscroller;

public class Avatar : MonoBehaviour {

	public NavNode firstNode;
	public bool FirstLevel = false;

	[System.NonSerialized]
	public NavNode targetNode;
	public float LevelTimerStart = 0;

	public Vector2 ConstantParallax = new Vector2(1.0f, 0.75f);

	private GameState.Power? lastPower;
	private float lastSpeed;

	private tk2dSprite sprite;
	private tk2dSpriteAnimator anim;

	void Start()
	{
		sprite = GetComponent<tk2dSprite>();
		anim = GetComponent<tk2dSpriteAnimator>();

		targetNode = firstNode;
		GameState.LevelTimer = LevelTimerStart;
		GameState.CurrentMode = GameState.PlayMode.NotStarted;
		if (FirstLevel) { GameState.GameReset(); }
		GameState.ResetPlayer();

		lastPower = GameState.ActivePower;
		lastSpeed = GameState.PlayerSpeed;
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
	private Vector2 parallaxScroll;
	void Update () {
		if (lastPower != GameState.ActivePower)
		{
			SetColorByStatus();
		}

		parallaxScroll.x = 0; parallaxScroll.y = 0;

		if (GameState.CurrentMode == GameState.PlayMode.Started)
		{
			// Update the timers
			if (GameState.LevelTimer > 0)
			{
				GameState.LevelTimer -= Time.deltaTime;
			}
			if (GameState.LevelTimer < 0)
			{
				GameState.LevelTimer = 0;
			}

			if (GameState.DiscardTimer > 0)
			{
				GameState.DiscardTimer -= Time.deltaTime;
			}

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
				if (!anim.IsPlaying("Player Walk"))
				{
					anim.Play ("Player Walk");
				}
				if (lastSpeed != GameState.PlayerSpeed)
				{
					anim.ClipFps = anim.CurrentClip.fps * (GameState.PlayerSpeed / GameState.DefaultPlayerSpeed);
					lastSpeed = GameState.PlayerSpeed;
				}

				// Figure out the next move
				remainingDistance = targetNode.transform.position - transform.position;
				targetDirection = remainingDistance.normalized;
				targetDirection *= (GameState.PlayerSpeed * Time.deltaTime);
				if (targetDirection.magnitude > remainingDistance.magnitude)
				{
					targetDirection = remainingDistance;
					if (GameState.FollowAlternatePath && targetNode.AltNavNode != null)
					{
						targetNode = targetNode.AltNavNode;
					}
					else if (targetNode.NextNavNode != null)
					{
						targetNode = targetNode.NextNavNode;
					}
					else
					{
						GameState.EndLevel();
					}
				}

				// Move the player
				targetDirection.z = 0;
				transform.position += targetDirection;
				sprite.FlipX = targetDirection.x < -0.01f;
				parallaxScroll.x = targetDirection.x;
				parallaxScroll.y = targetDirection.y;

			}
			// Otherwise, decrement the stun timer
			else
			{
				lastSpeed = 0;

				if (!anim.IsPlaying("Player Stand"))
				{
					anim.Play ("Player Stand");
				}

				GameState.StunTimer -= Time.deltaTime;
				if (GameState.StunTimer < 0)
				{
					SetColorByStatus();
				}
			}
		}
		else
		{
			anim.Stop ();
			sprite.color = ColorNormal;
		}

		parallaxScroll += (ConstantParallax * Time.deltaTime * 0.1f);
		Parallax.Scroll(parallaxScroll);
	}

	private NavNode tmpNode;
	void OnTriggerEnter(Collider collider)
	{
		switch (collider.gameObject.tag)
		{
			case "Coin":
			GameState.Score += GameState.CoinPoints;
			DestroyObject(collider.gameObject);
			SoundBoard.PlayCoin();
			break;

			case "Enemy":
			if (!GameState.PlayerInvincible)
			{
				GameState.Score -= GameState.EnemyPointLoss;
				if (GameState.Score < 0) { GameState.Score = 0; }
				DestroyObject(collider.gameObject);
				GameState.StunTimer = GameState.StunTimerMax;
				SetColorByStatus();
				SoundBoard.PlayEnemy();
			}
			break;
		}
	}
}
