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

public class CoinController : MonoBehaviour {


	// Use this for initialization
	void Start () {
        // 回転を開始する角度を設定
        this.transform.Rotate(0, UnityEngine.Random.Range(0, 360), 0);
        
    }
	
	// Update is called once per frame
	void Update () {
        //回転
        this.transform.Rotate(0, 3, 0);

    }
}
