using UnityEngine;
using System.Collections;

public class PlayerData : MonoBehaviour 
{
	int _AchievedRarity = 1;
	public int _PlayerWinCount {get; private set;}
	public int _PlayerDrawCount {get; private set;}
	public int _PlayerGamesCount {get; private set;}

	public void SetStats(bool playerIsAlive, bool enemyIsAlive)
	{
		_PlayerGamesCount++;
		
		if ( !playerIsAlive && !enemyIsAlive )
		{
			_PlayerDrawCount++;
			PopupManager.GetInstance().CreateMessagePopup("DRAW GAME", "Everybody is dead, Dave...\n\n"+GetStatsLabel()); 
		}
		else if ( playerIsAlive )
		{
			_PlayerWinCount++;
			PopupManager.GetInstance().CreateMessagePopup("YOU WON", "Dr. Hildegarde Lanstrom would know.\n\n"+GetStatsLabel());
		}
		else 
		{
			PopupManager.GetInstance().CreateMessagePopup("YOU LOST YOUR SHIP", "...\n\n"+GetStatsLabel());
		}
	}

	public string GetStatsLabel()
	{
		return "WINS TOTAL: "+_PlayerWinCount+"\nDRAWS TOTAL: "+_PlayerDrawCount+"\nBATTLES TOTAL: " + _PlayerGamesCount + "\n\nELO:" + LootManager.GetInstance().GetELO() + "\nXP: " + LootManager.GetInstance().GetXP();
	}

	public void Backup()
	{
		PlayerPrefs.SetInt("Player_AchievedRarity", _AchievedRarity);
		PlayerPrefs.SetInt("Player_GamesWon_"+1, _PlayerWinCount);
		PlayerPrefs.SetInt("Player_GamesPlayed_"+1, _PlayerGamesCount);
		PlayerPrefs.SetInt("Player_GamesDraw_"+1, _PlayerDrawCount);
		InventoryManager.GetInstance().BackupInventory();
	}

	public void Restore()
	{
		_AchievedRarity = PlayerPrefs.GetInt("Player_AchievedRarity");
		_PlayerWinCount = PlayerPrefs.GetInt("Player_GamesWon_"+1);
		_PlayerGamesCount =  PlayerPrefs.GetInt("Player_GamesPlayed_"+1);
		_PlayerDrawCount = PlayerPrefs.GetInt("Player_GamesDraw_"+1);

		if ( _AchievedRarity < 1 )
		{
			_AchievedRarity = 1;
			InventoryManager.GetInstance().FillInventory(_AchievedRarity);
		}
		else
		{
			InventoryManager.GetInstance().RestoreInventory();
		}
	}

	public void DeleteAll()
	{
		FleetManager.GetShip().EraseShip();
		FleetManager.GetInstance().DeleteAllScans();
		PlayerPrefs.DeleteAll();
		_AchievedRarity = 0;

		FleetManager.GetInstance().AddTutorialShip();
		Restore();
	}
}
