using UnityEngine;
using System.Collections;

public class ButtonManager : MonoBehaviour 
{
	private static ButtonManager mInstance = null;
	
	public static ButtonManager GetInstance()
	{
		if ( mInstance == null )
		{
			GameObject instanceObject_ = GameObject.Find("#ButtonManager");
			if ( instanceObject_ != null )
				mInstance = instanceObject_.transform.GetComponent<ButtonManager>();
			else
				mInstance = new GameObject("#ButtonManager").AddComponent<ButtonManager>();
		}
		return mInstance;
	}
	

}
