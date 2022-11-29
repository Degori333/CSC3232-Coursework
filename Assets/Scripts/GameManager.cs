using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    static GameManager gameManager;

    /*
    [SerializeField]
    TextMeshProUGUI textFPS;
    [SerializeField]
    TextMeshProUGUI textMemory;
    */

    // Start is called before the first frame update
    void Awake()
    {
        gameManager = this;
    }

    private void Update()
    {
        // To be refined
        //textFPS.text = "FPS: " + (1 / Time.unscaledDeltaTime);
        //textMemory.text = "Memory Usage: " + System.GC.GetTotalMemory(false);
    }

}
