using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Handle user notifications.
/// </summary>
public class UserNotification : MonoBehaviour {

    public static UserNotification instance;

    private Animator notificationAnimator;
    private TMP_Text notificationTxt;
    private GameObject notificationContainer;
    private IEnumerator notificationCoRoutine;
    private Animator errorAnimator;
    private TMP_Text errorTxt;
    private GameObject errorContainer;
    private IEnumerator errorCoRoutine;

    /// <summary>
    /// Notifications in queue currently.
    /// </summary>
    private Queue<string> notificationQueue = new Queue<string>();

    /// <summary>
    /// Is notification animator currently animating notification.
    /// </summary>
    private bool isAnimatingNotifications = false;

    [SerializeField] private Color errorColor;
    [SerializeField] private Color notificationColor;

    [SerializeField] private float errorDuration;
    [SerializeField] private float notificationDuration;


    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Debug.LogError("[UserErrorNotification]: Trying to instance but another instance already exists! [" + gameObject + "]");
            Destroy(gameObject);
        }
    }


    private void Start() {
        // Set notification references
        notificationTxt = ReferencesUI.instance.notificationText;
        notificationTxt.color = notificationColor;
        notificationAnimator = ReferencesUI.instance.notificationAnimator;
        notificationAnimator.speed = 1 / notificationDuration;
        notificationContainer = notificationAnimator.gameObject;

        // Set error references
        errorTxt = ReferencesUI.instance.errorNotificationText;
        errorTxt.color = errorColor;
        errorAnimator = ReferencesUI.instance.errorNotificationAnimator;
        errorAnimator.speed = 1 / errorDuration;
        errorContainer = errorAnimator.gameObject;

        TimeManager.instance.AddTimeScaleToggleHandler(UpdateAnimationSpeed);
    }


    private void Update() {
        if (GameManager.instance.devMode) {
            if (Input.GetKeyDown(KeyCode.F2))
                QueueNotification(NotificationCode.notEnoughCoffee);
            if (Input.GetKeyDown(KeyCode.F3))
                RaiseError(ErrorMsgCode.notEnoughCoffee);
        }
    }


    public void QueueNotification(NotificationCode notificationCode, (int, int)? waveInfo = null) {
        notificationQueue.Enqueue(Config.NotificationMessage(notificationCode, waveInfo));

        if (!isAnimatingNotifications)
            StartCoroutine(AnimateNotifications());
    }


    public void RaiseError(ErrorMsgCode _error) {
        // Make sure that previous error is finished and container is disabled
        if (errorCoRoutine != null) StopCoroutine(errorCoRoutine);
        errorContainer.SetActive(false);

        // Start animating error notification
        errorTxt.text = Config.ErrorMessage(_error);
        errorCoRoutine = AnimateText(errorAnimator, errorContainer);
        StartCoroutine(errorCoRoutine);
    }


    /// <summary>
    /// Update animation speed to match timescale.
    /// </summary>
    private void UpdateAnimationSpeed(float timescale) {
        float multiplier = 1 / timescale;
        errorAnimator.speed = 1 / errorDuration * multiplier;
        notificationAnimator.speed = 1 / notificationDuration * multiplier;
    }


    /// <summary>
    /// Animate all notifications in notificationQueue.
    /// </summary>
    private IEnumerator AnimateNotifications() {
        isAnimatingNotifications = true;

        // Loop trough every notification in queue
        while (notificationQueue.Count > 0) {
            // Animate notification and wait for it to finish
            notificationTxt.text = notificationQueue.Dequeue();
            notificationCoRoutine = AnimateText(notificationAnimator, notificationContainer);
            yield return StartCoroutine(notificationCoRoutine);
        }

        isAnimatingNotifications = false;
    }


    /// <summary>
    /// Animate text fadeout.
    /// </summary>
    private IEnumerator AnimateText(Animator _Animator, GameObject _Container) {
        _Container.SetActive(true);

        while (_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1) yield return null;

        _Container.SetActive(false);
    }
}
