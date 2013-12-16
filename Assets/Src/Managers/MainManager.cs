﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainManager : MonoBehaviour {
	
	public CameraHandler _BattleCamera;
	public CameraHandler _HangarCamera;
	public CameraHandler _EnemyCamera;
	public Camera _InventoryCamera;
	public Camera _GUICamera;
	
	private static MainManager mInstance = null;
	
	Vector3 [] mFingerPositions = new Vector3[10];
	Vector3 [] mFingerWorldPositions = new Vector3[10];
	
	List<GameObject> mTouchListeners = new List<GameObject>();
	
	public static MainManager GetInstance()
	{
		if ( mInstance == null )
		{
			GameObject instanceObject_ = GameObject.Find("#MainManager");
			if ( instanceObject_ != null )
				mInstance = instanceObject_.transform.GetComponent<MainManager>();
			else
				mInstance =  new GameObject("#MainManager").AddComponent<MainManager>();
		}
		return mInstance;
	}
	
	void Start () 
	{
		name = "#MainManager";
		
		PartManager.GetInstance();
	//	NetworkManager.GetInstance();
		InventoryManager.GetInstance();
		HangarManager.GetInstance();
		BattleManager.GetInstance();
		FleetManager.GetInstance();
		
	}
	
	Ray mDebugRayInventory;
	Ray mDebugRayHangar;
	Ray mDebugRayGUI;

	void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawLine(mDebugRayInventory.origin, mDebugRayInventory.origin + mDebugRayInventory.direction * 100.0f);
		Gizmos.color = Color.red;
		Gizmos.DrawLine(mDebugRayHangar.origin, mDebugRayHangar.origin + mDebugRayHangar.direction * 100.0f);
	}
	
	void TouchBegin(int fingerID, Vector3 position)
	{
		mFingerPositions[fingerID] = position;

		CheckTouchDown(_GUICamera, fingerID, position, out mDebugRayGUI);
		CheckTouchDown(_InventoryCamera, fingerID, position, out mDebugRayInventory);
		CheckTouchDown(_HangarCamera._RealCamera, fingerID, position, out mDebugRayHangar);

	}

	void CheckTouchDown(Camera cam, int fingerID, Vector3 position, out Ray ray)
	{
		Vector3 touchPos_ = cam.ScreenToWorldPoint(position);
		ray = new Ray(touchPos_ - cam.transform.forward * 50.0f, cam.transform.forward);
		
		SetWorldPos(fingerID, position);
		RaycastHit hit_;

		/*
		RaycastHit [] hits = Physics.RaycastAll(ray, 200, 1 << cam.gameObject.layer);
		for ( int i = 0; i < hits.Length; ++i )
		{
			Debug.Log ("We hit " + hits[i].collider.gameObject.name);
			hits[i].collider.gameObject.SendMessage("OnTouchDown", fingerID, SendMessageOptions.DontRequireReceiver);
		}
		/*/
		if ( Physics.Raycast(ray, out hit_, 200, 1 << cam.gameObject.layer) )
		{
			Debug.Log (cam.name + " hit " + hit_.collider.gameObject.name);
			hit_.collider.gameObject.SendMessage("OnTouchDown", fingerID, SendMessageOptions.DontRequireReceiver);
		}
		//*/
	}
	
	public Vector3 GetPos(int fingerID)
	{
		return mFingerPositions[fingerID];
	}
	
	public Vector3 GetWorldPos(int fingerID)
	{
		return mFingerWorldPositions[fingerID];
	}
	
	public void AttachListner(GameObject who)
	{
		mTouchListeners.Add(who);
	}
	
	public void DetachListener(GameObject who)
	{
		mTouchListeners[mTouchListeners.IndexOf(who)] = null;
	}
	
	void SetWorldPos(int fingerID, Vector3 position)
	{
		Vector3 pointA_ = Camera.main.ScreenToWorldPoint(position);
		Vector3 pointB_ = Camera.main.ScreenToWorldPoint(position + Vector3.forward * 1.0f);
		Vector3 normalized_ = (pointA_ - pointB_).normalized;		
		mFingerWorldPositions[fingerID] = -(normalized_ / normalized_.y) * pointA_.y + pointA_;
		
	}

	void TouchMoved(int fingerID, Vector3 position)
	{
		mFingerPositions[fingerID] = position;
		
		SetWorldPos(fingerID, position);
		
		for ( int i = 0; i < mTouchListeners.Count; ++i )
		{
			if ( mTouchListeners[i] == null )
			{
				mTouchListeners.RemoveAt(i);
				--i;
				continue;
			}
			
			mTouchListeners[i].SendMessage("OnTouchMove", fingerID, SendMessageOptions.DontRequireReceiver);
		}
	}
	
	void TouchEnded(int fingerID, Vector3 position)
	{
		mFingerPositions[fingerID] = position;
		Vector3 pointA_ = Camera.main.ScreenToWorldPoint(position);
		Vector3 pointB_ = Camera.main.ScreenToWorldPoint(position + Vector3.forward * 1.0f);
		Vector3 normalized_ = (pointA_ - pointB_).normalized;		
		mFingerWorldPositions[fingerID] = -(normalized_ / normalized_.y) * pointA_.y + pointA_;
		
		for ( int i = 0; i < mTouchListeners.Count; ++i )
		{
			if ( mTouchListeners[i] == null )
			{
				mTouchListeners.RemoveAt(i);
				--i;
				continue;
			}
			
			mTouchListeners[i].SendMessage("OnTouchUp", fingerID, SendMessageOptions.DontRequireReceiver);
		}
	}

	
	bool mMouseDown = false;
	
	void Update () 
	{
		
#if UNITY_EDITOR
		if ( Input.GetMouseButtonDown(0) )
		{
			TouchBegin(0, Input.mousePosition);
			mMouseDown = true;
		}
		
		if ( Input.GetMouseButtonUp(0) )
		{
			TouchEnded(0, Input.mousePosition);
			mMouseDown = false;
		}
		
		if ( mMouseDown )
		{
			TouchMoved(0, Input.mousePosition);
		}
#else
		for ( int i = 0; i < Input.touches.Length; ++i )
		{
			switch (Input.touches[i].phase)
			{
			case TouchPhase.Began:
				TouchBegin(Input.touches[i].fingerId, Input.touches[i].position);
				break;
			case TouchPhase.Moved:
			case TouchPhase.Stationary:
				TouchMoved(Input.touches[i].fingerId, Input.touches[i].position);
				break;
			case TouchPhase.Ended:
			case TouchPhase.Canceled:
				TouchEnded(Input.touches[i].fingerId, Input.touches[i].position);
				break;
			}
		}
#endif
	}
}
