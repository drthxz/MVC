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
using Messagebean;

public class FriendModel
{
    public int Id;
    public FriendInfo Info;
    public bool IsFriendList;
    public bool IsAppliedList;
    public bool IsRequestList;
}

public class UIFriendModel : BaseModel
{
    private List<FriendModel> _friendList = new List<FriendModel>();
    public List<FriendModel> FriendList
    {
        get { return _friendList; }
    }
    private List<FriendModel> _applyList = new List<FriendModel>();
    public List<FriendModel> ApplyList
    {
        get { return _applyList; }
    }
    private List<FriendModel> _requestList = new List<FriendModel>();
    public List<FriendModel> RequestList
    {
        get { return _requestList; }
    }
    public GameObjectPool FriendObjPool;

    /// <summary>
    /// init game data
    /// </summary>
    public override void Init()
    {
    }

    public void SaveFriendList(FriendList rsp)
    {
        _friendList.Clear();

        List<FriendModel> tempList = new List<FriendModel>();
        for (int i = 0; i < rsp.FriendInfos.Count; i++)
        {
            tempList.Add(new FriendModel()
            {
                Id = i,
                Info = rsp.FriendInfos[i],
                IsFriendList = true,
                IsAppliedList = false,
                IsRequestList = false
            });
        }
 
        _friendList = tempList.OrderByDescending(x => x.Info.State == 1).ThenByDescending(x => x.Info.Lev).ThenBy(x => x.Info.Name).ToList();
    }
    public void RemoveFriendDataById(int id)
    {
        var data = _friendList.FirstOrDefault(x => x.Info.Uid == id);
        _friendList.Remove(data);
    }

    public void SaveApplyList(FriendList rsp)
    {
        _applyList.Clear();
        
        List<FriendModel> tempList = new List<FriendModel>();
        for (int i = 0; i < rsp.FriendInfos.Count; i++)
        {
            tempList.Add(new FriendModel()
            {
                Id = i,
                Info = rsp.FriendInfos[i],
                IsFriendList = false,
                IsAppliedList = true,
                IsRequestList = false
            });
        }
        _applyList = tempList.OrderByDescending(x => x.Info.Lev).ThenBy(x => x.Info.Name).ToList();
    }

    public void RemoveApplyDataById(int id)
    {
        var data = _applyList.FirstOrDefault(x => x.Info.Uid == id);
        _applyList.Remove(data);
    }

    public void SaveRequestList(FriendList rsp)
    {
        _requestList.Clear();
        
        List<FriendModel> tempList = new List<FriendModel>();
        for (int i = 0; i < rsp.FriendInfos.Count; i++)
        {
            tempList.Add(new FriendModel()
            {
                Id = i,
                Info = rsp.FriendInfos[i],
                IsFriendList = false,
                IsAppliedList = false,
                IsRequestList = true
            });
        }
        _requestList = tempList.OrderByDescending(x => x.Info.Createtime).ToList();
        foreach (var friend in _friendList)
        {
            var request = _requestList.FirstOrDefault(x => x.Info.Uid == friend.Info.Uid);
            if (request != null)
            {
                _requestList.Remove(request);
            }
        }
    }

    public void RemoveRequestDataById(int id)
    {
        var data = _requestList.FirstOrDefault(x => x.Info.Uid == id);
        _requestList.Remove(data);
    }

    /// <summary>
    /// reset game data
    /// </summary>
    public override void Reset()
    {
    }
}