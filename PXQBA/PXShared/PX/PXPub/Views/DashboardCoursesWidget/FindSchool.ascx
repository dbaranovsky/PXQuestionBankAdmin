<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Bfw.PX.PXPub.Models.Course>" %>
<div id="schoolfinder" style="display:none">
    <div id="search-by" class="school_search-top">
        <label>
            Search by:</label><input type="radio" id="schoolsearchbycity" name="schoolsearchmode"
                value="City" />City, State, Country
        <input type="radio" id="schoolsearchbyzip" name="schoolsearchmode" value="Zip" />Zip
        Code
    </div>
    <div id="schoolbox" class="school_search-top">
        <ol class="find-school">
            <li>
                <div class="left-col">
                    Country:</div>
                <div class="right-col">
                    <select id="Country" class="comboStyle">
                        <option value="US">United States</option>
                        <option value="CA">Canada</option>
                    </select></div>
            </li>
            <li>
                <div class="left-col">
                    State/Province:</div>
                <div class="right-col">
                    <select id="State" class="school_search-width comboStyle">
                        <option>Michigan</option>
                    </select></div>
            </li>
            <li>
                <div class="left-col">
                    City:</div>
                <div class="right-col">
                    <input id="searchcity" class="school_search-width inputStyle comboStyle" /></div>
            </li>
        </ol>
    </div>
    <div id="zipbox" class="school_search-top">
        <p class="school_search-criteria disable">
            Enter the zip code:</p>
        <ol class="find-school">
            <li>
                <div class="left-col">
                    Zip code:</div>
                <div class="right-col">
                    <input id="ZipCode" class="school_search-width inputStyle comboStyle" /></div>
            </li>
        </ol>
    </div>
    <div id="schoolresult" class="school_search-result">
        <select size="4" class="school_search-width" id="school_search_result">
        </select>
    </div>
    <div class="search-btns">
        <input type="button" id="FindSchools" class="search-submit" value="Find Schools" />
        <span id="findSchoolIcon" class="find-icon"></span>
        <input type="button" id="CloseFindPopupSchool" value="Cancel" class="search-cancel" /><br />
        <a href="#">Can't find your school?</a>
    </div>
</div>
