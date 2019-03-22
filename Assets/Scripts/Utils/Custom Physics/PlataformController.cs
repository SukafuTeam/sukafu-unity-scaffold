using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(GroundCheck))]
public class PlataformController : MonoBehaviour
{
	private GroundCheck _groundCheck;
	public GroundCheck GroundCheck
	{
		get { return _groundCheck ?? (_groundCheck = GetComponent<GroundCheck>()); }
	}
	
	[Header("Wall Check Fields")]
	public Transform WallCheckTransform;
	public LayerMask WallMask;
	public bool FreeLeft;
	public bool FreeRight;

	[Header("Movement Fields")]
	public bool LookRight;
	public float MoveSpeed;
	public bool Moving;

	[Header("Jump Fields")]
	public float JumpForce;
	public bool CanDoubleJump;
	private bool _doubleJumping;
	public UnityEvent OnGroundedEvent;
	
	void Start ()
	{
		GroundCheck.TouchedGround += OnGrounded;
	}

	void Update()
	{
		UpdatePosition();
	}
	
	void LateUpdate()
	{
		if (!GroundCheck.CheckCeilingFree() && GroundCheck.VerticalSpeed > 0)
			GroundCheck.VerticalSpeed = 0;
	}

	public void MoveRight()
	{
		var pos = transform.position;
		LookRight = true;
		Moving = true;
		if (FreeRight)
			pos.x += MoveSpeed * Time.deltaTime;
		transform.position = pos;
	}

	public void MoveLeft()
	{
		var pos = transform.position;
		LookRight = false;
		Moving = true;
		if (FreeLeft)
			pos.x -= MoveSpeed * Time.deltaTime;
		transform.position = pos;
	}

	public void Jump()
	{
		if (GroundCheck.Grounded)
		{
			GroundCheck.Jump(JumpForce);
			return;
		}

		if (!CanDoubleJump || _doubleJumping)
			return;

		GroundCheck.Jump(JumpForce);
		_doubleJumping = true;
	}

	void UpdatePosition()
	{
		var pos = transform.position;
		FreeRight = WallCheck.IsFree(ref pos, WallCheckTransform, WallMask);
		FreeLeft = WallCheck.IsFree(ref pos, WallCheckTransform, WallMask, false);
		transform.position = pos;
	}

	
	void OnGrounded(float speed)
	{
		if (CanDoubleJump)
			_doubleJumping = false;
		
		OnGroundedEvent.Invoke();
	}

	void OnDestroy()
	{
		GroundCheck.TouchedGround -= OnGrounded;
	}
}
