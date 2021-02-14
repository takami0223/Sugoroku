using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    
    public GameObject[] Players;    // プレイヤー
    public GameObject   Dice;       // ダイス
    public Panel_Event  PanelEvent; // マスのイベント集
    
    private int  mNowPlayerID      = 0;     // 現在のプレイヤーID
    private int  mDiceValue        = 0;     // ダイスの出目（DiceControllerが持つべき）
    private bool mIsFinishOneTarn  = false; // ターン終了したか
    
    // スクリプト用変数
    private DiceController mDiceCountroller;

    // インスタンス取得（シングルトン）
    private static GameDirector instance;
    public static GameDirector Instance => instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy( this.gameObject );
        }
        else
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        mDiceCountroller = Dice.GetComponent<DiceController>();

        // ターン開始
        StartCoroutine( SeaquenceOneTarn_() );
    }


    // Update is called once per frame
    void Update()
    {
        if( mIsFinishOneTarn )
        {

            // プレイヤーNo更新
            if( mNowPlayerID < Players.Length - 1)
            {
                ++mNowPlayerID;
            }
            else
            {
                mNowPlayerID = 0;
            }

            // ターン終了条件を解除
            mIsFinishOneTarn = false;

            // 次ターンを開始
            StartCoroutine( SeaquenceOneTarn_() );
        }
    }

    //-------------------------------------------
    //
    // 1ターンのシーケンス
    //
    //-------------------------------------------
    private IEnumerator SeaquenceOneTarn_()
    {
        // 名前表示
        yield return StartCoroutine( ShowName_() );

        // ダイスを投げる
        yield return StartCoroutine( DiceThrow_() );

        // ダイスを削除
        yield return StartCoroutine( mDiceCountroller.DiceDestroy() );

        // プレイヤー移動
        yield return StartCoroutine( PlayerMove_() );

        // マスイベント
        yield return StartCoroutine( PanelEvent_() );

        // ターン終了
        mIsFinishOneTarn = true;

    }

    //-------------------------------------------
    // プレイヤー名を表示
    //-------------------------------------------
    private IEnumerator ShowName_()
    {
        Debug.Log( Players[ mNowPlayerID ].name );
        yield return new WaitForSeconds( 3.0f );
    }

    //-------------------------------------------
    // ダイスを投げる
    //-------------------------------------------
    private IEnumerator DiceThrow_()
    {
        // ダイスを出現
        yield return StartCoroutine( mDiceCountroller.DiceRota( 20 ) );

        // ダイスの目が止ったか
        yield return new WaitUntil( mDiceCountroller.IsStoping );

        // ダイスの目を取得
        mDiceValue = Dice.GetComponent<Die_d6>().value;
        Debug.Log( mDiceValue );
    }

    //-------------------------------------------
    // プレイヤー移動
    //-------------------------------------------
    private IEnumerator PlayerMove_()
    {
        // 移動リクエスト
        Players[ mNowPlayerID ].GetComponent<PlayerController>().RequestMove( mDiceValue );

        // 移動終了まで処理停止
        yield return new WaitWhile( Players[ mNowPlayerID ].GetComponent<PlayerController>().IsMoving );
    }

    //-------------------------------------------
    // マスイベント
    //-------------------------------------------
    private IEnumerator PanelEvent_()
    {
        // 現在のマスの種類を取得
        var panel_kind      = Players[ mNowPlayerID ].GetComponent<PlayerController>().GetPanelKind();
        var panel_event     = PanelEvent.sheets[(int)panel_kind].list;
        int panel_event_num = panel_event.Count;
        int event_idx       = Random.Range( 0, panel_event_num );

        string sentence = panel_event[ event_idx ].Sentence;
        int    amount   = panel_event[ event_idx ].Amount;
        
        Debug.LogFormat( "[ {0} ] {1}「{2}」", panel_kind.ToString(), sentence, amount );
        yield return new WaitForSeconds( 2.0f );
    }
}