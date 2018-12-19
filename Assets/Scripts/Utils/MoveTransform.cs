using UnityEngine;

public class MoveTransform : MonoBehaviour
{
	public Vector3 MoveSpeed;
	public Space Space = Space.World;
	
	void Update () {
		transform.Translate(MoveSpeed * Time.deltaTime, Space);
	}
}
