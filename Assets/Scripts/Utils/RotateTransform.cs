using UnityEngine;

public class RotateTransform : MonoBehaviour
{
	public Vector3 Angle;
	
	void Update ()
	{
		transform.Rotate(Angle * Time.deltaTime);
	}
}
