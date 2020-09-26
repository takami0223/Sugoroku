using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    // ターンステート
    public enum TarnState
    {
        ShowName,
        DiceThrow,
        PlayerMove,
    }
    
    public GameObject[] Players = new GameObject[ 1 ]; // プレイヤー
    public GameObject   Dice;                          // ダイス
    
    private TarnState mCurrentTarnState = TarnState.ShowName; // 現在のターンステート
    private int       mNowPlayerID      = 0;                  // 現在のプレイヤーID
    private int       mDiceValue        = 0;                  // ダイスの出目（DiceControllerが持つべき）
    
    // スクリプト用変数
    private DiceController mDiceCountroller;

    // インスタンス取得（シングルトン）
    private static GameDirector instance;// = new GameDirector();
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
        dispatch( TarnState.ShowName );
    }


    // Update is called once per frame
    void Update()
    {
        // 名前表示

        // ダイスが出現していなければ出現をリクエスト
        if (mCurrentTarnState != TarnState.DiceThrow &&
            Input.GetMouseButtonDown( 0 ))
        {
            dispatch( TarnState.DiceThrow );
        }

        // ダイスの出目が確定したか
        if (mCurrentTarnState == TarnState.DiceThrow &&
            mDiceCountroller.IsStoping())
        {
            mDiceValue = Dice.GetComponent<Die_d6>().value;
            Debug.Log( mDiceValue );
            StartCoroutine( mDiceCountroller.DiceDestroy() );
            dispatch( TarnState.PlayerMove );
        }

        // プレイヤーの移動 & マスイベントが終了したか

    }

    public void dispatch( TarnState i_state )
    {
        // 前のステートを記録
        TarnState old_state = mCurrentTarnState;
        // 現在のステートを更新
        mCurrentTarnState = i_state;

        // 現在のステートに応じて処理
        switch (mCurrentTarnState)
        {
            case TarnState.ShowName:
                StartCoroutine( ShowName_() );
                break;
            case TarnState.DiceThrow:
                DiceThrow_();
                break;
            case TarnState.PlayerMove:
                PlayerMove_();
                break;
            default:
                break;
        }
    }

    private IEnumerator ShowName_()
    {
        Debug.Log( "Player_1" );
        yield return new WaitForSeconds( 3.0f );
        dispatch( TarnState.DiceThrow );
        yield break;
    }

    private void DiceThrow_()
    {
        StartCoroutine( mDiceCountroller.DiceRota( 20 ) );

        /*
        if (!mDiceCountroller.IsStoping())
        {
            yield return null;

        }

        Debug.Log( mDiceCountroller.GetValue() );
        mDiceCountroller.SetActive( false );
        dispatch( TarnState.PlayerMove );
        */
    }

    private void PlayerMove_()
    {
        Players[ mNowPlayerID ].GetComponent<PlayerController>().RequestMove( mDiceValue );
    }
}
