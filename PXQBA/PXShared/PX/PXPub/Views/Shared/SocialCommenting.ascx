<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.Biz.DataContracts.ContentItem>" %>

<div id="social-commenting">
            <% if (Model.SocialCommenting == SocialCommentingState.Active)
               { %>
                <!-- Display the comments -->
		<div id="Comments_Plugin">
		<div id="disqus_thread"></div>
		<script type="text/javascript">
		    /* * * CONFIGURATION VARIABLES: EDIT BEFORE PASTING INTO YOUR WEBPAGE * * */
		    var disqus_developer = 1;
		    var disqus_shortname = 'kclocal'; // required: replace example with your forum shortname

		    /* * * DON'T EDIT BELOW THIS LINE * * */
		    (function() {
			var dsq = document.createElement('script'); dsq.type = 'text/javascript'; dsq.async = true;
			dsq.src = 'http://' + disqus_shortname + '.disqus.com/embed.js';
			(document.getElementsByTagName('head')[0] || document.getElementsByTagName('body')[0]).appendChild(dsq);
		    })();
		</script>
		<noscript>Please enable JavaScript to view the <a href="http://disqus.com/?ref_noscript">comments powered by Disqus.</a></noscript>
		<a href="http://disqus.com" class="dsq-brlink">comments powered by <span class="logo-disqus">Disqus</span></a>
		
		</div>
		<!-- along with a disable button -->
            	<input type="button" class="Disable_Comments" id="Disable_Comments" value="Disable Comments" onclick="PxSocialCommenting.DisableCommentsForItem()"/>
		<input type="button" class="Enable_Comments" id="Enable_Comments" value="Enable Comments" onclick="PxSocialCommenting.EnableCommentsForItem()"/ style='display:none'><br>
            <% }
               else if (Model.SocialCommenting == SocialCommentingState.DisabledByUser)
               { %>
                (Gregory Berman)<br>
                <!-- Social Commenting is disabled<br> -->
                <!-- Display a (turn comments on) button<br> -->
		<input type="button" class="Enable_Comments" id="Enable_Comments" value="Enable Comments" onclick="PxEPortfolioBrowser.EnableCommentsForItem()"/><br>
		<input type="button" id="Disable_Comments" value="Disable Comments" onclick="PxEPortfolioBrowser.DisableCommentsForItem()" style='display:none'/>
		and a stub just in case the button is clicked the comments will render
		<div id="Comments_Plugin">
		</div>
            <% }
               else if (Model.SocialCommenting == SocialCommentingState.None)
               { %>
                (Gregory Berman)
                Social Commenting has been turned off by course
                Need to leave a stub just in case it is dynamically turned back on
		<div id="Comments_Plugin">
		</div>
            <% } %>

</div>
            
