using UnityEngine;


[RequireComponent(typeof(Canvas))]
public class UICanvasSorting : MonoBehaviour, ILocalSorting
{
    [Range(-1, 30)]
    public int nSorting = 0;

    private Canvas mCanvas;

    private void TryFindComponents()
    {
        if (mCanvas == null)
            mCanvas = GetComponent<Canvas>();
    }

    public void SetRootSorting(int sorting)
    {
        TryFindComponents();
        if (mCanvas)
        {
            mCanvas.sortingOrder = nSorting + sorting;
        }
    }
}
