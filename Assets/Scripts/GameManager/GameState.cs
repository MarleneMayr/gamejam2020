using DG.Tweening;
using System.Collections;
using UnityEngine;

public class GameState : State
{
    [SerializeField] private Menu pauseMenu;

    [Header("GameObjects")]
    [SerializeField] private Goal goal;
    [SerializeField] private CharacterController player;
    [SerializeField] private Camera UIcam;
    [SerializeField] private VisualHints visualHints;
    [SerializeField] private float lightSeconds;
    [SerializeField] private float maxDistance;
    public AnimationCurve progressCurve;

    private Vector3 currentGoalPos;
    /// <summary>linear distance in the range [0..1]</summary>
    public static float distance { get; private set; }

    /// <summary>curve-influenced distance in the range [0..1]</summary>
    public static float progress { get; private set; }

    public static IntEvent FoundGoal = new IntEvent();
    private bool kinematic;

    private Level lvl;

    public override void AfterActivate()
    {
        LoadLevel();

        player.gameObject.SetActive(true);
        UIcam.gameObject.SetActive(false);
        visualHints.Activate();

        goal.StopEffects();
        goal.OnReached.AddListener(ReachedGoal);

        audioManager.StartMusic();
    }

    public override void BeforeDeactivate()
    {
        player.gameObject.SetActive(false);
        UIcam.gameObject.SetActive(true);
        visualHints.Deactivate();

        menu.Hide();
        pauseMenu.Hide();

        goal.OnReached.RemoveListener(ReachedGoal);
    }

    private void Update()
    {
        distance = CalculateDistance();
        if (!kinematic) progress = progressCurve.Evaluate(distance);
    }

    private void LoadLevel()
    {
        lvl = levelManager.GetNext();
        Spawn(lvl.spawnPoint); // spawn player
        currentGoalPos = goal.Spawn(lvl.GetNextGoal()); // spawn goal at new position

        ((GameMenu)menu).SetIconAmount(lvl.goalLocations.Length);
    }

    private float CalculateDistance()
    {
        float d = Vector3.Distance(currentGoalPos, player.transform.position);
        return Mathf.InverseLerp(0, maxDistance, d);
    }

    private void Spawn(Transform spawnPoint)
    {
        player.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
    }

    private void ReachedGoal()
    {
        FoundGoal?.Invoke(lvl.index);

        lvl.DeactivateEffects();
        if (lvl.isLastGoal())
        {
            stateMachine.GoTo<EndState>();
            return;
        }
        else
        {
            //menu.SetText(lvl.index.ToString());
            ((GameMenu)menu).UpdateIconCount(lvl.index);
            StartCoroutine(Euphoria(lightSeconds, 2));
            audioManager.PlayFoundShort();
        }
    }

    private IEnumerator Euphoria(float seconds, float fadeIn)
    {
        yield return new WaitForSeconds(seconds - 0.5f);
        goal.StopEffects();
        yield return new WaitForSeconds(0.5f);

        // spawn goal at new position
        currentGoalPos = goal.Spawn(lvl.GetNextGoal());

        // restart env effects
        kinematic = true;
        float targetProgress = progressCurve.Evaluate(CalculateDistance());
        DOTween.To(() => progress, x => progress = x, targetProgress, fadeIn);
        yield return new WaitForSeconds(fadeIn);

        kinematic = false;
    }
}
