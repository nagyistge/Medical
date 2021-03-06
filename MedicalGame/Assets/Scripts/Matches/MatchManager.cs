﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Gamedonia.Backend;
using LitJson_Gamedonia;
using System;

[Prefab("MatchManager", true, "")]
public class MatchManager : Singleton<MatchManager> {

	public Matches matchManager;
	//	public static MatchManager mInstance = null;

	// Used for debugging, test purposes
	public List<Match> matches;
	public string currentMatchID;
	public int currentCategory;
	public bool checkUpdates = false;
	public bool winningMatch = false;
	public bool tie = false;
	public int lastAttributeKey = -1;


	public bool Load() {return true;}

	void Awake() {
		//		matchManager = null;
		if (matchManager == null) {
			matchManager = new Matches ();
		}
		LoadMatches ();
		matches = new List<Match> (); 
		matches = matchManager.matches;
	}
	
		
	public void StartRandomMatch() {
		Loader.I.CheckInternetConnection();
		if(!Loader.I.noInternet) {
			currentMatchID = "";
			currentCategory = 0;
			// Enable Loader
			Loader.I.enableLoader ();
			
			// Search for random fame in random queue tabel
			
			GamedoniaData.Search("randomqueue", "{'uid': {$nin:['"+PlayerManager.I.player.playerID+"'"+getPlayingFriendsIds()+"]}}", delegate (bool success, IList data){
				if (success){
					// If there isnt anyone waiting for a game we start a match then send it as an option to others
					if(data == null || data.Count == 0) {
						Debug.Log("not matches available");

						Match match  = new Match();
						// Add empty player since we dont know against we play yet
						match.AddPlayer("");
						// Set match status to playing
						match.m_status = "waiting";
						// Add ourself
						match.AddPlayer(PlayerManager.I.player.playerID);
						// We are the current player
						match.m_cp = PlayerManager.I.player.playerID;
						// Set current Date
						match.m_date = RuntimeData.I.getCorrectDateTime(System.DateTime.Now);
						//Debug.Log(match.m_date);
						//Debug.Log("random"+DateTime.Parse(match.m_date));
						//string myDate = "30-12-2016 07:50:00:AM";
						//DateTime dt1 = DateTime.ParseExact(myDate, "dd-MM-yyyy hh:mm:ss:tt", System.Globalization.CultureInfo.InvariantCulture);
						// Add match to local list and gamedonia server
						 AddMatch(match, true, false, true);
					} else { 
						// We found a random player, delete entry from randomqueue
						GamedoniaData.Delete("randomqueue",((IDictionary) data[0])["_id"].ToString(), null);

						// Add player to friend list
						if(!PlayerManager.I.friends.ContainsKey(((IDictionary) data[0])["uid"].ToString()) && ((IDictionary) data[0])["uid"].ToString() != "") {
							PlayerManager.I.AddFriend(((IDictionary) data[0])["uid"].ToString());
						}

						GamedoniaData.Search ("matches", "{_id: { $oid: '"+((IDictionary) data[0])["m_ID"].ToString()+"' } }", 1, "{m_trns:1}" , delegate (bool _success, IList match) {
							if (success) {
								if (data != null) {
									Dictionary<string, object> matchD = (Dictionary<string, object>)match[0];
									
									List<Turn> turns = new List<Turn>();
									// Convert incoming turn data to Turn class
									List<object> t_turns = new List<object>();
									t_turns = (List<object>)matchD["m_trns"];
									foreach(Dictionary<string, object> t_turn  in t_turns) {
										Turn turn = new Turn(int.Parse(t_turn["t_ID"].ToString()), t_turn["p_ID"].ToString(), t_turn["q_ID"].ToString(), int.Parse(t_turn["c_ID"].ToString()), int.Parse(t_turn["t_st"].ToString()));
										turns.Add(turn);
									}

									List<string> uids = JsonMapper.ToObject<List<string>>(JsonMapper.ToJson(matchD["u_ids"]));
									uids[0] = PlayerManager.I.player.playerID;
									Match existingMatch = new Match (matchD ["_id"].ToString (), uids, "", "", matchD ["m_status"].ToString (), 0, matchD ["m_cp"].ToString (), 0 , matchD ["m_date"].ToString());
									existingMatch.m_trns = turns;
									existingMatch.m_cp = PlayerManager.I.player.playerID;
									existingMatch.m_status = "playing";
									currentMatchID = existingMatch.m_ID;
									AddMatch(existingMatch, false, false, true);
								} 
							}
						});
					}
				}
			});
		}
		

	}

	public void StartFriendMatch(string friendId) {
		Debug.Log ("STARTING FRIEND MATCH" + friendId);
		Loader.I.enableLoader ();
        //checking for achiement
   
        Match match  = new Match();
		//  Add opponent
		match.AddPlayer(friendId);
		// Add ourself
		match.AddPlayer(PlayerManager.I.player.playerID);
		// Set match status to waiting
		match.m_status = "inviteStart";
		// We are the current player
		match.m_cp = PlayerManager.I.player.playerID;
		// Set current Date
		match.m_date = RuntimeData.I.getCorrectDateTime(System.DateTime.Now);
		Debug.Log("Friend invite"+match.m_date);
		// Add match to local list and gamedonia server
		AddMatch(match, true, false, false);
		Loader.I.LoadScene("Category");
    }

	public void AcceptMatch(Match match) {

        if (PlayerPrefs.GetInt("Beginnende vriendschap") == 0)
        {
            PlayerManager.I.firstInviteAccept = true;
        }
        //add TOTAl accepted matches 
        if (PlayerPrefs.GetInt("Populair") == 0)
        {
            PlayerManager.I.player.acceptedMatches++;
        }
        Loader.I.enableLoader ();
		match.m_status = "playing";
		currentMatchID = match.m_ID;
		Save ();
		Loader.I.LoadScene("Category");
	}

	public void DenyMatch(Match match) {
		match.m_status = "deny";
		match.m_cp = GetOppenentId (match);
		currentMatchID = match.m_ID;
		Dictionary<string, object> matchUpdate = MatchManager.I.getDictionaryMatch (match, "", true);
		GamedoniaData.Update("matches", matchUpdate, delegate (bool success, IDictionary data){
			if (success){
				matches.Remove (match);
				Save ();
				GameObject.FindObjectOfType<CurrentMatches> ().showInvites ();
				GameObject.FindObjectOfType<CurrentMatches> ().deleteRow (match.m_ID);
				GameObject.FindObjectOfType<CurrentMatches> ().checkMatchCount();
				
			} 
			else{
				//TODO Your fail processing
			}
		});

	}

	public void EndMatch() {
		// Set status to finished
		GetMatch(currentMatchID).m_status = "finished";
	}

	public void LoadCurrentMatch(string id) {
		Loader.I.enableLoader ();
		currentMatchID = id;
		Loader.I.LoadScene("Category");
	}

	public void AddMatch(Match match, bool addToServer = true, bool queueObject = false, bool sq = false) {
		if (addToServer) {
			GamedoniaData.Create("matches", getDictionaryMatch (match), delegate (bool success, IDictionary data){
				if (success) {
					match.m_ID = data["_id"].ToString();
					currentMatchID = match.m_ID;
					Save();
					if(queueObject) {
						Debug.Log("create game queue object");
						createGameQueueObject();
					}
				}
				else{

				}
			});
		}
		if (!matches.Contains (match)) {
			matches.Add (match);
			Save ();
			if (sq) {
				Loader.I.LoadScene("Category");
			}
		}
	}

	public void AddTurn(Turn turn, string match_ID = "") {
		if (match_ID == "") {
			match_ID = currentMatchID;
		}
		Match match = GetMatch (match_ID);

		if (match.m_trns == null) {
			match.m_trns = new List<Turn> ();
		
			if (match.m_trns.Count == 0 && match.m_status != "inviteStart") {
				createGameQueueObject ();
			}
		}
		if(!match.m_trns.Contains(turn)) {
			match.AddTurn (turn);
			if (turn.t_st == 0) {
				if (match.m_status != "waiting") {
					match.m_cp = GetOppenentId (match);
				} else {
					match.m_cp = "";
				}
				// Update match to gamedonia
				if(match.m_status == "inviteStart") {
					match.m_status = "invite";
				}
				match.m_date = RuntimeData.I.getCorrectDateTime(System.DateTime.Now);
				Dictionary<string, object> matchUpdate = MatchManager.I.getDictionaryMatch (match, null, true);
				GamedoniaData.Update ("matches", matchUpdate);
			} else {

			}
		}

		// Save locally
		Save ();
	}

	public void ChangeLastTurn(Turn turn, bool finish, bool directlyNinth = false) {
		Debug.Log("In last turn");
		Dictionary<string, object> matchUpdate = null;
		Match match = GetMatch (currentMatchID);
		string winner = "";
		if(directlyNinth) {
			Debug.Log("added last turn");
			AddTurn(turn);
		} else {
			for (int i = 0; i < match.m_trns.Count; i++) {
				if (match.m_trns [i].p_ID == PlayerManager.I.player.playerID && match.m_trns [i].t_ID == turn.t_ID) {
					match.m_trns [i] = turn;
				}
			}
		}

		if (finish) {
			match.m_status = "finished";
			Debug.Log("Set status to finished");
			match.m_cp = GetOppenentId (match);
			winner = getWinner(match);
			// Gamedonia server script handles opponent push message
		}
		if (turn.t_st == 0) {
			match.m_cp = GetOppenentId (match);
		}
		if(finish) {
			matchUpdate = MatchManager.I.getDictionaryMatch (match, null, true, winner);
		} else {
			matchUpdate = MatchManager.I.getDictionaryMatch (match, null, true);
		}
		
		GamedoniaData.Update("matches", matchUpdate);
		Save ();
	}
	
	public void setWinner(Match match, string winner) {
		match.m_status = "finished";
		match.m_cp = winner;
		Dictionary<string, object> matchUpdate =  MatchManager.I.getDictionaryMatch (match, null, true, winner);
		GamedoniaData.Update("matches", matchUpdate);
		Save ();
	}
	
	public string getWinner(Match match = null, bool currentMatch = false) {
		if(currentMatch) {
			match = GetMatch(currentMatchID);
		}
		string winner = "";
		string opponentId = GetOppenentId (match);
		string score = getMatchScore(match.m_ID, opponentId);
		string[] scoreArray = score.Split (new string[] {"-"}, System.StringSplitOptions.None);

		if(int.Parse(scoreArray[0]) > int.Parse(scoreArray[1])) {
			// Player won
			winner = PlayerManager.I.player.playerID;
		} else if (int.Parse(scoreArray[1]) > int.Parse(scoreArray[0])) {
			// It's a tie
			winner = opponentId;
		} else {
			// Opponent won
			winner = "tie";
		}
		
		return winner;
		
	}


	public void clearCurrentCategory() {
		Match match = GetMatch (currentMatchID);
		match.m_cc = 0;
		Save ();
	}

	public Match GetMatch(string match_ID) {

		for (int i = 0; i < matchManager.matches.Count; i++) {
			if (matchManager.matches[i].m_ID == match_ID) {
				return matchManager.matches [i];
			}
		}
		return null;
	}
	
	public bool checkForPlayingWithFriend(string playerId) {
		for (int i = 0; i < matches.Count; i++) {
			string opponentId = GetOppenentId(matches[i]);
			if (matchManager.matches[i].m_status != "finished") {
				if(opponentId == playerId) {
					return true;
				}
		
			}
		}
		return false;
	}
	
	public string getPlayingFriendsIds() {
		string friendsString = "";
		for (int i = 0; i < matches.Count; i++) {
			string opponentId = GetOppenentId(matches[i]);
			if (matchManager.matches[i].m_status != "finished") {
				friendsString+= ",'"+opponentId+"'";
			}
		}
		return friendsString;
	}
	public void RemoveMatch(Match match = null, string matchID = "", bool completely = false, bool deleteInScene = false) {
		if(match == null) {
			if(matchID != "") {
				match = GetMatch(matchID);
			}
		}
		Debug.Log("Removing match");
		currentMatchID = "";
		currentCategory = 0;
		matches.Remove(match);
		if(completely) {
			Debug.Log("Removing match completely");
			GamedoniaData.Delete("matches", match.m_ID, delegate (bool success){ 
				if (success){
					Debug.Log("Removed match!!");
				}
				else{
		
				}
			});
		}
		if(deleteInScene) {
			GameObject.FindObjectOfType<CurrentMatches> ().deleteRow (match.m_ID);
		}
		Save();
	}

	public List<Match> GetPlayingMatches(bool all = false, string type = "player") {
		List<Match> tempList = new List<Match> ();
		string pID = PlayerManager.I.player.playerID;
		if(matches.Count > 0) {
			for (int i = 0; i < matches.Count; i++) {
				// Delete match if it's not online anymore
				if (matches[i].m_status != "finished") {
					
					if (all) {
						Debug.Log("ALL FIRST");
						tempList.Add (matchManager.matches [i]);
					} else {
						if (type == "player") {
							Debug.Log("rgrgrgrgr");
							if (matchManager.matches [i].m_cp == pID) {
								if (matchManager.matches [i].m_status != "invite") {
									tempList.Add (matchManager.matches [i]);
								}
							}
						} else if(type =="opponent") {
							if (matchManager.matches [i].m_cp != pID) {
								if (matchManager.matches [i].m_status != "invite") {
									tempList.Add (matchManager.matches [i]);
								}
							}
						} else if(type =="invite") {
							if(matchManager.matches[i].m_status == "invite") {
								tempList.Add (matchManager.matches [i]);
							}
						}
					}
				}
			}
		}
		return tempList;
	}
	
	public int getTotalActiveMatches() {
		int amount = 0;
		if(matches.Count > 0) {
			for (int i = 0; i < matchManager.matches.Count; i++) {
				if (matchManager.matches[i].m_status != "finished") {
					amount++;
				}
			}
		}
		Debug.Log(amount);
		return amount;
	}
	
	public void getAllMatchesOnStartGame() {
		Debug.Log("get all matches on start game");
		//	GamedoniaData.Search ("matches", "{'user_ids': {$in:['"+PlayerManager.I.player.playerID+"']}}", delegate (bool success, IList data) {
		if(matches.Count == 0) {
			Debug.Log("search matches for user");
			Debug.Log("u_ids:'"+PlayerManager.I.player.playerID+"' }");
			GamedoniaData.Search ("matches", "{u_ids:'"+PlayerManager.I.player.playerID+"' }", delegate (bool success, IList data) {
				if (success) {
					if (data != null) {
						for(int i = 0; i < data.Count; i++) {
							Debug.Log("Add match for user");
							Dictionary<string, object> matchD = (Dictionary<string, object>)data[i];
							
							Match match  = new Match();
							match.m_ID = matchD["_id"].ToString();
							List<string> uids = JsonMapper.ToObject<List<string>> (JsonMapper.ToJson (matchD ["u_ids"]));
							match.u_ids = uids;

							List<Turn> turns = new List<Turn> ();
							List<object> t_turns = new List<object> ();
							t_turns = (List<object>)matchD ["m_trns"];
							foreach (Dictionary<string, object> t_turn  in t_turns) {
								Turn turn = new Turn (int.Parse (t_turn ["t_ID"].ToString ()), t_turn ["p_ID"].ToString (), t_turn ["q_ID"].ToString (), int.Parse (t_turn ["c_ID"].ToString ()), int.Parse (t_turn ["t_st"].ToString ()));
								turns.Add (turn);
							}
							match.m_cp = matchD ["m_cp"].ToString ();
							match.m_trns = turns;
							match.m_date = matchD ["m_date"].ToString();
							match.m_status = matchD ["m_status"].ToString ();
							if(!matches.Contains (match)) {
								AddMatch(match, false, false);
							}
							
						}
						MatchManager.I.checkUpdates = true;
					} else {
						MatchManager.I.checkUpdates = true;
					}
				} else {
					MatchManager.I.checkUpdates = true;
				}
			});
		}
	}
	
	public void checkForUpdateMatches() {
		currentMatchID = "";
		currentCategory = 0;
		List<Match> yourTurn = GetPlayingMatches (true, "opponent");
		if (yourTurn.Count > 0) {

			for (int i = 0; i < yourTurn.Count; i++) {
				string matchID = yourTurn [i].m_ID;
				checkIfMatchExists(matchID);
				if (yourTurn [i].m_status != "finished" ) {
					GamedoniaData.Search ("matches", "{_id: { $oid: '" + matchID + "' } }", delegate (bool success, IList data) {
						if (success) {
							if (data != null) {
								Match match = GetMatch (matchID);
								Dictionary<string, object> matchD = (Dictionary<string, object>)data [0];
								Debug.Log(matchD ["m_status"].ToString ());
								if(matchD ["m_status"].ToString () == "deny") {
									RemoveMatch(match, "", true, true);
								} else {
									List<string> uids = JsonMapper.ToObject<List<string>>(JsonMapper.ToJson(matchD["u_ids"]));

									// Add friend if it is a new player
									string oppId = (match.u_ids[0] != PlayerManager.I.player.playerID ? uids[0] : uids[1]);

									if (!PlayerManager.I.friends.ContainsKey (oppId) && oppId != "") {
										PlayerManager.I.AddFriend (oppId);
									}

									// Update match if we are the currentplayer
									if (matchD ["m_cp"].ToString () == PlayerManager.I.player.playerID && oppId != "" || matchD ["m_status"].ToString () == "finished") {
										if (match.u_ids [0] == "") {
											match.u_ids [0] = uids [0];
										}

										List<Turn> turns = new List<Turn> ();
										List<object> t_turns = new List<object> ();
										t_turns = (List<object>)matchD ["m_trns"];
										foreach (Dictionary<string, object> t_turn  in t_turns) {
											Turn turn = new Turn (int.Parse (t_turn ["t_ID"].ToString ()), t_turn ["p_ID"].ToString (), t_turn ["q_ID"].ToString (), int.Parse (t_turn ["c_ID"].ToString ()), int.Parse (t_turn ["t_st"].ToString ()));
											turns.Add (turn);
										}
										match.m_cp = matchD ["m_cp"].ToString ();
										match.m_trns = turns;
										match.m_date = matchD ["m_date"].ToString();
										match.m_status = matchD ["m_status"].ToString ();
										Save ();
										checkUpdates = true;
									} else {
										checkUpdates = true;
									}
								}
							}
						}
						checkUpdates = true;
					});
				}
				checkUpdates = true;
			} 
			checkUpdates = true;
		} else {
			checkUpdates = true;
		}
	}

	public void CheckForInvites() {
		Debug.Log ("CHECK FOR INVITES");
		GamedoniaData.Search ("matches", "{$and: [ { \"m_status\":\"invite\" }, { \"m_cp\":'"+PlayerManager.I.player.playerID+"' }]}", delegate (bool success, IList data) {
			if (success) {
				if (data != null) {
					for(int i = 0; i < data.Count; i++) {
						Dictionary<string, object> matchD = (Dictionary<string, object>)data[i];
						Match match = GetMatch(matchD["_id"].ToString());
						if(match == null) {
							match  = new Match();
							match.m_ID = matchD["_id"].ToString();
							List<string> uids = JsonMapper.ToObject<List<string>> (JsonMapper.ToJson (matchD ["u_ids"]));
							match.u_ids = uids;

							List<Turn> turns = new List<Turn> ();
							List<object> t_turns = new List<object> ();
							t_turns = (List<object>)matchD ["m_trns"];
							foreach (Dictionary<string, object> t_turn  in t_turns) {
								Turn turn = new Turn (int.Parse (t_turn ["t_ID"].ToString ()), t_turn ["p_ID"].ToString (), t_turn ["q_ID"].ToString (), int.Parse (t_turn ["c_ID"].ToString ()), int.Parse (t_turn ["t_st"].ToString ()));
								turns.Add (turn);
							}
							match.m_cp = matchD ["m_cp"].ToString ();
							match.m_trns = turns;
							match.m_date = matchD ["m_date"].ToString();
							match.m_status = matchD ["m_status"].ToString ();
							AddMatch(match, false, false);
						}
					}
				}
			} 
		});
	}

	public List<Match> GetFinishedMatches() {
		List<Match> tempList = new List<Match> ();
		int amount = 0;
		for (int i = 0; i < matches.Count; i++) {
			if (matches[i].m_status == "finished") {
				amount++;
				if(amount > 3) {
					RemoveMatch(matches[i],"",true);
				} else {
					tempList.Add(matchManager.matches[i]);
				}
			}
		}
		return tempList;
	}

	public string getMatchScore(string id, string oppId) {
		string score = "";
		int playerScore = 0;
		int oppScore = 0;
		Match match = GetMatch (id);
		if(match.m_trns !=null && match.m_trns.Count > 0) {
			List<Turn> playerTurnL = GetMatchTurnsByPlayerID (PlayerManager.I.player.playerID, match);
			List<Turn> oppTurnL = GetMatchTurnsByPlayerID (MatchManager.I.GetOppenentId(match), match);
			if (match.m_trns != null) {
				for (int i = 0; i < playerTurnL.Count; i++) {
					if (playerTurnL [i].t_st == 1) {
						playerScore++;
					}
				}
				if (oppId != "" && oppTurnL.Count > 0 && oppTurnL != null) {
					for (int i = 0; i < oppTurnL.Count; i++) {
						if (oppTurnL [i].t_st == 1) {
							oppScore++;
						}
					}
				}
				score = playerScore.ToString ()+ "-" +oppScore.ToString ();
			}
		} 
		return score;
	}


	public List<Turn> GetMatchTurnsByPlayerID(string player_ID, Match match = null, string match_ID = "") {
		List<Turn> returnTurns = new List<Turn> ();
		if(match == null && match_ID != "") {
			match = GetMatch (match_ID);
		}
			
		if (match.m_trns != null) {
			for (int i = 0; i < match.m_trns.Count; i++) {
				if (match.m_trns [i].p_ID == player_ID) {
					returnTurns.Add (match.m_trns [i]);
				}
			}
			return returnTurns;
		} else {
			return null;
		}
	}

	public string GetOppenentId(Match match = null, string match_ID = "") {
		if (match == null && match_ID != "") {
			match = GetMatch (match_ID);
		} 
		for (int i = 0; i < match.u_ids.Count; i++) {

		}
		return (match.u_ids[0] != PlayerManager.I.player.playerID ? match.u_ids[0] : match.u_ids[1]);
	}

	public string[] GetQuestionsInMatch(string mId = "") {
		List<Turn> turns = new List<Turn>();
		string[] questionIds = new string[1];
		questionIds[0] = "none";
		
		if(mId != "") {
			turns = GetMatchTurnsByPlayerID(PlayerManager.I.player.playerID, null, mId);
		} else {
			turns = GetMatchTurnsByPlayerID(PlayerManager.I.player.playerID, null, currentMatchID);
		}
		
		if(turns != null && turns.Count > 0) {
			questionIds = new string[turns.Count];
			
			if (turns != null) {
				for (int i = 0; i < turns.Count; i++) {
					questionIds[i] = turns [i].q_ID;
				}
			}
		}

		return questionIds;
	}

	public List<Match> returnAllMatches() {
		return matches;
	}

	private void clearAllMatches() {
		matchManager.matches = new List<Match> ();
	}

	public void Save() {
		BinaryFormatter bf = new BinaryFormatter();

		FileStream file = File.Create(Application.persistentDataPath + "/matches.gd");
		bf.Serialize(file, matchManager.matches);

		file.Close();
	}

	public void LoadMatches() {
		if(File.Exists(Application.persistentDataPath + "/matches.gd")) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/matches.gd", FileMode.Open);
			matchManager.matches = (List<Match>)bf.Deserialize(file);
			file.Close();
		}
	}
	
	public void RemoveLocalMatches() {
		if(File.Exists(Application.persistentDataPath + "/matches.gd")) {
			File.Delete( Application.persistentDataPath + "/matches.gd" );
			matches = null;
			matchManager = new Matches ();
			matches = new List<Match> (); 
			matches = matchManager.matches;
		}
	}
	private bool checkCode(string code) {
		for (int i = 0; i < matchManager.matches.Count; i++) {
			if (matchManager.matches[i].m_ID == code) {
				return false;
			}
		}
		return true;
	}

	public Dictionary<string, object> getDictionaryMatch(Match match = null, string m_id = "", bool update = false, string playerWon = "") {
		Dictionary<string, object> dicMatch = new Dictionary<string, object> ();

		if (match == null) {
			match = GetMatch (m_id); 
		}
		// If we need to update we need to store the id in the dictionary
		if (update) {
			dicMatch ["_id"] = match.m_ID;
		}
		dicMatch ["u_ids"] = match.u_ids;
		dicMatch ["m_status"] = match.m_status;
		dicMatch ["m_cp"] = match.m_cp;
		dicMatch ["m_date"] = match.m_date;
		dicMatch ["m_trns"] = match.m_trns;
		if(playerWon != "") {
			dicMatch ["m_won"] = playerWon;
		}

		return dicMatch;

	}
	// *********************** GAMEDONIA SERVER FUNCTIONS*******************************************
	private void createGameQueueObject() {
		Dictionary<string,object> randomGame = new Dictionary<string,object>();

		randomGame.Add("uid", PlayerManager.I.player.playerID);
		randomGame.Add("nickname", PlayerManager.I.player.profile.name);
		randomGame.Add("p_ID", PlayerManager.I.player.playerID);
		randomGame.Add("m_ID", currentMatchID);

		GamedoniaData.Create ("randomqueue", randomGame);
	}
	
	private void checkIfMatchExists(string matchId) {
		
		GamedoniaData.Count("matches", "{_id:{$oid:'"+matchId+"'}}", delegate (bool success, int count){
			if (success){
				Debug.Log("matches"+count);
				if(count == 0) {
					RemoveMatch(null, matchId);
				}
			}
			else {
				//TODO Your fail processing
			}
		});
	}


}