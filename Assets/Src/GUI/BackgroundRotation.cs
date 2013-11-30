using UnityEngine;
using System.Collections;

public class BackgroundRotation : MonoBehaviour 
{
    public float rotatespeed = 1F;
	public float positionchangeX = 1F;
	public float positionchangeY = 1F;
	public float positionchangeZ = 1F;
	
	void Update() 
	{
	//	Debug.Log("Time is " + Time.time + " and delta from previous frame is " + Time.deltaTime);
		transform.localPosition = transform.localPosition + new Vector3(Mathf.Sin(Time.time) / 100f * positionchangeX ,0 , Mathf.Sin(Time.time) * positionchangeZ / 100f);
		transform.Rotate(Vector3.up * /*Time.time **/ rotatespeed / 100f);
		
		// renderer.material.SetTextureOffset("_MainTex", new Vector2(offset, 0));
		
    }
}