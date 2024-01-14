using UnityEngine;
using UnityEngine.Rendering;

[DisallowMultipleComponent]
[RequireComponent(typeof(SortingGroup))]
public class UILocalSortingGroup : MonoBehaviour, ILocalSorting
{
    [Range(-1, 30)]
    public int nSorting = 0;

    public SortingGroup mSortingGroup;

    private void TryFindComponents()
    {
        if (mSortingGroup == null)
            mSortingGroup = GetComponent<SortingGroup>();
    }

    public void SetRootSorting(int sorting)
    {
        TryFindComponents();
        if (mSortingGroup)
        {
            mSortingGroup.sortingOrder = nSorting + sorting;
        }
    }
}
