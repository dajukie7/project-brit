<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ProjectBrit.Models.StatusesViewModel>"%>
<%@ Import Namespace="ProjectBrit.Data"%>

<asp:Content ID="aboutTitle" ContentPlaceHolderID="TitleContent" runat="server">
    About Us
</asp:Content>

<asp:Content ID="aboutContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>About</h2>
        <div style="width:700px; margin: 0 auto; padding: 10px;">
        <% foreach (var stat in Model.Statuses)
           { 
               if(stat.UserName.ToLower() == "britneemichele")
               { %>
                   <div style="float:right; clear:both;">
              <% }
               else
               { %>
                    <div style="float:left; clear:both;">        
               <% }%> 
                <fieldset style="width:300px;">
                    <legend><%= stat.UserName %></legend>
                    <%= stat.Message %>
                </fieldset>
           </div>
           <%} %>
           <br style="clear:both;" />
        </div>
</asp:Content>