﻿using UnityEngine;
using System.Collections;

public class Utils : MonoBehaviour 
{
	public const bool RenderToTexture = false;
	
	//Vector3.slerp sometimes moves object in wrong axis first and is broken
	public static Vector3 Interpolate(Vector3 source, Vector3 target, float relativeAddition = 0.1f)
	{
		if ((target - source).magnitude < 0.01f )
		{
			return target;
		}
		
		return source + (target - source) * relativeAddition;
	}
	
	public static void SetLayer(Transform parent, int layer)
	{
		for ( int i = 0; i < parent.childCount; ++i )
		{
			SetLayer(parent.GetChild(i), layer);
		}
		
		
		Light light_ = parent.gameObject.GetComponent<Light>();
		if ( light_ != null )
		{
			light_.cullingMask = 1<<layer;
		}
		
		parent.gameObject.layer = layer;
	}

	public static void SetTransformCamera(Transform parent, Camera from, Camera to)
	{
		SetLayer(parent, to.gameObject.layer);

		Vector3 oldViewportPosition_ = from.WorldToViewportPoint(parent.position);
		Vector3 newWorldPositon_ = to.ViewportToWorldPoint(oldViewportPosition_);

		parent.transform.position = newWorldPositon_;
	}
	
	public static void ChangeColor(Transform parent, Color color)
	{
		for ( int i = 0; i < parent.childCount; ++i )
		{
			ChangeColor(parent.GetChild(i), color);
		}
		
		Renderer rend_ = parent.gameObject.GetComponent<Renderer>();
		Light light_ = parent.gameObject.GetComponent<Light>();
		if ( rend_ != null )
		{
			rend_.material.color = color;
			rend_.material.SetColor("_TintColor", color);
		}
		if ( light_ != null )
		{
			light_.color = color;
		}
	}
}
