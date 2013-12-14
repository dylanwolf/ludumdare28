using UnityEngine;
using System.Collections;

public class EnemyFloat : MonoBehaviour {

	public float MinRotate = -10;
	public float MaxRotate = 15;
	public float RotateSpeed = 0.5f;

	public float BobDistance = 0.001f;
	private float bobAnchorY = 0;

	private float timer = 0f;

	void Start()
	{
		timer = Random.Range(0, 1);
		bobAnchorY = transform.localPosition.y;
	}

	Vector3 tmpRot;

	Vector3 tmpPos;
	void Update()
	{
		timer += (RotateSpeed * Time.deltaTime);
		if (timer > 1) { timer = 0; }

		tmpRot = transform.localEulerAngles;
		tmpRot.z = ((MaxRotate - MinRotate) * ((Mathf.Sin(timer * Mathf.PI * 2) + 1) * 0.5f)) + MinRotate;
		transform.localEulerAngles = tmpRot;

		tmpPos = transform.localPosition;
		tmpPos.y = bobAnchorY + (Mathf.Sin (timer * Mathf.PI) * BobDistance);
		transform.localPosition = tmpPos;
	}
}
