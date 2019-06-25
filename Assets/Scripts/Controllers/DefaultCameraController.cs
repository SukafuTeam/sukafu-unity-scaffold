using UnityEngine;

public class DefaultCameraController : MonoBehaviour {

	void Update () {
		transform.Translate(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"),0) * 0.3f);
	}
}
