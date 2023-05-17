using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class LevelSelectItem : MonoBehaviour
{
    public static int selectedLevel;
    public int levelID;
    public bool isLocked;
    public int numStars; 
    private RectTransform rectTransform;
    [SerializeField] public GameObject Text;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    public void setLevelSelectItem(int _levelID, bool _isLocked, int _numStars)
    {
        levelID = _levelID;
        isLocked = _isLocked;
        numStars = _numStars;
        Debug.Log("hi");
        Debug.Log(transform);
        Debug.Log(transform.GetComponentInChildren<Button>());
        Debug.Log(transform.GetComponentInChildren<Button>().GetComponentInChildren<TextMeshProUGUI>());
        //myText = GetComponent<TextMeshProUGUI>();
        //string textString = myText.text;
        //Debug.Log("Text: " + textString);
        //transform.GetComponentInChildren<Button>().GetComponentInChildren<Text>().text="Level "+levelID.ToString();
        //Text.GetComponent<Text>().text="Level "+levelID.ToString();
        transform.GetComponentInChildren<Button>().GetComponentInChildren<TextMeshProUGUI>().text="Level "+levelID.ToString();
    }   
    public void OnRectTransformDimensionsChange()
    {
        //rectTransform = GetComponent<RectTransform>();
        //Debug.Log("Rezise");
        //float scaleFactor = rectTransform.rect.width / rectTransform.rect.height;
//
        //foreach (Transform child in transform)
        //{
        //    RectTransform childRectTransform = child.GetComponent<RectTransform>();
        //    if (childRectTransform != null)
        //    {
        //        childRectTransform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
        //        Vector2 childAnchoredPosition = childRectTransform.anchoredPosition;
        //        childRectTransform.sizeDelta = new Vector2(childRectTransform.sizeDelta.x / childRectTransform.localScale.x, childRectTransform.sizeDelta.y / childRectTransform.localScale.y);
        //        childRectTransform.anchoredPosition = childAnchoredPosition;
        //    }
        //}
    }

    public void SelectLevel()
    {
        selectedLevel = levelID;
        SceneManager.LoadScene("Gameplay");
    }
}
