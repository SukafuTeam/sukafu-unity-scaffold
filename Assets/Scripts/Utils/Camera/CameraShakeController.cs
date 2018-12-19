using UnityEngine;

public class CameraShakeController : MonoBehaviour
{
	public static CameraShakeController Instance;

	private Camera _camera;
	public Camera Camera
	{
		get { return _camera ?? (_camera = Camera.main); }
	}

	private float _actualShakeTime;
	public float ShakeTime;

	public Vector2 ShakeAmount;
	public Transform CameraRef;

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			return;
		}
		
		Destroy(this);
	}

	void Update ()
	{
		var pos = CameraRef.position;
		
		if (_actualShakeTime > 0)
		{
			_actualShakeTime -= Time.deltaTime;
			pos += new Vector3(
				Random.Range(-ShakeAmount.x, ShakeAmount.x),
				Random.Range(-ShakeAmount.y, ShakeAmount.y),
				-10);

			pos.x += Random.Range(-ShakeAmount.x, ShakeAmount.x);
			pos.y += Random.Range(-ShakeAmount.y, ShakeAmount.y);
		}


		Camera.transform.position = pos;
	}

	public static void ShakeCamera(float shakeTime = 1f)
	{
		if (Instance == null)
			return;
		
		var actualShake = Mathf.Abs(shakeTime - -1) < 0.01f ? Instance.ShakeTime : shakeTime;
		Instance._actualShakeTime = actualShake;
	}
	
	[ContextMenu("Test Shake")]
	public void TestShake()
	{
		ShakeCamera();
	}
}
