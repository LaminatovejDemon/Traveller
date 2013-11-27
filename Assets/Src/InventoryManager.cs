using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour 
{
	private static InventoryManager mInstance = null;
	public static InventoryManager GetInstance()
	{
		if ( mInstance == null )
		{
			GameObject instanceObject_ = GameObject.Find("#InventoryManager");
			if ( instanceObject_ != null )
				mInstance = instanceObject_.transform.GetComponent<InventoryManager>();
			else
				mInstance =  new GameObject("#InventoryManager").AddComponent<InventoryManager>();
		}
		return mInstance;
	}
	
	List <GameObject> mInventoryList = new List<GameObject>();
	
	Transform mContainer = null;
	
	public bool IsLocated(Vector3 world)
	{		
		bool located_ = collider.bounds.center.x - collider.bounds.extents.x < world.x &&
			collider.bounds.center.x + collider.bounds.extents.x > world.x && 
			collider.bounds.center.z - collider.bounds.extents.z < world.z &&
			collider.bounds.center.z + collider.bounds.extents.z > world.z;
		
		return located_;
	}
	
	public void RetrievePart(Transform partObject)
	{
		int index = -1;
		for ( int i = 0; i < mInventoryList.Count; ++i )
		{
			if ( mInventoryList[i] == partObject.gameObject )
			{
				index = i;
			}
		}
		
		if ( index == -1 )
		{
			Debug.Log("InventoryManager, RetrievePart: Object is not in inventory.");
			return;
		}
		
		partObject.transform.parent = null;
			
		mInventoryList[index] = PartManager.GetInstance().GetPattern(partObject.GetComponent<Part>().mPattern.mID);
		
		AddCaption(mInventoryList[index].GetComponent<Part>());
		
		SortInventory();
	}
	
	public void InsertPart(Transform partObject)
	{
		GameObject.Destroy(partObject.gameObject);
	}
	
	void SortInventory()
	{
		if ( mContainer == null )
		{
			transform.parent = Camera.main.transform;
			transform.localRotation = Quaternion.AngleAxis(90, Vector3.left);
			
			BoxCollider mainBox_ = transform.gameObject.AddComponent<BoxCollider>();
			mainBox_.size = new Vector3(4, 1, Camera.main.orthographicSize * 2);
			mainBox_.center = new Vector3(1.5f, -1, -2.7f);
			Vector3 position_ = Camera.main.ViewportToWorldPoint(Vector3.up) + Vector3.right * 1.0f + Vector3.back * 1.0f;
			position_.y = 0;
			transform.position = position_;
				
			mContainer = new GameObject("#Container").transform;
			mContainer.parent = transform;
			mContainer.transform.localRotation = Quaternion.identity;
			mContainer.transform.localPosition = Vector3.zero;
		}
		
		for ( int i = 0; i < mInventoryList.Count; ++i )
		{
			mInventoryList[i].transform.parent = mContainer;
			mInventoryList[i].transform.localPosition = (i == 0 ? Vector3.right * 3.0f : mInventoryList[i-1].transform.localPosition + Vector3.back * (mInventoryList[i-1].GetComponent<Part>().GetHeight() + 0.5f) );
		}
	}
	
	public void FillInventory()
	{
		int index_ = 0;
		GameObject part_ = null;
		
		while ( true)
		{
			part_ = PartManager.GetInstance().GetPattern(index_);
			
			if ( part_ == null )
			{
				break;
			}
			
			AddCaption(part_.GetComponent<Part>());
			
			mInventoryList.Add(part_);
			++index_;
		}
		
		SortInventory();
	}
	
	public void AddCaption(Part part)
	{		
		PartManager.Pattern pattern_ =  part.mPattern;
		
		string data_ = pattern_.mName + "\n PRICE:     " + pattern_.mPrice + " Ff";
		if ( pattern_.mPower > 0 ) 
		{
			data_ = data_ + "\n GENERATES: " + pattern_.mPower + " Rb";
		}
		else
		{
			data_ += "\n CONSUMES:  " + -pattern_.mPower + " Rb";
		}
		
		GameObject capName_ = TextManager.GetInstance().GetText(data_, 0.6f);
		capName_.transform.parent = part.transform;
		capName_.name = "caption";
		capName_.transform.localPosition = Vector3.right * 0.002f + Vector3.forward * 0.013f;
		capName_.transform.rotation = Camera.main.transform.rotation;		
	}
	
	int mDragFinger = -1;
	Vector3 mBeginPosition;
	
	public void OnTouchDown(int fingerID)
	{
		if ( mDragFinger != -1 )
		{
			return;
		}
		
		mDragFinger = fingerID;
		mBeginPosition = MainManager.GetInstance().GetWorldPos(fingerID) - mContainer.transform.localPosition;
		MainManager.GetInstance().AttachListner(gameObject);
	}
	
	public void OnTouchMove(int fingerID)
	{
		if ( mDragFinger != fingerID )
		{
			return;
		}
		
		Vector3 containerPosition_ =  MainManager.GetInstance().GetWorldPos(fingerID) - mBeginPosition;
		
		containerPosition_.x = 0;
		containerPosition_.y = 0;
		
	//	if ( containerPosition_.z < 0 )
	//	{
	//		containerPosition_.z = 0;
	//	}
		
		
		mContainer.transform.localPosition = containerPosition_;
		
		
	}
	
	public void OnTouchUp(int fingerID)
	{
		if ( mDragFinger != fingerID)
		{
			return;
		}
		
		mDragFinger = -1;
		MainManager.GetInstance().DetachListener(gameObject);
	}
}
