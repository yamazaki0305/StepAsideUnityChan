using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UnityChanController : MonoBehaviour
{
    //アニメーションするためのコンポーネントを入れる
    private Animator myAnimator;
    //Unityちゃんを移動させるコンポーネントを入れる（追加）
    private Rigidbody myRigidbody;
    //前進するための力
    private float forwardForce = 800.0f;
    //左右に移動するための力
    private float turnForce = 500.0f;
    //左右の移動できる範囲
    private float movableRange = 3.4f;
    //動きを減速させる係数
    private float coefficient = 0.95f;
    //ジャンプするための力
    private float upForce = 500.0f;
    //ゲームの終了判定
    private bool isEnd = false;

    //ゲーム終了時に表示するテキスト（追加）
    private GameObject stateText;

    // Use this for initialization
    void Start()
    {

        //アニメータコンポーネントを取得
        this.myAnimator = GetComponent<Animator>();

        //走るアニメーションを開始
        this.myAnimator.SetFloat("Speed", 1);

        //Rigidbodyコンポーネントを取得（追加）
        this.myRigidbody = GetComponent<Rigidbody>();

        isEnd = false;

        // HirerarchyにあるGameObjectを探して代入
        stateText = GameObject.Find("GameResultText");
        stateText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //ゲーム終了判定ならユニティちゃんを減速させる
        if (this.isEnd)
        {
            this.forwardForce *= this.coefficient;
            this.turnForce *= this.coefficient;
            this.upForce *= this.coefficient;
            this.myAnimator.speed *= this.coefficient;
        }

        //Unityちゃんに前方向の力を加える
        this.myRigidbody.AddForce(this.transform.forward * this.forwardForce);
        //this.myRigidbody.AddForce(0,0,forwardForce);


        //Unityちゃんを矢印キーまたはボタンに応じて左右に移動させる（追加）
        if (Input.GetKey(KeyCode.LeftArrow) && -this.movableRange < this.transform.position.x)
        {
            //左に移動（追加）
            this.myRigidbody.AddForce(-this.turnForce, 0, 0);
        }
        else if (Input.GetKey(KeyCode.RightArrow) && this.transform.position.x < this.movableRange)
        {
            //右に移動（追加）
            this.myRigidbody.AddForce(this.turnForce, 0, 0);
        }

        //Jumpステートの場合はJumpにfalseをセットする（追加）
        if (this.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
        {
            this.myAnimator.SetBool("Jump", false);
        }

        //ジャンプしていない時にスペースが押されたらジャンプする（追加）
        if (Input.GetKeyDown(KeyCode.Space) && this.transform.position.y < 0.5f)
        {
            Debug.Log("jump");
            //ジャンプアニメを再生（追加）
            this.myAnimator.SetBool("Jump", true);
            //Unityちゃんに上方向の力を加える（追加）
            this.myRigidbody.AddForce(this.transform.up * this.upForce);
        }


    }

    //トリガーモードで他のオブジェクトと接触した場合の処理
    void OnTriggerEnter(Collider other)
    {

        //障害物に衝突した場合
        if (other.gameObject.tag == "CarTag" || other.gameObject.tag == "TrafficConeTag")
        {
            Debug.Log("over");
            stateText.GetComponent<Text>().text = "GameOver!";
            stateText.SetActive(true);
            this.isEnd = true;
        }

        //ゴール地点に到達した場合
        if (other.gameObject.tag == "GoalTag")
        {
            stateText.GetComponent<Text>().text = "GameClear!";
            stateText.SetActive(true);
            this.isEnd = true;
        }

        //コインに衝突した場合
        if (other.gameObject.tag == "CoinTag")
        {
            Debug.Log("coin");
            //パーティクルを再生
            GetComponent<ParticleSystem>().Play();

            //接触したコインのオブジェクトを破棄（追加）
            Destroy(other.gameObject);
        }
    }
}