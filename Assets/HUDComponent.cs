using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDComponent : MonoBehaviour
{
    // Start is called before the first frame update
    /// <summary>
    /// Base class for a script that controls a specific part of the HUD.
    /// </summary>
    [Header("HUD Component")]

    [Tooltip("The camera that is displaying this HUD component.")]
    [SerializeField]
    protected Camera hudCamera;
    public Camera HUDCamera
    {
        get { return hudCamera; }
        set
        {
            hudCamera = value;
            foreach (Canvas canvas in canvases)
            {
                canvas.worldCamera = hudCamera;
            }
        }
    }

    protected List<Canvas> canvases = new List<Canvas>();
    
    [Tooltip("Whether to activate this HUD component when the scene starts.")]
    [SerializeField]
    protected bool activateOnAwake = false;

    [Tooltip("Whether to update this HUD component every frame. Used when it is not being managed by a HUD Manager component.")]
    [SerializeField]
    protected bool updateIndividuallyEveryFrame = false;

    [SerializeField]
    protected Vector3 parentToCameraOffset;

    protected bool activated = false;
    public virtual bool Activated { get { return activated; } }


    protected virtual void Awake()
    {
        if (activateOnAwake)
        {
            Activate();
        }

        canvases = new List<Canvas>(GetComponentsInChildren<Canvas>(true));
    }

    /// <summary>
    /// Activate this HUD Component
    /// </summary>
    public virtual void Activate()
    {
        gameObject.SetActive(true);
        activated = true;
    }

    /// <summary>
    /// Deactivate this HUD component
    /// </summary>
    public virtual void Deactivate()
    {
        gameObject.SetActive(false);
        activated = false;
    }


    /// <summary>
    /// Parent this HUD component to a specified transform.
    /// </summary>
    /// <param name="parentTransform">The parent transform.</param>
    public virtual void ParentToTransform(Transform parentTransform)
    {
        transform.SetParent(parentTransform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    /// <summary>
    /// Parent this HUD component to the HUD camera.
    /// </summary>
    public virtual void ParentToCamera()
    {
        ParentToTransform(hudCamera.transform);
        transform.localPosition = parentToCameraOffset;
    }


    /// <summary>
    /// Clear the parent of this HUD component.
    /// </summary>
    public virtual void ClearParent()
    {
        transform.parent = null;
    }

    /// <summary>
    /// Called to update this HUD Component.
    /// </summary>
    public virtual void OnUpdateHUD() { }


    // Called every frame
    protected virtual void Update()
    {
        if (updateIndividuallyEveryFrame && activated)
        {
            OnUpdateHUD();
        }
    }
}
