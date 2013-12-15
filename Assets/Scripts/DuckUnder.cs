using UnityEngine;
using System.Collections;

public class DuckUnder : MonoBehaviour {

	public NavNode OnTheWayTo;
	public Transform[] TopLayer;
	public float ZShift = -3;

	private Vector3 tmpPos;
	void ShiftLayers(bool reverse)
	{
		if (TopLayer == null) { return; }

		foreach (Transform t in TopLayer)
		{
			tmpPos = t.localPosition;
			tmpPos.z += ZShift * (reverse ? -1 : 1);
			t.localPosition = tmpPos;
		}
	}

	private Avatar avatar;
	void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.tag == "Player")
		{
			avatar = collider.GetComponent<Avatar>();
			if (avatar != null && avatar.targetNode == OnTheWayTo)
			{
				ShiftLayers(false);
			}
		}
	}

	void OnTriggerExit(Collider collider)
	{
		if (collider.gameObject.tag == "Player")
		{
			avatar = collider.GetComponent<Avatar>();
			if (avatar != null && avatar.targetNode == OnTheWayTo)
			{
				ShiftLayers(true);
			}
		}
	}
}
