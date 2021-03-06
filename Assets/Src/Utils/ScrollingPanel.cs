using UnityEngine;
using System.Collections;

public class ScrollingPanel : MonoBehaviour 
{
	public GameObject _Container;
	public Transform _ContentContainer;
	
	public Camera _Camera;
	public float _CellSize = 1.0f;
	
	public BoxCollider _BoundaryCollider;
	
	public bool _RefreshNow;
	
	int mDragFinger = -1;
	Vector3 mBeginPosition;
	
	float[]  		mHistoryTimestamp = new float[10];
	Vector3[]		mHistoryPosition = new Vector3[10];
	int mHistoryIndex = 0;

	public void Clear()
	{
		if ( _ContentContainer == null )
		{
			return;
		}

		_ContentContainer.transform.DetachChildren();

	}
	
	public bool IsWithin(Vector3 worldCoordintates)
	{
		Ray checkRay_ = new Ray(worldCoordintates + _BoundaryCollider.transform.rotation * Vector3.up * 3.0f, _BoundaryCollider.transform.rotation * Vector3.down);
		RaycastHit info_;
		if ( Physics.Raycast(checkRay_, out info_, 6.0f, 1<<_BoundaryCollider.gameObject.layer) )
		{
			return info_.collider == _BoundaryCollider;
		}

		return false;
	}
	
	void AlignToCamera()
	{
		transform.rotation = _Camera.transform.rotation;
		transform.position = Camera.main.ViewportToWorldPoint(Vector3.up) + Vector3.right * 1.0f + Vector3.back * 1.0f;
	}

	bool _Initialized = false;
	
	public void Initialize()
	{
		if ( _Initialized )
		{
			return;
		}

		if ( _Container == null )
		{
			_Container = gameObject;
		}
		
		if ( _Camera == null )
		{
			_Camera = Camera.main;
		}
		
		if ( _BoundaryCollider == null )
		{
			_BoundaryCollider = gameObject.AddComponent<BoxCollider>();
			_BoundaryCollider.size = new Vector3(4, 1, Camera.main.orthographicSize * 2);
			_BoundaryCollider.center = new Vector3(1.5f, -1, -2.7f);
			AlignToCamera();	
		}
		else
		{
			_BoundaryCollider.gameObject.AddComponent<TouchForward>()._Target = transform;
		}
		
		_Container.transform.position = _BoundaryCollider.transform.position;
		_Container.transform.rotation = _BoundaryCollider.transform.rotation;
		
		if ( _ContentContainer == null )
		{
			_ContentContainer = new GameObject("ContentContainer").transform;
			_ContentContainer.transform.parent = _Container.transform;
			_ContentContainer.gameObject.layer = _ContentContainer.transform.parent.gameObject.layer;
			_ContentContainer.transform.localPosition = Vector3.zero;
			_ContentContainer.transform.localRotation = Quaternion.identity;
		}

		mTargetContainerPosition = Vector3.up * 0.2f;
		_ContentContainer.transform.localPosition = mTargetContainerPosition;

		_Initialized = true;
	}
	
	void Refresh()
	{
		for ( int i = 0; i < _ContentContainer.childCount; ++i )
		{
			_ContentContainer.GetChild(i).position = _ContentContainer.transform.position + Camera.main.transform.rotation * Vector3.down * i * _CellSize;
		}	

		LimitTargetContainerPosition();
	}
	
	void Update()
	{
		if ( _RefreshNow )
		{
			_RefreshNow = false;
			Refresh();
		}

		Initialize();

		if ( mDragFinger != -1 ) 
		{
			_ContentContainer.transform.localPosition = mTargetContainerPosition;
		}
		else if ( _ContentContainer.transform.localPosition != mTargetContainerPosition )
		{
			_ContentContainer.transform.localPosition = Utils.Interpolate(_ContentContainer.transform.localPosition, mTargetContainerPosition);
		}
	}
	
	public void OnTouchDown(int fingerID)
	{
		if ( mDragFinger != -1 )
		{
			return;
		}
		
		mDragFinger = fingerID;
		mBeginPosition = _ContentContainer.transform.localPosition - MainManager.Instance.GetWorldPos(fingerID) ;
		MainManager.Instance.AttachListner(gameObject);
	}
	
	void SaveToHistory(Vector3 position)
	{
		mHistoryPosition[mHistoryIndex % 10] = position;
		mHistoryTimestamp[mHistoryIndex % 10] = Time.time;
		mHistoryIndex++;
	}
	
	void ClearHistory()
	{
		mHistoryIndex = 0;
	}
	
	float GetSpeedScrolling()
	{
		if ( mHistoryIndex < 1 )
		{
			return 0;
		}
		
		int endAge_ = (mHistoryIndex-1) % 10;
		int beginAge_ = mHistoryIndex > 10 ? (mHistoryIndex) % 10 : 0;
		
		float deltaTime_ = mHistoryTimestamp[endAge_] - mHistoryTimestamp[beginAge_];
		Vector3 deltaPosition_ = (mHistoryPosition[endAge_] - mHistoryPosition[beginAge_]);
		float magnitude_ = deltaPosition_.magnitude * (deltaPosition_.z > 0 ? 1.0f : -1.0f);
		
		return deltaTime_ > 0 ? (magnitude_ / deltaTime_) : 0;
	}
	
	public Vector3 mTargetContainerPosition;
	
	void OnTouchMove(int fingerID)
	{
		if ( mDragFinger != fingerID )
		{
			return;
		}
		mTargetContainerPosition =  mBeginPosition + MainManager.Instance.GetWorldPos(fingerID);
				
		mTargetContainerPosition.x = 0;
		mTargetContainerPosition.y = 0.2f;
		
		SaveToHistory(mTargetContainerPosition);
	}
	
	void LimitTargetContainerPosition()
	{
	//	mTargetContainerPosition.z = (int)(mTargetContainerPosition.z / _CellSize) * _CellSize;

		float lowerCap_ = _BoundaryCollider.bounds.extents.y + _CellSize * (Mathf.Max(_ContentContainer.childCount - 3,0));
		float upperCap_ = -_BoundaryCollider.bounds.extents.y + _CellSize * (Mathf.Min(2, _ContentContainer.childCount - 1)) + 1.0f;

		if ( mTargetContainerPosition.z < upperCap_ )
		{
			mTargetContainerPosition.z = upperCap_;
		}
		else if ( mTargetContainerPosition.z > lowerCap_ )
		{
			mTargetContainerPosition.z =  lowerCap_;
		}
	}
	
	public void StopScrolling()
	{
		if ( mDragFinger != -1 )
		{
			OnTouchUp(mDragFinger);
		}
	}
	
	void OnTouchUp(int fingerID)
	{
		if ( mDragFinger != fingerID)
		{
			return;
		}
		
		mDragFinger = -1;
		MainManager.Instance.DetachListener(gameObject);

		mTargetContainerPosition += Vector3.forward * GetSpeedScrolling() * 0.2f;
		
		LimitTargetContainerPosition();
		ClearHistory();
	}
}
