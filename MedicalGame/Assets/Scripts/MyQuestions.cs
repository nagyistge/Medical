﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using Gamedonia.Backend;
using System.Collections.Generic;

public class MyQuestions : MonoBehaviour {
	
	private int currentStep = 1;

	public GameObject Smoke1;
	public GameObject Smoke2;
	public Color correctColor;
	public Color inProgressColor;
	public Color rejectedColor;
	private List<Question> questions = new List<Question>();

	// Use this for initialization
	void Start () {

		
		Sequence smoke1 = DOTween.Sequence();
		smoke1.Append(Smoke1.transform.DOMoveY(230, 10f));
		smoke1.PrependInterval(2);
		smoke1.SetLoops(-1);
		smoke1.Insert(0, Smoke1.GetComponent<Image>().DOFade(0, 15f));
		smoke1.Insert(0, Smoke1.transform.DOScale(1.2f, 10f));
		
		Sequence smoke2 = DOTween.Sequence();
		smoke2.Append(Smoke2.transform.DOMoveY(230, 12f));
		smoke2.PrependInterval(4);
		smoke2.SetLoops(-1);
		smoke2.Insert(0, Smoke2.GetComponent<Image>().DOFade(0, 15f));
		smoke2.Insert(0, Smoke2.transform.DOScale(1.4f, 10f));
		
		Loader.I.enableLoader();
		questions = QuestionBackend.I.getQuestionsByPlayerId();
		StartCoroutine(showMyQuestions());
		
	}
	
	private IEnumerator showMyQuestions() {
		while(!QuestionBackend.I.retrievedQuestions) {
			yield return new WaitForSeconds(1f);
		}
		QuestionBackend.I.retrievedQuestions = false;
		if(questions.Count > 0) {
			for(int i =0; i < questions.Count; i++) {
				GameObject questionRow = Instantiate(Resources.Load("QuestionRow")) as GameObject;
				questionRow.name = questions[i].q_Id;
				questionRow.transform.SetParent(this.transform, false);
				GameObject titleParent = questionRow.transform.GetChild(0).transform.GetChild(0).gameObject;
				GameObject content = questionRow.transform.GetChild(1).transform.GetChild(0).gameObject;
				
				titleParent.GetComponentInChildren<Text>().text = questions[i].qT;
				content.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = questions[i].qA;
				content.transform.GetChild(2).GetChild(1).GetComponent<Text>().text = questions[i].qB;
				content.transform.GetChild(3).GetChild(1).GetComponent<Text>().text = questions[i].qC;
				content.transform.GetChild(4).GetChild(1).GetComponent<Text>().text = questions[i].qD;
				if(questions[i].qCA == "A") {
					content.transform.GetChild(1).GetComponent<Image>().color = correctColor;
				} else if(questions[i].qCA == "B") {
					content.transform.GetChild(2).GetComponent<Image>().color = correctColor;
				} else if(questions[i].qCA == "C") {
					content.transform.GetChild(3).GetComponent<Image>().color = correctColor;
				} else if(questions[i].qCA == "D") {
					content.transform.GetChild(4).GetComponent<Image>().color = correctColor;
				}
				content.transform.GetChild(5).GetChild(1).GetComponent<Text>().text = Categories.getCategoryNameById(questions[i].cId);
				if(questions[i].qAp == 0) {
					questionRow.transform.GetChild(0).GetComponent<Image>().color = inProgressColor;
					content.transform.GetChild(6).GetChild(3).gameObject.SetActive(true);
				} else if(questions[i].qAp == 1) {
					questionRow.transform.GetChild(0).GetComponent<Image>().color = correctColor;
					content.transform.GetChild(6).GetChild(2).gameObject.SetActive(true);
				} else {
					questionRow.transform.GetChild(0).GetComponent<Image>().color = rejectedColor;
					content.transform.GetChild(6).GetChild(1).gameObject.SetActive(true);

					content.transform.GetChild(6).GetChild(1).GetChild(1).GetComponent<Button>().onClick.AddListener (delegate {
						QuestionBackend.I.changeQuestion = true;
						QuestionBackend.I.currentQuestion = questions[(i-1)];
						Loader.I.LoadScene("QuestionFactory");
					});
				}
			}
		}
		Loader.I.disableLoader();

	}
}
