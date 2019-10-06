using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PriestsAndDevils;

public class FirstController : MonoBehaviour, SceneController, UserAction {

	readonly Vector3 water_pos = new Vector3(0,0,0);
	readonly float speed = 20;

	UserGUI userGUI;
	Judger judge;
	public bankController frombank;
	public bankController tobank;
	public BoatController boat;
	private MyCharacterController[] characters;
	public ActionManager actionManager;
	void Awake() {
		Director director = Director.getInstance ();
		director.currentSceneController = this;
		userGUI = gameObject.AddComponent<UserGUI>() as UserGUI;
		characters = new MyCharacterController[6];
		loadResources ();
		actionManager = gameObject.AddComponent<ActionManager>() as ActionManager;
	}

	public void loadResources() {
		GameObject water = Instantiate (Resources.Load ("River", typeof(GameObject)), water_pos, Quaternion.identity, null) as GameObject;
		water.name = "river";

		frombank = new bankController ("from");
		tobank = new bankController ("to");
		boat = new BoatController ();

		loadCharacter ();
	}

	private void loadCharacter() {
		for (int i = 0; i < 3; i++) {
			MyCharacterController cha = new MyCharacterController ("priest");
			cha.setName("priest" + i);
			cha.setPosition (frombank.getEmptyPosition ());
			cha.getOnbank (frombank);
			frombank.getOnbank (cha);

			characters [i] = cha;
		}

		for (int i = 0; i < 3; i++) {
			MyCharacterController cha = new MyCharacterController ("devil");
			cha.setName("devil" + i);
			cha.setPosition (frombank.getEmptyPosition ());
			cha.getOnbank (frombank);
			frombank.getOnbank (cha);

			characters [i+3] = cha;
		}
	}


	public void moveBoat() {
		if (boat.isEmpty ())
			return;
		actionManager.moveBoat(boat,boat.getBoatDest(),speed);
		boat.pos_change();

		userGUI.status = check_game_over ();
	}

	public void characterIsClicked(MyCharacterController characterCtrl) {
		if (characterCtrl.isOnBoat ()) {
			bankController whichbank;
			if (boat.get_to_or_from () == -1) { 
				whichbank = tobank;
			} else {
				whichbank = frombank;
			}

			boat.GetOffBoat(characterCtrl.getName());

			Vector3 dest = whichbank.getEmptyPosition ();
			actionManager.moveChar(characterCtrl,dest,speed);

			characterCtrl.getOnbank (whichbank);
			whichbank.getOnbank (characterCtrl);

		} else {									
			bankController whichbank = characterCtrl.getbankController ();

			if (boat.getEmptyIndex () == -1) {		
				return;
			}

			if (whichbank.get_to_or_from () != boat.get_to_or_from ())	
				return;

			whichbank.getOffbank(characterCtrl.getName());

			Vector3 dest = boat.getEmptyPosition ();
			actionManager.moveChar(characterCtrl,dest,speed);
			
			characterCtrl.getOnBoat (boat);
			boat.GetOnBoat (characterCtrl);
		}
		userGUI.status = check_game_over ();

	}

	int check_game_over() {	
		int from_priest = 0;
		int from_devil = 0;
		int to_priest = 0;
		int to_devil = 0;

		int[] fromCount = frombank.getCharacterNum ();
		from_priest += fromCount[0];
		from_devil += fromCount[1];

		int[] toCount = tobank.getCharacterNum ();
		to_priest += toCount[0];
		to_devil += toCount[1];

		if (to_priest + to_devil == 6)		
			return 2;

		int[] boatCount = boat.getCharacterNum ();
		if (boat.get_to_or_from () == -1) {
			to_priest += boatCount[0];
			to_devil += boatCount[1];
		} else {	
			from_priest += boatCount[0];
			from_devil += boatCount[1];
		}
		if (from_priest < from_devil && from_priest > 0) {		
			return 1;
		}
		if (to_priest < to_devil && to_priest > 0) {
			return 1;
		}
		return 0;		
	}

	public void restart() {
		boat.reset ();
		frombank.reset ();
		tobank.reset ();
		for (int i = 0; i < characters.Length; i++) {
			characters [i].reset ();
		}
	}


}