using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RequestItemController : MonoBehaviour
{
    [SerializeField] private TMP_Text _usernameText;
    [SerializeField] private Button _acceptButton;
    [SerializeField] private Button _rejectButton;
    
    private string _friendId;
    private FriendRequestManager _manager;

    public void Initialize(string friendId, string username, FriendRequestManager manager)
    {
        _friendId = friendId;
        _manager = manager;
        _usernameText.text = $"{username} quiere ser tu amigo";
        
        _acceptButton.onClick.AddListener(Accept);
        _rejectButton.onClick.AddListener(Reject);
    }

    private void Accept()
    {
        _manager.AcceptRequest(_friendId);
    }

    private void Reject()
    {
        _manager.RejectRequest(_friendId);
    }
}