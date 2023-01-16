using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

[RequireComponent(typeof(GroundCheck))]
public class PlatformController : MonoBehaviour
{
	[Header("Controller Fields")]
	public bool Dead;

	private GroundCheck _groundCheck;
	/// <summary> Reference to the GroundCheck component, responsible for checking ground and ceiling collision </summary>
	public GroundCheck GroundCheck
	{
		get { return _groundCheck ?? (_groundCheck = GetComponent<GroundCheck>()); }
	}
	
	[Header("Wall Check Fields")]
	public Transform WallCheckTransform;
	/// <summary> Which layers should the script check for walls </summary>
	public LayerMask WallMask;
	/// <summary> Control boolean to check if there's something block the way to the right </summary>
	public bool FreeLeft;
	/// <summary> Control boolean to check if there's something block the way to the left </summary>
	public bool FreeRight;

	/// <summary> Can the player wall jump from a right wall? </summary>
	public bool RightHugging
	{
		get { return EnableWallJump && !WallJumpFreeRight && !GroundCheck.Grounded; }
	}
	
	/// <summary> Can the player wall jump from a left wall? </summary>
	public bool LeftHugging
	{
		get { return EnableWallJump && !WallJumpFreeLeft && !GroundCheck.Grounded; }
	}
	
	/// <summary> Is the player sliding from a right wall? </summary>
	public bool RightSliding
	{
		get { return RightHugging && _currentWallHugTime > 0f; }
	}

	/// <summary> Is the player sliding from a left wall? </summary>
	public bool LeftSliding
	{
		get { return LeftHugging && _currentWallHugTime > 0f; }
	}

	[Header("Movement Fields")]
	public bool CutScene;
	/// <summary> Is the player currently looking right? Used for visual and logical checks </summary>
	public bool LookRight;
	/// <summary> How fast the player can move while running? Will be delta timed </summary>
	public float MoveSpeed;
	/// <summary> Can the player move horizontally at this frame? </summary>
	public bool CanMove;
	public float CanMoveTime;
	public float LockedSpeed;
	/// <summary> Is the player moving this frame? Used for visual and logical checks </summary>
	public bool Moving;
	/// <summary> Can the player have different speed depending on the ground angle? </summary>
	public bool EnableHillModifiers = true;
	/// <summary> How slower will the player move while climbing up? </summary>
	public float UphillModifier = 0.8f;
	/// <summary> How faster will the player move while climbing down? </summary>
	public float DownhillModifier = 1.2f;

	[Header("Dash Fields")]
	public bool EnableDash;
	/// <summary> Is the player dashing this frame? Used for visual and logical checks </summary>
	public bool Dashing;
	/// <summary> Can the player dash currently? For logical check </summary>
	private bool _canDash;
	/// <summary> How fast the player will move while dashing? </summary>
	public float DashSpeed;
	/// <summary> How long the player dash in seconds </summary>
	public float DashTime;
	/// <summary> internal field to check how long the player will still dash </summary>
	private float _currentDashTime;

	[Header("Jump Fields")]
	public float JumpForce;
	/// <summary> Can the player double jump? </summary>
	public bool EnableDoubleJump;
	/// <summary> is the player double jumping? </summary>
	private bool _doubleJumping;
	/// <summary> How much longer after leaving the ground can the player still jump? </summary>
	public float CoyoteTime = 0.2f;
	/// <summary> internal field to control how long until the player can't jump anymore </summary>
	private float _currentCoyoteTime;
	/// <summary> Should the visual body animate when jumping and landing? </summary>
	public bool EnableSquishStretch = true;
	/// <summary> How much will the player transform horizontally and vertically when jumping </summary>
	public Vector2 JumpSquishAmount = new Vector2(0.5f, 1.2f);
	/// <summary> How much will the player transform horizontally and vertically when landing </summary>
	public Vector2 LandSquishAmount = new Vector2(1.2f, 0.5f);
	/// <summary> Method that will be called when touching the ground </summary>
	public UnityEvent OnGroundedEvent;
	
	[Header("Wall Jump Fields")]
	public bool EnableWallJump;
	/// <summary> From which layers can the player wall jump from? </summary>
	public LayerMask WallJumpLayer;
	/// <summary> Check if there's jumpable wall to the right of the player </summary>
	public bool WallJumpFreeRight;
	/// <summary> Check if there's jumpable wall to the left of the player </summary>
	public bool WallJumpFreeLeft;
	/// <summary> How fast can the player fall while wall sliding </summary>
	public float SlidingMaxSpeed;
	/// <summary> How long will the player move horizontally when wall jumping </summary>
	public float WallJumpMoveTime;
	/// <summary> Variable to determine how long the user can let go of hugging the wall while still being able to wall jump </summary>
	public float WallHugTime = 0.1f;
	/// <summary> Internal variable to control wall hug time </summary>
	private float _currentWallHugTime;
	/// <summary> regular fall speed without wall sliding </summary>
	private float _originalMaxSpeed;
	
	[Header("Effect Fields")]
	public ParticleSystem GroundSmoke;
	/// <summary> Scake to go back to after applying squish and stretch transformations </summary>
	public Vector3 OriginalScale;

	private bool _movingRight
	{
		get
		{
			return InputController.MoveRight(0.25f) || InputController.Right;
		}
	}
	private bool _movingLeft
	{
		get
		{
			return InputController.MoveLeft(-0.25f) || InputController.Left;
		}
	}
	
	
	void Start ()
	{
		GroundCheck.TouchedGround += OnGrounded;
		_originalMaxSpeed = GroundCheck.MaxVerticalSpeed;
		CanMove = true;
		_canDash = true;
		Dead = false;

		if (GroundCheck.FixBody == null)
			return;
		
		var currentScale = GroundCheck.FixBody.transform.localScale;
		OriginalScale = currentScale;
	}

	void Update()
	{
		if (Dead)
		{
			GroundCheck.enabled = false;
			return;
		}

		var pos = transform.position;
		if (!CutScene)
		{
			UpdateJump();
			UpdateWallJump();
			pos = UpdateDash(pos);	
		}
		pos = UpdateMovement(pos);
		UpdateEffects();
		transform.position = pos;
		UpdatePosition();
	}

	private void UpdateJump()
	{
		if (GroundCheck.Grounded)
			_currentCoyoteTime = CoyoteTime;
		else
			_currentCoyoteTime -= Time.deltaTime;

		if (!InputController.JumpDown)
			return;

		if (_currentCoyoteTime > 0f)
		{
			GroundCheck.Jump(JumpForce);
			JumpSquish();
			_currentCoyoteTime = 0f;
			return;
		}

		if (!EnableDoubleJump || _doubleJumping || RightSliding || LeftSliding)
			return;

		GroundCheck.Jump(JumpForce);
		JumpSquish();
		_doubleJumping = true;
	}

	private void UpdateWallJump()
	{
		if (!EnableWallJump)
			return;

		if (RightHugging && _movingRight || LeftHugging && _movingLeft)
			_currentWallHugTime = WallHugTime;

		_currentWallHugTime -= Time.deltaTime;

		GroundCheck.MaxVerticalSpeed = _originalMaxSpeed;

		if (RightSliding || LeftSliding)
		{
			GroundCheck.MaxVerticalSpeed = SlidingMaxSpeed;
			if (!Dashing)
			{
				_doubleJumping = false;
				_canDash = true;
			}

			if (InputController.JumpDown)
			{
				GroundCheck.Jump(JumpForce);
				JumpSquish();
				if (RightSliding)
					MoveWallJump(true);
				if(LeftSliding)
					MoveWallJump(false);
				_currentWallHugTime = 0f;
				
			}
		}
	}

	private Vector3 UpdateDash(Vector3 pos)
	{
		if (!EnableDash)
			return pos;
		
		if (_currentDashTime > 0)
		{
			_currentDashTime -= Time.deltaTime;
			if (_currentDashTime <= 0f || (!LookRight && !FreeLeft) || (LookRight && !FreeRight))
			{
				Dashing = false;
				_currentDashTime = 0f;
				return pos;
			}
			
			var moveAmount = DashSpeed * Time.deltaTime;

			var newPos = pos;
			newPos.x += LookRight ? moveAmount : -moveAmount;

			if(LookRight && WallCheck.IsFree(ref pos, newPos, WallMask))
				pos.x += moveAmount;
			if(!LookRight && WallCheck.IsFree(ref pos, newPos, WallMask, false))
				pos.x -= moveAmount;
			
			GroundCheck.VerticalSpeed = 0;
			return pos;
		}
		
		if (_doubleJumping || !_canDash)
			return pos;

		if (InputController.DashDown)
		{
			if (LeftSliding)
				LookRight = true;
			if(RightSliding)
				LookRight = false;
			if (!GroundCheck.Grounded)
				_canDash = false;
			_currentDashTime = DashTime;
			Dashing = true;
			SoundController.PlaySfx("dash");
			GroundCheck.VerticalSpeed = 0;
		}

		return pos;
	}

	private Vector3 UpdateMovement(Vector3 pos)
	{
		if (Dashing)
			return pos;
		
		Moving = false;
		
		if (!CanMove && CanMoveTime > 0f)
		{
			Moving = LockedSpeed != 0f;
			LookRight = LockedSpeed > 0;
			pos.x += LockedSpeed * Time.deltaTime;

			CanMoveTime -= Time.deltaTime;
			if (CanMoveTime <= 0f)
			{
				CanMove = true;
				LockedSpeed = 0f;
			}

			return pos;
		}

		if (CutScene)
			return pos;

		var speed = MoveSpeed;
		var slopeAngle = GroundCheck.LastSlope;
		var upSlopeModifier = Mathf.InverseLerp(30, 0, Mathf.Abs(slopeAngle));
		var downSlopeModifier = Mathf.InverseLerp(0, 30, Mathf.Abs(slopeAngle));
		
		if(_movingRight)
		{
			LookRight = true;

			if (EnableHillModifiers)
			{
				if (slopeAngle > 0.1f) // is going uphill
					speed *= Mathf.Lerp(UphillModifier, 1.0f, upSlopeModifier);
				else if (slopeAngle < -0.1f) // is going downhill
					speed *= Mathf.Lerp(1.0f, DownhillModifier, downSlopeModifier);	
			}

			if (FreeRight)
			{
				pos.x += speed * Time.deltaTime;
				Moving = true;
			}
		} 
		else if(_movingLeft)
		{
			LookRight = false;

			if (EnableHillModifiers)
			{
				if (slopeAngle < -0.1f) // is going uphill
					speed *= Mathf.Lerp(UphillModifier, 1.0f, upSlopeModifier);
				else if (slopeAngle > 0.1f) // is going downhill
					speed *= Mathf.Lerp(1.0f, DownhillModifier, downSlopeModifier);	
			}

			if (FreeLeft)
			{
				pos.x -= speed * Time.deltaTime;
				Moving = true;
			}
		}

		return pos;
	}

	
	private void UpdatePosition()
	{
		var pos = transform.position;
		var wallPos = WallCheckTransform.position;
		FreeRight = WallCheck.IsFree(ref pos, wallPos, WallMask);
		FreeLeft = WallCheck.IsFree(ref pos, wallPos, WallMask, false);
		transform.position = pos;
		
		WallJumpFreeRight = WallCheck.IsFree(ref pos, wallPos, WallJumpLayer);
		WallJumpFreeLeft = WallCheck.IsFree(ref pos, wallPos, WallJumpLayer, false);
	}

	private void UpdateEffects()
	{
		// Ground particle
		if (GroundSmoke != null)
		{
			var emission = GroundSmoke.emission;
			emission.enabled = GroundCheck.Grounded && Moving;	
		}
	}
	
	private void MoveWallJump(bool right)
	{
		MoveTime(right ? -MoveSpeed : MoveSpeed, WallJumpMoveTime);
	}

	private void MoveTime(float speed, float time)
	{
		LockedSpeed = speed;
        CanMove = false;
        CanMoveTime = time;
	}
	
	void OnGrounded(float speed)
	{
		if (EnableDoubleJump)
			_doubleJumping = false;
		
		if(EnableDash)
			_canDash = true;

		if (speed < -2f)
		{
			OnGroundedEvent.Invoke();
		}

		LandSquish();
	}

	private void LandSquish()
	{
		if (GroundCheck.FixBody == null || !EnableSquishStretch)
			return;

		GroundCheck.FixBody.DOKill();
		var scale = OriginalScale;
		GroundCheck.FixBody.localScale = scale;

		scale.x *= LandSquishAmount.x;
		scale.y *= LandSquishAmount.y;

		var squishSeq = DOTween.Sequence();
		squishSeq.Append(GroundCheck.FixBody.DOScale(scale, 0.05f));
		squishSeq.Append(GroundCheck.FixBody.DOScale(OriginalScale, 0.05f));
		squishSeq.Play();
	}

	private void JumpSquish()
	{
		if (GroundCheck.FixBody == null || !EnableSquishStretch)
			return;

		GroundCheck.FixBody.DOKill();
		var scale = OriginalScale;
		GroundCheck.FixBody.localScale = scale;
		
		scale.x *= JumpSquishAmount.x;
		scale.y *= JumpSquishAmount.y;

		var squishSeq = DOTween.Sequence();
		squishSeq.Append(GroundCheck.FixBody.DOScale(scale, 0.05f));
		squishSeq.Append(GroundCheck.FixBody.DOScale(OriginalScale, 0.05f));
		squishSeq.Play();
	}

	public void Die()
	{
		Dead = true;
	}

	void OnDestroy()
	{
		GroundCheck.TouchedGround -= OnGrounded;
	}
}
