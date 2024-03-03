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
    public void setLevelSelectItem(int _levelID, int _numStars)
    {

        levelID = _levelID;
        numStars = _numStars;
        isLocked = numStars < 0;

        transform.GetComponentInChildren<Button>().GetComponentInChildren<TextMeshProUGUI>().text = "Level " + levelID.ToString();

        if (isLocked)
        {
            transform.Find("lock").gameObject.SetActive(true);
            transform.Find("Button").gameObject.SetActive(false);

            setStars(numStars);
        }
        else
        {
            transform.Find("lock").gameObject.SetActive(false);
            transform.Find("Button").gameObject.SetActive(true);

            setStars(numStars);
        }

    }
    public void setStars(int numStars)
    {
        transform.Find("star1").gameObject.SetActive(false);
        transform.Find("star2").gameObject.SetActive(false);
        transform.Find("star3").gameObject.SetActive(false);
        if (numStars >= 1) { transform.Find("star1").gameObject.SetActive(true); } else { transform.Find("star1").gameObject.SetActive(false); }
        if (numStars >= 2) { transform.Find("star2").gameObject.SetActive(true); } else { transform.Find("star2").gameObject.SetActive(false); }
        if (numStars >= 3) { transform.Find("star3").gameObject.SetActive(true); } else { transform.Find("star3").gameObject.SetActive(false); }

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
        Debug.Log("===============++++++++++++");
        selectedLevel = levelID;
        SceneManager.LoadScene("Gameplay");
    }
}
