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

    //コイン獲得時のスコア加算
    private GameObject scoreText;
    private int score = 0;

    //ボタンの押し判定
    private bool isLButtonDown = false;
    private bool isRButtonDown = false;

    //Unityちゃんとカメラの距離
    private float difference;

    //Main Cameraのオブジェクト
    private GameObject camera;

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

        // HirerarchyにあるGameObjectを探して代入
        scoreText = GameObject.Find("GameScore");
        scoreText.GetComponent<Text>().text = "Score "+score+"pt";

        //Unityちゃんのオブジェクトを取得
        camera = GameObject.Find("Main Camera");
        //Unityちゃんとカメラの位置（z座標）の差を求める
        this.difference = this.transform.position.z - camera.transform.position.z;

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

        // 左右上キー
        if ( this.isLButtonDown && -this.movableRange < this.transform.position.x)
        {
            //左に移動（追加）
            this.myRigidbody.AddForce(-this.turnForce, 0, 0);
        }
        else if (this.isRButtonDown && this.transform.position.x < this.movableRange)
        {
            //右に移動（追加）
            this.myRigidbody.AddForce(this.turnForce, 0, 0);
        }

        //Jumpステートの場合はJumpにfalseをセットする（追加）
        if (this.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
        {
            this.myAnimator.SetBool("Jump", false);
        }

        //ユニティちゃんが通り過ぎて画面外に出たアイテムを直ちに破棄
        GameObject[]descar = GameObject.FindGameObjectsWithTag("CarTag");
        foreach (GameObject obj in descar)
        {
            if (obj.transform.position.z+this.difference < this.transform.position.z)
                Destroy(obj);

        }
        GameObject[] descone = GameObject.FindGameObjectsWithTag("TrafficConeTag");
        foreach (GameObject obj in descone)
        {
            if (obj.transform.position.z + this.difference < this.transform.position.z)
                Destroy(obj);

        }
        GameObject[] descoin = GameObject.FindGameObjectsWithTag("CoinTag");
        foreach (GameObject obj in descoin)
        {
            if (obj.transform.position.z + this.difference < this.transform.position.z)
                Destroy(obj);

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

            //スコア加算
            score += 10;
            scoreText = GameObject.Find("GameScore");
            scoreText.GetComponent<Text>().text = "Score " + score + "pt";

            //接触したコインのオブジェクトを破棄
            Destroy(other.gameObject);
        }
    }

    //ジャンプボタンを押した場合の処理（追加）
    public void GetMyJumpButtonDown()
    {
        if (this.transform.position.y < 0.5f)
        {
            this.myAnimator.SetBool("Jump", true);
            this.myRigidbody.AddForce(this.transform.up * this.upForce);
        }
    }

    //左ボタンを押し続けた場合の処理（追加）
    public void GetMyLeftButtonDown()
    {
        this.isLButtonDown = true;
    }
    //左ボタンを離した場合の処理（追加）
    public void GetMyLeftButtonUp()
    {
        this.isLButtonDown = false;
    }

    //右ボタンを押し続けた場合の処理（追加）
    public void GetMyRightButtonDown()
    {
        this.isRButtonDown = true;
    }
    //右ボタンを離した場合の処理（追加）
    public void GetMyRightButtonUp()
    {
        this.isRButtonDown = false;
    }
}