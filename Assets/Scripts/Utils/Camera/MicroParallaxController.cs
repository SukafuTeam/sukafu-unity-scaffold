using UnityEngine;

public class MicroParallaxController : MonoBehaviour
{
	public Vector3 MinPosition;
	public Vector3 MaxPosition;

	private Camera _camera;
	public Camera Camera { get { return _camera ?? (_camera = Camera.main);}}
	public Vector2 CameraSize
	{
		get
		{
			return new Vector2(Camera.orthographicSize * Screen.width / Screen.height, Camera.orthographicSize);
		}
	}
	
	void LateUpdate ()
	{
		var cameraSize = CameraSize;
		var cameraMin = Camera.transform.position;
		cameraMin.x -= cameraSize.x / 2.0f;
		cameraMin.y += cameraSize.y / 2.0f;

		var cameraMax = Camera.transform.position;
		cameraMax.x += cameraSize.x / 2.0f;
		cameraMax.y -= cameraSize.y / 2.0f;
		
		var xPerc = Mathf.InverseLerp(cameraMin.x, cameraMax.x, transform.position.x);
		var yPerc = Mathf.InverseLerp(cameraMin.y, cameraMax.y, transform.position.y);
		
		transform.localPosition = new Vector3(Mathf.Lerp(MinPosition.x, MaxPosition.x, xPerc),
											  Mathf.Lerp(MinPosition.y, MaxPosition.y, yPerc));
	}

	[ContextMenu("Get Minimum Position")]
	public void GetMin()
	{
		MinPosition = transform.localPosition;
	}

	[ContextMenu("Get Maximum Position")]
	public void GetMax()
	{
		MaxPosition = transform.localPosition;
	}
}
