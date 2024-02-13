/**
*  ---------------------------------------
*    Date      Author    Description
*  ---------------------------------------
*    2023-10-03    xxxxx    Initial version
*
*/

using System;
using System.Collections.Generic;
using Google.Protobuf;
using Messagebean;
using UnityEngine;
using YooAsset;
using static Define;

public class UIFriendCtrl : BaseCtrl
{
    private UIFriendView mUIFriendView;
    private AssetOperationHandle _friendHandle;
    public UIFriendCtrl(ModuleEnum moduleEnum) : base(moduleEnum)
    {

    }

    public override void OnInit()
    {
        EventManager.AddListener(NetCmd.MsgStcPlayerFind, OnFindFriendRsp);
        EventManager.AddListener(NetCmd.MsgStcFriendList, OnShowFriendListRsp);
        EventManager.AddListener(NetCmd.MsgStcFriendRecommend, OnShowApplyListRsp);
        EventManager.AddListener(NetCmd.MsgStcFriendAddReqList, OnRequestListRsp);
        EventManager.AddListener(NetCmd.MsgStcFriendAddReq, OnAddFriendRsp);
        EventManager.AddListener(NetCmd.MsgStcFriendAdd, OnAcceptFriendRsp);
        EventManager.AddListener(NetCmd.MsgStcFriendAddRefuse, OnRefuseFriendRsp);
        EventManager.AddListener(NetCmd.MsgStcFriendAddAcceptAll, OnAllAcceptFriendRsp);
        EventManager.AddListener(NetCmd.MsgStcFriendAddRefuseAll, OnAllRefuseFriendRsp);
        EventManager.AddListener(NetCmd.MsgStcFriendDel, OnDeleteFriendRsp);
    }

    public override void OnCreate()
    {
    }

    public override void OnShow(object userData)
    {
        if (mUIFriendView == null)
        {
            moduleView = mUIFriendView = new UIFriendView();
        }
        mUIFriendView.OnShow(userData);
        
        // friendCell
        _friendHandle = GameEntry.Res.LoadUIPrefab("UIFriendCell");
        var parent = new GameObject("FriendCellRoot");
        GameEntry.DataModel.Friend.FriendObjPool = new GameObjectPool((GameObject)_friendHandle.AssetObject, parent);

        var objList = new List<GameObject>();
        for (int i = 0; i < 10; i++)
        {
            var obj = GameEntry.DataModel.Friend.FriendObjPool.Get();
            objList.Add(obj);
            obj.GetComponent<FriendCellViewHolder>().Init(mUIFriendView);
        }

        foreach (var obj in objList)
        {
            GameEntry.DataModel.Friend.FriendObjPool.Release(obj);
        }
    }

    #region FriendList
    private void OnShowFriendListRsp(object sender, GameEventArgs e)
    {
        using (var rsp = MessagePool.Instance.Fetch<FriendList>())
        {
            NetPacketEventArgs packet = (NetPacketEventArgs)e;
            rsp.MergeFrom(packet.Buffer);
            
            GameEntry.DataModel.Friend.SaveFriendList(rsp);
            mUIFriendView?.ShowFriendList(1);
        }
    }
    private void OnDeleteFriendRsp(object sender, GameEventArgs e)
    {
        using (var rsp = MessagePool.Instance.Fetch<FriendOptReq>())
        {
            NetPacketEventArgs packet = (NetPacketEventArgs)e;
            rsp.MergeFrom(packet.Buffer);

            GameEntry.DataModel.Friend.RemoveFriendDataById(rsp.Uid[0]);
            mUIFriendView?.ShowFriendList(1);
        }
    }
    #endregion

    #region ApplyList
    private void OnShowApplyListRsp(object sender, GameEventArgs e)
    {
        using (var rsp = MessagePool.Instance.Fetch<FriendList>())
        {
            NetPacketEventArgs packet = (NetPacketEventArgs)e;
            rsp.MergeFrom(packet.Buffer);
            
            GameEntry.DataModel.Friend.SaveApplyList(rsp);
            mUIFriendView?.ShowApplyList();
        }
    }

    private void OnAddFriendRsp(object sender, GameEventArgs e)
    {
        using (var rsp = MessagePool.Instance.Fetch<FriendOptReq>())
        {
            NetPacketEventArgs packet = (NetPacketEventArgs)e;
            rsp.MergeFrom(packet.Buffer);
            
            GameEntry.DataModel.Friend.RemoveApplyDataById(rsp.Uid[0]);
            mUIFriendView?.ShowApplyList();
        }
    }
    private void OnFindFriendRsp(object sender, GameEventArgs e)
    {
        if(mUIFriendView == null)
            return;
        
        using (var rsp = MessagePool.Instance.Fetch<FriendOptReq>())
        {
            NetPacketEventArgs packet = (NetPacketEventArgs)e;
            rsp.MergeFrom(packet.Buffer);
            if (rsp.Uid.Count == 0)
            {
                GlobalManager.Instance.ShowTipLocalizeKey("");
            }
            else
            {
                var uId = rsp.Uid[0];
                MessageManager.SendFriendAddReq(uId);
            }
        }
    }
    #endregion

    #region RequestList
    private void OnRequestListRsp(object sender, GameEventArgs e)
    {
        using (var rsp = MessagePool.Instance.Fetch<FriendList>())
        {
            NetPacketEventArgs packet = (NetPacketEventArgs)e;
            rsp.MergeFrom(packet.Buffer);
            
            GameEntry.DataModel.Friend.SaveRequestList(rsp);
            mUIFriendView?.ShowRequestList(1);
        }
    }
    private void OnAcceptFriendRsp(object sender, GameEventArgs e)
    {
        if(mUIFriendView == null)
            return;
        
        using (var rsp = MessagePool.Instance.Fetch<FriendOptReq>())
        {
            NetPacketEventArgs packet = (NetPacketEventArgs)e;
            rsp.MergeFrom(packet.Buffer);
            
            GameEntry.DataModel.Friend.RemoveRequestDataById(rsp.Uid[0]);
            mUIFriendView?.ShowRequestList(1);
        }
    }
    private void OnAllAcceptFriendRsp(object sender, GameEventArgs e)
    {
        using (var rsp = MessagePool.Instance.Fetch<FriendOptReq>())
        {
            NetPacketEventArgs packet = (NetPacketEventArgs)e;
            rsp.MergeFrom(packet.Buffer);
            
            GameEntry.DataModel.Friend.RequestList.Clear();
            mUIFriendView?.ShowRequestList(1);
        }
    }
    private void OnRefuseFriendRsp(object sender, GameEventArgs e)
    {
        using (var rsp = MessagePool.Instance.Fetch<FriendOptReq>())
        {
            NetPacketEventArgs packet = (NetPacketEventArgs)e;
            rsp.MergeFrom(packet.Buffer);
            
            GameEntry.DataModel.Friend.RemoveRequestDataById(rsp.Uid[0]);
            mUIFriendView?.ShowRequestList(1);
        }
    }
    private void OnAllRefuseFriendRsp(object sender, GameEventArgs e)
    {
        using (var rsp = MessagePool.Instance.Fetch<FriendOptReq>())
        {
            NetPacketEventArgs packet = (NetPacketEventArgs)e;
            rsp.MergeFrom(packet.Buffer);
            
            GameEntry.DataModel.Friend.RequestList.Clear();
            mUIFriendView?.ShowRequestList(1);
        }
    }
    #endregion

    public override void OnDestroy(object userData)
    {
        if(mUIFriendView != null)
        {
            _friendHandle.Release();
            mUIFriendView.Close(userData);
			moduleView = mUIFriendView = null;
        }
    }
}