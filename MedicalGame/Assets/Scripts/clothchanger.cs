﻿using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using Gamedonia.Backend;
using UnityEngine.SceneManagement;

public class clothchanger : MonoBehaviour {

	public GameObject poppetje;
	//  Array of available customization
	public Mesh[] hairstyles;
	public string[] hairstyleOrder;
	public int[] heads;
	public Mesh[] shirts;
	public string[] shirtOrder;
	public Mesh[] trousers;
	public string[] trouserOrder;
	public int[] trouserMaterials;

	// Root meshes
	public MeshFilter HairstyleMesh;
	public MeshRenderer RootHairstyle;
	public MeshRenderer RootHead;
	public SkinnedMeshRenderer RootShirt;
	public SkinnedMeshRenderer RootTrouser;
	private Mesh firstMesh;
	// Selections
	private int skinCode = 0;
//	private string sexCode = "Male";
	// Keeping track of current selection
	int currentHairstyle = 0;
	int currentHairstyleMaterial = 0;
	int currentShirt = 0;
	int currentShirtMaterial = 0;
	int currentTrouser = 0;
	int currentTrouserMaterial = 0;
	int currentHead  = 0;

	string a_hairstyle = "";
	string a_shirt = "";
	string a_trouser = "";


	// Use this for initialization
	void Start (){
		
		if (PlayerManager.I.changingAvatar) {
			string avatarString = PlayerManager.I.player.avatar;
			string[] avatar = avatarString.Split (new string[] {"_"}, System.StringSplitOptions.None);
			currentHairstyle = DetermineOrder ("hairstyle", int.Parse (avatar [1]));
			currentHairstyleMaterial = DetermineOrder ("hairstyle", int.Parse (avatar [2]), true);
			currentHead = int.Parse (avatar [3]);
			currentShirt = DetermineOrder ("shirt", int.Parse (avatar [4]));
			currentShirtMaterial = DetermineOrder ("shirt", int.Parse (avatar [5]), true);
			currentTrouser = DetermineOrder ("trouser", int.Parse (avatar [6]));
			currentTrouserMaterial = DetermineOrder ("trouser", int.Parse (avatar [7]), true);
		}


			changeHairStyle("skip");
			changeShirt("skip");
			changeTrouser ("skip");
	
	
		
	}

	public int DetermineOrder(string name, int id, bool isMaterial = false) {
		int returnvalue = 0;
		if (name == "hairstyle") {
			for (int i = 0; i < hairstyleOrder.Length; i++) {
				string[] splitCurrentOrder;
				splitCurrentOrder = hairstyleOrder[i].Split (new string[] {"_"}, System.StringSplitOptions.None);
				if (isMaterial) {
					if (int.Parse(splitCurrentOrder [1]) == id) {
						returnvalue = i;
					}
				} else {
					if (int.Parse (splitCurrentOrder [0]) == id) {
						returnvalue = i;
					}
				}
			}
		}


		 else if (name == "shirt") {
			for (int i = 0; i < shirtOrder.Length; i++) {
				string[] splitCurrentOrder;
				splitCurrentOrder = shirtOrder[i].Split (new string[] {"_"}, System.StringSplitOptions.None);
				if (isMaterial) {
					if (int.Parse(splitCurrentOrder [1]) == id) {
						returnvalue = i;
					}
				} else {
					if (int.Parse (splitCurrentOrder [0]) == id) {
						returnvalue = i;
					}
				}
			}
		}

		else if (name == "trouser") {
			for (int i = 0; i < trouserOrder.Length; i++) {
				string[] splitCurrentOrder;
				splitCurrentOrder = trouserOrder[i].Split (new string[] {"_"}, System.StringSplitOptions.None);
				if (isMaterial) {
					if (int.Parse(splitCurrentOrder [1]) == id) {
						returnvalue = i;
					}
				} else {
					if (int.Parse (splitCurrentOrder [0]) == id) {
						returnvalue = i;
					}
				}
			}
		}

	
		return returnvalue;
	}

	public void changeHairStyle(string state) {

		if (state != "skip") {
			SetNextHairstyle (state);
		}

		string currentOrder = hairstyleOrder [currentHairstyle];
		string[] splitCurrentOrder;
		splitCurrentOrder = currentOrder.Split (new string[] {"_"}, System.StringSplitOptions.None);
		a_hairstyle = splitCurrentOrder [0];
		HairstyleMesh.sharedMesh = hairstyles[int.Parse(splitCurrentOrder[0])];

		if (hairstyles [currentHairstyle] != null) {
			string hairstylename = "Hairstyle_"+ splitCurrentOrder[0] + "_" + currentHairstyleMaterial.ToString ();
			RootHairstyle.sharedMaterial = (Material)Resources.Load ("Materials/" + "Avatar" + "/Hairstyles/" + hairstylename, typeof(Material));
		}
	}

	public void changeHead(string state) {

		if (state != "skip") {
			SetNextHead (state);
		}

		string headName = "skin_"+skinCode.ToString() + "_"+currentHead.ToString();

		RootHead.sharedMaterial = (Material)Resources.Load("Materials/"+"Avatar"+"/Heads/"+ headName, typeof(Material));
	}

	public void changeShirt(string state) {

		if (state != "skip") {
			SetNextShirt (state);
		}

		string currentOrder = shirtOrder [currentShirt];
		string[] splitCurrentOrder;
		splitCurrentOrder = currentOrder.Split (new string[] {"_"}, System.StringSplitOptions.None);
		a_shirt = splitCurrentOrder [0];
		RootShirt.sharedMesh = shirts[int.Parse (splitCurrentOrder [0])];

		if (shirts [currentShirt] !=null) {
			string shirtname = "Shirt_"+skinCode.ToString() +"_"+ splitCurrentOrder[0] + "_" + currentShirtMaterial.ToString ();
			RootShirt.sharedMaterial = (Material)Resources.Load ("Materials/" + "Avatar" + "/Shirts/" + shirtname, typeof(Material));
		}
	
	}

	public void changeTrouser(string state) {

		if (state != "skip") {
			SetNextTrouser (state);
		}
		Debug.Log (currentTrouser);
		string currentOrder = trouserOrder [currentTrouser];
		string[] splitCurrentOrder;
		splitCurrentOrder = currentOrder.Split (new string[] {"_"}, System.StringSplitOptions.None);
		a_trouser = splitCurrentOrder [0];
		RootTrouser.sharedMesh = trousers[int.Parse(splitCurrentOrder[0])];

		if (trousers [currentTrouser] != null) {
			Debug.Log (currentTrouserMaterial);
			string trousername = "Trouser_"+ splitCurrentOrder[0] + "_" + currentTrouserMaterial.ToString ();
			Debug.Log (trousername);
			RootTrouser.sharedMaterial = (Material)Resources.Load ("Materials/" + "Avatar" + "/Trousers/" + trousername, typeof(Material));
		}
	}

//	public void changeTrouser(string state) {
//		Debug.Log ("test");
//		if (state != "skip") {
//			SetNextTrouser (state);
//		}
//
//		RootTrouser.sharedMesh = trousers[currentTrouser];
//
//
//		string trouserName = trousers [currentTrouser].name + "_"+currentTrouserMaterial;
//
//		RootTrouser.sharedMaterial = (Material)Resources.Load("Materials/"+sexCode+"/Trousers/"+ trouserName, typeof(Material));
//	}

	private void SetNextShirt(string state) {
		int allShirts = shirts.Length;

		string currentOrder = shirtOrder [currentShirt];
		string[] splitCurrentOrder;
		splitCurrentOrder = currentOrder.Split (new string[] { "_" }, System.StringSplitOptions.None);
		int materialLenght = int.Parse (splitCurrentOrder [1]);


		if (state == "next") {
			if (currentShirtMaterial < (materialLenght - 1)) {
				currentShirtMaterial++;

			} else {
				if (currentShirt < (allShirts - 1)) {
					currentShirt++;
					currentShirtMaterial = 0;
				}
			}
		} else {
			if (currentShirtMaterial > 0) {
				currentShirtMaterial--;
			} else {
				if (currentShirt > 0) {
					currentShirt--;
					string currentOrderBack = shirtOrder [currentShirt];
					string[] splitCurrentOrderBack;
					splitCurrentOrderBack = currentOrderBack.Split (new string[] {"_"}, System.StringSplitOptions.None);
					currentShirtMaterial = (int.Parse (splitCurrentOrderBack [1]) -1);
					Debug.Log ("current material" + currentShirtMaterial);

				}
			}
		}
	}



	private void SetNextHairstyle(string state) {
		int allHairstyles = hairstyles.Length;

		string currentOrder = hairstyleOrder [currentHairstyle];
		string[] splitCurrentOrder;
		splitCurrentOrder = currentOrder.Split (new string[] {"_"}, System.StringSplitOptions.None);
		int materialLength = int.Parse (splitCurrentOrder [1]);

		if (state == "next") {
			if (currentHairstyleMaterial < (materialLength - 1)) {
				currentHairstyleMaterial++;

			} else {
				if (currentHairstyle < (allHairstyles - 1)) {
					currentHairstyle++;
					currentHairstyleMaterial = 0;
				}
			}
		} else {
			if (currentHairstyleMaterial > 0) {
				currentHairstyleMaterial--;
			} else {
				if (currentHairstyle > 0) {
					currentHairstyle--;
					string currentOrderBack = hairstyleOrder [currentHairstyle];
					string[] splitCurrentOrderBack;
					splitCurrentOrderBack = currentOrderBack.Split (new string[] {"_"}, System.StringSplitOptions.None);
					currentHairstyleMaterial = (int.Parse (splitCurrentOrderBack [1]) - 1);
					Debug.Log ("current material" + currentHairstyleMaterial);
				}
			}
		}
	}

	private void SetNextTrouser(string state) {
		int allTrousers = trousers.Length;

		string currentOrder = trouserOrder [currentTrouser];
		string[] splitCurrentOrder;
		splitCurrentOrder = currentOrder.Split (new string[] {"_"}, System.StringSplitOptions.None);
		int materialLength = int.Parse (splitCurrentOrder [1]);
		Debug.Log (materialLength);

		if (state == "next") {
			if (currentTrouserMaterial < (materialLength - 1)) {;
				currentTrouserMaterial++;

			} else {
				if (currentTrouser < (allTrousers - 1)) {
					currentTrouser++;
					currentTrouserMaterial = 0;
				}
			}
		} else {
			if (currentTrouserMaterial > 0) {
				currentTrouserMaterial--;
			} else {
				if (currentTrouser > 0) {
					currentTrouser--;
					string currentOrderBack = trouserOrder [currentTrouser];
					string[] splitCurrentOrderBack;
					splitCurrentOrderBack = currentOrderBack.Split (new string[] {"_"}, System.StringSplitOptions.None);
					currentTrouserMaterial = (int.Parse (splitCurrentOrderBack [1]) - 1);
					Debug.Log ("current material" + currentTrouserMaterial);
				}
			}
		}
	}

	private void SetNextHead(string state) {
		int allHeads = heads [skinCode];

		if (state == "next") {
			if (currentHead < (allHeads-1)) {
				currentHead++;
			}
		} else {
			if (currentHead > 0) {
				currentHead--;
			}
		}
	}


	public void changeSkinCode(int code) {
		skinCode = code;
		currentHead = 0;
		currentShirt = 0;
		changeShirt ("skip");
		changeHead ("skip");
	}

	public void saveAvatar () {
		string avatar = skinCode.ToString() + "_" +
						a_hairstyle + "_"	+
						currentHairstyleMaterial.ToString() + "_"	+
						currentHead.ToString() + "_" +
						a_shirt + "_"	+
						currentShirtMaterial.ToString() + "_"	+
						a_trouser + "_"	+
						currentTrouserMaterial.ToString();
	
		Dictionary<string,object> profile = new Dictionary<string, object>();
		profile.Add ("avatar",avatar);
		PlayerManager.I.player.avatar = avatar;
		GamedoniaUsers.UpdateUser(profile, OnUpdate);

	}
		
	private void OnUpdate (bool success)
	{
		if (success) {
			if (PlayerManager.I.changingAvatar) {
				SceneManager.LoadScene ("Profile"); 
			} else {
				SceneManager.LoadScene ("Introduction"); 

			}

		}


	}
//	int currentHairstyle = 0;
//	int currentHairstyleMaterial = 0;
//	int currentShirt = 0;
//	int currentShirtMaterial = 0;
//	int currentTrouser = 0;
//	int currentTrouserMaterial = 0;
//	int currentHead  = 0;

} 
