// -------------------------------------
//  Written by : Nicholas Ventimiglia for OneMinute GUI.
//  https://github.com/NVentimiglia/Unity3d-Databinding-Mvvm-Mvc
//  https://www.assetstore.unity3d.com/en/#!/content/32346
//  -------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// USE :
// 1) Augment MenuViewEnum with the views you need
// 2) Move MenuView into it's own File.
// 3) Place MenuView on the root of each view canvas
// 4) Place MenuManager with your viewmodel in a static location
// 5) Call MenuManager.Open(id) and MenuManager.Back() to transition between views.

/// <summary>
/// View Identifier
/// </summary>
public enum MenuViewEnum
{
    /// <summary>
    /// Initial View
    /// </summary>
    Intro,
    MainMenu,
    Credits,
    Options,
    Leaderboard,
    Maps,
    Quit,

    StoreItems,
    StoreMoney,

    GamePlay,
    GameScore,

    // Todo Add your own Views Here
}


/// <summary>
/// Place on the root of each managed view
/// </summary>
[AddComponentMenu("OMG/MenuView")]
public class MenuView : MonoBehaviour
{
    /// <summary>
    /// Unique Id for this view.
    /// </summary>
    public MenuViewEnum Id;


    // For resetting animator state on deactivation
    // Fixed Animator 'Stuck'
    private Animator[] Animators;
    void Awake()
    {
        // For resetting animator state on deactivation
        Animators = GetComponentsInChildren<Animator>(transform);
    }
    void OnEnable()
    {
        // Fixed Animator 'Stuck'
        for (int i = 0;i < Animators.Length;i++)
        {
            Animators[i].enabled = true;
        }
    }
    void OnDisable()
    {
        // Fixed Animator 'Stuck'
        for (int i = 0;i < Animators.Length;i++)
        {
            Animators[i].enabled = false;
        }

    }
}
/// <summary>
/// Static Menu Manager. Controls View Navigation.
/// </summary>
[AddComponentMenu("OMG/MenuManager")]
public class MenuManager : MonoBehaviour
{
    /// <summary>
    /// Static Instance
    /// </summary>
    private static MenuManager _instance;
    public static MenuManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MenuManager>();
            }
            return _instance;
        }
    }

    /// <summary>
    /// Static Instance
    /// </summary>
    public static readonly Dictionary<MenuViewEnum, GameObject> AllViews = new Dictionary<MenuViewEnum, GameObject>();

    /// <summary>
    /// Will search for child views from these locations on awake
    /// </summary>
    [Tooltip("Will search for child views from these locations on awake")]
    public GameObject[] MenuRoots;

    /// <summary>
    /// Animation Clip
    /// </summary>
    [Tooltip("Animation Clip")]
    public string AnimationPropertyName = "SlideOut";

    /// <summary>
    /// Back Button Support
    /// </summary>
    protected List<GameObject> NavigationHistory = new List<GameObject>();

    /// <summary>
    /// Returns to back in Navigation History.
    /// If Navigation History is null, goes to intro
    /// </summary>
    public void GoBack()
    {
        if (NavigationHistory.Count > 1)
        {
            var index = NavigationHistory.Count - 1;
            Animate(NavigationHistory[index - 1], true);

            var target = NavigationHistory[index];
            NavigationHistory.RemoveAt(index);
            Animate(target, false);
        }
        else
        {
            Open(MenuViewEnum.Intro);
        }
    }

    /// <summary>
    /// Close all Views
    /// </summary>
    public void CloseAll()
    {
        foreach (var view in AllViews)
        {
            view.Value.SetActive(false);
        }
    }

    /// <summary>
    /// Opens a specific View Object by its Id
    /// </summary>
    /// <param name="id"></param>
    public void Open(MenuViewEnum id)
    {
        GoToMenu(AllViews[id]);
    }

    /// <summary>
    /// Opens a specific View Object
    /// </summary>
    /// <param name="target"></param>
    public void GoToMenu(GameObject target)
    {
        if (target == null)
        {
            return;
        }

        if (NavigationHistory.Count > 0)
        {
            Animate(NavigationHistory[NavigationHistory.Count - 1], false);
        }

        NavigationHistory.Add(target);
        Animate(target, true);
    }

    /// <summary>
    /// Animates a View Object
    /// </summary>
    /// <param name="target"></param>
    /// <param name="direction"></param>
    public void Animate(GameObject target, bool direction)
    {
        if (target == null)
        {
            return;
        }

        target.SetActive(true);

        var canvasComponent = target.GetComponent<Canvas>();
        if (canvasComponent != null)
        {
            canvasComponent.overrideSorting = true;
            canvasComponent.sortingOrder = NavigationHistory.Count;
        }

        var animatorComponent = target.GetComponent<Animator>();
        if (animatorComponent != null)
        {
            animatorComponent.SetBool(AnimationPropertyName, direction);
            // Deactivate object when done !
            if (!direction)
                StartCoroutine(DeactivateViewAsync(target, animatorComponent));
        }
        else
        {
            // Deactivate object when done !
            if (!direction)
                target.SetActive(false);
        }
    }

    IEnumerator DeactivateViewAsync(GameObject view, Animator anim)
    {
        var state = anim.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(state.length);
        view.SetActive(false);
    }

    protected void Awake()
    {
        _instance = this;
        // Find Views
        for (int i = 0;i < MenuRoots.Length;i++)
        {
            var views = MenuRoots[i].GetComponentsInChildren<MenuView>(true);
            for (int j = 0;j < views.Length;j++)
            {
                var view = views[j];
                AllViews.Add(view.Id, view.gameObject);
            }
        }

        CloseAll();

        // Init
        Open(MenuViewEnum.Intro);
    }
}
