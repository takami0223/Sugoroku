using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceController : MonoBehaviour
{

    public GameObject mValueFixedEffect; // 出目確定時のエフェクト

    private const string cDiceTagName = "Dice";
    private Vector3      mDiceInitPos; //ダイスの初期位置
    private Quaternion   mDiceInitRot; //ダイスの初期角度

    private bool mIsRaycastWait = true;  // ダイスのクリック待ちか
    private bool mIsRataWait    = true;  // ダイスの回転待ちか

    // Start is called before the first frame update
    void Start()
    {
        // ダイスを非表示
        SetActive( false );

        // ダイスの姿勢初期設定
        mDiceInitPos = this.gameObject.transform.localPosition;
        mDiceInitRot = this.gameObject.transform.localRotation;
        
        // ダイスの物理初期設定
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
    }

    // ダイスを回転させる
    public IEnumerator DiceRota( int i_rota_count )
    {
        // ダイスが出ていなければ有効化
        if (!this.gameObject.activeSelf)
        {
            this.SetActive( true );
            mIsRaycastWait   = true;
            mIsRataWait      = true;
        }

        if (i_rota_count > 0 && mIsRataWait)
        {
            // 回転方向を設定（ランダム）
            Vector3 rota_velocity = new Vector3( Random.Range( -50, 50 ), Random.Range( -50, 50 ), Random.Range( -50, 50 ) ) * 10;
            this.gameObject.transform.GetComponent<Rigidbody>().angularVelocity = rota_velocity;

            i_rota_count--;

            // 指定秒数待機
            yield return new WaitForSeconds( 0.3f );

            // 別コルーチンで回転処理を呼ぶ（回転数を減らしたもの）
            StartCoroutine( DiceRota( i_rota_count ) );
            
            // 自身は終了
            yield break;
        }
        // 指定回数に到達したら、強制的に投げる
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
        // すべてのコルーチン（ダイスに回転を与えている）を止める
        StopAllCoroutines();

        // 重力を付与
        i_dice.GetComponent<Rigidbody>().useGravity = true;

        // 回転の影響を無くす
        i_dice.GetComponent<Rigidbody>().velocity = Vector3.zero;

        // 指定の方向（上）に飛ばす
        i_dice.GetComponent<Rigidbody>().AddForce( 0, 1500, 100 );

        mIsRaycastWait = false;
    }

    // ダイスの非表示処理
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

        // サイコロを初期姿勢に戻す
        this.transform.localPosition = mDiceInitPos;
        this.transform.localRotation = mDiceInitRot;

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
