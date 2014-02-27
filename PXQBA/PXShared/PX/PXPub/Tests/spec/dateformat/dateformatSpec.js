/// <reference path="../../../Scripts/jquery/jquery-1.10.2.js" />
/// <reference path="../../../Scripts/jquery/jquery-1.6.4-vsdoc.js" />

/// <reference path="../../lib/jasmine-1.3.1/jasmine.js" />
/// <reference path="../../lib/jquery/jquery.js" />
/// <reference path="../../lib/jasmine-jquery.js" />
/// <reference path="../../lib/mock-ajax.js" />
/// <reference path="../../lib/jasmine-1.3.1/jasmine-beforeAll.js" />

describe('dateFormat', function () {
    
    describe('can convert to UTC date ', function() {
        it('in PST time zone during daylight savings', function () {
            setFixtures('' +
                '<input id="TimeZoneDaylightOffset" name="TimeZoneDaylightOffset" type="hidden" value="-420" />' +
                '<input id="TimeZoneStandardOffset" name="TimeZoneStandardOffset" type="hidden" value="-480" />' +
                '<input id="TimeZoneDaylightStartTime" name="TimeZoneDaylightStartTime" type="hidden" value="3/10/2013 7:00:00 AM" />' +
                '<input id="TimeZoneStandardStartTime" name="TimeZoneStandardStartTime" type="hidden" value="11/3/2013 7:00:00 AM" />');
            
            var date = "8/1/2013";
            var time = "11:59 PM";

            var result = dateFormat.dateConvertToUtc(date, time);
            var expectedResult = new Date("8/2/2013 6:59 AM GMT");
            expect(result).toEqual(expectedResult);

        });
        it('in PST time zone outside daylight savings', function () {           
            setFixtures('' +
                '<input id="TimeZoneDaylightOffset" name="TimeZoneDaylightOffset" type="hidden" value="-420" />' +
                '<input id="TimeZoneStandardOffset" name="TimeZoneStandardOffset" type="hidden" value="-480" />' +
                '<input id="TimeZoneDaylightStartTime" name="TimeZoneDaylightStartTime" type="hidden" value="3/10/2013 7:00:00 AM" />' +
                '<input id="TimeZoneStandardStartTime" name="TimeZoneStandardStartTime" type="hidden" value="11/3/2013 7:00:00 AM" />');

            var date = "12/1/2013";
            var time = "11:59 PM";

            var result = dateFormat.dateConvertToUtc(date, time);
            var expectedResult = new Date("12/2/2013 7:59 AM GMT");
            expect(result).toEqual(expectedResult);
        });
        
        it('in PST time zone during daylight savings, saving next year', function () {
            setFixtures('' +
                '<input id="TimeZoneDaylightOffset" name="TimeZoneDaylightOffset" type="hidden" value="-420" />' +
                '<input id="TimeZoneStandardOffset" name="TimeZoneStandardOffset" type="hidden" value="-480" />' +
                '<input id="TimeZoneDaylightStartTime" name="TimeZoneDaylightStartTime" type="hidden" value="3/10/2013 7:00:00 AM" />' +
                '<input id="TimeZoneStandardStartTime" name="TimeZoneStandardStartTime" type="hidden" value="11/3/2013 7:00:00 AM" />' +
                '<input id="TimeZoneDaylightStartTimeNextYear" name="TimeZoneDaylightStartTimeNextYear" type="hidden" value="3/9/2014 7:00:00 AM" >' +
                '<input id="TimeZoneStandardStartTimeNextYear" name="TimeZoneStandardStartTimeNextYear" type="hidden" value="11/2/2014 7:00:00 AM">');

            var date = "8/1/2014";
            var time = "11:59 PM";

            var result = dateFormat.dateConvertToUtc(date, time);
            var expectedResult = new Date("8/2/2014 6:59 AM GMT");
            expect(result).toEqual(expectedResult);

        });
        it('in PST time zone after daylight savings, saving next year', function () {
            setFixtures('' +
                '<input id="TimeZoneDaylightOffset" name="TimeZoneDaylightOffset" type="hidden" value="-420" />' +
                '<input id="TimeZoneStandardOffset" name="TimeZoneStandardOffset" type="hidden" value="-480" />' +
                '<input id="TimeZoneDaylightStartTime" name="TimeZoneDaylightStartTime" type="hidden" value="3/10/2013 7:00:00 AM" />' +
                '<input id="TimeZoneStandardStartTime" name="TimeZoneStandardStartTime" type="hidden" value="11/3/2013 7:00:00 AM" />' +
                '<input id="TimeZoneDaylightStartTimeNextYear" name="TimeZoneDaylightStartTimeNextYear" type="hidden" value="3/9/2014 7:00:00 AM" >' +
                '<input id="TimeZoneStandardStartTimeNextYear" name="TimeZoneStandardStartTimeNextYear" type="hidden" value="11/2/2014 7:00:00 AM">');

            var date = "12/1/2014";
            var time = "11:59 PM";

            var result = dateFormat.dateConvertToUtc(date, time);
            var expectedResult = new Date("12/2/2014 7:59 AM GMT");
            expect(result).toEqual(expectedResult);
        });
        
        it('in EST time zone in daylight savings', function () {
            setFixtures('' +
                '<input id="TimeZoneDaylightOffset" name="TimeZoneDaylightOffset" type="hidden" value="-240" />' +
                '<input id="TimeZoneStandardOffset" name="TimeZoneStandardOffset" type="hidden" value="-300" />' +
                '<input id="TimeZoneDaylightStartTime" name="TimeZoneDaylightStartTime" type="hidden" value="3/10/2013 7:00:00 AM" />' +
                '<input id="TimeZoneStandardStartTime" name="TimeZoneStandardStartTime" type="hidden" value="11/3/2013 7:00:00 AM" />');

            var date = "8/1/2013";
            var time = "11:59 PM";

            var result = dateFormat.dateConvertToUtc(date, time);
            var expectedResult = new Date("8/2/2013 3:59 AM GMT");
            expect(result).toEqual(expectedResult);
        });

        it('in EST time zone outside daylight savings', function () {
            setFixtures('' +
                '<input id="TimeZoneDaylightOffset" name="TimeZoneDaylightOffset" type="hidden" value="-240" />' +
                '<input id="TimeZoneStandardOffset" name="TimeZoneStandardOffset" type="hidden" value="-300" />' +
                '<input id="TimeZoneDaylightStartTime" name="TimeZoneDaylightStartTime" type="hidden" value="3/10/2013 7:00:00 AM" />' +
                '<input id="TimeZoneStandardStartTime" name="TimeZoneStandardStartTime" type="hidden" value="11/3/2013 7:00:00 AM" />');

            var date = "12/1/2013";
            var time = "11:59 PM";

            var result = dateFormat.dateConvertToUtc(date, time);
            var expectedResult = new Date("12/2/2013 4:59 AM GMT");
            expect(result).toEqual(expectedResult);
        });
        
        it('in PST time zone during daylight savings transition', function () {
            setFixtures('' +
                '<input id="TimeZoneDaylightOffset" name="TimeZoneDaylightOffset" type="hidden" value="-420" />' +
                '<input id="TimeZoneStandardOffset" name="TimeZoneStandardOffset" type="hidden" value="-480" />' +
                '<input id="TimeZoneDaylightStartTime" name="TimeZoneDaylightStartTime" type="hidden" value="3/10/2013 7:00:00 AM" />' +
                '<input id="TimeZoneStandardStartTime" name="TimeZoneStandardStartTime" type="hidden" value="11/3/2013 7:00:00 AM" />');

            var date = "3/9/2013";
            var time = "11:59 PM";

            var result = dateFormat.dateConvertToUtc(date, time);
            var expectedResult = new Date("3/10/2013 7:59 AM GMT");
            expect(result).toEqual(expectedResult);
            
            date = "3/11/2013";
            time = "12:01 AM";

            result = dateFormat.dateConvertToUtc(date, time);
            expectedResult = new Date("3/11/2013 7:01 AM GMT");
            expect(result).toEqual(expectedResult);

        });

     
    });

    describe('can convert from Course date ', function() {
        it('in PST time zone during daylight savings', function () {
            setFixtures('' +
                '<input id="TimeZoneDaylightOffset" name="TimeZoneDaylightOffset" type="hidden" value="-420" />' +
                '<input id="TimeZoneStandardOffset" name="TimeZoneStandardOffset" type="hidden" value="-480" />' +
                '<input id="TimeZoneDaylightStartTime" name="TimeZoneDaylightStartTime" type="hidden" value="3/10/2013 7:00:00 AM" />' +
                '<input id="TimeZoneStandardStartTime" name="TimeZoneStandardStartTime" type="hidden" value="11/3/2013 7:00:00 AM" />');

            var date = "8/1/2013 11:59 PM PDT";
            

            var result = dateFormat.dateConvertFromCourseTimeZone(date);
            var expectedResult = new Date("8/1/2013 11:59 PM");
            expect(result).toEqual(expectedResult);

        });
        
        it('in PST time zone outside daylight savings', function () {
            setFixtures('' +
                '<input id="TimeZoneDaylightOffset" name="TimeZoneDaylightOffset" type="hidden" value="-420" />' +
                '<input id="TimeZoneStandardOffset" name="TimeZoneStandardOffset" type="hidden" value="-480" />' +
                '<input id="TimeZoneDaylightStartTime" name="TimeZoneDaylightStartTime" type="hidden" value="3/10/2013 7:00:00 AM" />' +
                '<input id="TimeZoneStandardStartTime" name="TimeZoneStandardStartTime" type="hidden" value="11/3/2013 7:00:00 AM" />');

            var date = "12/1/2013 11:59 PM PST";


            var result = dateFormat.dateConvertFromCourseTimeZone(date);
            var expectedResult = new Date("12/1/2013 11:59 PM");
            expect(result).toEqual(expectedResult);

        });
        
        it('in EST time zone in daylight savings', function () {
            setFixtures('' +
                '<input id="TimeZoneDaylightOffset" name="TimeZoneDaylightOffset" type="hidden" value="-240" />' +
                '<input id="TimeZoneStandardOffset" name="TimeZoneStandardOffset" type="hidden" value="-300" />' +
                '<input id="TimeZoneDaylightStartTime" name="TimeZoneDaylightStartTime" type="hidden" value="3/10/2013 7:00:00 AM" />' +
                '<input id="TimeZoneStandardStartTime" name="TimeZoneStandardStartTime" type="hidden" value="11/3/2013 7:00:00 AM" />');

            var date = "8/1/2013 11:59 PM EDT";


            var result = dateFormat.dateConvertFromCourseTimeZone(date);
            var expectedResult = new Date("8/1/2013 11:59 PM");
            expect(result).toEqual(expectedResult);

        });
        
        it('in EST time zone outside daylight savings', function () {
            setFixtures('' +
                '<input id="TimeZoneDaylightOffset" name="TimeZoneDaylightOffset" type="hidden" value="-240" />' +
                '<input id="TimeZoneStandardOffset" name="TimeZoneStandardOffset" type="hidden" value="-300" />' +
                '<input id="TimeZoneDaylightStartTime" name="TimeZoneDaylightStartTime" type="hidden" value="3/10/2013 7:00:00 AM" />' +
                '<input id="TimeZoneStandardStartTime" name="TimeZoneStandardStartTime" type="hidden" value="11/3/2013 7:00:00 AM" />');

            var date = "12/1/2013 11:59 PM EST";

            var result = dateFormat.dateConvertFromCourseTimeZone(date);
            var expectedResult = new Date("12/1/2013 11:59 PM");
            expect(result).toEqual(expectedResult);
        });
    });

});