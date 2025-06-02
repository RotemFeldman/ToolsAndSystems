using UnityEngine;

namespace UnityUtils
{
	public static class Vector3Extensions
    {
	    public static Vector3 WithX(this Vector3 vector, float x)
	    {
		    vector.x = x;
		    return vector;
	    }
	    public static Vector3 WithY(this Vector3 vector, float y)
	    {
		    vector.y = y;
		    return vector;
	    }
	    public static Vector3 WithZ(this Vector3 vector, float z)
	    {
		    vector.z = z;
		    return vector;
	    }

	    public static Vector3 Add(this Vector3 vector, float x = 0, float y=0, float z=0)
	    {
		    vector.x += x;
		    vector.y += y;
		    vector.z += z;
		    return vector;
	    }
	    
	    public static Vector3 Rotate(this Vector3 vector, Vector3 axis, float angle)
	    {
		    return Quaternion.AngleAxis(angle, axis) * vector;
	    }
	    
    	public static Quaternion LookTo(this Vector3 from, Vector3 to, Vector3 up)
    	{
    		Vector3 direction = to - from;
    		// If direction is zero, return identity rotation.
    		if (direction.sqrMagnitude < Mathf.Epsilon)
    			return Quaternion.identity;
    		return Quaternion.LookRotation(direction, up);
    	}

	    public static float DistanceTo(this Vector3 from, Vector3 to)
	    {
		    return Vector3.Distance(from, to);
	    }
	    
	    public static Vector3 RotateTowards(this Vector3 from, Vector3 to, float maxDegrees)
	    {
		    return Vector3.RotateTowards(from, to, Mathf.Deg2Rad * maxDegrees, float.MaxValue);
	    }
	    
	    public static Vector3 LerpRotateTowards(this Vector3 from, Vector3 to, float t)
	    {
		    if (from == Vector3.zero || to == Vector3.zero)
			    return Vector3.Lerp(from, to, t);

		    Vector3 fromNorm = from.normalized;
		    Vector3 toNorm = to.normalized;
		    Vector3 result = Vector3.Lerp(fromNorm, toNorm, t);
		    return result.normalized * Mathf.Lerp(from.magnitude, to.magnitude, t);
	    }
		
		public static Vector3 RotateAroundPoint(this Vector3 vector, Vector3 point, float angleDegrees, float distance = 0, Vector3 axis = default)
		{
			if(axis == default)
				axis = Vector3.forward;
		
			float radians = angleDegrees * Mathf.Deg2Rad;
			float cos = Mathf.Cos(radians); 
			float sin = Mathf.Sin(radians);
		
			Vector3 dir = vector - point;
			if(distance > 0)
				dir = dir.normalized * distance;
		
			if(axis == Vector3.up)
			{
				float x = dir.x * cos - dir.z * sin;
				float y = dir.y;
				float z = dir.x * sin + dir.z * cos;
				return new Vector3(x, y, z) + point;
			}
			else if(axis == Vector3.right)
			{
				float x = dir.x;
				float y = dir.y * cos - dir.z * sin;
				float z = dir.y * sin + dir.z * cos;
				return new Vector3(x, y, z) + point;
			}
			else //Vector3.forward or other
			{
				float x = dir.x * cos - dir.y * sin;
				float y = dir.x * sin + dir.y * cos;
				float z = dir.z;
				return new Vector3(x, y, z) + point;
			}
		}
		


    }
}


