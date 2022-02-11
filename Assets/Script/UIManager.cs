using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI scoreText;
    [SerializeField] public TextMeshProUGUI moveText;
    public static UIManager instante;
    private int score;
    private int move;
    // Start is called before the first frame update
    void Awake()
    {
        instante = this;
    }

    public void AddScore(int cloneScore)
    {
        score += cloneScore;
        scoreText.text = score.ToString();
    }
    
    public void AddMove(int cloneMove)
    {
        move += cloneMove;
        moveText.text = move.ToString();
    }
}
