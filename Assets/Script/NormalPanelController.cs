using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalPanelController : MonoBehaviour
{
    public GameObject PrevPanel; // 前のパネル
    public GameObject NextPanel; // 次のパネル

    private AudioSource mAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        mAudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter( Collision collision )
    {
        GameObject hit_object = collision.gameObject;

        // プレイヤーがマスに乗った時のSE
        if( hit_object.tag == "Player" )
        {
            mAudioSource.Play();
        }
    }
}
