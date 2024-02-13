/**
*  ---------------------------------------
*    Date      Author    Description
*  ---------------------------------------
*    2023-10-03    xxxxx    Initial version
*
*/

using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public partial class UIFriendView
{
    private TextMeshProUGUI _friendNoFriendText;
    private GameObject _friendBottomObj;
    private TextMeshProUGUI _requestNoFriendText;
    private GameObject _requestBottomObj;
    private int _maxPage, _currentPage;
    private List<GameObject> _tempFriendList = new List<GameObject>();

    public override void OnShow(object userData)
    {
        CreateView("UIFriendView", CoreUILayer.Low, userData);
    }

    public override void OnCreate(object userData)
    {
        _friendNoFriendText = UIHelper.GetTextMeshControl(mFriendGroup, "Friend_NoFriendText");
        _friendBottomObj = Util.Child(mFriendGroup, "Bottom");
        _requestNoFriendText = UIHelper.GetTextMeshControl(mRequestGroup, "Request_NoFriendText");
        _requestBottomObj = Util.Child(mRequestGroup, "Bottom");
        
        mAppliedGroup.SetActive(false);
        mRequestGroup.SetActive(false);
        
        // get firend list
        MessageManager.SendFriendListReq();
    }

    public override void OnClick(string name)
    {
        if (name == "CloseButton")
        {
            UIManager.ClosePopup(ModuleEnum.UIFriend);
        }

        #region Friend List
        if (name == "Friend_PreButton")
        {
            if (_currentPage <= 1)
            {
                GlobalManager.Instance.ShowTipLocalizeKey("");
                return;
            }
                
            _currentPage--;
            ShowFriendList(_currentPage);
        }

        if (name == "Friend_NextButton")
        {
            if (_currentPage >= _maxPage)
            {
                GlobalManager.Instance.ShowTipLocalizeKey("");
                return;
            }
            
            _currentPage++;
            ShowFriendList(_currentPage);
        }
        #endregion

        #region Applied List
        if (name == "RefreshButton")
        {
            MessageManager.SendFriendRecommendReq();
        }

        if (name == "FindButton")
        {
            MessageManager.SendFriendFindReq(mSearchText.text);
        }
        #endregion

        #region Request List
        if (name == "RefuseAllButton")
        {
            int[] friendIds = new int[GameEntry.DataModel.Friend.RequestList.Count];
            for (int i = 0; i < GameEntry.DataModel.Friend.RequestList.Count; i++)
            {
                friendIds[i] = GameEntry.DataModel.Friend.RequestList[i].Info.Uid;
            }
            MessageManager.SendFriendAllRefuseReq(friendIds);
        }

        if (name == "AgreedAllButton")
        {
            int[] friendIds = new int[GameEntry.DataModel.Friend.RequestList.Count];
            for (int i = 0; i < GameEntry.DataModel.Friend.RequestList.Count; i++)
            {
                friendIds[i] = GameEntry.DataModel.Friend.RequestList[i].Info.Uid;
            }
            MessageManager.SendFriendAllAcceptReq(friendIds);
        }
        
        if (name == "Request_PreButton")
        {
            if (_currentPage <= 1)
            {
                GlobalManager.Instance.ShowTipLocalizeKey("");
                return;
            }
            
            _currentPage--;
            ShowRequestList(_currentPage);
        }

        if (name == "Request_NextButton")
        {
            if (_currentPage >= _maxPage)
            {
                GlobalManager.Instance.ShowTipLocalizeKey("");
                return;
            }
            
            _currentPage++;
            ShowRequestList(_currentPage);
        }
        #endregion
    }

    public override void OnToggleChanged(string name, bool isOn)
    {
        switch (name)
        {
            case "FriendToggle":
                if(isOn)
                {
                    mFriendToggle.interactable = false;
                    MessageManager.SendFriendListReq();
                }
                else
                {
                    mFriendToggle.interactable = true;
                    mFriendGroup.SetActive(false);
                }
                break;
            case "AppliedToggle":
                if(isOn)
                {
                    mAppliedToggle.interactable = false;
                    MessageManager.SendFriendRecommendReq();
                }
                else
                {
                    mAppliedToggle.interactable = true;
                    mAppliedGroup.SetActive(false);
                }
                break;
            case "RequestToggle":
                if(isOn)
                {
                    mRequestToggle.interactable = false;
                    MessageManager.SendFriendApplyListReq();
                }
                else
                {
                    mRequestToggle.interactable = true;
                    mRequestGroup.SetActive(false);
                }
                break;
        }
    }

    public void ShowFriendList(int page)
    {
        foreach (var obj in _tempFriendList)
        {
            GameEntry.DataModel.Friend.FriendObjPool.Release(obj);
        }
        _tempFriendList.Clear();
        
        var list = GameEntry.DataModel.Friend.FriendList;
        mFriendGroup.SetActive(true);
        SetTextMesh(mFriend_FriendCount, $"{list.Count}/100");
        var onlineCount = list.Select(x => x.Info.State == 1).Count();
        SetTextMesh(mFriend_OnlineCount, onlineCount.ToString());
        if (list.Count == 0)
        {
            _friendNoFriendText.SetActive(true);
            _friendBottomObj.SetActive(false);
        }
        else
        {
            _friendNoFriendText.SetActive(false);
            _friendBottomObj.SetActive(true);
            
            _maxPage = (int)Mathf.Ceil(list.Count / 10f);
            _currentPage = page;
            SetTextMesh(mFriend_Page, $"{page}/{_maxPage}");
        
            var minCount = (page - 1) * 10;
            var maxCount = minCount + (list.Count % 10);

            for (int i = minCount; i < maxCount; i++)
            {
                var obj = GameEntry.DataModel.Friend.FriendObjPool.Get(mContent.transform);
                _tempFriendList.Add(obj);
                var cell = obj.GetComponent<FriendCellViewHolder>();
                cell.UpdateData(list[i]);
            }
        }
    }

    public void ShowApplyList()
    {
        foreach (var obj in _tempFriendList)
        {
            GameEntry.DataModel.Friend.FriendObjPool.Release(obj);
        }
        _tempFriendList.Clear();

        var list = GameEntry.DataModel.Friend.ApplyList;
  
        mAppliedGroup.SetActive(true);
        for (int i = 0; i < list.Count; i++)
        {
            var obj = GameEntry.DataModel.Friend.FriendObjPool.Get(mContent.transform);
            _tempFriendList.Add(obj);
            var cell = obj.GetComponent<FriendCellViewHolder>();
            cell.UpdateData(list[i]);
        }
    }

    public void ShowRequestList(int page)
    {
        foreach (var obj in _tempFriendList)
        {
            GameEntry.DataModel.Friend.FriendObjPool.Release(obj);
        }
        _tempFriendList.Clear();
        
        var list = GameEntry.DataModel.Friend.RequestList;
        mRequestGroup.SetActive(true);
        SetTextMesh(mRequest_FriendCount, $"{list.Count}/100");
        var onlineCount = list.Select(x => x.Info.State == 1).Count();
        SetTextMesh(mRequest_OnlineCount, onlineCount.ToString());
        if (list.Count == 0)
        {
            _requestNoFriendText.SetActive(true);
            _requestBottomObj.SetActive(false);
        }
        else
        {
            _requestNoFriendText.SetActive(false);
            _requestBottomObj.SetActive(true);
            
            _maxPage = (int)Mathf.Ceil(list.Count / 10f);
            _currentPage = page;
            SetTextMesh(mFriend_Page, $"{page}/{_maxPage}");

            var minCount = (page - 1) * 10;
            var maxCount = minCount + (list.Count % 10);

            for (int i = minCount; i < maxCount; i++)
            {
                var obj = GameEntry.DataModel.Friend.FriendObjPool.Get(mContent.transform);
                _tempFriendList.Add(obj);
                var cell = obj.GetComponent<FriendCellViewHolder>();
                cell.UpdateData(list[i]);
            }
        }
    }

    public void ShowRoleDetail(FriendModel data)
    {
        
    }

    public void DeleteFriend(FriendModel data)
    {
        MessageManager.SendFriendDeleteReq(data.Info.Uid);
    }

    public void AddFriend(FriendModel data)
    {
        if (GameEntry.DataModel.Friend.FriendList.Count >= 100)
        {
            GlobalManager.Instance.ShowTipLocalizeKey("");
            return;
        }
        
        MessageManager.SendFriendAddReq(data.Info.Uid);
    }

    public void AcceptFriend(FriendModel data)
    {
        if (GameEntry.DataModel.Friend.FriendList.Count >= 100)
        {
            GlobalManager.Instance.ShowTipLocalizeKey("");
            return;
        }
        
        MessageManager.SendFriendAcceptReq(data.Info.Uid);
    }

    public void RefuseFriend(FriendModel data)
    {
        MessageManager.SendFriendRefuseReq(data.Info.Uid);
    }

    public override void OnClose(object userData)
    {
        GameEntry.DataModel.Friend.FriendObjPool.Clear();
        _tempFriendList.Clear();
        
        base.OnClose(userData);
    }
}