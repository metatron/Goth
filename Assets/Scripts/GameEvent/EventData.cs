using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

[Serializable]
public class EventData: MonoBehaviour {
	public enum FukidashiType: int
	{
		None,
		Normal,
		Think,
		Yell
	}

	public enum EmotionType: int
	{
		None,
		Bikkuri,
		Ira,
		Love,
		Nothing,
		NoticeTop,
		Question,
		Surprise
	}

	public enum TweenAnimType: int
	{
		None,
		Shake,
		Rotate,
		Fall,
		Alpha,
		MoveLeftRight,
		MoveUpDown
	}

	public enum EffectType: int
	{
		None,
		WhiteOut
	}
		

	public FukidashiType fType; //吹き出しの種類
	public EmotionType eType; //表示する表情の種類

	private static Dictionary<string, GameObject> IdTargetMap = new Dictionary<string, GameObject> (); //id, targetObject map.

	public string id; //id
	public EventListObject eventListObj; //eventlistObject that contains this eventdata.

	public enum TargetType: int { //type of target.
		Path,
		EnemyStatus,
		Object,
		Name,
		Player
	}
	public TargetType targetType = TargetType.Path; //ディフォルトはパス指定
	public string targetPath; //ターゲットがシーンにいない場合、Instance化する。
	public EnemyParameterStatus targetEnemyStatus = null; //パスが設定されていない場合にInstance化。
	public GameObject targetObject; //どのターゲットに設置するか
	public string targetByName; //名前によるターゲット特定

	public bool posSetOnInstantiate = false; //オブジェクトがインスタンス化されたさいに自身のクラスで初期化される。
	public Vector3 targetInitPos; //screen上のポジション。（上記がfalseの時のみ有効）
	public Vector3 targetInitScale = Vector3.one; //初期サイズ
	public BaseParameterStatus.CharacterDirection direction; //右or左に設置
	public string animationName; //吹き出しと同時に再生するアニメーション

	public TweenAnimType tweenAnimType = TweenAnimType.None; //iTweenを使用したあにめーしょん
	public float tweenAnimTime;
	public float tweenValue; //どれだけ進むか、どれだけ回転するかなど。

	public EffectType effectType = EffectType.None; //effectを再生（Particles）

	public string stagePrefPath; //ステージのプレハブ（あればステージをロード）
	public string animAftrStgLoad; //ステージロード後にプレイするアニメーション

	public GameObject OnDestroyEnemyPrefab; //targetObjectがEnemyでかつ倒した際にアクティブにあるEventListObjectのPrefab

	public float totalTime = 3.0f; //次のEventまでのWaitTime

	private GameObject fukidashiObject;
	private GameObject emotionObject;

	public void InitEvent() {
		Debug.Log ("*** EventData: " + gameObject + ", id: " + id + ", time: " + Time.realtimeSinceStartup);
		/****** init fukidashi ******/
		if (fType != FukidashiType.None) {
			string fukidashiPath = "Prefabs/GameEvent/Fukidashi/";
			switch (fType) {
			case FukidashiType.Think:
				fukidashiPath += "Fukidashi_Think";
				break;
			case FukidashiType.Yell:
				fukidashiPath += "Fukidashi_Yell";
				break;
			case FukidashiType.Normal:
				fukidashiPath += "Fukidashi_Normal";
				break;
			}
			fukidashiObject = (GameObject)Instantiate (Resources.Load (fukidashiPath));
		}

		//instantiate target if theres path set OR targetEnemy prefab is set.
		if (targetType == TargetType.Path || targetType == TargetType.EnemyStatus) {
			if (!string.IsNullOrEmpty (id)) {
				if (!IdTargetMap.ContainsKey (id)) {
					if (targetType == TargetType.Path && !string.IsNullOrEmpty (targetPath)) {
						targetObject = (GameObject)Instantiate (Resources.Load (targetPath));
						targetObject.name = targetObject.name.Replace ("(Clone)", "");
						targetObject.transform.localScale = targetInitScale;
						Debug.Log ("targetInitScale: " + targetObject + ", " + targetInitScale);
					} else if (targetType == TargetType.EnemyStatus && targetEnemyStatus != null) {
						//load EnemyAI w/ status
						targetObject = GameManager.Instance.SummonEnemyByStatus (targetEnemyStatus);
					} else {
						Debug.LogError ("WARNING! Check the targetType and targetPath or targetEnemyStatus!");
					}
					//if pos flag is not set use targetInitPos as default position.
					//otherwise the position must be defined on its Start method.
					if (!posSetOnInstantiate) {
						targetObject.transform.position = targetInitPos;
					}
					IdTargetMap.Add (id, targetObject);

					//if instantiating object is EnemyAI, set DoNothing flag
					if (targetObject.GetComponent<EnemyAI> () != null) {
						targetObject.GetComponent<EnemyAI> ().doNothing = true;
						//add to gameManager enemylist
						targetObject.transform.SetParent(GameManager.Instance.transform);
						GameManager.Instance.totalEnemyList.Add (targetObject);
					}

					//if theres any eventlist that is set after the enemy died,
					//set it to Weapon object.
					if (OnDestroyEnemyPrefab != null && targetObject.GetComponentInChildren<EnemyAI> () != null) {
						targetObject.GetComponentInChildren<EnemyAI> ().OnDestroyEnemyPrefab = OnDestroyEnemyPrefab;
					}
				} else {
					targetObject = IdTargetMap [id];
				}
			} else {
				Debug.LogError ("id needs to be defined if you are instantiating NPC!");
			}
		}
		//instantiate by name.
		else if (targetType == TargetType.Name) {
			if (string.IsNullOrEmpty (targetByName)) {
				Debug.LogError ("Set the targetByName!");
			}
			targetObject = GameObject.Find (targetByName);
			if (targetObject == null) {
				Debug.LogError (targetByName + " does not exists!");
			}
//			targetObject.transform.localScale = targetInitScale;
		}
		else {
			targetObject = GameManager.Instance.player;
			Debug.Log ("set to player character.");
		}

		/****** target direction *******/

		Vector3 defaultSizeVec = targetInitScale;
		if (targetObject.GetComponent<EnemyAI> () != null) {
			defaultSizeVec = targetObject.GetComponent<EnemyAI> ().defaultSizeVec;
		} else if (targetObject.GetComponent<PlayerCharacter> () != null) {
			defaultSizeVec = new Vector3 (TouchDetection.PLAYER_DEFAULT_SCALE, TouchDetection.PLAYER_DEFAULT_SCALE, 1.0f);
		}
			
		if (direction == BaseParameterStatus.CharacterDirection.RIGHT) {
			targetObject.transform.localScale = new Vector3 (-defaultSizeVec.x, defaultSizeVec.y, 1.0f);
			if (fukidashiObject != null) {
				fukidashiObject.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
			}
		} else if (direction == BaseParameterStatus.CharacterDirection.LEFT) {
			targetObject.transform.localScale = new Vector3 (defaultSizeVec.x, defaultSizeVec.y, 1.0f);
			if (fukidashiObject != null) {
				fukidashiObject.transform.localScale = new Vector3 (-1.0f, 1.0f, 1.0f);
			}
		}

		/****** init animation ******/
		BoneAnimation anim = targetObject.GetComponent<BoneAnimation> ();
		if (!string.IsNullOrEmpty (animationName) && anim != null) {
			anim.Stop ();
			anim.Play (animationName);
		}

		/****** init tween animation ******/
		if (tweenAnimType != TweenAnimType.None) {
			iTweenAnimation (tweenAnimType);
		}

		/****** init Effect ******/
		if (effectType != EffectType.None) {
			InitEffect ();
		}

		/****** init direction ******/
		if (fukidashiObject != null) {
			Vector3 pos = targetObject.transform.position;
			if (direction == BaseParameterStatus.CharacterDirection.LEFT) {
				pos.x -= 200.0f;
			} else {
				pos.x += 200.0f; 
			}
			pos.y += 500.0f;
			pos.z -= 50.0f;
			fukidashiObject.transform.position = pos;
			fukidashiObject.transform.SetParent (targetObject.transform);
		}

		/****** init emotion ******/
		if (eType != EmotionType.None && fukidashiObject != null) {
			string emotionPath = "Prefabs/GameEvent/Emotions/";
			switch (eType) {
			case EmotionType.Bikkuri:
				emotionPath += "Emotion_Bikkuri";
				break;
			case EmotionType.Ira:
				emotionPath += "Emotion_Ira";
				break;
			case EmotionType.Love:
				emotionPath += "Emotion_Love";
				break;
			case EmotionType.Nothing:
				emotionPath += "Emotion_Nothing";
				break;
			case EmotionType.NoticeTop:
				emotionPath += "Emotion_NoticeTop";
				break;
			case EmotionType.Question:
				emotionPath += "Emotion_Question";
				break;
			case EmotionType.Surprise:
				emotionPath += "Emotion_Surprise";
				break;
			}
			emotionObject = (GameObject)Instantiate (Resources.Load (emotionPath));
			emotionObject.transform.SetParent (fukidashiObject.transform);
			emotionObject.transform.localPosition = new Vector3 (0.0f, 0.0f, -10.0f);
		}

		/****** init stage ******/
		if (!string.IsNullOrEmpty (stagePrefPath)) {
			//InitStage ();
			MenuManager.Instance.ActivateAndStartFading(totalTime, () => { InitStage (); });
		}

	} //InitEvent

	public void DestroyEventDataObj() {
		if (!string.IsNullOrEmpty(id) && IdTargetMap.ContainsKey (id)) {
			IdTargetMap.Remove (id);
		}

		if (emotionObject != null) {
			Destroy (emotionObject);
		}

		if (fukidashiObject != null) {
			Destroy (fukidashiObject);
		}
	}

	private void iTweenAnimation(TweenAnimType animType) {
		switch (animType) {
		case TweenAnimType.Shake:
			iTween.ShakePosition (targetObject, iTween.Hash (
				"amount", new Vector3 (3.0f, 0.0f, 0.0f),
				"time", tweenAnimTime));
			break;
		
		case TweenAnimType.Fall:
			iTween.MoveTo (targetObject, iTween.Hash (
				"position", new Vector3 (targetObject.transform.localPosition.x, tweenValue, targetObject.transform.localPosition.z),
				"islocal", true,
				"easetype", iTween.EaseType.linear,
				"time", tweenAnimTime));
			break;

		case TweenAnimType.Rotate:
			iTween.RotateTo (targetObject, iTween.Hash (
				"rotation", new Vector3 (0.0f, 0.0f, -90.0f),
				"time", tweenAnimTime));
			break;

		case TweenAnimType.MoveLeftRight:
			iTween.MoveTo (targetObject, iTween.Hash (
				"position", new Vector3 (targetObject.transform.localPosition.x+tweenValue, targetObject.transform.localPosition.y, targetObject.transform.localPosition.z),
				"islocal", true,
				"easetype", iTween.EaseType.linear,
				"time", tweenAnimTime));
			break;

		case TweenAnimType.MoveUpDown:
			iTween.MoveTo (targetObject, iTween.Hash (
				"position", new Vector3 (targetObject.transform.localPosition.x, targetObject.transform.localPosition.y+tweenValue, targetObject.transform.localPosition.z),
				"islocal", true,
				"easetype", iTween.EaseType.linear,
				"time", tweenAnimTime));
			break;

		}
	}

	private void InitEffect() {
		GameObject particleObj = null;

		switch (effectType) {
		case EffectType.WhiteOut:
			particleObj = (GameObject)Instantiate (Resources.Load ("Prefabs/Particle/Disappear"));
			break;
		}

		if (particleObj != null) {
			particleObj.transform.SetParent (targetObject.transform);
			particleObj.transform.localPosition = Vector3.zero;
		}
	}

	private void InitStage() {
		//need to prepare before move to next stage.

		DestroyEventDataObj ();

		EventListObject.isEventOn = false;
		//make all other enemies move
		GameManager.Instance.SetDoNothingAllCharacters(false);

		//save done
		if (eventListObj.willStoreOnDoneList) {
			GameManager.Instance.eventDoneList.Add (eventListObj.id);
			SaveLoadStatus.SaveUserParameters ();
		}

		//make all iTween animation off
//		iTween.Stop();

		//set reference to null.
		StageManager.crntEvent = null;

		//move to next stage
		StageManager.Instance.InitStageReposPlayer(stagePrefPath, animAftrStgLoad);
	}
}
