using UnityEngine;

public class TextProperties : MonoBehaviour {
	public enum TextType {
		Normal,
		Header,
		Small,
		Expand,
		Custom
	}
	public TextType type;
	public bool useCustomColor;
	public bool noShadow;
	public bool useCustomShadow;
}
