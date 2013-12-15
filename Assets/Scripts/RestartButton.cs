using UnityEngine;
using System.Collections;

public class RestartButton : MonoBehaviour {

	void OnMouseUpAsButton()
	{
		GameOverScreen.Hide();
		GameState.ResetPlayer();
		GameState.LevelReset();
		GameState.GameReset();
		GameState.CurrentMode = GameState.PlayMode.Started;
		SoundBoard.PlayCard();
	}
}
