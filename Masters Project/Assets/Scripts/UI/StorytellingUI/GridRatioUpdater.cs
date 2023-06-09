using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridRatioUpdater : MonoBehaviour
{
    private GridLayoutGroup grid;
    private RectTransform t;

    [SerializeField] private int maxRows = 1;
    [SerializeField] private int maxCols = 1;

    private void OnEnable()
    {
        grid = GetComponent<GridLayoutGroup>(); 
        t = GetComponent<RectTransform>();

        float width = t.rect.width - ((maxCols-1)*grid.spacing.x);
        float height = t.rect.height - ((maxRows - 1) * grid.spacing.y);
        Vector2 temp = new Vector2(width, height);

        grid.cellSize = new Vector2(temp.x / maxCols, temp.y / maxRows);
    }

}
