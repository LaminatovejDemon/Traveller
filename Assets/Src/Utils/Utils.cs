using UnityEngine;
using System.Collections;

public class Utils : MonoBehaviour 
{
	//Vector3.slerp sometimes moves object in wrong axis first and is broken
	public static Vector3 Interpolate(Vector3 source, Vector3 target, float relativeAddition = 0.1f)
	{
		if ((target - source).magnitude < 0.01f )
		{
			return target;
		}
		
		return source + (target - source) * relativeAddition;
	}
}
