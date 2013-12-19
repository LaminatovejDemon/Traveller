using UnityEngine;
using System.Collections;

public class TierData : MonoBehaviour 
{
	int TIER_COUNT = 10;
	
	public int _Tier;
	public int _AchievedRarity = 0;
	public int _TotalGamesCount;
	public float [] _WinCount;
	public float [] _DrawCount;
	public float [] _GamesCount;

	public string _ParentName;
	
	bool _Initialized;
	public void Initialize()
	{
		if ( _Initialized )
		{
			return;
		}

		_WinCount = new float[TIER_COUNT];
		_DrawCount = new float[TIER_COUNT];
		_GamesCount = new float[TIER_COUNT];

		_Initialized = true;
	}

	public int GetTier()
	{
		float elo_ = GetELO();
		if ( elo_ < 11 )
			_Tier = 0;
		else if ( elo_ < 31 )
			_Tier = 1;
		else if ( elo_ < 61 )
			_Tier = 2;
		else if ( elo_ < 101 )
			_Tier = 3;
		else if ( elo_ < 146 )
			_Tier = 4;
		else if ( elo_ < 201 )
			_Tier = 5;
		else if ( elo_ < 301 )
			_Tier = 6;
		else if ( elo_ < 421 )
			_Tier = 7;
		else 
			_Tier = 8;

		return _Tier;
	}



	public void SetStats(bool ownerIsAlive, bool enemyIsAlive, int enemyTier)
	{
		int _tier = GetTier();

		++_GamesCount[enemyTier];
		++_TotalGamesCount;

		if ( !ownerIsAlive && !enemyIsAlive )
		{
			++_DrawCount[enemyTier];
		}
		else if ( ownerIsAlive )
		{
			++_WinCount[enemyTier];
		}
	}
	
	public float GetVictoryCoef(int tier)
	{
		float coef_ = (_WinCount[tier] + 10.0f) / (_GamesCount[tier] + 20.0f - _DrawCount[tier]);
		Debug.Log ("victory coef for iter " + tier + " is " + coef_);
		return coef_;
	}

	public float GetXP()
	{
		float xp_ = (float)(_TotalGamesCount + GetELO());
		
		Debug.Log ("Player has " + xp_ + "XP");
		return xp_;
	}
	
	public float GetELO()
	{
		float sum_ = 0;
		for ( int i = 0; i < TIER_COUNT; ++i )
		{
			sum_ += (GetVictoryCoef(i) - 0.5f) * (i+1);
		}
		
		return sum_ * 100.0f;
	}
	
	public string GetStatsLabel()
	{
		return "TIER WINS: "+_WinCount[0]+"\nTIER DRAWS: "+_DrawCount[0]+"\nBATTLES TOTAL: " + _TotalGamesCount + "\n\nELO:" + GetELO();
	}
	
	public void Backup()
	{
		Debug.Log ("Backing tier data for ship " + _ParentName);
		PlayerPrefs.SetInt(_ParentName+"_AchievedRarity", _AchievedRarity);
		PlayerPrefs.SetInt(_ParentName+"_GamesTotal", _TotalGamesCount);

		for ( int i = 0; i < TIER_COUNT; ++i )
		{
			PlayerPrefs.SetInt(_ParentName+"_GamesWon_"+i, (int)(_WinCount[i]));
			PlayerPrefs.SetInt(_ParentName+"_GamesPlayed_"+i, (int)(_GamesCount[i]));
			PlayerPrefs.SetInt(_ParentName+"_GamesDraw_"+i, (int)(_DrawCount[i]));
		}
	}

	public void Restore()
	{
		Debug.Log ("Restoring tier data for ship " + _ParentName);
		_AchievedRarity = PlayerPrefs.GetInt(_ParentName+"_AchievedRarity");
		_TotalGamesCount = PlayerPrefs.GetInt(_ParentName+"_GamesTotal");

		for ( int i = 0; i < TIER_COUNT; ++i )
		{
			_WinCount[i] = PlayerPrefs.GetInt(_ParentName+"_GamesWon_"+i);
			_GamesCount[i] =  PlayerPrefs.GetInt(_ParentName+"_GamesPlayed_"+i);
			_DrawCount[i] = PlayerPrefs.GetInt(_ParentName+"_GamesDraw_"+i);
		}
	}

	public void DeleteAll()
	{
		_AchievedRarity = 0;
	}
}
