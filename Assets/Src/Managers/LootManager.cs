﻿using UnityEngine;
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

		int count_ = PartManager.GetInstance().GetPatternCount();

		for ( int i = 0; i < count_; ++i )
		{
			_PatternRarityList.Add(PartManager.GetInstance().GetRarity(i));
			_PatternRatioList.Add(0);
			_PatternPercentList.Add(0);
		}

		_Initialzed = true;

	}
	
	public float GetELO()
	{
		return MainManager.GetInstance()._PlayerData._PlayerWinCount / MainManager.GetInstance()._PlayerData._PlayerGamesCount;
	}
	
	public float GetXP()
	{
		float xp_ = (float)(MainManager.GetInstance()._PlayerData._PlayerGamesCount+GetELO());

		Debug.Log ("Player has " + xp_ + "XP");
		return xp_;
	}

	public void GetLoot()
	{
		Initialize();

		if ( Random.value > GetLootProbability() )
		{
			return;
		}

		SetLootRatio();

		int rollIndex_ = RollIndex();

		GameObject LootPart_ = PartManager.GetInstance().GetPattern(rollIndex_);

		InventoryManager.GetInstance().InsertPart(LootPart_);

		PopupManager.GetInstance().CreatePartPopup(LootPart_, LootPart_.GetComponent<Part>().mPattern);


	}

	int RollIndex()
	{
		float loot_ = Random.Range(0, 100);
		Debug.Log ("Rolling starts at" + loot_);
		for ( int i = 0; i < _PatternPercentList.Count; ++i )
		{
			loot_ -= _PatternPercentList[i];
			Debug.Log ("\tRemoving " + _PatternPercentList[i] + " to " + loot_);
			if ( loot_ < 0 )
			{
				Debug.Log ("WE'VE GOT AN INDEX!" + i);
				return i;
			}
		}

		Debug.Log ("\t\tRolling BADLY!!!");

		return _PatternPercentList.Count-1;
	}

	void SetLootRatio()
	{
		float total_ = 0;
		for ( int i = 0; i < _PatternRatioList.Count; ++i )
		{
			float val_ = Mathf.Max(0, 0.5f - Mathf.Abs(0.5f - ((GetXP() - _PatternRarityList[i])/GetXP())));
			total_ += val_;
			_PatternRatioList[i] = val_;
		}

		float percentTotal_ = 0;

		for ( int i = 0; i < _PatternRatioList.Count; ++i )
		{
			float val_ = _PatternRatioList[i] * 100.0f / total_;
			percentTotal_ += val_;
			_PatternPercentList[i] = val_;
			Debug.Log("\t" + _PatternRarityList[i] + ": " + _PatternPercentList[i]);
		}

		Debug.Log ("total percent :" + percentTotal_);
	}

	float GetLootProbability()
	{
		float lootProbability_ = Mathf.Min(Mathf.Sqrt(1.0f/(float)MainManager.GetInstance()._PlayerData._PlayerGamesCount), 1.0f);
		Debug.Log ("Loot probability is " + lootProbability_);
		return lootProbability_;
	}
}