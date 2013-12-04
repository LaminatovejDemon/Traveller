using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour 
{
	public ScrollingPanel _ScrollingPanel;
	public FrameSlider _ContainerSlider;
	public FrameSlider _ScrollingPanelSlider;
	
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
			mInstance.Init();
		}
		return mInstance;
	}
	
	List <GameObject> mInventoryList = new List<GameObject>();
	
	bool _Initialized = false;
	public void Init()
	{
		if ( _Initialized )
		{
			return;
		}
		
		gameObject.AddComponent<TouchForward>()._Target = _ScrollingPanel.transform;
		_Initialized = true;
	}
	
	void Start()
	{
		if ( mInstance == null )
		{
			mInstance = this;
			Init ();
		}
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
		
		_ScrollingPanel.StopScrolling();
		
		partObject.transform.parent = null;
		partObject.name = FleetManager.GetShip().transform.childCount + "_" + partObject.name;
		
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
		for ( int i = 0; i < mInventoryList.Count; ++i )
		{
			mInventoryList[i].transform.parent = _ScrollingPanel._ContentContainer;
			//mInventoryList[i].transform.localPosition = (i == 0 ? Vector3.right * 3.0f : mInventoryList[i-1].transform.localPosition + Vector3.back * (mInventoryList[i-1].GetComponent<Part>().GetHeight() + 0.5f) );
		}
		
		_ScrollingPanel._RefreshNow = true;
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
	
	public void SetVisibility(bool state)
	{
		_ContainerSlider.SlideIn = state;
		_ScrollingPanelSlider.SlideIn = state;
	}
	
	public void AddCaption(Part part)
	{		
		PartManager.Pattern pattern_ =  part.mPattern;
		
		//string [] name_ = pattern_.mName.Split('(');
		
		
		string data_ = pattern_.mName/*[0] + "\n(" + name_[1]*/ + "\n PRICE:     " + pattern_.mPrice + " Ff";
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
		
		
		capName_.transform.localPosition = Vector3.zero;
		capName_.transform.rotation = Camera.main.transform.rotation;		
	}
}
