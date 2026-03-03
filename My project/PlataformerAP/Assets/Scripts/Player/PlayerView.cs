using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerView
{
    public Action OnFeedback;
    private Animator _animator;


    public PlayerView()
    {
        Debug.Log("PlayerView created");
    }

    public PlayerView SetAnimator(Animator animator)
    {
        _animator = animator;
        return this;
    }
    public void setHealFeedback(TextMeshProUGUI textHeal, Image healImage, float life)
    {
        textHeal.text =Convert.ToString(life);
        healImage.fillAmount = Convert.ToInt32(life) / 100f;

    }
    public void changeAnimation(string animName, bool bl)
    {
        if (_animator != null)
        {
            _animator.SetBool(animName, bl);
        }
    }
    public void changeAnimationTrigger(string name)
    {
        if (_animator != null)
            _animator.SetTrigger(name);
    }



}