using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEvent : UnityEvent<string>
{
    
}

public class WeaponAnimationEvents : MonoBehaviourPun
{
    public AnimationEvent WeaponAnimationEvent = new AnimationEvent();
    public Animator riggingAnimator { private set; get; }

    private void Start()
    {
        riggingAnimator = GetComponent<Animator>();
    }
    public void OnAnimationEvent(string eventName)
    {
        if(photonView.IsMine)
            WeaponAnimationEvent.Invoke(eventName);
    }
}
