using System;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
	/// <summary> Updates whether or not the object is in contact with the ground </summary>
    public bool Grounded;
	/// <summary> The amout of force that pull the object down </summary>
	public float Gravity = 1;
	/// <summary> The amount of extra gravity when falling down </summary>
	public float FallMultiplier = 1.5f;
	/// <summary>The maximum momentum the object can get (must be negative)</summary>
	public float MaxVerticalSpeed = -20;
	/// <summary>The current vertical speed of the object</summary>
	public float VerticalSpeed;
	
	/// <summary>The reference object </summary>
    public Transform GroundCheckTransform;
	/// <summary>The distance between the center of object that the ray will be casted</summary>
	public float DistanceOffset;
	/// <summary> Check if more than one Ray should be cast downwards, and the distance between them and the center one </summary>
	public float SideCheck;
	/// <summary>Which layers of physics myst be looked for</summary>
    public LayerMask GroundMask;
	/// <summary> The referecente to the ceiling check transform </summary>
	public Transform CeilingCheckTransform;
	/// <summary> Which layers should be treated as ceiling </summary>
	public LayerMask CeilingMask;
	/// <summary> Transform to fix the direction accordinly with the hit normal </summary>
	public Transform FixBody;
	/// <summary> How intense should the rotation of the fix body be </summary>
	public float FixBodyLerp;
	/// <summary> Current FixBodyAngle, so rotations can be lerped </summary>
	private float _currentBodyAngle;
	/// <summary> Flag to check for different degrees of slopes, enlarging the search for ground when grounded </summary>
	public bool ShouldSlope;
	/// <summary> Stores the angle of the last ground check slope, used to check if ground is uphill or downhill </summary>
	public float LastSlope;

	public delegate void OnGroundTouch(float verticalSpeed);

	public OnGroundTouch TouchedGround;
	
	void LateUpdate ()
	{
		ApplyGravity();
		var previous = Grounded;
		var previousSpeed = VerticalSpeed;
		Grounded = IsGrounded();
		if (Grounded != previous && Grounded && TouchedGround != null)
			TouchedGround(previousSpeed);

		if (!CheckCeilingFree() && VerticalSpeed > 0f)
			VerticalSpeed = 0;
		
		UpdateFixBodyRotation();
	}

	public void Jump(float jumpForce)
	{
		VerticalSpeed = jumpForce;
	}
 
	public void AddForce(float jumpForce)
	{
		VerticalSpeed += jumpForce;
	}
	
	private void ApplyGravity()
	{
		if (Time.deltaTime > 0.2f || Grounded)
			return;
		
		if (VerticalSpeed < 0)
			VerticalSpeed -= Gravity * FallMultiplier * Time.deltaTime;
		else
			VerticalSpeed -= Gravity * Time.deltaTime;

		if (VerticalSpeed < MaxVerticalSpeed)
			VerticalSpeed = MaxVerticalSpeed;
		
		var pos = transform.position;
		pos.y += VerticalSpeed * Time.deltaTime;
		transform.position = pos;
	}

    private bool IsGrounded()
    {
	    if (FixBody != null)
		    FixBody.rotation = Quaternion.identity;

	    var pos = transform.position;
	    var origin =  new Vector3(pos.x, pos.y - DistanceOffset , pos.z);	    
        var distance = origin.y - GroundCheckTransform.position.y + (Grounded && ShouldSlope ? 0.5f : 0f);
	    
	    if (VerticalSpeed > 0)
	    {
        	Debug.DrawLine(origin, GroundCheckTransform.position,	Color.green);
		    return false;
	    }

	    var hit = CheckFree(origin, distance, Vector2.down, GroundMask);
	    if (hit.collider == null)
	    {
		    LastSlope = 0.0f;
		    return false;
	    }
		
		transform.position = new Vector3(
			pos.x,
			hit.point.y + (pos.y - GroundCheckTransform.position.y - 0.01f),
			pos.z
		);
		
		var angle = Mathf.Atan2(hit.normal.y, hit.normal.x) * Mathf.Rad2Deg - 90;
		LastSlope = Mathf.Clamp(angle, -30, 30);
		
		if (FixBody != null)
			FixBody.rotation = Quaternion.Euler(0, 0, Mathf.Clamp(angle, -30, 30));
		
		VerticalSpeed = 0;
		return true;		
    }

    private void UpdateFixBodyRotation()
    {
	    if (FixBody == null || !ShouldSlope)
		    return;
	    
	    var rotation = FixBody.rotation.eulerAngles;
	    
	    var desiredAngle = LastSlope;

	    if (!Grounded)
		    desiredAngle = 0f;

	    var desiredRotation = Quaternion.Euler(rotation.x, rotation.y, desiredAngle);

	    var lerpedRotation = Quaternion.Lerp(FixBody.rotation, desiredRotation, FixBodyLerp);
	    
	    FixBody.rotation = lerpedRotation;
    }

	private RaycastHit2D CheckFree(Vector3 origin, float distance, Vector2 direction, LayerMask layerMask)
	{	
		var hit = Physics2D.Raycast(origin, direction, distance, layerMask);
		if (Math.Abs(SideCheck) < 0.01f || hit.collider != null)
		{
			Debug.DrawLine(origin, origin + new Vector3(0, -distance, 0), Color.red);
			return hit;
		}
		Debug.DrawLine(origin, origin + new Vector3(0, -distance, 0), Color.green);
		
		var leftCheck = origin;
		leftCheck.x -= SideCheck;
		hit = Physics2D.Raycast(leftCheck, direction, distance, layerMask);
		if (hit.collider != null)
		{
			Debug.DrawLine(leftCheck, leftCheck + new Vector3(0, -distance, 0), Color.red);
			return hit;
		}
		Debug.DrawLine(leftCheck, leftCheck + new Vector3(0, -distance, 0), Color.green);

		var rightCheck = origin;
		rightCheck.x += SideCheck;
		hit = Physics2D.Raycast(rightCheck, direction, distance, layerMask);
		if (hit.collider != null)
		{
			Debug.DrawLine(rightCheck, rightCheck + new Vector3(0, -distance, 0), Color.red);
			return hit;
		}
		
		Debug.DrawLine(rightCheck, rightCheck + new Vector3(0, -distance, 0), Color.green);

		return new RaycastHit2D();
	}

	public bool CheckCeilingFree()
	{
		if (CeilingCheckTransform == null)
			return true;
		
		var pos = transform.position;
		var distance = CeilingCheckTransform.position.y - pos.y;
		
		var hit = CheckFree(pos, distance, Vector2.up, CeilingMask);
		if (hit.collider == null)
		{
			Debug.DrawLine(pos, CeilingCheckTransform.position,	Color.green);
			return true;
		}
		
		var bodySize = CeilingCheckTransform.position.y - pos.y;
		var hitPoint = hit.point.y - bodySize;
		pos = new Vector3(pos.x, hitPoint, pos.z);
		transform.position = pos;
		
		Debug.DrawLine(pos, CeilingCheckTransform.position, Color.red);
		return false;
	}
}
