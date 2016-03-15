﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[Prefab("MatchManager", true)]
public class MatchManager : Singleton<MatchManager> {

	public Matches matchManager;
//	public static MatchManager mInstance = null;

	// Used for debugging, test purposes
	public List<Match> matches;

	[SerializeField]
	public string currentMatchID;
	public int currentCategory;

	// Use this for initialization
//	void Awake () {
//
//		if(instance == null)
//		{
//			instance = this;
//			DontDestroyOnLoad(this.gameObject);
//			return;
//		}
//		Destroy(this.gameObject);
//
//
//	}

	public bool Load() {Debug.Log ("test"); return true;}

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
		// Generate Match code
		string matchCode = GenerateMatchCode();
		// Store for later use
		currentMatchID = matchCode;
		// Create match, set player ids, category id
		Match match  = new Match(matchCode, 1, 1, "playing", 1, 1);
		AddMatch (match);
		// Switch to category scene
		SceneManager.LoadScene("Category");

	}

	private void AddMatch(Match match) {
		matchManager.matches.Add (match);
		Save ();
	}

	public void AddTurn(Turn turn, string match_ID) {
		Match match = GetMatch (match_ID);
		match.AddTurn (turn);
	}


	public Match GetMatch(string match_ID) {
		for (int i = 0; i < matchManager.matches.Count; i++) {
			if (matchManager.matches[i].m_ID == match_ID) {
				return matchManager.matches [i];
			}
		}
		return null;
	}

	public void returnAllMatches() {
		Debug.Log (matchManager.matches.Count);
//		Debug.Log (this.matchManager.matches.Count);
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