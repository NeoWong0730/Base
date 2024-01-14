using UnityEngine;

public class UILocalSorting : MonoBehaviour, ILocalSorting
{
    [Range(-1, 5)]
    public int nSorting = 0;

    public ParticleSystemRenderer mParticleSystemRenderer;
    public TrailRenderer trailRenderer;
    public MeshRenderer meshRenderer;
    public SpriteRenderer spriteRenderer;
    private Canvas mCanvas;

    private void TryFindComponents()
    {
        if (this.mParticleSystemRenderer == null)
        {
            this.mParticleSystemRenderer = this.GetComponent<ParticleSystemRenderer>();
        }
        if (this.trailRenderer == null)
        {
            this.trailRenderer = this.GetComponent<TrailRenderer>();
        }
        if (this.mCanvas == null)
        {
            this.mCanvas = this.GetComponentInParent<Canvas>();
        }
        if (this.meshRenderer == null)
        {
            this.meshRenderer = this.GetComponent<MeshRenderer>();
        }
        if (this.spriteRenderer == null)
        {
            this.spriteRenderer = this.GetComponent<SpriteRenderer>();
        }
    }

    public void SetRootSorting(int sorting)
    {
        this.TryFindComponents();
        if (this.mParticleSystemRenderer)
        {
            this.mParticleSystemRenderer.sortingOrder = this.nSorting + sorting;
        }
        if (this.trailRenderer)
        {
            this.trailRenderer.sortingOrder = this.nSorting + sorting;
        }
        if (this.mCanvas)
        {
            if (!mCanvas.isRootCanvas)
            {
                this.mCanvas.sortingOrder = this.nSorting + sorting;
            }
        }
        if (this.meshRenderer)
        {
            this.meshRenderer.sortingOrder = this.nSorting + sorting;
        }
        if (this.spriteRenderer)
        {
            this.spriteRenderer.sortingOrder = this.nSorting + sorting;
        }
    }
}
