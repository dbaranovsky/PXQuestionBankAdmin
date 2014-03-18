﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.235
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.235.
// 
#pragma warning disable 1591

namespace Bfw.PX.QuestionEditor.Biz.Services.RA.EnterActivationCode {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.ComponentModel;
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="RAEnterActivationCodeSoap", Namespace="RAEnterActivationCode")]
    public partial class RAEnterActivationCode : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback EnterActivationCodeOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public RAEnterActivationCode() {
            this.Url = global::Bfw.PX.QuestionEditor.Biz.Services.Properties.Settings.Default.Bfw_PX_Biz_Direct_Services_RA_EnterActivationCode_RAEnterActivationCode;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event EnterActivationCodeCompletedEventHandler EnterActivationCodeCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("RAEnterActivationCode/EnterActivationCode", RequestNamespace="RAEnterActivationCode", ResponseNamespace="RAEnterActivationCode", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string EnterActivationCode(string sActivationCode, int iUserID, int iSiteID, out string sErrorMsg) {
            object[] results = this.Invoke("EnterActivationCode", new object[] {
                        sActivationCode,
                        iUserID,
                        iSiteID});
            sErrorMsg = ((string)(results[1]));
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void EnterActivationCodeAsync(string sActivationCode, int iUserID, int iSiteID) {
            this.EnterActivationCodeAsync(sActivationCode, iUserID, iSiteID, null);
        }
        
        /// <remarks/>
        public void EnterActivationCodeAsync(string sActivationCode, int iUserID, int iSiteID, object userState) {
            if ((this.EnterActivationCodeOperationCompleted == null)) {
                this.EnterActivationCodeOperationCompleted = new System.Threading.SendOrPostCallback(this.OnEnterActivationCodeOperationCompleted);
            }
            this.InvokeAsync("EnterActivationCode", new object[] {
                        sActivationCode,
                        iUserID,
                        iSiteID}, this.EnterActivationCodeOperationCompleted, userState);
        }
        
        private void OnEnterActivationCodeOperationCompleted(object arg) {
            if ((this.EnterActivationCodeCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.EnterActivationCodeCompleted(this, new EnterActivationCodeCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void EnterActivationCodeCompletedEventHandler(object sender, EnterActivationCodeCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class EnterActivationCodeCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal EnterActivationCodeCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
        
        /// <remarks/>
        public string sErrorMsg {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[1]));
            }
        }
    }
}

#pragma warning restore 1591