using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour 
{	

	public Camera targetCamera;

	void Update () 
	{
		if ( targetCamera == null )
		{
			targetCamera = FleetManager.GetShip().cameraHandler._RealCamera;
		}


		transform.rotation = targetCamera.transform.rotation;
	}
}
