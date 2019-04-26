using UnityEngine;

public class TextProperties : MonoBehaviour {
	public enum TextType {
		Normal,
		Header,
		Expand
	}
	public TextType type;
	public bool useCustomColor;
}
