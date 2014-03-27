using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipScan : MonoBehaviour
{
	public struct PatternPlan
	{
		public PatternPlan(string patternID, Vector3 position)
		{
			mPatternID = patternID;
			mPosition = position;
		}
		
		//public PartManager.Pattern mPattern {get; private set;}
		public string mPatternID;
		public Vector3 mPosition {get; private set;}
	};
	
	public string mName;
	public Vector3 mCenter;
	public int mBoundaryH;
	public int mBoundaryV;
	public int mOffsetH;
	public int mOffsetV;

	List<PatternPlan> mList;

	TierData _TierData;

	public TierData GetTierData()
	{
		if ( _TierData == null )
		{
			_TierData = gameObject.AddComponent<TierData>();
			_TierData._ParentName = mName;
			_TierData.Initialize();
		}
		return _TierData;
	}

	public List <PatternPlan> mPartList {get; private set;}
	
	public ShipScan()
	{
		mPartList = new List<PatternPlan>();
	}

	public float GetAverageRarity()
	{
		float ret_ = 0;
		for ( int i = 0; i < mPartList.Count; ++i )
		{
			ret_ += PartManager.GetInstance().GetRarity(mPartList[i].mPatternID);
		}

		return ret_ / (float)mPartList.Count;
	}
	
	//public void AddPart(Part part)
	public void AddPart(string patternID, Vector3 localPosition)
	{
		//if ( part == null )
		//{
		//	return;
		//}
		//mPartList.Add(new PatternPlan(part.mPattern, part.transform.localPosition));
		//mPartList.Add(new PatternPlan(PartManager.GetInstance().GetPattern(patternID).GetComponent<PatternPlan>(), localPosition));
		mPartList.Add(new PatternPlan(patternID, localPosition));
	}

	public void Backup()
	{
		PlayerPrefs.SetFloat(mName+"_centerX", mCenter.x);
		PlayerPrefs.SetFloat(mName+"_centerY", mCenter.y);
		PlayerPrefs.SetFloat(mName+"_centerZ", mCenter.z);
		
		PlayerPrefs.SetInt(mName+"_boundaryH", mBoundaryH);
		PlayerPrefs.SetInt(mName+"_boundaryV", mBoundaryV);
		PlayerPrefs.SetInt(mName+"_offsetH", mOffsetH);
		PlayerPrefs.SetInt(mName+"_offsetV", mOffsetV);
		
		PlayerPrefs.SetInt(mName+"_partCount", mPartList.Count);
		for ( int i = 0; i < mPartList.Count; ++i )
		{
			PlayerPrefs.SetString(mName+"_part_"+i+"_id", mPartList[i].mPatternID);
			PlayerPrefs.SetFloat(mName+"_part_"+i+"_posX", mPartList[i].mPosition.x);
			PlayerPrefs.SetFloat(mName+"_part_"+i+"_posY", mPartList[i].mPosition.y);
			PlayerPrefs.SetFloat(mName+"_part_"+i+"_posZ", mPartList[i].mPosition.z);
		}

		GetTierData().Backup();
	}

	public void Restore()
	{
		Vector3 center_;
		center_.x = PlayerPrefs.GetFloat(mName+"_centerX");
		center_.y = PlayerPrefs.GetFloat(mName+"_centerY");
		center_.z = PlayerPrefs.GetFloat(mName+"_centerZ");
		mCenter = center_;
		
		mBoundaryH = PlayerPrefs.GetInt(mName+"_boundaryH");
		mBoundaryV = PlayerPrefs.GetInt(mName+"_boundaryV");
		mOffsetH = PlayerPrefs.GetInt(mName+"_offsetH");
		mOffsetV = PlayerPrefs.GetInt(mName+"_offsetV");
		
		int count_ = PlayerPrefs.GetInt(mName+"_partCount");
		for ( int i = 0; i < count_; ++i )
		{
			string id_ = PlayerPrefs.GetString(mName+"_part_"+i+"_id");
			Vector3 pos_;
			pos_.x = PlayerPrefs.GetFloat(mName+"_part_"+i+"_posX");
			pos_.y = PlayerPrefs.GetFloat(mName+"_part_"+i+"_posY");
			pos_.z = PlayerPrefs.GetFloat(mName+"_part_"+i+"_posZ");
			
			AddPart(id_, pos_);
		}
		
		GetTierData().Restore();
	}
}

