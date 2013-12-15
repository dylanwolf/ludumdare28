using UnityEngine;
using System.Collections;

public class StartButton : MonoBehaviour {

	float lastAspect;
	const string DiscardButton = "button-discard";
	private tk2dSprite sprite;

	void Start()
	{
		AlignToCamera();
		sprite = GetComponent<tk2dSprite>();
	}

	private Vector3 tmpPosition;
	void AlignToCamera()
	{
		tmpPosition = transform.localPosition;
		tmpPosition.y = Camera.main.orthographicSize;
		tmpPosition.x = -(Camera.main.aspect * Camera.main.orthographicSize);
		transform.localPosition = tmpPosition;
		lastAspect = Camera.main.aspect;
	}

	void OnMouseUpAsButton()
	{
		if (GameState.CurrentMode == GameState.PlayMode.NotStarted)
		{
			GameState.CurrentMode = GameState.PlayMode.Started;
			sprite.SetSprite(DiscardButton);
			SoundBoard.PlayCard();
		}
		else if (GameState.PowerTimer <= 0 && GameState.DiscardTimer <= 0)
		{
			GameState.DiscardTimer = GameState.DiscardTimerMax;
			GameState.PickNewPower(false);
			SoundBoard.PlayCard();
		}
	}

	void Update()
	{
		renderer.enabled = (GameState.CurrentMode != GameState.PlayMode.Finished) && (GameState.PowerTimer <= 0) && (GameState.DiscardTimer <= 0);
		if (lastAspect != Camera.main.aspect)
		{
			AlignToCamera();
		}
	}
}
