using UnityEngine;
using System.Collections;

public class CameraBackgroundColor : MonoBehaviour 
{
    public Color color1 = Color.red;
    public Color color2 = Color.blue;
    public float duration = 3.0F;
	
    void Update() 
	{
	//	Debug.Log("Time is " + Time.time + " and delta from previous frame is " + Time.deltaTime);
	 
        float t = Mathf.PingPong(Time.time, duration) / duration;
        Camera.main.backgroundColor = Color.Lerp(color1, color2, t);
    }
}