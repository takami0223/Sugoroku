using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceController : MonoBehaviour
{

    public GameObject mValueFixedEffect; // 出目確定時のエフェクト

    private const string cDiceTagName = "Dice";
    private Vector3      mDicePos;                 //サイコロの初期位置 追加!!
    private Quaternion   mDiceRot;                 //サイコロの初期角度 追加!!
    private bool         mIsRaycastWait   = true;  // ダイスのクリック待ちか
    private bool         mIsRataWait      = true;  // ダイスの回転待ちか
    private bool         mIsShowDiceValue = false; // ダイスの出目確定か

    // Start is called before the first frame update
    void Start()
    {
        SetActive( false );
        mDicePos = this.gameObject.transform.localPosition;
        mDiceRot = this.gameObject.transform.localRotation;
        this.gameObject.GetComponent<Rigidbody>().useGravity         = false;
        this.gameObject.GetComponent<Rigidbody>().maxAngularVelocity = 15.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (mIsRaycastWait)
        {
            if (Input.GetMouseButtonDown( 0 ))
            {
                // クリック時のマウスポジションに光線を飛ばす
                Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
                RaycastHit hit;

                // 光線が何かにぶつかったか
                if (Physics.Raycast( ray, out hit, 50.0f ))
                {
                    // オブジェクトにダイス用タグが付いているか
                    if (hit.collider.gameObject.tag == cDiceTagName)
                    {
                        ThrowDice( hit.collider.gameObject );
                    }
                }
            }
        }

        // 出目を更新をどうにかする
        //mDiceValue = this.gameObject.GetComponent<Die_d6>().value;

        // 出た目を出力
        if (mIsShowDiceValue)
        {
            // ダイスが止まったときに出目を取得する
            if (IsStoping())
            {
                //Instantiate( mValueFixedEffect, this.transform.position, Quaternion.identity );
                // 出目を更新
                //mDiceValue = this.gameObject.GetComponent<Die_d6>().value;
                //コンソールに出目を表示する
                //Debug.Log( mDiceValue );
                //結果表示をやめる
                mIsShowDiceValue = false;
            }
        }
    }

    // ダイスを回転させる
    public IEnumerator DiceRota( int i_rota_count )
    {
        if (!this.gameObject.activeSelf)
        {
            this.SetActive( true );
            mIsRaycastWait   = true;
            mIsRataWait      = true;
            mIsShowDiceValue = false;
        }

        if (i_rota_count > 0 && mIsRataWait)
        {
            // 回転方向を設定（ランダム）
            Vector3 rota_velocity = new Vector3( Random.Range( -50, 50 ), Random.Range( -50, 50 ), Random.Range( -50, 50 ) ) * 10;
            this.gameObject.transform.GetComponent<Rigidbody>().angularVelocity = rota_velocity;

            i_rota_count--;

            // 指定秒数待機し、もう一度回転処理を呼ぶ
            yield return new WaitForSeconds( 0.3f );
            StartCoroutine( DiceRota( i_rota_count ) );

            yield break;
        }
        else if (i_rota_count == 0)
        {
            mIsRataWait = false;
            ThrowDice( this.gameObject );

            yield break;
        }
        else
        {
            yield break;
        }
    }

    // ダイスを投げる
    private void ThrowDice( GameObject i_dice )
    {
        // すべてのコルーチンを止める
        StopAllCoroutines();

        // 重力を付与
        i_dice.GetComponent<Rigidbody>().useGravity = true;

        // 回転の影響を無くす
        i_dice.GetComponent<Rigidbody>().velocity = Vector3.zero;

        // 指定の方向に飛ばす
        i_dice.GetComponent<Rigidbody>().AddForce( 0, 1500, 100 );

        mIsRaycastWait   = false;
        mIsShowDiceValue = true;
    }

    // サイコロの出目取得・非表示処理 追加!!
    public IEnumerator DiceDestroy()
    {

        //yield return new WaitForSeconds( 0.5f );

        // ダイスエフェクト
        //this.transform.Find( "DiceEffect" ).GetComponent<ParticleSystem>().Play();
        //Instantiate( mValueFixedEffect, transform.position, Quaternion.identity );

        //yield return new WaitForSeconds( 0.5f );

        // サイコロを非表示にする
        this.SetActive( false );

        // サイコロの重力をなくす
        this.GetComponent<Rigidbody>().useGravity = false;

        // 回転の影響をなくす
        this.GetComponent<Rigidbody>().velocity = Vector3.zero;

        // サイコロを初期位置・初期角度に戻す
        this.transform.localPosition = mDicePos;
        this.transform.localRotation = mDiceRot;

        yield break;
    }

    // ダイスの有効・無効を設定
    public void SetActive( bool is_actirve )
    {
        this.gameObject.SetActive( is_actirve );
    }

    // ダイスが停止しているか
    public bool IsStoping()
    {
        return this.gameObject.GetComponent<Rigidbody>().IsSleeping();
    }
}
