using UnityEngine;
using System.Collections;

public class GameOverScreen : MonoBehaviour {

	public static GameOverScreen Current;

	public tk2dTextMesh ScoreMesh;
	public tk2dTextMesh BestMesh;

	// Use this for initialization
	void Start () {
		Current = this;
		ToggleSelfAndChildren(false);
	}

	void ToggleSelfAndChildren(bool state)
	{
		foreach (Renderer r in GetComponentsInChildren<Renderer>())
		{
			r.enabled = state;
		}
		foreach (Collider c in GetComponentsInChildren<Collider>())
		{
			c.enabled = state;
		}
	}

	public static void Show()
	{
		if (Current != null)
		{
			Current.Show_Internal();
		}
	}

	public void Show_Internal()
	{
		ScoreMesh.text = GameState.Score.ToString();
		ScoreMesh.Commit ();

		BestMesh.text = GameState.GetBestScore(GameState.Score).ToString();
		BestMesh.Commit();

		ToggleSelfAndChildren(true);
	}

	public static void Hide()
	{
		if (Current != null)
		{
			Current.Hide_Internal();
		}
	}

	public void Hide_Internal()
	{
		ToggleSelfAndChildren(false);
	}
}
