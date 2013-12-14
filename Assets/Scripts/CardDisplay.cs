using UnityEngine;
using System.Collections;

public class CardDisplay : MonoBehaviour {

	private GameState.Power lastPower;
	private tk2dSprite sprite;
	private float lastY;

	public GameObject Button;

	void Start()
	{
		sprite = GetComponent<tk2dSprite>();
		AlignToButton();
	}

	private Vector3 tmpPosition;
	void AlignToButton()
	{
		tmpPosition = transform.localPosition;
		tmpPosition.y = Button.transform.localPosition.y - ((BoxCollider)Button.collider).bounds.size.y;
		tmpPosition.x = Button.transform.localPosition.x;
		transform.localPosition = tmpPosition;
		lastY = Button.transform.localPosition.y;
	}

	string imageName;
	void SetImage()
	{
		switch (GameState.CurrentPower)
		{
			case GameState.Power.AlternatePath:
			imageName = "card-altpath";
			break;

			case GameState.Power.Coin2x:
			imageName = "card-coins2x";
			break;

			case GameState.Power.Speed2x:
			imageName = "card-speed2x";
			break;

			case GameState.Power.HalvePenalty:
			imageName = "card-halvelosses";
			break;

			case GameState.Power.Invincible:
			imageName = "card-invincible";
			break;

			default:
			return;
		}

		sprite.SetSprite(imageName);
	}

	void Update ()
	{
		renderer.enabled = (GameState.CurrentMode == GameState.PlayMode.Started) && (GameState.PowerTimer <= 0);
		if (GameState.PowerTimer <= 0)
		{
			if (lastY != Button.transform.localPosition.y)
			{
				AlignToButton();
			}
			if (lastPower != GameState.CurrentPower)
			{
				SetImage();
				lastPower = GameState.CurrentPower;
			}
		}
	}
}
