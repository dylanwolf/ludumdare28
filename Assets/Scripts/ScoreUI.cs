using UnityEngine;
using System.Collections;

public class ScoreUI : MonoBehaviour {

	private tk2dTextMesh score;
	public tk2dSprite ScoreLabel;
	public tk2dTextMesh Bonus;
	public tk2dSprite BonusLabel;

	float lastAspect;
	int lastScore;
	int lastBonus;

	void Start () {
		score = GetComponent<tk2dTextMesh>();
		AlignToCamera();

		lastScore = GameState.Score;
		lastBonus = Mathf.CeilToInt(GameState.LevelTimer);
	}

	Vector3 tmpPos;
	void AlignToCamera()
	{
		// Align score
		tmpPos = score.transform.localPosition;
		tmpPos.y = -Camera.main.orthographicSize;
		tmpPos.x = -(Camera.main.aspect * Camera.main.orthographicSize);
		score.transform.localPosition = tmpPos;

		// Align score label
		tmpPos = ScoreLabel.transform.localPosition;
		tmpPos.y = score.transform.localPosition.y + (score.GetEstimatedMeshBoundsForString(score.text).size.y * 1.1f);
		tmpPos.x = score.transform.localPosition.x;
		ScoreLabel.transform.localPosition = tmpPos;

		// Align bonus
		tmpPos = Bonus.transform.localPosition;
		tmpPos.y = ScoreLabel.transform.localPosition.y + ScoreLabel.GetBounds().size.y;
		tmpPos.x = score.transform.localPosition.x;
		Bonus.transform.localPosition = tmpPos;

		// Align bonus label
		tmpPos = BonusLabel.transform.localPosition;
		tmpPos.y = Bonus.transform.localPosition.y + (Bonus.GetEstimatedMeshBoundsForString(Bonus.text).size.y * 1.1f);
		tmpPos.x = score.transform.localPosition.x;
		BonusLabel.transform.localPosition = tmpPos;

		lastAspect = Camera.main.aspect;
	}

	void Update () {
		if (lastAspect != Camera.main.aspect)
		{
			AlignToCamera();
		}

		if (lastScore != GameState.Score)
		{
			score.text = GameState.Score.ToString();
			score.Commit();
			lastScore = GameState.Score;
		}

		if (lastBonus != Mathf.CeilToInt(GameState.LevelTimer))
		{
			Bonus.text = Mathf.CeilToInt(GameState.LevelTimer).ToString();
			Bonus.Commit();
			lastBonus = Mathf.CeilToInt(GameState.LevelTimer);
		}
	}
}
