using UnityEngine;
using System.Collections;

public class StartButton : MonoBehaviour {

	float lastAspect;
	const string ActivateButton = "button-activate";
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
			sprite.SetSprite(ActivateButton);
		}
		else if (GameState.PowerTimer <= 0)
		{
			GameState.ActivatePower(GameState.CurrentPower);
		}
	}

	void Update()
	{
		renderer.enabled = (GameState.CurrentMode != GameState.PlayMode.Finished) && (GameState.PowerTimer <= 0);
		if (lastAspect != Camera.main.aspect)
		{
			AlignToCamera();
		}
	}
}
