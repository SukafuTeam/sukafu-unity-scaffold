using UnityEngine;

public class WallCheck
{
    /// <summary>
    /// Used to determine if an object can or cannot go to right or left based on which physics layer it should look for
    /// </summary>
    /// <param name="origin"> The object transform</param>
    /// <param name="wallCheck"> The side distance reference object transform</param>
    /// <param name="wallMask"> Which layers of physics should be checked </param>
    /// <param name="lookRight"> If the object is looking to move to the right </param>
    /// <param name="yOffset"> The optional Y offset to shot the ray</param>
    /// <returns></returns>	
    public static bool IsFree(ref Vector3 origin,Vector3 wallCheck, LayerMask wallMask, bool lookRight = true, float yOffset=0.0f)
    {
        var distance = wallCheck.x - origin.x;

        var rayOrigin = origin;
        if (Mathf.Abs(yOffset) > 0.01f)
            rayOrigin.y += yOffset;
		
        var hit = Physics2D.Raycast(rayOrigin, lookRight ? Vector2.right : Vector2.left, distance, wallMask);
        if (hit.collider == null)
        {
            Debug.DrawLine(rayOrigin, rayOrigin + (lookRight ? Vector3.right : Vector3.left) * distance, Color.green);
            return true;
        }

        var bodySize = wallCheck.x - origin.x;
        var hitPoint = lookRight ? hit.point.x - bodySize : hit.point.x + bodySize;
        origin = new Vector3(hitPoint, origin.y, origin.z);
		
        Debug.DrawLine(rayOrigin, rayOrigin + (lookRight ? Vector3.right : Vector3.left) * distance, Color.red);

        return false;
    }	
}