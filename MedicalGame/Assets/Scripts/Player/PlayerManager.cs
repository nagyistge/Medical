﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Gamedonia.Backend;

[Prefab("PlayerManager", true, "")]
public class PlayerManager : Singleton<PlayerManager> {

	// Player ranking
	public List<Rank> ranks = new List<Rank>();

	public Player player;
	public Dictionary<string, object> currentOpponentInfo;
	public Dictionary<string, object> friends;
	public bool Load() {return true;}
    public bool lvlUp = false;

	void Awake() {
		
		LoadPlayer ();
		if (friends == null) {
			friends = new Dictionary<string, object>();
		}
		LoadFriends ();
//		player = null;
		if (player == null) {
			player = new Player ();
			Save ();
		} else {
		}
		CheckCurrentRank ();
		CheckLevelUp ();
	}
		

	public void Save() {
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/player.gd");
		bf.Serialize(file, player);
		file.Close();
	}

	public void LoadPlayer() {
		if(File.Exists(Application.persistentDataPath + "/player.gd")) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/player.gd", FileMode.Open);

			Player _player = (Player)bf.Deserialize(file);
			if (_player.loggedIn && _player.createdProfile) {
				player = _player;
			}

			file.Close();
		}
			
	}

	public void LoadFriends() {
		GamedoniaUsers.GetUser(player.playerID, delegate (bool success, GDUserProfile data) { 
			if (success) {
				friends = (Dictionary<string, object>)data.profile["friends"];
			}
		});
	}

	public void changeProfile(PlayerProfile playerprofile) {
		if (player.profile != null) {
			player.profile = playerprofile;
			Save ();
		}

	}

	public void CheckCurrentRank() {
		for (int i = 0; i < ranks.Count; i++) {
			string[] splitScope = ranks [i].levelScope.Split (new string[]{"/"}, System.StringSplitOptions.None);
			int low = int.Parse(splitScope[0]);
			int high = int.Parse(splitScope[1]);

			if(player.playerLvl >= low && player.playerLvl <= high) {
				player.playerRank = ranks [i].name;
			}
		}
	}

	public int CurrentRankKey(int lvl = 0) {
		if (lvl == 0) {
			lvl = player.playerLvl;
		}
		int key = 0;
		for (int i = 0; i < ranks.Count; i++) {
			string[] splitScope = ranks [i].levelScope.Split (new string[]{"/"}, System.StringSplitOptions.None);
			int low = int.Parse(splitScope[0]);
			int high = int.Parse(splitScope[1]);
			if(low < lvl && high > lvl) {
				key = i;
			}
		}
		return key;
	}

	/**
	 * Get experience percentage needed for experience bar
	 * - retrun value is between 0 and 1 where 1 is 100 % and the total of the required experience
	 */
	public float GetExperiencePercentage() {
		// Total experience required in current rank
		float totalXP = ranks [CurrentRankKey ()].reqXP;
		float PSum = (player.playerXP / totalXP) * 1f;

		return PSum;

	}

	public int GetRemainingLevels() {
		// Total experience required in current rank
		string[] splitScope = ranks [CurrentRankKey ()].levelScope.Split (new string[]{"/"}, System.StringSplitOptions.None);

		return int.Parse(splitScope[1]) - player.playerLvl;
	}

	public string GetNextRankName() {
		return ranks [(CurrentRankKey () + 1)].name;
	}

	public Sprite GetRankSprite(int lvl = 0) {
		return ranks [CurrentRankKey(lvl)].sprite;
	}

	public Sprite GetNextRankSprite() {
		return ranks [(CurrentRankKey () + 1)].sprite;
	}

	public void GetPlayerInformationById(string playerID) {;
		GamedoniaUsers.GetUser(playerID, delegate (bool success, GDUserProfile data) { 
			if (success) {
				//returnInformation["name"] = data.profile["name"].ToString();
				currentOpponentInfo = data.profile;
			}
		});
	}

	public Dictionary<string, object> GetPlayerById(string playerID) {
		Dictionary<string, object> returnprofile = new Dictionary<string, object> ();
		GamedoniaUsers.GetUser(playerID, delegate (bool success, GDUserProfile data) { 
			if (success) {
				returnprofile = data.profile;
			}
		});
		return returnprofile;
	}

	public void CheckLevelUp() {
		float neededXP = ranks [CurrentRankKey ()].reqXP;
		// subtract player experience with needed experience
		 float XPSum = neededXP - player.playerXP;
		// We need to level up
		if (XPSum <= 0) {
			// Add 1 to new player level
			player.playerLvl++;
            //setting lvlUp bool to true for popup
            lvlUp = true;
			// Check new ranking
			CheckCurrentRank();
			// Remaining experience;
			player.playerXP = Mathf.Abs (XPSum);
			// Check if we need to level up more then once
			if (player.playerXP > neededXP)
            {
				CheckLevelUp ();
			} 
		}
	}

	public void AddFriend(string name) {
		friends.Add (name, new List<int> ());
		Dictionary<string, object> profile = GetPlayerById (player.playerID);
		profile ["friends"] = friends;
		GamedoniaUsers.UpdateUser (profile);
	}

	private void OnApplicationQuit() { Save (); }

	private void OnApplicationPause() { Save (); }
}
