using UnityEngine;
using UnityEngine.UI;

public class Flash : MonoBehaviour {
	public float flashSpeed = 3f;
	Text textField;

	void Start () {
		textField = GetComponent<Text>();		
	}

	public void FlashMessage(string msg) {
		textField.text = msg;
		textField.color = new Color(1, 1, 1, 1);
	}

	void Update () {
		float alpha = Mathf.Lerp(textField.color.a, 0, flashSpeed * Time.deltaTime);
		textField.color = new Color(textField.color.r, textField.color.g, textField.color.b, alpha);
	}
}
