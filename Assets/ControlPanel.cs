using UnityEngine;

public class ControlPanel : MonoBehaviour {

    public float widthPercentage = 0.4f;

    bool expanded = true;
    RectTransform rt;
    RectTransform parent;
	// Use this for initialization
	void Start () {
        Canvas cvs = GetComponentInParent<Canvas>();
        parent = cvs.GetComponent<RectTransform>();
        rt = GetComponent<RectTransform>();

        float width = Mathf.Clamp(parent.sizeDelta.x * widthPercentage, 280f, 350f);
        float height = parent.sizeDelta.y;
        float x = parent.sizeDelta.x / 2;
        rt.sizeDelta = new Vector2(width, height);
        rt.localPosition = new Vector3(x, 0, 0);
    }

    void Update () {
        float x;
        float width = Mathf.Clamp(parent.sizeDelta.x * widthPercentage, 280f, 350f);
        float height = parent.sizeDelta.y;
        if (expanded)
        {
            x = parent.sizeDelta.x / 2;
        }
        else
        {
            x = parent.sizeDelta.x / 2 + width;
        }
        rt.sizeDelta = new Vector2(width, height);

        Vector3 currentPosition = rt.localPosition;
        rt.localPosition = Vector3.Lerp(currentPosition, new Vector3(x, 0, 0), 0.25f);

        if (Input.GetKeyDown(KeyCode.F12))
        {
            expanded = !expanded;
        }
    }
}
