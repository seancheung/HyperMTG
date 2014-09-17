﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.18063
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace HyperMTG.DraftSVC {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="DraftSVC.IDraft", CallbackContract=typeof(HyperMTG.DraftSVC.IDraftCallback), SessionMode=System.ServiceModel.SessionMode.Required)]
    public interface IDraft {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDraft/Connect", ReplyAction="http://tempuri.org/IDraft/ConnectResponse")]
        void Connect(HyperService.Common.Client client);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IDraft/Connect", ReplyAction="http://tempuri.org/IDraft/ConnectResponse")]
        System.IAsyncResult BeginConnect(HyperService.Common.Client client, System.AsyncCallback callback, object asyncState);
        
        void EndConnect(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IDraft/SetMaxPlayers")]
        void SetMaxPlayers(int count);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, AsyncPattern=true, Action="http://tempuri.org/IDraft/SetMaxPlayers")]
        System.IAsyncResult BeginSetMaxPlayers(int count, System.AsyncCallback callback, object asyncState);
        
        void EndSetMaxPlayers(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IDraft/SendMsg")]
        void SendMsg(HyperService.Common.Message msg);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, AsyncPattern=true, Action="http://tempuri.org/IDraft/SendMsg")]
        System.IAsyncResult BeginSendMsg(HyperService.Common.Message msg, System.AsyncCallback callback, object asyncState);
        
        void EndSendMsg(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IDraft/StartDraft")]
        void StartDraft(System.Collections.Generic.List<string> setCodes);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, AsyncPattern=true, Action="http://tempuri.org/IDraft/StartDraft")]
        System.IAsyncResult BeginStartDraft(System.Collections.Generic.List<string> setCodes, System.AsyncCallback callback, object asyncState);
        
        void EndStartDraft(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IDraft/SwitchPack")]
        void SwitchPack(HyperService.Common.Client client, System.Collections.Generic.List<string> cardIDs);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, AsyncPattern=true, Action="http://tempuri.org/IDraft/SwitchPack")]
        System.IAsyncResult BeginSwitchPack(HyperService.Common.Client client, System.Collections.Generic.List<string> cardIDs, System.AsyncCallback callback, object asyncState);
        
        void EndSwitchPack(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IDraft/EndDraft")]
        void EndDraft();
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, AsyncPattern=true, Action="http://tempuri.org/IDraft/EndDraft")]
        System.IAsyncResult BeginEndDraft(System.AsyncCallback callback, object asyncState);
        
        void EndEndDraft(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, IsTerminating=true, Action="http://tempuri.org/IDraft/Disconnect")]
        void Disconnect(HyperService.Common.Client client);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, IsTerminating=true, AsyncPattern=true, Action="http://tempuri.org/IDraft/Disconnect")]
        System.IAsyncResult BeginDisconnect(HyperService.Common.Client client, System.AsyncCallback callback, object asyncState);
        
        void EndDisconnect(System.IAsyncResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IDraftCallback {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IDraft/RefreshMaxPlayers")]
        void RefreshMaxPlayers(int count);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, AsyncPattern=true, Action="http://tempuri.org/IDraft/RefreshMaxPlayers")]
        System.IAsyncResult BeginRefreshMaxPlayers(int count, System.AsyncCallback callback, object asyncState);
        
        void EndRefreshMaxPlayers(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IDraft/RefreshClients")]
        void RefreshClients(System.Collections.Generic.List<HyperService.Common.Client> clients);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, AsyncPattern=true, Action="http://tempuri.org/IDraft/RefreshClients")]
        System.IAsyncResult BeginRefreshClients(System.Collections.Generic.List<HyperService.Common.Client> clients, System.AsyncCallback callback, object asyncState);
        
        void EndRefreshClients(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IDraft/OnConnect")]
        void OnConnect(HyperService.Common.ConnectionResult result);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, AsyncPattern=true, Action="http://tempuri.org/IDraft/OnConnect")]
        System.IAsyncResult BeginOnConnect(HyperService.Common.ConnectionResult result, System.AsyncCallback callback, object asyncState);
        
        void EndOnConnect(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IDraft/OnReceive")]
        void OnReceive(HyperService.Common.Message msg);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, AsyncPattern=true, Action="http://tempuri.org/IDraft/OnReceive")]
        System.IAsyncResult BeginOnReceive(HyperService.Common.Message msg, System.AsyncCallback callback, object asyncState);
        
        void EndOnReceive(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IDraft/OnJoin")]
        void OnJoin(HyperService.Common.Client client);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, AsyncPattern=true, Action="http://tempuri.org/IDraft/OnJoin")]
        System.IAsyncResult BeginOnJoin(HyperService.Common.Client client, System.AsyncCallback callback, object asyncState);
        
        void EndOnJoin(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IDraft/OnLeave")]
        void OnLeave(HyperService.Common.Client client);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, AsyncPattern=true, Action="http://tempuri.org/IDraft/OnLeave")]
        System.IAsyncResult BeginOnLeave(HyperService.Common.Client client, System.AsyncCallback callback, object asyncState);
        
        void EndOnLeave(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IDraft/OnPick")]
        void OnPick(HyperService.Common.Client client);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, AsyncPattern=true, Action="http://tempuri.org/IDraft/OnPick")]
        System.IAsyncResult BeginOnPick(HyperService.Common.Client client, System.AsyncCallback callback, object asyncState);
        
        void EndOnPick(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IDraft/OnWait")]
        void OnWait();
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, AsyncPattern=true, Action="http://tempuri.org/IDraft/OnWait")]
        System.IAsyncResult BeginOnWait(System.AsyncCallback callback, object asyncState);
        
        void EndOnWait(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IDraft/OnSwitchPack")]
        void OnSwitchPack(System.Collections.Generic.List<string> cardIDs);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, AsyncPattern=true, Action="http://tempuri.org/IDraft/OnSwitchPack")]
        System.IAsyncResult BeginOnSwitchPack(System.Collections.Generic.List<string> cardIDs, System.AsyncCallback callback, object asyncState);
        
        void EndOnSwitchPack(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IDraft/OnOpenBooster")]
        void OnOpenBooster(string setCode);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, AsyncPattern=true, Action="http://tempuri.org/IDraft/OnOpenBooster")]
        System.IAsyncResult BeginOnOpenBooster(string setCode, System.AsyncCallback callback, object asyncState);
        
        void EndOnOpenBooster(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IDraft/OnStartDraft")]
        void OnStartDraft();
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, AsyncPattern=true, Action="http://tempuri.org/IDraft/OnStartDraft")]
        System.IAsyncResult BeginOnStartDraft(System.AsyncCallback callback, object asyncState);
        
        void EndOnStartDraft(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IDraft/OnEndDraft")]
        void OnEndDraft();
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, AsyncPattern=true, Action="http://tempuri.org/IDraft/OnEndDraft")]
        System.IAsyncResult BeginOnEndDraft(System.AsyncCallback callback, object asyncState);
        
        void EndOnEndDraft(System.IAsyncResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IDraftChannel : HyperMTG.DraftSVC.IDraft, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class DraftClient : System.ServiceModel.DuplexClientBase<HyperMTG.DraftSVC.IDraft>, HyperMTG.DraftSVC.IDraft {
        
        private BeginOperationDelegate onBeginConnectDelegate;
        
        private EndOperationDelegate onEndConnectDelegate;
        
        private System.Threading.SendOrPostCallback onConnectCompletedDelegate;
        
        private BeginOperationDelegate onBeginSetMaxPlayersDelegate;
        
        private EndOperationDelegate onEndSetMaxPlayersDelegate;
        
        private System.Threading.SendOrPostCallback onSetMaxPlayersCompletedDelegate;
        
        private BeginOperationDelegate onBeginSendMsgDelegate;
        
        private EndOperationDelegate onEndSendMsgDelegate;
        
        private System.Threading.SendOrPostCallback onSendMsgCompletedDelegate;
        
        private BeginOperationDelegate onBeginStartDraftDelegate;
        
        private EndOperationDelegate onEndStartDraftDelegate;
        
        private System.Threading.SendOrPostCallback onStartDraftCompletedDelegate;
        
        private BeginOperationDelegate onBeginSwitchPackDelegate;
        
        private EndOperationDelegate onEndSwitchPackDelegate;
        
        private System.Threading.SendOrPostCallback onSwitchPackCompletedDelegate;
        
        private BeginOperationDelegate onBeginEndDraftDelegate;
        
        private EndOperationDelegate onEndEndDraftDelegate;
        
        private System.Threading.SendOrPostCallback onEndDraftCompletedDelegate;
        
        private BeginOperationDelegate onBeginDisconnectDelegate;
        
        private EndOperationDelegate onEndDisconnectDelegate;
        
        private System.Threading.SendOrPostCallback onDisconnectCompletedDelegate;
        
        public DraftClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public DraftClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public DraftClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public DraftClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public DraftClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> ConnectCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> SetMaxPlayersCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> SendMsgCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> StartDraftCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> SwitchPackCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> EndDraftCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> DisconnectCompleted;
        
        public void Connect(HyperService.Common.Client client) {
            base.Channel.Connect(client);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginConnect(HyperService.Common.Client client, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginConnect(client, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public void EndConnect(System.IAsyncResult result) {
            base.Channel.EndConnect(result);
        }
        
        private System.IAsyncResult OnBeginConnect(object[] inValues, System.AsyncCallback callback, object asyncState) {
            HyperService.Common.Client client = ((HyperService.Common.Client)(inValues[0]));
            return this.BeginConnect(client, callback, asyncState);
        }
        
        private object[] OnEndConnect(System.IAsyncResult result) {
            this.EndConnect(result);
            return null;
        }
        
        private void OnConnectCompleted(object state) {
            if ((this.ConnectCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.ConnectCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void ConnectAsync(HyperService.Common.Client client) {
            this.ConnectAsync(client, null);
        }
        
        public void ConnectAsync(HyperService.Common.Client client, object userState) {
            if ((this.onBeginConnectDelegate == null)) {
                this.onBeginConnectDelegate = new BeginOperationDelegate(this.OnBeginConnect);
            }
            if ((this.onEndConnectDelegate == null)) {
                this.onEndConnectDelegate = new EndOperationDelegate(this.OnEndConnect);
            }
            if ((this.onConnectCompletedDelegate == null)) {
                this.onConnectCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnConnectCompleted);
            }
            base.InvokeAsync(this.onBeginConnectDelegate, new object[] {
                        client}, this.onEndConnectDelegate, this.onConnectCompletedDelegate, userState);
        }
        
        public void SetMaxPlayers(int count) {
            base.Channel.SetMaxPlayers(count);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginSetMaxPlayers(int count, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginSetMaxPlayers(count, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public void EndSetMaxPlayers(System.IAsyncResult result) {
            base.Channel.EndSetMaxPlayers(result);
        }
        
        private System.IAsyncResult OnBeginSetMaxPlayers(object[] inValues, System.AsyncCallback callback, object asyncState) {
            int count = ((int)(inValues[0]));
            return this.BeginSetMaxPlayers(count, callback, asyncState);
        }
        
        private object[] OnEndSetMaxPlayers(System.IAsyncResult result) {
            this.EndSetMaxPlayers(result);
            return null;
        }
        
        private void OnSetMaxPlayersCompleted(object state) {
            if ((this.SetMaxPlayersCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.SetMaxPlayersCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void SetMaxPlayersAsync(int count) {
            this.SetMaxPlayersAsync(count, null);
        }
        
        public void SetMaxPlayersAsync(int count, object userState) {
            if ((this.onBeginSetMaxPlayersDelegate == null)) {
                this.onBeginSetMaxPlayersDelegate = new BeginOperationDelegate(this.OnBeginSetMaxPlayers);
            }
            if ((this.onEndSetMaxPlayersDelegate == null)) {
                this.onEndSetMaxPlayersDelegate = new EndOperationDelegate(this.OnEndSetMaxPlayers);
            }
            if ((this.onSetMaxPlayersCompletedDelegate == null)) {
                this.onSetMaxPlayersCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnSetMaxPlayersCompleted);
            }
            base.InvokeAsync(this.onBeginSetMaxPlayersDelegate, new object[] {
                        count}, this.onEndSetMaxPlayersDelegate, this.onSetMaxPlayersCompletedDelegate, userState);
        }
        
        public void SendMsg(HyperService.Common.Message msg) {
            base.Channel.SendMsg(msg);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginSendMsg(HyperService.Common.Message msg, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginSendMsg(msg, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public void EndSendMsg(System.IAsyncResult result) {
            base.Channel.EndSendMsg(result);
        }
        
        private System.IAsyncResult OnBeginSendMsg(object[] inValues, System.AsyncCallback callback, object asyncState) {
            HyperService.Common.Message msg = ((HyperService.Common.Message)(inValues[0]));
            return this.BeginSendMsg(msg, callback, asyncState);
        }
        
        private object[] OnEndSendMsg(System.IAsyncResult result) {
            this.EndSendMsg(result);
            return null;
        }
        
        private void OnSendMsgCompleted(object state) {
            if ((this.SendMsgCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.SendMsgCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void SendMsgAsync(HyperService.Common.Message msg) {
            this.SendMsgAsync(msg, null);
        }
        
        public void SendMsgAsync(HyperService.Common.Message msg, object userState) {
            if ((this.onBeginSendMsgDelegate == null)) {
                this.onBeginSendMsgDelegate = new BeginOperationDelegate(this.OnBeginSendMsg);
            }
            if ((this.onEndSendMsgDelegate == null)) {
                this.onEndSendMsgDelegate = new EndOperationDelegate(this.OnEndSendMsg);
            }
            if ((this.onSendMsgCompletedDelegate == null)) {
                this.onSendMsgCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnSendMsgCompleted);
            }
            base.InvokeAsync(this.onBeginSendMsgDelegate, new object[] {
                        msg}, this.onEndSendMsgDelegate, this.onSendMsgCompletedDelegate, userState);
        }
        
        public void StartDraft(System.Collections.Generic.List<string> setCodes) {
            base.Channel.StartDraft(setCodes);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginStartDraft(System.Collections.Generic.List<string> setCodes, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginStartDraft(setCodes, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public void EndStartDraft(System.IAsyncResult result) {
            base.Channel.EndStartDraft(result);
        }
        
        private System.IAsyncResult OnBeginStartDraft(object[] inValues, System.AsyncCallback callback, object asyncState) {
            System.Collections.Generic.List<string> setCodes = ((System.Collections.Generic.List<string>)(inValues[0]));
            return this.BeginStartDraft(setCodes, callback, asyncState);
        }
        
        private object[] OnEndStartDraft(System.IAsyncResult result) {
            this.EndStartDraft(result);
            return null;
        }
        
        private void OnStartDraftCompleted(object state) {
            if ((this.StartDraftCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.StartDraftCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void StartDraftAsync(System.Collections.Generic.List<string> setCodes) {
            this.StartDraftAsync(setCodes, null);
        }
        
        public void StartDraftAsync(System.Collections.Generic.List<string> setCodes, object userState) {
            if ((this.onBeginStartDraftDelegate == null)) {
                this.onBeginStartDraftDelegate = new BeginOperationDelegate(this.OnBeginStartDraft);
            }
            if ((this.onEndStartDraftDelegate == null)) {
                this.onEndStartDraftDelegate = new EndOperationDelegate(this.OnEndStartDraft);
            }
            if ((this.onStartDraftCompletedDelegate == null)) {
                this.onStartDraftCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnStartDraftCompleted);
            }
            base.InvokeAsync(this.onBeginStartDraftDelegate, new object[] {
                        setCodes}, this.onEndStartDraftDelegate, this.onStartDraftCompletedDelegate, userState);
        }
        
        public void SwitchPack(HyperService.Common.Client client, System.Collections.Generic.List<string> cardIDs) {
            base.Channel.SwitchPack(client, cardIDs);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginSwitchPack(HyperService.Common.Client client, System.Collections.Generic.List<string> cardIDs, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginSwitchPack(client, cardIDs, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public void EndSwitchPack(System.IAsyncResult result) {
            base.Channel.EndSwitchPack(result);
        }
        
        private System.IAsyncResult OnBeginSwitchPack(object[] inValues, System.AsyncCallback callback, object asyncState) {
            HyperService.Common.Client client = ((HyperService.Common.Client)(inValues[0]));
            System.Collections.Generic.List<string> cardIDs = ((System.Collections.Generic.List<string>)(inValues[1]));
            return this.BeginSwitchPack(client, cardIDs, callback, asyncState);
        }
        
        private object[] OnEndSwitchPack(System.IAsyncResult result) {
            this.EndSwitchPack(result);
            return null;
        }
        
        private void OnSwitchPackCompleted(object state) {
            if ((this.SwitchPackCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.SwitchPackCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void SwitchPackAsync(HyperService.Common.Client client, System.Collections.Generic.List<string> cardIDs) {
            this.SwitchPackAsync(client, cardIDs, null);
        }
        
        public void SwitchPackAsync(HyperService.Common.Client client, System.Collections.Generic.List<string> cardIDs, object userState) {
            if ((this.onBeginSwitchPackDelegate == null)) {
                this.onBeginSwitchPackDelegate = new BeginOperationDelegate(this.OnBeginSwitchPack);
            }
            if ((this.onEndSwitchPackDelegate == null)) {
                this.onEndSwitchPackDelegate = new EndOperationDelegate(this.OnEndSwitchPack);
            }
            if ((this.onSwitchPackCompletedDelegate == null)) {
                this.onSwitchPackCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnSwitchPackCompleted);
            }
            base.InvokeAsync(this.onBeginSwitchPackDelegate, new object[] {
                        client,
                        cardIDs}, this.onEndSwitchPackDelegate, this.onSwitchPackCompletedDelegate, userState);
        }
        
        public void EndDraft() {
            base.Channel.EndDraft();
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginEndDraft(System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginEndDraft(callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public void EndEndDraft(System.IAsyncResult result) {
            base.Channel.EndEndDraft(result);
        }
        
        private System.IAsyncResult OnBeginEndDraft(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return this.BeginEndDraft(callback, asyncState);
        }
        
        private object[] OnEndEndDraft(System.IAsyncResult result) {
            this.EndEndDraft(result);
            return null;
        }
        
        private void OnEndDraftCompleted(object state) {
            if ((this.EndDraftCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.EndDraftCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void EndDraftAsync() {
            this.EndDraftAsync(null);
        }
        
        public void EndDraftAsync(object userState) {
            if ((this.onBeginEndDraftDelegate == null)) {
                this.onBeginEndDraftDelegate = new BeginOperationDelegate(this.OnBeginEndDraft);
            }
            if ((this.onEndEndDraftDelegate == null)) {
                this.onEndEndDraftDelegate = new EndOperationDelegate(this.OnEndEndDraft);
            }
            if ((this.onEndDraftCompletedDelegate == null)) {
                this.onEndDraftCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnEndDraftCompleted);
            }
            base.InvokeAsync(this.onBeginEndDraftDelegate, null, this.onEndEndDraftDelegate, this.onEndDraftCompletedDelegate, userState);
        }
        
        public void Disconnect(HyperService.Common.Client client) {
            base.Channel.Disconnect(client);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginDisconnect(HyperService.Common.Client client, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginDisconnect(client, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public void EndDisconnect(System.IAsyncResult result) {
            base.Channel.EndDisconnect(result);
        }
        
        private System.IAsyncResult OnBeginDisconnect(object[] inValues, System.AsyncCallback callback, object asyncState) {
            HyperService.Common.Client client = ((HyperService.Common.Client)(inValues[0]));
            return this.BeginDisconnect(client, callback, asyncState);
        }
        
        private object[] OnEndDisconnect(System.IAsyncResult result) {
            this.EndDisconnect(result);
            return null;
        }
        
        private void OnDisconnectCompleted(object state) {
            if ((this.DisconnectCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.DisconnectCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void DisconnectAsync(HyperService.Common.Client client) {
            this.DisconnectAsync(client, null);
        }
        
        public void DisconnectAsync(HyperService.Common.Client client, object userState) {
            if ((this.onBeginDisconnectDelegate == null)) {
                this.onBeginDisconnectDelegate = new BeginOperationDelegate(this.OnBeginDisconnect);
            }
            if ((this.onEndDisconnectDelegate == null)) {
                this.onEndDisconnectDelegate = new EndOperationDelegate(this.OnEndDisconnect);
            }
            if ((this.onDisconnectCompletedDelegate == null)) {
                this.onDisconnectCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnDisconnectCompleted);
            }
            base.InvokeAsync(this.onBeginDisconnectDelegate, new object[] {
                        client}, this.onEndDisconnectDelegate, this.onDisconnectCompletedDelegate, userState);
        }
    }
}
