﻿using UnityEngine;
using System.Collections;
using System;
using Gamedonia.Backend;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class EndScreenScript : MonoBehaviour {


	public Image rankingImg;
	public Text rankingName;
	public Text playerLevel;
	public Text xpPercentage;
	public Image xpImage;

	public Text currentLevel;
	public Text nextLevel;
	public Image timeLine;
	public Image timeLineBranch1;
	public Image timeLineBranch2;
	public Text nextRankIn;
	public Text levelText;
	public Text nextRank;
	public Image nextRankImg;
	public GameObject nextRankInPanel;
	public GameObject nextRankPanel;
    public Text matchScore;
	private float percent;
	
    void Awake()
    {
        PlayerManager.I.player.rightAnswersRow = 0;
        PlayerManager.I.CheckLevelUp();
    }

	void Start() {

        /******** PLAYER INFO*********/
        //set match score
        matchScore.text = (MatchManager.I.getMatchScore(MatchManager.I.currentMatchID)).ToString() + ("-0");
		// Set ranking name info
		rankingName.text = PlayerManager.I.player.playerRank;
		rankingName.DOFade(1, 3f);
		// Set player level info
		playerLevel.text = "Level "+ PlayerManager.I.player.playerLvl.ToString();
		playerLevel.DOFade(1, 3f);
		// Current rank sprite
		rankingImg.sprite = PlayerManager.I.GetRankSprite();
		/******** EXPERIENCE*********/
		percent = PlayerManager.I.GetExperiencePercentage ();
		xpImage.DOFillAmount(xpImage.fillAmount + percent, 2f).SetEase(Ease.OutElastic);
		xpPercentage.text = (percent*100).ToString ("F0")+"%";
		xpPercentage.DOFade (1f, 1f);
		currentLevel.text = PlayerManager.I.player.playerLvl.ToString();
		nextLevel.text = (PlayerManager.I.player.playerLvl+1).ToString();
        
//		StartCoroutine(fillBar(1f));

		/******* TIMELINE ***********/
		timeLine.DOFillAmount(1, 3f);
		// first section
		// Set value
		if(PlayerManager.I.GetRemainingLevels() == 1) {
			levelText.text = "level";
		}
		nextRankIn.text = PlayerManager.I.GetRemainingLevels ().ToString ();
		// Tweening
		foreach (Text text in nextRankInPanel.GetComponentsInChildren<Text>()) {
			text.DOFade (1, 2f).SetDelay(0.3f);
		}
		nextRankInPanel.GetComponentInChildren<Image>().DOFade (1, 2f).SetDelay(0.3f);
		// Second section
		// Set next rank text
		nextRank.DOText (PlayerManager.I.GetNextRankName(), 2f,true,ScrambleMode.All).SetDelay(1.2f);
		nextRankImg.sprite = PlayerManager.I.GetNextRankSprite ();
		// Tweening
		timeLineBranch1.DOFillAmount(1, 1f).SetEase(Ease.InCirc).SetDelay(0.5f);
		timeLineBranch2.DOFillAmount(1, 1f).SetEase(Ease.InCirc).SetDelay(0.5f);
		foreach (Text text in nextRankPanel.GetComponentsInChildren<Text>()) {
			text.DOFade (1, 2f).SetDelay(1.5f);
		}
		nextRankPanel.GetComponentInChildren<Image>().DOFade (1, 2f).SetDelay(1.5f);

	}

	
		

}