<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<div class="loading-overlay" style="width:100%; height:100%; position:fixed;">
    <h2>Loading your <%= ViewData["loadingEntity"].ToString() %>. Please Wait...</h2>
    <div class="progress-bar blue stripes shine">
        <span style="width: 100%"></span>
    </div>
</div>