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
		}
		return mInstance;
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
		Utils.SetTransformCamera(partObject.transform, MainManager.GetInstance()._InventoryCamera, MainManager.GetInstance()._HangarCamera._RealCamera); 

		partObject.name = FleetManager.GetShip().transform.childCount + "_" + partObject.name;
		
		mInventoryList[index] = null;

		//mInventoryList[index] = PartManager.GetInstance().GetPattern(partObject.GetComponent<Part>().mPattern.mID);
		
		//AddCaption(mInventoryList[index].GetComponent<Part>());
		
		SortInventory();
		//BackupInventory();
		MainManager.GetInstance().Backup();
	}

	// We don't want to override backup after restoring first modul of ship and inserting into, that's backup parameter
	void InsertPart(string patternID)
	{
		GameObject new_ = PartManager.GetInstance().GetPattern(patternID);
		
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
		MainManager.GetInstance().Backup();
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
	
	public void FillInventory(int maxRarity)
	{
		ClearInventory();

		int index_ = 0;
		GameObject part_ = null;
		float rarity_;
		
		while ( true)
		{
			rarity_ = PartManager.GetInstance().GetRarity(index_);

			if ( rarity_ == -1 )
			{
				break;
			}

			if ( rarity_ > maxRarity )
			{
				++index_;
				continue;
			}

			/*part_ = PartManager.GetInstance().GetPattern(index_);
			
			if ( part_ == null )
			{
				break;
			}
			
			AddCaption(part_.GetComponent<Part>());
			
			mInventoryList.Add(part_);*/
			InsertPart(PartManager.GetInstance().GetPattern(index_));

			++index_;
		}
		
		SortInventory();
		//BackupInventory();
		MainManager.GetInstance().Backup();
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
		
		/* No caption in inventory
		 * GameObject capName_ = TextManager.GetInstance().GetText(data_, 0.6f);
	 	
		capName_.transform.parent = part.transform;
		capName_.name = "caption";

		capName_.transform.localPosition = Vector3.zero;
		capName_.transform.rotation = Camera.main.transform.rotation;		
		*/
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
}
