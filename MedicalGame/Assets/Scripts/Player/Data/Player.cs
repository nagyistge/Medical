﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Player
{


    /************** Player general data ******************/
    // Player id from gamedonia
    [SerializeField] public string playerID = "";
    // Facebook user id
    [SerializeField] public string fbuserid = "";
    // Player logged in
    [SerializeField] public bool loggedIn = false;
    // Player created profile
    [SerializeField] public bool createdProfile = false;
    //code verification completed
    [SerializeField] public bool verificationComplete = false;

    /************** Player game data *********************/
	[SerializeField] public bool admin;
    [SerializeField] public string avatar = "";
    [SerializeField] public float playerXP = 0;
    [SerializeField] public string playerRank = "";
    [SerializeField] public int playerLvl = 1;
    [SerializeField] public int playedMatches = 0;
    [SerializeField] public int playerWonAttr = 0;
    /************** Player game data without logics *********************/

    // counting TOTAL won matches
    [SerializeField] public int wonMatches = 0;

    // counting won mathes in a ROW
    [SerializeField]public int wonMatchesRow = 0;

    //counting TOTAL active games
    [SerializeField] public int activeGames = 0;

    //counting right answers in a ROW
    [SerializeField] public int rightAnswersRow = 0;

    //counting TOTAl right answers
    [SerializeField] public int rightAnswersTotal = 0;

    //counting TOTAL answers in catagory SPORT
    [SerializeField] public int sportAnswers = 0;

    //counting TOTAL answers in catagory TV & ENTERTAINMENT
    [SerializeField]public int entertainmentAnswers = 0;

    //counting TOTAL answers in catagory HISTORY
    [SerializeField]public int historyAnswers = 0;

    //counting TOTAL answers in catagory GEOGRAPHICS
    [SerializeField]public int geographicAnswers = 0;

    //counting TOTAL answers in catagory CARE
    [SerializeField]public int careAnswers = 0;

    //Counting TOTAL answers in catagory religion
    [SerializeField]public int religionAnswers = 0;

    /************** Player profile data *********************/
    [SerializeField] public PlayerProfile profile;

    [SerializeField]public bool completedIntro = false;

    /************** Player game data without logics new catagories*********************/
    //counting TOTAL answers in catagory Ziekhuiszorg
    [SerializeField]public int ziekenhuisAnswers = 0;

    //Counting TOTAL answers in catagory tand/huisarts
    [SerializeField]public int artsAnswers = 0;

    //Counting TOTAL answers in catagory Gehandicaptenzorg
    [SerializeField]public int gehandicaptenAnswers = 0;

    //Counting TOTAL answers in catagory Ouderenzorg
    [SerializeField]public int oldieAnswers = 0;

    //Counting TOTAL answers in catagory Algemenezorg
    [SerializeField]public int algemeenAnswers = 0;

    //Counting TOTAL answers in catagory GGZ & verslavingszorg
    [SerializeField]public int verslavingsAnswers = 0;

    // counting accepted invited
    [SerializeField]public int acceptedMatches = 0;

    // counting avatar changes
    [SerializeField] public int avatarChanges = 0;

    public Player
        (
             string _playerID = "",
             bool _loggedIn = false,
             bool _createdProfile = false,
             bool _verificationComplete = false,
             float _playerXP = 0,
             string _playerRank = "",
             int _playerLvl = 1,
             int _playedMatches = 0,
             int _wonMatches = 0,
             int _wonMatchesRow = 0,
             int _activeGames = 0,
             int _rightAnswersRow = 0,
             int _rightAnswersTotal = 0,
             int _sportAnswers = 0,
             int _entertainmentAnswers = 0,
             int _historyAnswers = 0,
             int _geographicAnswers = 0,
             int _careAnswers = 0,
             int _religionAnswers = 0,

             int _ziekenhuisAnswers = 0,
             int _artsAnswers = 0,
             int _gehandicaptenAnswers = 0,
             int _oldieAnswers = 0,
             int _algemeenAnswers = 0,
             int _verslavingsAnswers = 0,
             PlayerProfile _profile = null,
             bool _completedIntro = false,
             int _acceptedMatches = 0,
             int _avatarChanges = 0
        )
    {
        playerID = _playerID;
        loggedIn = _loggedIn;
        createdProfile = _createdProfile;
        verificationComplete = _verificationComplete;
        playerXP = _playerXP;
        playerRank = _playerRank;
        playerLvl = _playerLvl;
        playedMatches = _playedMatches;
        completedIntro = _completedIntro;

        /************** Player profile data without logics *********************/
        wonMatches = _wonMatches;
        wonMatchesRow = _wonMatchesRow;
        activeGames = _activeGames;
        rightAnswersRow = _rightAnswersRow;
        rightAnswersTotal = _rightAnswersTotal;
        sportAnswers = _sportAnswers;
        entertainmentAnswers = _entertainmentAnswers;
        historyAnswers = _historyAnswers;
        geographicAnswers = _geographicAnswers;
        careAnswers = _careAnswers;
        religionAnswers = _religionAnswers;

        /************** Player profile data without logics new catagories *********************/
        ziekenhuisAnswers = _ziekenhuisAnswers;
        artsAnswers = _artsAnswers;
        gehandicaptenAnswers = _gehandicaptenAnswers;
        oldieAnswers = _oldieAnswers;
        algemeenAnswers = _algemeenAnswers;
        verslavingsAnswers = _verslavingsAnswers;
        acceptedMatches = _acceptedMatches;
        avatarChanges = _avatarChanges;

        if (_profile != null)
        {
            profile = _profile;
        }
    } 
}//end class Player