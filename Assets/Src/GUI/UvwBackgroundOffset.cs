using UnityEngine;
using System.Collections;

public class UvwBackgroundOffset : MonoBehaviour 
{
    public float scrollspeedx = 1F;
	public float scrollspeedy = 1F;
    
    void Update() 
	{
	//	Debug.Log("Time is " + Time.time + " and delta from previous frame is " + Time.deltaTime);
	 
		renderer.material.mainTextureOffset = new Vector2(Time.time * scrollspeedx / 100f , Time.time * scrollspeedy / 100f );
		
		// renderer.material.SetTextureOffset("_MainTex", new Vector2(offset, 0));
		
    }
}