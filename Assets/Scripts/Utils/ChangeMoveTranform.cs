using UnityEngine;

public class ChangeMoveTranform : MonoBehaviour
{
	public Vector3 MoveChange;

	public void OnTriggerEnter2D(Collider2D coll)
	{
		var move = coll.GetComponent<MoveTransform>();
		if (move == null)
			return;

		var spe = move.MoveSpeed;

		spe.x *= MoveChange.x;
		spe.y *= MoveChange.y;
		spe.z *= MoveChange.z;

		move.MoveSpeed = spe;
	}
}
