using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainManager : MonoBehaviour {
	
	public CameraHandler _BattleCamera;
	public CameraHandler _HangarCamera;
	public CameraHandler _EnemyCamera;
	public Camera _InventoryCamera;
	public Camera _GUICamera;
	public PlayerData _PlayerData;
	
	private static MainManager mInstance = null;
	
	Vector3 [] mFingerWorldPositions = new Vector3[10];
	Vector3 [] mFingerScreenPos = new Vector3[10];

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
		//PlayerPrefs.DeleteAll();

		name = "#MainManager";

		InventoryManager.GetInstance();
		PartManager.GetInstance().Initialize();
		InventoryManager.GetInstance().Initialize();
	//	NetworkManager.GetInstance();
		HangarManager.GetInstance();
		BattleManager.GetInstance();
		FleetManager.GetInstance();

		RestorePlayer();

	}

	bool _BackupDirty = true;

	public void Backup()
	{
		_BackupDirty = true;
	}

	void BackupUpdate()
	{
		if ( _BackupDirty )
		{
			FleetManager.GetShip().BackupShip();
			BackupPlayer();
			_BackupDirty = false;
		}
	}
	
	void BackupPlayer()
	{
		_PlayerData.Backup();
	}

	void RestorePlayer()
	{
		_PlayerData.Restore();
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
		SetScreenPos(fingerID, position);

		CheckTouchDown(_GUICamera, fingerID, position, ref mDebugRayGUI, true);
		CheckTouchDown(_InventoryCamera, fingerID, position, ref mDebugRayInventory, true);
		CheckTouchDown(_HangarCamera._RealCamera, fingerID, position, ref mDebugRayHangar);

	}

	void CheckTouchDown(Camera cam, int fingerID, Vector3 position, ref Ray ray, bool allCast = false)
	{
		if ( cam == null )
		{
			return;
		}

		Vector3 touchPos_ = cam.ScreenToWorldPoint(position);
		ray = new Ray(touchPos_ - cam.transform.forward * 50.0f, cam.transform.forward);

		SetScreenPos(fingerID, position);
		SetWorldPos(fingerID, position);
		RaycastHit hit_;

		if ( allCast )
		{
			RaycastHit [] hits = Physics.RaycastAll(ray, 200, 1 << cam.gameObject.layer);
			for ( int i = 0; i < hits.Length; ++i )
			{
				hits[i].collider.gameObject.SendMessage("OnTouchDown", fingerID, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			if ( Physics.Raycast(ray, out hit_, 200, 1 << cam.gameObject.layer) )
			{
				hit_.collider.gameObject.SendMessage("OnTouchDown", fingerID, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public Vector3 GetWorldPos(int fingerID)
	{
		return mFingerWorldPositions[fingerID];
	}

	public Vector3 GetScreenPos(int fingerID)
	{
		return mFingerScreenPos[fingerID];
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

	void SetScreenPos(int fingerID, Vector3 position)
	{
		mFingerScreenPos[fingerID] = position;
	}

	void TouchMoved(int fingerID, Vector3 position)
	{
		//mFingerPositions[fingerID] = position;

		SetScreenPos(fingerID, position);
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
		SetScreenPos(fingerID, position);
		SetWorldPos(fingerID, position);
		
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
		BackupUpdate();
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
