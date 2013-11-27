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
	
	public PartManager.Pattern mPattern;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
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
	
	void OnTouchDown(int fingerID)
	{
		if ( /*mLocation == Location.Ship ||*/ mDragFingerID != -1 )
		{
			return;
		}
		MainManager.GetInstance().AttachListner(gameObject);
		
		mDragFingerID = fingerID;
		
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
			
		mDragBeginPosition = MainManager.GetInstance().GetWorldPos(fingerID) - transform.localPosition;
		mLocation = Location.Handle;
		
		SetCaptionVisibility(true);
		
		if ( mHandleShadow == null )
		{
			mHandleShadow = CreateShadow();
		}
		
		mHandleShadow.transform.parent = null;
		mHandleShadow.SetActive(true);
		
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
		
		Vector3 localPosition_ = MainManager.GetInstance().GetWorldPos(fingerID) - mDragBeginPosition;
		transform.localPosition = localPosition_;
		
		Vector3 position_ = transform.position;
		position_.y = 1;
		transform.position = position_;
		
		if ( !FleetManager.GetShip().IsOccupied(mPattern.mHash ,position_) )
		{
			mHandleShadow.transform.position = new Vector3((int)(transform.position.x + 1000.5f) - 1000, 0, (int)(transform.position.z + 1000.5f) - 1000);
		}
	}
	
	void OnTouchUp(int fingerID)
	{		
		if ( mDragFingerID != fingerID )
		{
			return;
		}
		MainManager.GetInstance().DetachListener(gameObject);
		
		SetCaptionVisibility(false);
		
		mDragFingerID = -1;
		
		transform.position = mHandleShadow.transform.position;
		
		mHandleShadow.SetActive(false);
		mHandleShadow.transform.parent = transform;
		UpdateLocation(InventoryManager.GetInstance().IsLocated(MainManager.GetInstance().GetWorldPos(fingerID)) ? Location.Inventory : Location.Ship);
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
