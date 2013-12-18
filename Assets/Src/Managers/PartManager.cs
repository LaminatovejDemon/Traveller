using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PartManager : MonoBehaviour 
{
	private static PartManager mInstance = null;
	public static PartManager GetInstance()
	{
		if ( mInstance == null )
		{
			GameObject instanceObject_ = GameObject.Find("#PartManager");
			if ( instanceObject_ != null )
				mInstance = instanceObject_.transform.GetComponent<PartManager>();
			else
				mInstance =  new GameObject("#PartManager").AddComponent<PartManager>();
		}
		return mInstance;
	}
	
	GameObject mTemplate;
	string[] mDataFile;
	int mDataIndex;
	
//id	name	description	price	power	hash	hp	weight	integrity model texture	spec1	spec1val	spec2	spec2val
	
	public class Ability
	{
		public Ability(AbilityType type, float val)
		{
			mType = type;
			mValue = val;
		}
		
		public AbilityType mType { get; private set; }
		public float mValue { get; private set; }
	};
		
	public struct Pattern
	{
		
		public Pattern(string id, string name, string description, int rarity, int power, int hash, 
			int hp, int weight, int integrity, string model, string texture, string spec1, int spec1val, string spec2, int spec2val)
		{
			mID = id;
			mName = name;
			mDescription = description;
			mRarity = rarity;
			mPower = power;
			mHash = hash;
			mHp = hp;
			mWeight = weight;
			mIntegrity = integrity;
			mModel = model;
			mTexture = texture;
			
			mAbilityList = new List<Ability>();
			
			AbilityType type_ = PartManager.GetInstance().GetAbilityType(spec1);
			if ( type_ != PartManager.AbilityType.Invalid )
			{
				mAbilityList.Add(new Ability(type_, spec1val));
			}
			
			type_ = PartManager.GetInstance().GetAbilityType(spec2);
			if ( type_ != PartManager.AbilityType.Invalid )
			{
				mAbilityList.Add(new Ability(type_, spec2val));
			}
		}
		
		public List<Ability> mAbilityList;
		
		public Ability GetAbility(AbilityType type)
		{
			for ( int i = 0; i < mAbilityList.Count; ++i )
			{
				if ( mAbilityList[i].mType == type)
				{
					return mAbilityList[i];
				}
			}
			return null;
		}
		
		public string mID {get; private set;}
		public string mName {get; private set;}
		public string mDescription {get; private set;}
		public int mRarity {get; private set;}
		public int mPower {get; private set;}
		public int mHash {get; private set;}
		public int mHp {get; private set;}
		public int mWeight {get; private set;}
		public int mIntegrity {get; private set;}
		public string mModel {get; private set;}
		public string mTexture {get; private set;}
	//	public string mSpec1 {get; private set;}
	//	public int mSpec1val {get; private set;}
	//	public string mSpec2 {get; private set;}
	//	public int mSpec2val {get; private set;}
		
	};
	
	List<Pattern> mPatternList = new List<Pattern>();
	
	public void AddPattern(string id, string name, string description, int rarity, int power, int hash, 
			int hp, int weight, int integrity, string model, string texture, string spec1, int spec1val, string spec2, int spec2val)
	{
		mPatternList.Add(new Pattern(id, name, description, rarity, power, hash, hp, weight, integrity, model, texture,
			spec1, spec1val, spec2, spec2val));
	}
	
	bool mInitialized = false;
	
	public enum AbilityType
	{
		Beam, 			//laser - DMG
		Phaser, 		//laser shooting also inside - DMG
		Tracer, 		//laser with ai
		TorpedoDamage,	  
		TorpedoCount,
		BeamRepeater,	//laser improvement (repeater)
		ShieldRecharge, 
		ShieldCapacity,
		EnginePower,
		Evade,
		AntiTorpedo, 	//probability of torpedo destroy
		CargoSpace,
		Scanner,		
		Mining,
		
		Invalid,
	};
	
	public void Initialize()
	{
		if ( mInitialized )
		{
			return;
		}
		Object dataAsset_ = Resources.Load("Mlist01");
		
		char[] separators = new char[2];
		separators[0] = ';';
		separators[1] = '\n';
		
		mDataFile = ((TextAsset)dataAsset_).text.Split(separators);
		
//		Debug.Log("Datafile has " + mDataFile.Length + " objects");
		
		string id;
		string name; 
		string description; 
		int rarity; 
		int power;
		int hash;
		int hp; 
		int weight; 
		int integrity;
		string model;
		string texture;
		string spec1; 
		int spec1val; 
		string spec2;
		int spec2val; 
		
		mDataIndex = 0;
		
		GetLine(false, out id, out  name, out  description, out rarity, out power, out hash, out hp, out weight,
			out integrity, out model, out texture, out spec1, out spec1val, out spec2, out spec2val);
		
		while ( GetLine(true, out id, out  name, out  description, out rarity, out power, out hash, out hp, out weight,
			out integrity, out model, out texture, out spec1, out spec1val, out spec2, out spec2val) )
		{
			AddPattern(id, name, description, rarity, power, hash, hp, weight, integrity, model, texture, spec1, spec1val, spec2, spec2val);
		}
		
		mTemplate = (GameObject)GameObject.Instantiate(Resources.Load("Modules"));
		mTemplate.transform.parent = transform;
		mTemplate.SetActive(false);
		
		mInitialized = true;
	}
	
	
	
	AbilityType GetAbilityType(string name)
	{
		switch (name)
		{
		case "beam":
			return AbilityType.Beam;
		case "beamrep":
			return AbilityType.BeamRepeater;
		case "phaser":	
			return AbilityType.Phaser;
		case "tracer":
			return AbilityType.Tracer;
		case "torpdam":
			return AbilityType.TorpedoDamage;
		case "torpcoun":
			return AbilityType.TorpedoCount;
		case "srech":
			return AbilityType.ShieldRecharge;
		case "scap":
			return AbilityType.ShieldCapacity;
		case "engipow":
			return AbilityType.EnginePower;
		case "evade":
			return AbilityType.Evade;
		case "antitorp":
			return AbilityType.AntiTorpedo;
		case "cargo":
			return AbilityType.CargoSpace;
		case "scanner":
			return AbilityType.Scanner;
		case "mining":
			return AbilityType.Mining;
		}
		return AbilityType.Invalid;
	}
	
	
	
	bool GetLine(bool relevant, out string id, out string name, out string description, out int rarity, out int power, out int hash, out int hp,
		out int weight, out int integrity, out string model, out string texture, out string spec1, out int spec1val, out string spec2, out int spec2val)
	{
		bool success_ = true;
		
		id = mDataFile[mDataIndex++];
		name = mDataFile[mDataIndex++];
		description = mDataFile[mDataIndex++];
		success_ = int.TryParse(mDataFile[mDataIndex++], out rarity) & success_;
		int.TryParse(mDataFile[mDataIndex++], out power);
		int.TryParse(mDataFile[mDataIndex++], out hash);
		int.TryParse(mDataFile[mDataIndex++], out hp);
		int.TryParse(mDataFile[mDataIndex++], out weight);
		int.TryParse(mDataFile[mDataIndex++], out integrity);
		model = mDataFile[mDataIndex++];
		texture = mDataFile[mDataIndex++];
		spec1 = mDataFile[mDataIndex++];
		int.TryParse(mDataFile[mDataIndex++], out spec1val);
		spec2 = mDataFile[mDataIndex++];
		int.TryParse(mDataFile[mDataIndex++], out spec2val);
			
		
		if ( mDataFile.Length <= mDataIndex+1 || (relevant && !success_) )
		{
			return false;
		}
		
/*		if ( relevant )
		{
			Debug.Log("Got Line" + id + ", " + name + ", " + description + ", " + rarity + "," + power + ","  + hash + ","  
			 + hp + "," + weight + "," + integrity + "," + model + "," + texture + "," + spec1 + "," + spec1val + "," + spec2 + "," + spec2val + ",");   
		}*/
		
		return true;
	}

	public GameObject GetPattern(string id)
	{
		if ( !mInitialized )
		{
			Initialize();
		}
		
		for ( int i = 0; i< mPatternList.Count; ++i )
		{
			if ( mPatternList[i].mID == id )
			{
				return GetPattern(i);
			}
		}
		return null;
	}

	public float GetRarity(int id)
	{
		if ( id < 0 || mPatternList.Count <= id )
		{
			return -1;
		}

		return mPatternList[id].mRarity;
	}
	
	public GameObject GetPattern(int id)
	{
		if ( id < 0 || mPatternList.Count <= id )
		{
			return null;
		}
		
		Pattern wantedPattern_ = mPatternList[id];
		
		GameObject template_ = mTemplate.transform.Find(wantedPattern_.mModel).gameObject;
		if ( template_ == null )
		{
			template_ = (GameObject)Resources.Load("GenericTile");
		}
		
		GameObject container_ = (GameObject)GameObject.Instantiate(template_);
		container_.name = wantedPattern_.mID;
		
		int maxHeight = 0;
		
		for ( int i = 0; i < 6; ++i )
		{
			for ( int j = 0; j < 4; ++j)
			{
				int compareHash_ = 1 << ((j * 6) + i);
				if ( (wantedPattern_.mHash & compareHash_) != 0 )
				{
					if ( j+1 > maxHeight )
					{
						maxHeight = j+1;
					}
					
					GameObject newTile_ = new GameObject();
					BoxCollider box_ = newTile_.AddComponent<BoxCollider>();
					box_.center = Vector3.one * -0.5f + Vector3.up * 1.0f;
					box_.size = Vector3.one;
					
					newTile_.name = "Tile_" + i + "_" + j;
					newTile_.transform.parent = container_.transform;
					newTile_.transform.localPosition = new Vector3(-i, j, 0) / newTile_.transform.parent.localScale.x;
					newTile_.AddComponent<TouchForward>()._Target = container_.transform;
					
				}
			}		
		}
		
		container_.AddComponent<Part>().mPattern = wantedPattern_;
		container_.GetComponent<Part>().SetHeight(maxHeight);
		
		
		return container_;
	}

	public string GetEnergyLabel(Pattern pattern)
	{
		if ( pattern.mPower == 0 )
		{
			return "";
		}

		return pattern.mPower > 0 ? "Produces: " + pattern.mPower + "E\n" : "Consumes: " + (-pattern.mPower) + "E\n"; 
	}

	public string GetEvadeLabel(Pattern pattern)
	{
		Ability evade_ = pattern.GetAbility(AbilityType.Evade);

		if ( evade_ == null )
		{
			return "";
		}
		
		return "Evade Power: " + evade_.mValue + "GWatt\n";
	}

	public string GetHPLabel(Pattern pattern)
	{
		return "Armor: " + pattern.mHp + "HU\n";
	}

	public string GetDamageLabel(Pattern pattern)
	{
		string damage_ = "";
		string amount_ = "";
		string torpedo_ = "";

		Ability beamAbility_ = pattern.GetAbility(AbilityType.Beam);
		Ability torpedoAbility_ = pattern.GetAbility(AbilityType.TorpedoDamage);
		Ability repeaterAbility_ = pattern.GetAbility(AbilityType.BeamRepeater);

		if ( repeaterAbility_ != null )
		{
			amount_ = " (3x)";
		}

		if ( beamAbility_ != null )
		{
			damage_ += beamAbility_.mValue + amount_ + "\n";
		}

		if ( torpedoAbility_ != null )
		{
			damage_ += torpedoAbility_.mValue + amount_ + " (Shield Piercing)\n";
		}

		if ( damage_ == "" )
		{
			return "";
		}

		return "Damage: " + damage_;
	}
}
