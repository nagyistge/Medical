﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[Prefab("MatchManager", true, "")]
public class MatchManager : Singleton<MatchManager> {

	public Matches matchManager;
//	public static MatchManager mInstance = null;

	// Used for debugging, test purposes
	public List<Match> matches;

	public string currentMatchID;
	public int currentCategory;
	public Loader loader;


	public bool Load() {return true;}


	void Start() {
//		matches = new List<Match> ();
//		matchManager = null;
		if (matchManager == null) {
			matchManager = new Matches ();
		}

		LoadMatches ();
		matches = new List<Match> (); 
		matches = matchManager.matches;
	}


	public void StartNewMatch() {
		Loader.Instance.enableLoader ();
		// Generate Match code
		string matchCode = GenerateMatchCode();
		// Store for later use
		currentMatchID = matchCode;
//		// Create match, set player ids, category id
		Match match  = new Match(matchCode, RuntimeData.Instance.LoggedInUser._id, "1", "playing", 1, 1, null);
		AddMatch (match);
//		// Switch to category scene
		Loader.Instance.LoadScene("Category");
	}
		
	private void AddMatch(Match match) {
		matchManager.matches.Add (match);
		Save ();
	}

	public void AddTurn(Turn turn, string match_ID = "") {
		if (match_ID == "") {
			match_ID = currentMatchID;
		}
		Match match = GetMatch (match_ID);
		match.AddTurn (turn);
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

	public int returnTurnId(string id = "") {
		if (id == "") {
			id = currentMatchID;
		}
		Match match = GetMatch (id);
		if (match.m_trns == null) {
			return 0;
		} else {
			return match.m_trns.Count;
		}
	}

	public void returnAllMatches() {
		Debug.Log (matchManager.matches.Count);
	}

	private void clearAllMatches() {
		matchManager.matches = new List<Match> ();
	}

	public void Save() {
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create (Application.persistentDataPath + "/matches.gd");
		bf.Serialize(file, matchManager.matches);

		file.Close();
	}

	public void LoadMatches() {
		if(File.Exists(Application.persistentDataPath + "/matches.gd")) {
			Debug.Log (Application.persistentDataPath + "/matches.gd");
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/matches.gd", FileMode.Open);
			matchManager.matches = (List<Match>)bf.Deserialize(file);
			file.Close();
		}
	}

	private string GenerateMatchCode() {
		int desiredCodeLength = 15;
		string code = "";

		char[] characters =  "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
		while(code.Length < desiredCodeLength) {
			code += characters[Random.Range(0, characters.Length)];
		}
		if (!checkCode(code)) {
			return(GenerateMatchCode ());
		} else {
			return code;
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
		
}