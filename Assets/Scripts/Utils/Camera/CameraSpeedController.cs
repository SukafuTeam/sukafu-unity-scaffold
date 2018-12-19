using UnityEngine;
using UnityEngine.UI;

public class CameraSpeedController : MonoBehaviour
{
	public float InitialSpeed;
	public float Speed;
	public float SpeedGain;

	private Camera _camera;
	public Camera Camera { get { return _camera ?? (_camera = Camera.main); } }
	public float CamY { get { return Camera.transform.position.y; }}
	public float CamX { get { return Camera.transform.position.x; }}

	private Rigidbody2D _rigidbody;
	public Rigidbody2D Rigidbody2D { get { return _rigidbody ?? (_rigidbody = GetComponent<Rigidbody2D>()); } }
	
	void Start ()
	{
		Speed = InitialSpeed;
	}

	void FixedUpdate()
	{
		Speed += SpeedGain;
		Rigidbody2D.velocity = new Vector2(0, Speed);
	}
}
