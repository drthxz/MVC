using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendCellViewHolder : MonoBehaviour
{
    public UIFriendView view;
    [SerializeField]
    private GameObject _views;
    private FriendModel _data;
    private GameObject _friendGroup;
    private GameObject _AppliedGroup;
    private GameObject _RequestGroup;

    private Image _roleIcon;
    private TextMeshProUGUI _nameText;
    private TextMeshProUGUI _levelText;
    private Image _professionIcon;
    private TextMeshProUGUI _commentText;
    private GameObject _onlineObj;
    private GameObject _lastOnlineObj;
    private TextMeshProUGUI _lastOnlineText;

    private Button _friendDetailButton;
    private Button _friendDeleteButton;
    private Button _appliedAddButton;
    private Button _appliedDetailButton;
    private Button _requestDetailButton;
    private Button _requestAddButton;
    private Button _requestRefuseButton;
    
    public void Init(UIFriendView friendView)
    {
        view = friendView;

        _roleIcon = UIHelper.GetImageControl(_views, "IconBg/RoleIcon");
        _nameText = UIHelper.GetTextMeshControl(_views, "NameText");
        _levelText = UIHelper.GetTextMeshControl(_views, "LevelText");
        _professionIcon = UIHelper.GetImageControl(_views, "ProfessionIcon");
        _commentText = UIHelper.GetTextMeshControl(_views, "Comment/CommentText");
        _onlineObj = UIHelper.GetTextMeshControl(_views, "OnlineText").gameObject;
        _lastOnlineObj = UIHelper.GetTextMeshControl(_views, "LastOnline").gameObject;
        _lastOnlineText = UIHelper.GetTextMeshControl(_views, "LastOnline/LastOnlineText");

        _friendGroup = UIHelper.FindChild(_views, "FriendGroup");
        _friendDetailButton = UIHelper.GetButtonControl(_friendGroup, "Friend_DetailButton");
        _friendDeleteButton = UIHelper.GetButtonControl(_friendGroup, "DeleteButton");
        _AppliedGroup = UIHelper.FindChild(_views, "AppliedGroup");
        _appliedAddButton = UIHelper.GetButtonControl(_AppliedGroup, "Applied_AddButton");
        _appliedDetailButton = UIHelper.GetButtonControl(_AppliedGroup, "Applied_DetailButton");
        _RequestGroup = UIHelper.FindChild(_views, "RequestGroup");
        _requestDetailButton = UIHelper.GetButtonControl(_RequestGroup, "Request_DetailButton");
        _requestAddButton = UIHelper.GetButtonControl(_RequestGroup, "Request_AddButton");
        _requestRefuseButton = UIHelper.GetButtonControl(_RequestGroup, "RefuseButton");
        
        view.AddButtonClick(_friendDetailButton, () =>
        {
            view.ShowRoleDetail(_data);
        });
        view.AddButtonClick(_appliedDetailButton, () =>
        {
            view.ShowRoleDetail(_data);
        });
        view.AddButtonClick(_requestDetailButton, () =>
        {
            view.ShowRoleDetail(_data);
        });
        
        view.AddButtonClick(_friendDeleteButton, () =>
        {
            view.DeleteFriend(_data);
        });
        
        view.AddButtonClick(_appliedAddButton, () =>
        {
            view.AddFriend(_data);
        });
        view.AddButtonClick(_requestAddButton, () =>
        {
            view.AcceptFriend(_data);
        });
        view.AddButtonClick(_requestRefuseButton, () =>
        {
            view.RefuseFriend(_data);
        });
    }
    
    public void UpdateData(FriendModel itemInfo)
    {
        _data = itemInfo;

        var iconPath = string.Format("Friend/txx-{0}", itemInfo.Info.Profession);
        view.SetImageIcon(_roleIcon, iconPath);
        var professionPath = string.Format("Friend/zytb-{0}", itemInfo.Info.Profession);
        view.SetImageIcon(_professionIcon, professionPath);
        
        view.SetTextMesh(_nameText, itemInfo.Info.Name);
        view.SetTextMesh(_levelText, $"Lv {itemInfo.Info.Lev}");
        //view.SetTextMesh(_commentText, "");
        if (itemInfo.Info.State == 1)
        {
            _onlineObj.SetActive(true);
            _lastOnlineObj.SetActive(false);
        }
        else
        {
            _lastOnlineObj.SetActive(true);
            _onlineObj.SetActive(false);
            var time = Util.GetIntervalTimeSpan(itemInfo.Info.LastOfftime);
            if (time.Days > 0)
            {
                var days = time.Days > 99 ? 99 : time.Days; 
                view.SetTextMesh(_lastOnlineText, $"{days}D ago");
            }
            else if(time.Hours > 0)
            {
                view.SetTextMesh(_lastOnlineText, $"{time.Hours}h ago");
            }
            else if(time.Minutes > 0)
            {
                view.SetTextMesh(_lastOnlineText, $"{time.Minutes}m ago");
            }
        }

        _friendGroup.SetActive(false);
        _AppliedGroup.SetActive(false);
        _RequestGroup.SetActive(false);
        
        if (itemInfo.IsFriendList)
        {
            _friendGroup.SetActive(true);
        }
        else if (itemInfo.IsAppliedList)
        {
            _AppliedGroup.SetActive(true);
        }
        else if (itemInfo.IsRequestList)
        {
            _RequestGroup.SetActive(true);
        }
    }
}
