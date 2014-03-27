using UnityEngine;
using System.Collections;

public class TierData : MonoBehaviour 
{
	public float _TotalWinCount;
	public float _TotalFightCount;

	public string _ParentName;
	
	bool _Initialized;
	public void Initialize()
	{
		if ( _Initialized )
		{
			return;
		}

		_Initialized = true;
	}

	public string GetStatsLabel()
	{
		return "Your victory coeficient: " + FleetManager.GetShip().GetTierData().GetVictoryCoef();
	}
	
	public void SetStats(bool ownerIsAlive, bool enemyIsAlive)
	{
		++_TotalFightCount;

		// normalize
		if ( _TotalFightCount > 100 )
		{
			_TotalWinCount *= 49.0f / _TotalFightCount;
			_TotalFightCount = 49.0f;
		}

		if ( !ownerIsAlive && !enemyIsAlive )
		{
			//DRAW 
		}
		else if ( ownerIsAlive )
		{
			++_TotalWinCount;
		}

		Backup();
	}
	
	public float GetVictoryCoef()
	{
		float coef_ = _TotalFightCount == 0 ? 0 : (_TotalWinCount / _TotalFightCount);
		return coef_;
	}
	
	public void Backup()
	{
		PlayerPrefs.SetInt(_ParentName+"_GamesTotal", (int)_TotalFightCount);
		PlayerPrefs.SetInt(_ParentName+"_GamesWon", (int)(_TotalWinCount));
	}

	public void Restore()
	{
		_TotalFightCount = PlayerPrefs.GetInt(_ParentName+"_GamesTotal");
		_TotalWinCount = PlayerPrefs.GetInt(_ParentName+"_GamesWon");
	}

	public void DeleteAll()
	{
		_TotalFightCount = 0;
		_TotalWinCount = 0;
		_Initialized = false;
		Initialize();
	}
}
