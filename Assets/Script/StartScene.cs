using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class StartScene : MonoBehaviour
{
    public Slider colorCount, levelWidth, levelHeight;
    public TextMeshProUGUI colorText, widthText, heightText;
    
    // Start is called before the first frame update
    void Awake()
    {
        levelHeight.value = PlayerPrefs.GetInt("levelHeight",9);     
        levelWidth.value = PlayerPrefs.GetInt("levelWidth",6);
        colorCount.value = PlayerPrefs.GetInt("colorCount",5);  

        colorText.text = colorCount.value.ToString();
        widthText.text = levelWidth.value.ToString();
        heightText.text = levelHeight.value.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        colorText.text = colorCount.value.ToString();
        widthText.text = levelWidth.value.ToString();
        heightText.text = levelHeight.value.ToString();

        PlayerPrefs.SetInt("colorCount",(int)colorCount.value);
        PlayerPrefs.SetInt("levelWidth",(int)levelWidth.value);
        PlayerPrefs.SetInt("levelHeight",(int)levelHeight.value);     
    }

    public void UpdateUI()
    {    
          
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }
}
