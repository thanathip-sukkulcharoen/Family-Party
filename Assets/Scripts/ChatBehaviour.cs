using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using System;

public class ChatBehaviour : NetworkBehaviour
{
    [SerializeField] private GameObject chatUI = null;
    [SerializeField] private TMP_InputField inputField = null;
    [SerializeField] private TMP_Text chatText = null;
    [SerializeField] private CanvasGroup cg;
    [SyncVar] public string playerName;
    private static event Action<ChatBehaviour, string> OnMessage;

    public override void OnStartAuthority()
    {
        chatUI.SetActive(true);
        OnMessage += HandleNewMessage;
    }
    [ClientCallback]
    private void OnDestroy()
    {
        if (!hasAuthority) { return; }
        OnMessage -= HandleNewMessage;
    }

    private void HandleNewMessage(ChatBehaviour sender, string message)
    {
        sender.StopAllCoroutines();
        sender.chatText.text = message;
        sender.StartCoroutine(DeleteChatMessage(sender.cg));
    }

    [Client]
    public void Send(string message)
    {
        if (!Input.GetKeyDown(KeyCode.Return)) { return; }
        if (string.IsNullOrWhiteSpace(message)) { return; }
        CmdSendMessage(inputField.text);
        inputField.text = string.Empty;
    }

    [Command]
    private void CmdSendMessage(string message)
    {
        RpcHandleMessage($"{message}");
    }

    [ClientRpc]
    private void RpcHandleMessage(string message)
    {
        OnMessage?.Invoke(this, $"{message}");
    }

    private IEnumerator DeleteChatMessage(CanvasGroup cg)
    {
        cg.alpha = 1f;
        yield return new WaitForSeconds(2f);
        while (cg.alpha > 0)
        {
            cg.alpha -= 0.05f;
            yield return new WaitForSeconds(0.05f);
        }
    }
}
