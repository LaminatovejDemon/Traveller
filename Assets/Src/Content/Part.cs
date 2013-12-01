using UnityEngine;
using System.Collections;

public class Part : MonoBehaviour 
{
		
	public enum Location
	{
		Inventory,
		Ship,
		Handle,
		Invalid,
	};
	
	public Location mLocation = Location.Inventory;
	int mDragFingerID = -1;
	Vector3 mDragBeginPosition;
	GameObject mHandleShadow = null;
	Transform mCaption = null;
	public float mHP = -1;
	
	Transform _DisabledContainer;
	
	public Transform DisabledContainer
	{
		get 
		{
			if ( _DisabledContainer == null )
			{
				_DisabledContainer = new GameObject().transform;
				_DisabledContainer.transform.parent = transform;
				_DisabledContainer.transform.localPosition = Vector3.zero;
				_DisabledContainer.name = name + "_Disabled";
				_DisabledContainer.gameObject.SetActive(false);
			}
			return _DisabledContainer;
		}
		set 
		{
			_DisabledContainer = value;
		}
	}
		
	
	bool mInitialized = false;
	
	void Initialize()
	{
		if ( mInitialized )
		{
			return;
		}
		
		mHP = mPattern.mHp;
		
		CreateDisabledContainer(transform);
	}
	
	void CreateDisabledContainer(Transform parent)
	{
		for ( int i = 0; i < parent.childCount; ++i )
		{
			CreateDisabledContainer(parent.GetChild(i));
		}
		
		if ( parent.collider != null )
		{
			GameObject new_ = (GameObject)GameObject.Instantiate((Resources.Load("Content/DisabledPart")));
			new_.transform.parent = parent;
			new_.transform.localPosition = new Vector3(-0.5f, 0.6f, -0.5f);
			new_.transform.localRotation = Quaternion.identity;
			new_.transform.parent = DisabledContainer;	
			//DisabledContainer.transform.localPosition = new Vector3(-0.005f, 0.005f, 0.006f);
		}
	}
	
	public PartManager.Pattern mPattern;

	// Use this for initialization
	void Start () 
	{
	 	Initialize();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	public Vector3 GetGunPoint()
	{
		return this.transform.position + this.transform.parent.rotation * new Vector3(-0.5f, 1.0f, -0.5f);
	}
	
	void DestroyComponent<T>(Transform parent)
	{
		for ( int i = 0; i < parent.childCount; ++i )
		{
			DestroyComponent<T>(parent.GetChild(i));
		}
		
		GameObject.Destroy( parent.GetComponent(typeof(T)) );
	}
	
	void SetMaterial(Transform parent, Material mat)
	{
		for ( int i = 0; i < parent.childCount; ++i )
		{
			SetMaterial(parent.GetChild(i), mat);
		}
		
		Renderer rend_ = parent.GetComponent<Renderer>();
		
		if ( rend_ != null )
		{
			rend_.material = mat;
		}
	}
	
	GameObject CreateShadow()
	{
		GameObject ret_ = (GameObject)GameObject.Instantiate(gameObject);
		
		GameObject.Destroy(ret_.GetComponent<Part>());
		
		DestroyComponent<BoxCollider>(ret_.transform);
		DestroyComponent<TouchForward>(ret_.transform);
		Material shadowMat_ = (Material)Resources.Load("ShadowMaterial", typeof(Material));
		SetMaterial(ret_.transform, shadowMat_);
		
		return ret_;
	}
	
	public void SetCaptionVisibility(bool state)
	{
		if ( mCaption == null )
		{
			mCaption = transform.Find("caption");
		}
		
		if ( mCaption != null )
		{
			mCaption.gameObject.SetActive(state);
		}
	}
	
	Location _TouchLocation;
	float _TouchLocationTimeStamp;
	Vector3 _TouchLocationPosition;
	
	void SetTouchLocation(Location at, Vector3 fingerPos)
	{
		_TouchLocation = at;
		_TouchLocationTimeStamp = Time.time;
		_TouchLocationPosition = fingerPos;
	}
	
	void ResetTouchLocation()
	{
		_TouchLocation = Location.Invalid;
		_TouchLocationTimeStamp = -1;
	}
	
	void CheckTouchLocation(Vector3 fingerPos)
	{
		if ( _TouchLocationTimeStamp == -1 )
		{
			return;
		}
		
		if ( (fingerPos - _TouchLocationPosition).magnitude > 0.3f )
		{
			ResetTouchLocation();
			return;
		}
		
		if ( Time.time - _TouchLocationTimeStamp  > 0.2f )
		{
			DragBegin();
		}
	}
	
	bool _Dragging = false;
	
	void DragBegin()
	{
		if ( _Dragging )
		{
			return;
		}
		
		switch ( mLocation )
		{
			case Location.Inventory:
				InventoryManager.GetInstance().RetrievePart(transform);
			break;
			
			case Location.Ship:
				FleetManager.GetShip().RetrievePart(transform);
			break;
			
			default:
			break;
		}
		
		DisabledContainer.gameObject.SetActive(false);	
		mDragBeginPosition = MainManager.GetInstance().GetWorldPos(mDragFingerID) - transform.localPosition;
		mLocation = Location.Handle;
		
		SetCaptionVisibility(true);
		
		if ( mHandleShadow == null )
		{
			mHandleShadow = CreateShadow();
		}
		
		mHandleShadow.transform.parent = null;
		mHandleShadow.SetActive(true);
		
		_Dragging = true;
	}
	
	void DragMove()
	{
		if ( !_Dragging )
		{
			return;
		}
		
		Vector3 localPosition_ = MainManager.GetInstance().GetWorldPos(mDragFingerID) + Vector3.forward * 1.5f + Vector3.left * 0.5f + Vector3.up * 1.0f - mDragBeginPosition;
		transform.localPosition = localPosition_;
		
		
		Vector3 targetPosition_ = localPosition_ - HangarManager.GetInstance()._HangarContainer.transform.localPosition;
		
		Vector3 placementPostiion = mHandleShadow.transform.position;
		
		if ( !FleetManager.GetShip().IsOccupied(mPattern.mHash, targetPosition_, ref placementPostiion) )
		{
			mHandleShadow.transform.position = placementPostiion + HangarManager.GetInstance()._HangarContainer.transform.localPosition;
		}
	}
	
	void DragEnd()
	{
		if ( !_Dragging )
		{
			return;
		}
		
		SetCaptionVisibility(false);
		
		ResetTouchLocation();
		
		transform.position = mHandleShadow.transform.position;
		
		mHandleShadow.SetActive(false);
		mHandleShadow.transform.parent = transform;
		UpdateLocation(InventoryManager.GetInstance()._ScrollingPanel.IsWithin(MainManager.GetInstance().GetWorldPos(mDragFingerID)) ? Location.Inventory : Location.Ship);
		
		_Dragging = false;
	}
	
	void OnTouchDown(int fingerID)
	{
		if ( /*mLocation == Location.Ship ||*/ mDragFingerID != -1 )
		{
			return;
		}
		MainManager.GetInstance().AttachListner(gameObject);
		
		mDragFingerID = fingerID;
		SetTouchLocation(mLocation, MainManager.GetInstance().GetWorldPos(fingerID));	
	}
	
	int mBoundaryHeight = 0;
	
	public void SetHeight(int height)
	{
		mBoundaryHeight = height;
	}
	
	public int GetHeight()
	{
		return mBoundaryHeight;
	}
	
	void OnTouchMove(int fingerID)
	{
		if ( mDragFingerID != fingerID )
		{
			return;
		}
		
		if ( _Dragging )
		{
			DragMove ();
		}
		else
		{
			CheckTouchLocation(MainManager.GetInstance().GetWorldPos(fingerID));
		}
	}
	
	void OnTouchUp(int fingerID)
	{		
		if ( mDragFingerID != fingerID )
		{
			return;
		}
		MainManager.GetInstance().DetachListener(gameObject);
		
		
		if ( _Dragging )
		{
			DragEnd();
		}
		
		ResetTouchLocation();
		
		mDragFingerID = -1;
	}
	
	void UpdateLocation(Location newLocation_)
	{
		if ( newLocation_ == mLocation )
		{
			return;
		}
		
		mLocation = newLocation_;
		
		switch (mLocation)
		{
			case Location.Ship:
				FleetManager.GetShip().InsertPart(transform);
				break;
			case Location.Inventory:
				InventoryManager.GetInstance().InsertPart(transform);
				break;
		}
	}
}
