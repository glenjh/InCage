using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;
using Sequence = DG.Tweening.Sequence;

public class MainMenu : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject mainMenuPanel;
    [Space]
    
    [Header("Player Name")]
    public TMP_InputField nameInput;
    [Space] 
    
    [Header("Name Check")] 
    public TextMeshProUGUI chkText;
    
    [Header("Fox")]
    public Animator foxAnim;
    public Transform fox;
    
    [Header("Cage")]
    public Transform cage;
    public Transform targetPos;
    public Transform originPos;

    Sequence startSequence;
    
    public void SetPlayerName()
    {
        GameManager.instance.playerName = nameInput.text;
        PhotonNetwork.NickName = GameManager.instance.playerName;
    }

    public void NameCheck()
    {
        chkText.text = "Is \"" + nameInput.text + "\" is your name?";
    }

    public void OnStartBtnClicked()
    {
        mainMenuPanel.SetActive(false);
        
        startSequence = DOTween.Sequence().OnStart(() =>
        {
            cage.DOMove(targetPos.position, 0.45f).SetEase(Ease.OutQuart)
                .OnComplete(() =>
                {
                    foxAnim.Play("InAir");
                });
        }).SetDelay(1.5f)
            .Append(cage.DOMove(originPos.position, 0.8f).SetEase(Ease.OutBack))
            .Join(fox.DOMove(originPos.position, 0.8f).SetEase(Ease.OutBack))
            .OnComplete(() =>
            {
                LoadingSceneManager.instance.ChangeScene("Lobby");
            });
        
        startSequence.Play();
    }
}
