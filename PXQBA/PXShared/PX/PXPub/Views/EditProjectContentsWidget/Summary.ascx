<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.AnnouncementWidget>" %>
    <style>
        p
        {
            margin: 0px;
        }
        h1, h2, h3, h4, h5, h6
        {
            margin: 0px;
        }
        
        #PX_EditProjectContents
        {
            width: 960px;
            background-color: white;
            min-height: 500px;
            margin: auto;
            padding: 20px;
            height: 1000px;
        }
        
        #PX_EditProjectContents #header
        {
            width: 100%;
            float: left;
            height: 150px;
        }
        #px_editprojectcontents #assign-info h2
        {
            color: #073B4F;
            border-bottom: 7px solid #D9E4DD;
            padding-bottom: 10px;
            margin-bottom: 10px;
            font-size: 20px;
        }
        
        #PX_EditProjectContents #assign-info
        {
            float: left;
            width: 300px;
            height: 148px;
        }
        #PX_EditProjectContents #container
        {
            float: left;
            height: 100%;
            clear: both;
        }
        
        #PX_EditProjectContents #instructions-container
        {
            float: right;
        }
        
        #PX_EditProjectContents #buttons
        {
            float: left;
            width: 100%;
            margin-top: 30px;
        }
        
        #PX_EditProjectContents .instruction
        {
            width: 200px;
            float: left;
            padding-right: 10px;
            padding-left: 10px;
            min-height: 100px;
        }
        #PX_EditProjectContents .instruction-middle
        {
            width: 200px;
            float: left;
            height: 100px;
            padding-left: 10px;
            border-right: 1px solid #D9E4DD;
            border-left: 1px solid #D9E4DD;
        }
        #PX_EditProjectContents .instruction-end
        {
            width: 200px;
            float: left;
            min-height: 100px;
            padding-left: 10px;
        }
        
        #PX_EditProjectContents .instruction-title
        {
            text-transform: uppercase;
            color: red;
            margin-bottom: 5px;
        }
        #PX_EditProjectContents #assignment-title
        {
            color: #1B8A95;
        }
        #PX_EditProjectContents #assignment-subtitle
        {
            padding-top: 5px;
            color: black;
        }
        
        #PX_AlternateSource
        {
            display: block;
            float: left;
            height: 400px;
            margin-left: 20px;
            width: 400px;
        }
        
        #PX_AlternateSource .source-container
        {
            border-top: 10px solid #D9E4DD;
            padding-top: 10px;
            min-height: 100px;
            float: left;
            width: 100%;
            height: 100%;
            overflow-y: auto;
            margin-top: 26px;
        }
        
        #PX_AlternateSource .source-container h3
        {
            margin-bottom: 10px;
            color: #073b4f;
        }
        
        #PX_AlternateSource .source-item
        {
            border-top: 1px solid #D9E4DD;
            height: 50px;
        }
        
        #PX_AlternateSource .source-item-end
        {
            border-top: 1px solid #D9E4DD;
            border-bottom: 1px solid #D9E4DD;
            height: 50px;
        }
        
        #PX_AlternateSource .source-title
        {
            width: 310px;
            float: left;
            padding: 10px 0px 0px 10px;
            color: #808285;
        }
        
        #PX_AlternateSource .preview-link
        {
            width: 60px;
            float: right;
            text-align: center;
            padding-top: 15px;
            margin: auto;
        }
        
        
        
        #PX_ProjectContents
        {
            float: left;
            position: relative;
            clear: both;
            width: 540px;
        }
        
        #PX_ProjectContents .item
        {
            min-height: 25px;
            border-bottom: 1px solid #D9E4DD;
            padding-top: 5px;
            float: left;
            width: 100%;
            padding-bottom: 4px;
        }
        
        #PX_ProjectContents #scroll
        {
            border-top: 10px solid #D9E4DD;
        }
        
        #PX_ProjectContents #outter-container
        {
            overflow-y: auto;
            height: 400px;
            float: left;
            width: 100%;
            border-bottom: 10px solid #D9E4DD;
            border-top: 5px solid #D9E4DD;
        }
        
        #PX_ProjectContents #add-source-link
        {
            background-color: #D9E4DD;
            float: left;
            padding: 5px;
            width: 131px;
        }
        
        #PX_ProjectContents .icon-plus
        {
            color: black;
        }
        #PX_ProjectContents ul
        {
            width: 100%;
        }
        #PX_ProjectContents ul.header-display li
        {
            float: left;
            width: 125px;
            padding: 0px 0px 0px 7px;
            color: #1B8A95;
            border-left: 1px solid black;
            margin-top: 5px;
        }
        
        #PX_ProjectContents #top-header
        {
            width: 100%;
        }
        
        #PX_ProjectContents .header-display .display-list-beginning
        {
            border-left: none;
        }
        
        
        #PX_ProjectContents .source-checkbox-visibility
        {
            float: left;
            margin-right: 10px;
        }
        #PX_ProjectContents .source-title
        {
            float: left;
            width: 400px;
            color: #808285;
            padding-right: 10px;
            padding-top: 3px;
        }
        
        
        #PX_ProjectContents .main-title
        {
            font-weight: bold;
            color: #808285;
            padding-top: 3px;
            width: 400px;
        }
        #PX_ProjectContents .preview-link
        {
            padding-top: 3px;
            float: left;
            width: 50px;
        }
        .icon-align-left:before
        {
            content: '\e734';
            float: left;
            width: 15px;
            padding: 5px;
        }
        #PX_EditProjectContents .primary
        {
            width: 60px;
            background-color: #DAD466;
            text-align: left;
            font-size: 9px;
            font-weight: 600;
            text-transform: uppercase;
            padding: 4px 6px;
        }
        #PX_EditProjectContents .secondary
        {
            width: 60px;
            background-color: #F0F0F0;
            text-align: center;
            font-size: 9px;
            font-weight: 600;
            margin-right: 5px;
            text-transform: uppercase;
            padding: 4px 6px;
        }
    </style>
    <div id="PX_EditProjectContents">
        <div id="container">
            <div id="header">
                <div id="assign-info">
                    <h2>
                        Edit Project Contents</h2>
                    <div id="assignment-title">
                        <b>Louisa Couselle:</b></div>
                    <div id="assignment-subtitle">
                        Reconstruction Life in the West</div>
                </div>
                <div id="instructions-container">
                    <div class="instruction">
                        <p class="instruction-title">
                            Add source</p>
                        <p class="instruction-description">
                            Click <b>Add a Source</b> below or drag items from Alternate Sources in the right
                            to the project contents.</p>
                    </div>
                    <div class="instruction-middle">
                        <p class="instruction-title">
                            Rearrange Sources</p>
                        <p class="instruction-description">
                            Drag and drop sources to change the order.</p>
                    </div>
                    <div class="instruction-end">
                        <p class="instruction-title">
                            Hide content</p>
                        <p class="instruction-description">
                            Uncheck any item to hide it.</p>
                    </div>
                </div>
            </div>
            <div id="container">
                <div id="PX_ProjectContents">
                    <div id="top-header">
                        <ul class="header-display">
                            <li class="display-list-beginning">Hide all Headnotes</li>
                            <li>Hide all Transcripts</li>
                            <li>Hide all Questions</li>
                        </ul>
                        <div id="add-source-link">
                            <b>Add Source <i class="icon-plus">+</i></b></div>
                    </div>
                    <div id="outter-container">
                        <div id="scroll">
                            <ul style="padding-top: 5px;">
                                <li class="item">
                                    <input type="checkbox" class="source-checkbox-visibility" /><p class="main-title">
                                        Introduction</p>
                                </li>
                                <li class="item">
                                    <input type="checkbox" class="source-checkbox-visibility" /><p class="main-title">
                                        Historical Background</p>
                                </li>
                                <li class="item">
                                    <p class="main-title">
                                        Primary Source</p>
                                </li>
                                <li class="item">
                                    <input type="checkbox" class="source-checkbox-visibility" /><p class="source-title">
                                        US Manuscript Census, 1870, Lewis & Clark County, Helena, Montana</p>
                                    <p class="preview-link">
                                        Preview</p>
                                    <i class="icon-align-left"></i></li>
                                <li class="item">
                                    <input type="checkbox" class="source-checkbox-visibility" /><p class="source-title">
                                        US Manuscript Census, 1880, Gallaatin County, Bozeman, Montana</p>
                                    <p class="preview-link">
                                        Preview</p>
                                    <i class="icon-align-left"></i></li>
                                <li class="item">
                                    <input type="checkbox" class="source-checkbox-visibility" /><p class="source-title">
                                        Subscription History, M.A. Leeson, History of Montana, 1739-1885: "Tax List 1867,
                                        Helena, Lewis & Clark County, Montana"</p>
                                    <p class="preview-link">
                                        Preview</p>
                                    <i class="icon-align-left"></i></li>
                                <li class="item">
                                    <input type="checkbox" class="source-checkbox-visibility" /><p class="source-title">
                                        Subscription History, M.A. Leeson, History of Montana, 1739-1885: Louisa Cousselle,
                                        Will & Probate, 1883 & 1886 (excerpt)</p>
                                    <p class="preview-link">
                                        Preview</p>
                                    <i class="icon-align-left"></i></li>
                                <li class="item">
                                    <input type="checkbox" class="source-checkbox-visibility" /><p class="source-title">
                                        Avant Courier (Bozeman, Montana Territory), June 26, 1886.</p>
                                    <p class="preview-link">
                                        Preview</p>
                                    <i class="icon-align-left"></i></li>
                                <li class="item">
                                    <input type="checkbox" class="source-checkbox-visibility" /><p class="source-title">
                                        Louisa Coussella, Headstone, Greenwood Cemetery, New York, 2009</p>
                                    <p class="preview-link">
                                        Preview</p>
                                    <i class="icon-align-left"></i></li>
                                <li class="item">
                                    <input type="checkbox" class="source-checkbox-visibility" /><p class="main-title">
                                        Module Questions</p>
                                </li>
                                <li class="item">
                                    <input type="checkbox" class="source-checkbox-visibility" /><p class="main-title">
                                        Project Assignment</p>
                                </li>
                            </ul>
                        </div>
                    </div>
                </div>
                <div id="PX_AlternateSource">
                    <div class="source-container">
                        <h3>
                            Alternate Sources</h3>
                        <ul>
                            <li class="source-item">
                                <p class="source-title">
                                    Baumler, Ellen. 1998. "Devil's Perch: Prositution from Suite to Cellar in Montana."</p>
                                <p class="preview-link">
                                    Preview</p>
                            </li>
                            <li class="source-item">
                                <p class="source-title">
                                    Baumler, Ellen. 1998. "Devil's Perch: Prositution from Suite to Cellar in Montana."</p>
                                <p class="preview-link">
                                    Preview</p>
                            </li>
                            <li class="source-item">
                                <p class="source-title">
                                    Baumler, Ellen. 1998. "Devil's Perch: Prositution from Suite to Cellar in Montana."</p>
                                <p class="preview-link">
                                    Preview</p>
                            </li>
                            <li class="source-item">
                                <p class="source-title">
                                    Baumler, Ellen. 1998. "Devil's Perch: Prositution from Suite to Cellar in Montana."</p>
                                <p class="preview-link">
                                    Preview</p>
                            </li>
                            <li class="source-item">
                                <p class="source-title">
                                    Baumler, Ellen. 1998. "Devil's Perch: Prositution from Suite to Cellar in Montana."</p>
                                <p class="preview-link">
                                    Preview</p>
                            </li>
                            <li class="source-item">
                                <p class="source-title">
                                    Baumler, Ellen. 1998. "Devil's Perch: Prositution from Suite to Cellar in Montana."</p>
                                <p class="preview-link">
                                    Preview</p>
                            </li>
                            <li class="source-item">
                                <p class="source-title">
                                    Baumler, Ellen. 1998. "Devil's Perch: Prositution from Suite to Cellar in Montana."</p>
                                <p class="preview-link">
                                    Preview</p>
                            </li>
                            <li class="source-item-end">
                                <p class="source-title">
                                    Baumler, Ellen. 1998. "Devil's Perch: Prositution from Suite to Cellar in Montana."</p>
                                <p class="preview-link">
                                    Preview</p>
                            </li>
                        </ul>
                    </div>
                </div>
                <div id="buttons">
                    <input type="button" value="Save" class="primary" />
                    <input type="button" value="Cancel" class="secondary" />
                </div>
            </div>
        </div>
    </div>
