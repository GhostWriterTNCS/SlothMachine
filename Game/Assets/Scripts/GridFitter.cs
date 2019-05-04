using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
[ExecuteInEditMode]
public class GridFitter : MonoBehaviour {
	GridLayoutGroup gridLayoutGroup;
	RectTransform rect;
	float width = 0;
	float height = 0;

	void Start() {
		gridLayoutGroup = GetComponent<GridLayoutGroup>();
		rect = GetComponent<RectTransform>();
		Fit();
	}

	void OnRectTransformDimensionsChange() {
		Fit();
	}

	void Fit() {
		if (gridLayoutGroup != null && rect != null) {
			if (gridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedColumnCount) {
				width = (rect.rect.width - gridLayoutGroup.padding.left - gridLayoutGroup.padding.right - gridLayoutGroup.spacing.x) / gridLayoutGroup.constraintCount;
			} else {
				width = gridLayoutGroup.cellSize.x;
			}
			if (gridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedRowCount) {
				height = (rect.rect.height - gridLayoutGroup.padding.top - gridLayoutGroup.padding.bottom - gridLayoutGroup.spacing.y) / gridLayoutGroup.constraintCount;
			} else {
				height = gridLayoutGroup.cellSize.y;
			}
			gridLayoutGroup.cellSize = new Vector2(width, height);
		}
	}
}
