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
		Utils.SetTransformCamera(partObject.transform, MainManager.GetInstance()._InventoryCamera, MainManager.GetInstance()._HangarCamera._RealCamera); 

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
			Utils.SetLayer(mInventoryList[i].transform, mInventoryList[i].transform.parent.gameObject.layer);
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
		
		
		string data_ = pattern_.mName/*[0] + "\n(" + name_[1]*/ + "\n HP: " + pattern_.mHp + " Ff";
		if ( pattern_.mPower > 0 ) 
		{
			data_ = data_ + "\n E: " + pattern_.mPower + " Rb";
		}
		else if ( pattern_.mPower < 0 )
		{
			data_ += "\n E:  " + -pattern_.mPower + " Rb";
		}
		
		GameObject capName_ = TextManager.GetInstance().GetText(data_, 0.6f);
	 	
		capName_.transform.parent = part.transform;
		capName_.name = "caption";
		
		
		capName_.transform.localPosition = Vector3.zero;
		capName_.transform.rotation = Camera.main.transform.rotation;		
	}
}
