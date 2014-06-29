using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryManager : ButtonHandler 
{
	public ScrollingPanel _ScrollingPanel;
	public FrameSlider _ContainerSlider;
	public FrameSlider _ScrollingPanelSlider;
	
	private static InventoryManager mInstance = null;
	public static InventoryManager Instance
	{
		get 
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
	}
	
	List <GameObject> mInventoryList = new List<GameObject>();
	
	bool _Initialized = false;
	public void Initialize()
	{
		if ( _Initialized )
		{
			return;
		}
		
		gameObject.AddComponent<TouchForward>()._Target = _ScrollingPanel.transform;

		_Initialized = true;
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

		Utils.SetLayer(partObject.transform, FleetManager.GetShip().cameraHandler._RealCamera.gameObject.layer);
	
		partObject.name = FleetManager.GetShip().transform.childCount + "_" + partObject.name;

		mInventoryList.RemoveAt(index);

		SortInventory();
		MainManager.Instance.Backup();
	}

	void InsertPart(string patternID)
	{
		GameObject new_ = PartManager.Instance.GetPattern(patternID);

		if ( new_ == null )
		{
			Debug.Log ("ERROR: There is missing FBX for part " + patternID);
		}

		InsertPart(new_);
	}
	
	public void InsertPart(GameObject partObject)
	{
		int index_ = GetFreeIndex();
		
		if ( index_ == -1 )
		{
			mInventoryList.Add(partObject);
		}
		else
		{
			mInventoryList[index_] = partObject;
		}

		partObject.transform.parent = _ScrollingPanel._ContentContainer;

		SortInventory();
		MainManager.Instance.Backup();
	}

	int GetFreeIndex()
	{
		for ( int i = 0; i < mInventoryList.Count; ++i )
		{
			if ( mInventoryList[i] == null )
			{
				return i;
			}
		}

		return -1;
	}
		
	void SortInventory()
	{		
		if ( _ScrollingPanel._ContentContainer == null )
		{
			_ScrollingPanel.Initialize();
		}

		for ( int i = 0; i < mInventoryList.Count; ++i )
		{
			if ( mInventoryList[i] == null )
			{
				continue;
			}

			mInventoryList[i].transform.parent = _ScrollingPanel._ContentContainer;

			Utils.SetLayer(mInventoryList[i].transform, mInventoryList[i].transform.parent.gameObject.layer);
		}
		
		_ScrollingPanel._RefreshNow = true;
	}

	void ClearInventory()
	{
		for ( int i = 0; i <mInventoryList.Count; ++i )
		{
			GameObject.Destroy(mInventoryList[i]);
		}
		mInventoryList.Clear();
		_ScrollingPanel.Clear();
	}

	public int GetCount()
	{
		return mInventoryList.Count;
	}
	
	public void FillInventory(int maxRarity)
	{
		ClearInventory();

		int index_ = 0;
		float rarity_;
		
		while ( true)
		{
			rarity_ = PartManager.Instance.GetRarity(index_);

			if ( rarity_ == -1 )
			{
				break;
			}

			if ( rarity_ > maxRarity )
			{
				++index_;
				continue;
			}

			InsertPart(PartManager.Instance.GetPattern(index_));

			++index_;
		}
		
		SortInventory();
		MainManager.Instance.Backup();
	}
	
	public void SetVisibility(bool state)
	{
		_ContainerSlider.SlideIn = state;
		_ScrollingPanelSlider.SlideIn = state;
	}
	
	void ClearBackup()
	{
		int count_ = PlayerPrefs.GetInt("Player_InventoryCount");
		for ( int i = 0; i < count_; ++i )
		{
			PlayerPrefs.DeleteKey("Player_Inventory_"+i+"_id");
		}

		PlayerPrefs.DeleteKey("Player_InventoryCount");
	}

	public void BackupInventory()
	{
		ClearBackup();

		int count_ = 0;
		for ( int i = 0; i < mInventoryList.Count; ++i )
		{
			if ( mInventoryList[i] == null )
			{
				continue;
			}
			PlayerPrefs.SetString("Player_Inventory_"+count_+"_id", mInventoryList[i].GetComponent<Part>().mPattern.mID);
			count_++;
		}

		PlayerPrefs.SetInt("Player_InventoryCount", count_);
	}

	public void RestoreInventory()
	{
		ClearInventory();
		int count_ = PlayerPrefs.GetInt("Player_InventoryCount");
		for ( int i = 0; i < count_; ++i )
		{
			string patternID_ = PlayerPrefs.GetString("Player_Inventory_"+i+"_id");
			InsertPart(patternID_);
		}
	}

	public override void ButtonPressed (Button target)
	{
		base.ButtonPressed (target);
		
		switch (target._Handle)
		{
		case ButtonHandler.ButtonHandle.CONFIRM:

			FleetManager.GetShip().EraseShip();
			InventoryManager.Instance.FillInventory(1);

			FleetManager.GetShip().GetTierData().DeleteAll();
			FleetManager.Instance.DeleteAllTierData();
			Utils.DestroyParentWindow(target.gameObject);
			break;
			
		case ButtonHandler.ButtonHandle.CANCEL:
			Utils.DestroyParentWindow(target.gameObject);
			break;

		case ButtonHandler.ButtonHandle.INVENTORY_CLEANUP:
			PopupManager.GetInstance().CreateConfirmPopup("ERASE INVENTORY", "Are you sure to ERASE your INVENTORY?", this);
			break;
		}
	}
}
