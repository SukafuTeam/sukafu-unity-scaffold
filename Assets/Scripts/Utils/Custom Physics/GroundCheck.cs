using System;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
	/// <summary> Updates whether or not the object is in contact with the ground </summary>
    public bool Grounded;
	/// <summary> The amout of force that pull the object down </summary>
	public int Gravity = 60;
	/// <summary>The maximum momentum the object can get (must be negative)</summary>
	public float MaxVerticalSpeed = -20;
	/// <summary>The actual vertical speed of the object</summary>
	public float VerticalSpeed;
	
	/// <summary>The reference object </summary>
    public Transform GroundCheckTransform;
	/// <summary>The distance between the center of object that the ray will be casted</summary>
	public float DistanceOffset;
	/// <summary> Check if more than one Ray should be cast downwars, and the distance between them and the center one </summary>
	public float SideCheck;
	/// <summary>Which layers of physics myst be looked for</summary>
    public LayerMask GroundMask;
	/// <summary> The referecente to the ceiling check transform </summary>
	public Transform CeilingCheckTransform;
	/// <summary> Which layers should be treated as ceiling </summary>
	public LayerMask CeilingMask;
	/// <summary> Transform to fix the direction accordinly with the hit normal </summary>
	public Transform FixBody;
	/// <summary> Flag to check for different degrees of slopes, enlarging the search for ground when grounded </summary>
	public bool ShouldSlope;

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
		
		VerticalSpeed -= Gravity * Time.deltaTime;

		if (VerticalSpeed < MaxVerticalSpeed)
			VerticalSpeed = MaxVerticalSpeed;
		
		var pos = transform.position;
		pos.y += VerticalSpeed * Time.deltaTime;
		transform.position = pos;
	}

    private bool IsGrounded()
    {
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
		    if (FixBody != null)
		    {
			    FixBody.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(Vector2.up.y, Vector2.up.x) * Mathf.Rad2Deg - 90);
		    }
		    return false;
	    }
		
		transform.position = new Vector3(
			transform.position.x,
			hit.point.y + (transform.position.y - GroundCheckTransform.position.y - 0.01f),
			transform.position.z
		);

	    if (FixBody != null)
	    {
		    var angle = Mathf.Atan2(hit.normal.y, hit.normal.x) * Mathf.Rad2Deg - 90;
		    FixBody.rotation = Quaternion.Euler(0, 0, Mathf.Clamp(angle, -30, 30));
	    }

	    VerticalSpeed = 0;
		return true;		
    }

	private RaycastHit2D CheckFree(Vector3 origin, float distance, Vector2 direction, LayerMask layerMask)
	{	
		var hit = Physics2D.Raycast(origin, direction, distance, layerMask);
		if (Math.Abs(SideCheck) < 0.01f || hit.collider != null)
		{
			Debug.DrawLine(origin, origin + new Vector3(0, -distance, 0), Color.red);
			return hit;
		}

		if(hit.collider == null)
			Debug.DrawLine(origin, origin + new Vector3(0, -distance, 0), Color.green);
		
		var leftCheck = origin;
		leftCheck.x -= SideCheck;
		hit = Physics2D.Raycast(leftCheck, direction, distance, layerMask);
		if (hit.collider != null)
		{
			Debug.DrawLine(leftCheck, leftCheck + new Vector3(0, -distance, 0), Color.red);
			return hit;
		}
		if(hit.collider == null)
			Debug.DrawLine(leftCheck, leftCheck + new Vector3(0,-distance,0), Color.green);	
		
		var rightCheck = origin;
		rightCheck.x += SideCheck;
		hit = Physics2D.Raycast(rightCheck, direction, distance, layerMask);
		if (hit.collider != null)
		{
			Debug.DrawLine(rightCheck, rightCheck + new Vector3(0, -distance, 0), Color.red);
			return hit;
		}
		
		if(hit.collider == null)
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
