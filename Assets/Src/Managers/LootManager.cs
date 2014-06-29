using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LootManager : MonoBehaviour 
{
	private static LootManager mInstance = null;
	public static LootManager GetInstance()
	{
		if ( mInstance == null )
		{
			GameObject instanceObject_ = GameObject.Find("#LootManager");
			if ( instanceObject_ != null )
				mInstance = instanceObject_.transform.GetComponent<LootManager>();
			else
				mInstance =  new GameObject("#LootManager").AddComponent<LootManager>();
		}
		return mInstance;
	}

	bool _Initialzed = false;
	List<float> _PatternRarityList = new List<float>();
	List<float> _PatternRatioList = new List<float>();
	List<float> _PatternPercentList = new List<float>();


	public void Initialize()
	{
		if ( _Initialzed )
		{
			return;
		}

		int count_ = PartManager.Instance.GetPatternCount();

		for ( int i = 0; i < count_; ++i )
		{
			_PatternRarityList.Add(PartManager.Instance.GetRarity(i));
			_PatternRatioList.Add(0);
			_PatternPercentList.Add(0);
		}

		_Initialzed = true;

	}

	public void GetLoot(ShipScan scanSource)
	{
		Initialize();

		if ( Random.value > GetLootProbability() )
		{
			return;
		}

		SetLootRatio(scanSource);

		int rollIndex_ = RollIndex();

		GameObject LootPart_ = PartManager.Instance.GetPattern(rollIndex_);

		InventoryManager.Instance.InsertPart(LootPart_);

		PopupManager.GetInstance().CreatePartPopup(LootPart_, LootPart_.GetComponent<Part>().mPattern, "YOU SALVAGED NEW MODULE!");
	}

	int RollIndex()
	{

		for ( int i = 0; i < _PatternPercentList.Count; ++i )
		{
			PartManager.Pattern dbgpart_ = PartManager.Instance.mPatternList[i];
			if ( _PatternPercentList[i] > 0 )
			{
				Debug.Log ("\t" + dbgpart_.mName + "\t(" + dbgpart_.mRarity + ") has ratio " + _PatternPercentList[i]);
			}
		}

		float loot_ = Random.Range(0, 100);
		for ( int i = 0; i < _PatternPercentList.Count; ++i )
		{
			loot_ -= _PatternPercentList[i];
			if ( loot_ < 0 )
			{
				return i;
			}
		}

		Debug.Log ("\t\tRolling BADLY!!!");

		return _PatternPercentList.Count-1;
	}

	void SetLootRatio(ShipScan source)
	{
		float total_ = 0;
		float averageRarity_ = source == null ? 1 : source.GetAverageRarity();
		Debug.Log ("Rolling loot ("+ averageRarity_ +"):");
		for ( int i = 0; i < _PatternRatioList.Count; ++i )
		{
			float val_ = Mathf.Max(0, 50.0f - Mathf.Abs(_PatternRarityList[i] - averageRarity_));
			total_ += val_;
			_PatternRatioList[i] = val_;
		}

		float percentTotal_ = 0;

		for ( int i = 0; i < _PatternRatioList.Count; ++i )
		{
			float val_ = _PatternRatioList[i] * 100.0f / total_;
			percentTotal_ += val_;
			_PatternPercentList[i] = val_;
		}
	}

	float GetLootProbability()
	{
		float lootProbability_ = Mathf.Clamp(1.0f - (InventoryManager.Instance.GetCount() / 10.0f), 0.05f, 1.0f);
		Debug.Log ("Loot probability is " + lootProbability_ + "(" + InventoryManager.Instance.GetCount() + ")");
		return lootProbability_;
	}
}
