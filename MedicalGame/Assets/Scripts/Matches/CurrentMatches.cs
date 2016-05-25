﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using Gamedonia.Backend;
using UnityEngine.EventSystems;


public class CurrentMatches : MonoBehaviour {

	public List<Match> invites;
	public List<Match> yourTurn;
	public List<Match> hisTurn;
	public List<Match> finishedMatches;
	public GameObject currentGamesPanel;
	public GameObject accept;
	public GameObject yourTurnUI;
	public GameObject hisTurnUI;

	// Use this for initialization
	void Start () {
		if (MatchManager.I.matches != null && MatchManager.I.matches.Count > 0) {
			StartCoroutine (StartUp ());
		} else {
			MatchManager.I.CheckForInvites ();
		}
	}

	private IEnumerator StartUp() {
		Loader.I.enableLoader ();
		MatchManager.I.checkForUpdateMatches ();
		MatchManager.I.CheckForInvites ();
		while(MatchManager.I.checkUpdates == false) {
			yield return null;
		}
		yield return new WaitForSeconds (0.1f);
		updateMatches ();
		Loader.I.disableLoader ();
	}

	public void LoadMatch(string id) {	
		MatchManager.I.LoadCurrentMatch (id);
	}

	public void updateMatches() {
		//emptyObjects ();
		Debug.Log("Update matches");
		showInvites ();
		showYourTurnGames ();
		showHisTurnGames ();
		showFinishedGames ();
//		if (yourTurn.Count == 0 && hisTurn.Count == 0) {
//			currentGamesPanel.SetActive (false);
//		}

	}

	public void deleteRow(string name) {
		GameObject row = GameObject.Find (name);
		//		Destroy (GameObject.Find (name));
		foreach (Text text in row.GetComponentsInChildren<Text>()) {
			text.DOFade (0, 0.5f);
		}
		foreach (Image img in row.GetComponentsInChildren<Image>()) {
			img.DOFade (0, 0.5f).OnComplete (new TweenCallback(delegate {
				Destroy(row);
				if(invites.Count == 0) {
					accept.SetActive(false);
				}
			}));
		}
	}


	public void emptyObjects() {
		foreach (Transform child in transform) {
			GameObject.Destroy(child.gameObject);
		}
	}

	public void AddInvite(string matchID) {
		Debug.Log (matchID);
		Loader.I.enableLoader ();
		Debug.Log ("Begin");
		accept.transform.GetChild (0).gameObject.SetActive (true);
		string matchId = matchID;
		Match match = MatchManager.I.GetMatch (matchId);
		Debug.Log ("Get MATCH");
		string opponentId = MatchManager.I.GetOppenentId (match);
		Debug.Log ("Get opponentId");
		GameObject matchUI = Instantiate (Resources.Load ("MatchUIInviteRow")) as GameObject;
		Debug.Log ("MatchUIROW");
		setChildInformation (opponentId, matchId, matchUI, "inviteTurn");
		matchUI.name = matchId;
		matchUI.transform.SetParent (accept.transform, false);
		Debug.Log ("SetParent");
		Loader.I.disableLoader ();
	}

	public void showInvites() {
		invites = MatchManager.I.GetPlayingMatches(false, "invite");
		Debug.Log (invites.Count);
		if (invites.Count != 0) {
			Debug.Log (invites [0].m_ID);
			Debug.Log (invites [0].m_ID);
			accept.transform.GetChild(0).gameObject.SetActive (true);
		} else {
			accept.transform.GetChild(0).gameObject.SetActive (false);
		}
		for (int i = 0; i < invites.Count; i++) {
			string matchId = invites [i].m_ID;
			if (!GameObject.Find (matchId)) {
				Match match = MatchManager.I.GetMatch (matchId);
				string opponentId = MatchManager.I.GetOppenentId (match);
				GameObject matchUI = null;
				if (match.m_cp != opponentId) {
					matchUI = Instantiate (Resources.Load ("MatchUIInviteRow")) as GameObject;
					matchUI.transform.GetChild (3).GetComponent<Button> ().onClick.AddListener (delegate {
						MatchManager.I.AcceptMatch (match);
					});
					matchUI.transform.GetChild (4).GetComponent<Button> ().onClick.AddListener (delegate {
						MatchManager.I.DenyMatch (match);
					});

				} else {
					matchUI = Instantiate (Resources.Load ("MatchUIRow")) as GameObject;
				}
				setChildInformation (opponentId, matchId, matchUI, "inviteTurn");
				matchUI.name = matchId;
				matchUI.transform.SetParent (accept.transform, false);
			}
		}
	}

	public void showYourTurnGames() {
		yourTurn = MatchManager.I.GetPlayingMatches(false, "player");
		float delay = 0;
		/************** YOUR TURN MATCHES *******************/
		// Set turn bar above playing matches
		if (yourTurn.Count != 0) {
			yourTurnUI.transform.GetChild(0).gameObject.SetActive (true);
		} else {
			yourTurnUI.transform.GetChild(0).gameObject.SetActive (false);
		}

		// Iterate through playing matches
		Debug.Log("yourturn"+yourTurn.Count);
		for (int i = 0; i < yourTurn.Count; i++)
		{
			string matchId = yourTurn [i].m_ID;
			if (!yourTurnUI.transform.FindChild (matchId) && !GameObject.Find(matchId)) {
				Match match = MatchManager.I.GetMatch (matchId);
				string opponentId = MatchManager.I.GetOppenentId (match);
				GameObject matchUI = Instantiate (Resources.Load ("MatchUIRow")) as GameObject;
				matchUI.SetActive (false);
				// Set all the information, playername, score etc
				setChildInformation (opponentId, matchId, matchUI, "yourTurn", i);

				// Housekeeping
				matchUI.name = matchId;
				matchUI.transform.SetParent (yourTurnUI.transform, false);
				// Set match information
				//			matchUI.GetC  = matchId;


				matchUI.GetComponent<Button> ().onClick.AddListener (delegate {
					LoadMatch (matchId);
				});

				matchUI.GetComponent<RectTransform> ().DOScale (1.1f, .5f).SetEase (Ease.InFlash).SetDelay (delay);
				matchUI.GetComponent<RectTransform> ().DOScale (1, 1f).SetEase (Ease.OutExpo).SetDelay ((.5f + delay));
				delay += .2f;
				if (i != (yourTurn.Count - 1)) {
					matchUI.transform.GetChild (3).gameObject.SetActive (true);
				}
			}  else {
				Match match = MatchManager.I.GetMatch (matchId);
				if(match.m_cp == PlayerManager.I.player.playerID) {
					if (hisTurnUI.transform.Find (matchId)) {
						GameObject matchUI = hisTurnUI.transform.Find (matchId).gameObject;
						string opponentId = MatchManager.I.GetOppenentId (match);
						matchUI.transform.SetParent (yourTurnUI.transform, false);
						setChildInformation (opponentId, matchId, matchUI, "yourTurn", i);
						matchUI.GetComponent<Button> ().onClick.AddListener (delegate {
							LoadMatch (matchId);
						});
					} else if (accept.transform.Find (matchId)) {
						GameObject matchUI = accept.transform.Find (matchId).gameObject;
						string opponentId = MatchManager.I.GetOppenentId (match);
						matchUI.transform.SetParent (yourTurnUI.transform, false);
						setChildInformation (opponentId, matchId, matchUI, "yourTurn", i);
						matchUI.GetComponent<Button> ().onClick.AddListener (delegate {
							LoadMatch (matchId);
						});
					}
				}
			}
		}
	}

	public void showHisTurnGames() {
		hisTurn = MatchManager.I.GetPlayingMatches(false, "opponent");
		float delay = 0;
		/************** HIS TURN MATCHES *******************/
		// Set turn bar above playing matches
		if (hisTurn.Count != 0) {
			hisTurnUI.transform.GetChild(0).gameObject.SetActive (true);
		} else {
			hisTurnUI.transform.GetChild(0).gameObject.SetActive (false);
		}
		// Iterate through playing matches
		for (int i = 0; i < hisTurn.Count; i++)
		{

			string matchId = hisTurn[i].m_ID;
			Debug.Log ("hisTurn" + matchId);
			if (!hisTurnUI.transform.FindChild (matchId) && !GameObject.Find(matchId)) {
				Debug.Log ("hisTurnNotFound");
				Match match = MatchManager.I.GetMatch (matchId);
				string opponentId = MatchManager.I.GetOppenentId (match);
				GameObject matchUI = Instantiate (Resources.Load ("MatchUIRow")) as GameObject;
				matchUI.SetActive (false);
				setChildInformation (opponentId, matchId, matchUI, "hisTurn", i);
				// Housekeeping
				matchUI.name = matchId;
				matchUI.transform.SetParent (hisTurnUI.transform, false);
				// Set match information
				// matchUI.GetC  = matchId;

				matchUI.GetComponent<RectTransform> ().DOScale (1.1f, .5f).SetEase (Ease.InFlash).SetDelay (delay);
				matchUI.GetComponent<RectTransform> ().DOScale (1, 1f).SetEase (Ease.OutExpo).SetDelay ((.5f + delay));
				delay += .2f;
				if (i != (hisTurn.Count - 1)) {
					matchUI.transform.GetChild (3).gameObject.SetActive (true);
				}
			} else {
//				Debug.Log ("testings");
				Match match = MatchManager.I.GetMatch (matchId);

				if (accept.transform.Find (matchId)) {
					GameObject matchUI = accept.transform.Find (matchId).gameObject;
					string opponentId = MatchManager.I.GetOppenentId (match);
					matchUI.transform.SetParent (hisTurnUI.transform, false);
					setChildInformation (opponentId, matchId, matchUI, "hisTurn", i);

				}
//				if(match.m_cp == PlayerManager.I.player.playerID) {
//					GameObject matchUI = GameObject.Find (matchId);
//					Debug.Log ("Set parent");
//					matchUI.transform.SetParent (yourTurnUI.transform);
//					Debug.Log ("Set parent2");
//					string opponentId = MatchManager.I.GetOppenentId (match);
//
//					setChildInformation (opponentId, matchId, matchUI, "yourTurn", i);
//				}
			}
		}
	}

	void showFinishedGames() {
		finishedMatches = MatchManager.I.GetFinishedMatches();
		float delay = 0;
		/************** FINISHED MATCHES *******************/
		// Set turn bar above finished matches
		if (finishedMatches.Count != 0) {
			GameObject finishedBar = Instantiate (Resources.Load ("Finished_Bar")) as GameObject;
			finishedBar.transform.SetParent (this.transform, false);
		}

		// Iterate through finished matches
		for (int i = 0; i < finishedMatches.Count; i++)
		{
			string matchId = finishedMatches [i].m_ID;
			Match match = MatchManager.I.GetMatch (matchId);
			string opponentId = MatchManager.I.GetOppenentId (match);
			GameObject matchUI = Instantiate (Resources.Load ("MatchUIRow")) as GameObject;
			matchUI.SetActive (false);
			setChildInformation (opponentId, matchId, matchUI, "finished", i);
			// Housekeeping
			matchUI.name = matchId;
			matchUI.transform.SetParent (this.transform, false);
			// Set match information
			//			matchUI.GetC  = matchId;


			matchUI.GetComponent<RectTransform>().DOScale (1.1f, .5f).SetEase(Ease.InFlash).SetDelay(delay);
			matchUI.GetComponent<RectTransform>().DOScale (1, 1f).SetEase(Ease.OutExpo).SetDelay((.5f+delay));
			delay += .2f;
			if (i != (finishedMatches.Count-1)) {
				matchUI.transform.GetChild (3).gameObject.SetActive (true);
			}
		}
	}

	private void setChildInformation(string oppId, string matchId, GameObject parent, string listname, int i = 0) {
		if (oppId != "") {
			Match match = MatchManager.I.GetMatch (matchId);
			GamedoniaUsers.GetUser (oppId, delegate (bool success, GDUserProfile data) { 
				if (success) {
					Dictionary<string, object> oppProfile = new Dictionary<string, object> ();
					oppProfile = data.profile;
					foreach (Transform child in parent.transform) {
						if (child.name == "playerName") {
							if (oppId != "") {
								string extraText = "";
								if(match.m_status == "invite" && match.m_cp == oppId) {
									extraText = " (wachten op acceptatie)";
								}
								child.GetComponent<Text> ().text = oppProfile ["name"].ToString ()+extraText;
							}
						}
						if (child.name == "Score") {
							child.GetComponent<Text> ().text = MatchManager.I.getMatchScore (matchId, oppId);
						}
						if (child.name == "line") {
							if (listname == "yourTurn") {
								if (yourTurn.Count == 1 || (yourTurn.Count - 1) == i) {
									child.gameObject.SetActive (false);
								}
							} else if (listname == "hisTurn") {
								if (hisTurn.Count == 1 || (hisTurn.Count - 1) == i) {
									child.gameObject.SetActive (false);
								}
							} else if (listname == "finished") {
								if (finishedMatches.Count == 1 || (finishedMatches.Count - 1) == i) {
									child.gameObject.SetActive (false);
								}
							}
						}
						if (child.name == "rankImg") {
							if (oppId != "") {
								child.GetComponent<Image> ().sprite = PlayerManager.I.GetRankSprite (int.Parse (oppProfile ["lvl"].ToString ()));
							}
						}
					}
					parent.SetActive (true);
				}
			});
		} else {
			foreach (Transform child in parent.transform) {
				if (child.name == "Score") {
					string _score = MatchManager.I.getMatchScore (matchId, oppId);
					child.GetComponent<Text> ().text = (_score == "" ? "0-0" : _score);
				}
			}
			parent.SetActive (true);
		}
	}

}
