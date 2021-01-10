using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody  mRigidbody;
    private Animator   mAnimator;

    private GameObject mPrevPanel;
    private GameObject mNextPanel;
    private Vector3    mPrevPanelPos;
    private Vector3    mNextPanelPos;
    private bool       mIsMoveRequest = false;
    private int        mMoveNum       = 0;

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
    }

    private void OnTriggerEnter( Collider other )
    {
        // マスの停止位置に触れたら
        if( other.tag == "StayPos" )
        {
            GameObject parent = other.transform.parent.gameObject;

            // 前後パネル更新
            mPrevPanel = parent.GetComponent<NormalPanelController>().PrevPanel;
            mNextPanel = parent.GetComponent<NormalPanelController>().NextPanel;

            // 前後パネル位置を更新
            if( mPrevPanel != null ) { mPrevPanelPos = mPrevPanel.transform.position; }
            if( mNextPanel != null ) { mNextPanelPos = mNextPanel.transform.position; }

            // マス移動管理を更新
            if( mMoveNum > 0 ) { --mMoveNum; }
            if( mMoveNum < 0 ) { ++mMoveNum; }
            if( mMoveNum == 0 ){ mIsMoveRequest = false; }
            //StartCoroutine( UpdateMoveNum_() );
        }
    }

    private IEnumerator UpdateMoveNum_()
    {
        yield return new WaitForSeconds( 0.03f );

        // マス移動管理を更新
        if (mMoveNum > 0) { --mMoveNum; }
        if (mMoveNum < 0) { ++mMoveNum; }
        if (mMoveNum == 0) { mIsMoveRequest = false; }
    }

    // マス移動
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

    // 停止
    private void Stop_()
    {
        // 剛体の速度を0に設定
        mRigidbody.velocity        = Vector3.zero;
        mRigidbody.angularVelocity = Vector3.zero;

        // アニメーターパラメータをリセット（停止時アニメにする）
        mAnimator.SetFloat( "Turn", 0f );
        mAnimator.SetFloat( "Forward", 0f );
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

    // 進行方向を向く
    private void SetLookRotation( Vector3 i_next_position )
    {
        Vector3 diff = i_next_position - transform.position;
        // y軸は考慮しない
        diff.y = 0;

        var look_rotation  = Quaternion.LookRotation( diff );
        transform.rotation = Quaternion.Lerp( transform.rotation, look_rotation, 100f );
    }

    // 移動をリクエスト
    public void RequestMove( int i_move_num )
    {
        if( i_move_num > 0)
        {
            mMoveNum       = i_move_num;
            mIsMoveRequest = true;
        }
    }
}
