using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody  mRigidbody; // 物理
    private Animator   mAnimator;  // アニメータ

    private GameObject mNowPanel;     // 現在のパネル
    private GameObject mPrevPanel;    // 前のパネル（オブジェクト）
    private GameObject mNextPanel;    // 次のパネル（オブジェクト）
    private Vector3    mPrevPanelPos; // 前のパネル位置
    private Vector3    mNextPanelPos; // 次のパネル位置

    private bool mIsMoveRequest = false; // マス移動中か
    private int  mMoveNum       = 0;     // マス移動数

    private void Awake()
    {
        mRigidbody = GetComponent<Rigidbody>();
        mAnimator  = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // アニメーターパラメータ初期化
        mAnimator.SetFloat( "Turn", 0f );
        mAnimator.SetFloat( "Forward", 0f );

        // 無指定に備えて、自身の位置で初期化しておく
        mPrevPanelPos = transform.position;
        mNextPanelPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if( mIsMoveRequest)
        {
            Move_();
        }
        else
        {
            Stop_();
        }
    }

    // プレイヤーが何かに衝突した瞬間
    private void OnCollisionEnter( Collision collision )
    {
        if( collision.gameObject.tag == "NormalPanel" )
        {
            mNowPanel = collision.gameObject;
        }
    }

    private void OnTriggerEnter( Collider other )
    {
        // マスの停止位置に触れたら
        if( other.tag == "StayPos" )
        {
            GameObject parent     = other.transform.parent.gameObject;
            var        panel_ctrl = parent.GetComponent<NormalPanelController>();

            // 前後パネル更新
            mPrevPanel = panel_ctrl.PrevPanel;
            mNextPanel = panel_ctrl.NextPanel;

            // 前後パネル位置を更新
            if( mPrevPanel != null ) { mPrevPanelPos = mPrevPanel.transform.position; }
            if( mNextPanel != null ) { mNextPanelPos = mNextPanel.transform.position; }

            // マス移動管理を更新
            if( mMoveNum > 0 ) { --mMoveNum; } // 進む
            if( mMoveNum < 0 ) { ++mMoveNum; } // 戻る
            if( mMoveNum == 0 ){ mIsMoveRequest = false; } // 停止
        }
    }

    //-------------------------------------------
    // マスを移動
    //-------------------------------------------
    private void Move_()
    {
        // 進む先が未指定の場合は反転
        if( ( mPrevPanel == null && mMoveNum < 0 ) ||
            ( mNextPanel == null && mMoveNum > 0 ) )
        {
            mMoveNum *= -1;
        }

        // 移動アニメーション再生
        mAnimator.SetFloat( "Turn", 0f );
        mAnimator.SetFloat( "Forward", 1f );

        // 前進
        if ( mMoveNum > 0)
        {
            SetLookRotation( mNextPanelPos );
            transform.position = Vector3.MoveTowards( transform.position, mNextPanelPos, 0.1f );
        }
        // 後退
        else if( mMoveNum < 0 )
        {
            SetLookRotation( mPrevPanelPos );
            transform.position = Vector3.MoveTowards( transform.position, mPrevPanelPos, 0.1f );
        }
    }

    //-------------------------------------------
    // 停止
    //-------------------------------------------
    private void Stop_()
    {
        // 剛体の速度を0に設定
        mRigidbody.velocity        = Vector3.zero;
        mRigidbody.angularVelocity = Vector3.zero;

        // アニメーターパラメータをリセット（停止時アニメにする）
        mAnimator.SetFloat( "Turn", 0f );
        mAnimator.SetFloat( "Forward", 0f );
    }

    //-------------------------------------------
    // 進行方向を設定
    //-------------------------------------------
    private void SetLookRotation( Vector3 i_next_position )
    {
        Vector3 diff = i_next_position - transform.position;
        // y軸は考慮しない
        diff.y = 0;

        var look_rotation  = Quaternion.LookRotation( diff );
        transform.rotation = Quaternion.Lerp( transform.rotation, look_rotation, 100f );
    }

    //-------------------------------------------
    // マス移動をリクエスト
    //-------------------------------------------
    public void RequestMove( int i_move_num )
    {
        if( i_move_num > 0)
        {
            mMoveNum       = i_move_num;
            mIsMoveRequest = true;
        }
    }

    //-------------------------------------------
    // マス移動中か
    //-------------------------------------------
    public bool IsMoving()
    {
        return mIsMoveRequest;
    }

    //-------------------------------------------
    // 止まっているマスの種類を取得
    //-------------------------------------------
    public NormalPanelController.PANEL_KIND GetPanelKind()
    {
        if( mNowPanel != null )
        {
            return mNowPanel.GetComponent<NormalPanelController>().PanelKind;
        }

        return NormalPanelController.PANEL_KIND.BLUE;
    }

    // マス移動（ジャンプ移動）
    /*
    private IEnumerator Move_()
    {
        // 進む先が未指定の場合は反転
        if( ( mPrevPanel == null && mMoveNum < 0 ) ||
            ( mNextPanel == null && mMoveNum > 0 ) )
        {
            mMoveNum *= -1;
        }

        // 前に進むとき
        if ( mMoveNum > 0 )
        {
            SetLookRotation( mNextPanelPos );

            --mMoveNum;
            mIsMoveRequest = false;

            yield return new WaitForSeconds( 0.25f );
            //transform.position = mNextPanelPos;
            // 移動処理
            
            iTween.MoveTo( this.gameObject, iTween.Hash(
                "position", mNextPanelPos + new Vector3( 0, 3f, 0 ),
                "time", 0.2f
                //"oncompletetarget", this.gameObject,
                //"oncomplete", "Move",
                //"oncompleteparams", i_move_num
                ) );
            
        }
        // 後ろに戻るとき
        else if ( mMoveNum < 0 )
        {
            SetLookRotation( mPrevPanelPos );

            ++mMoveNum;
            mIsMoveRequest = false;

            yield return new WaitForSeconds( 0.25f );
            //transform.position = mPrevPanelPos;
            
            iTween.MoveTo( this.gameObject, iTween.Hash(
                "position", mPrevPanelPos + new Vector3( 0, 3f, 0 ),
                //"y", move_y,
                //"z", -move_z,
                "time", 0.2f
                //"oncompletetarget", this.gameObject,
                //"oncomplete", "Move",
                //"oncompleteparams", i_move_num
                ) );
        }
        yield break;
    }
    */
}
