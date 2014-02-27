using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Web;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Serialization;
using Bfw.Agilix.Dlap;
using System.ComponentModel;
using System.Net;
 
namespace WebApplicationTest {
    public class XMLDataFactory {
        XDocument xdoc;
 
        public string[] StartDSTest( int NumRequests ) {
            
            string[] resultTimes = new string[NumRequests];
            double average = 0;
            for ( int i = 0; i <= ( NumRequests - 1 ); i++ ) {
                DateTime start = DateTime.Now;
                string uriBase = "http://qa.dlap.bfwpub.com/dlap.ashx?cmd=getcommandlist";
                xdoc = XDocument.Load( uriBase );
                DateTime end = DateTime.Now;
                average = average + TimeDiff( start, end ).TotalMilliseconds; 
                resultTimes[i] = "<br/>Total Milliseconds " + TimeDiff( start, end ).TotalMilliseconds.ToString();
                //System.Threading.Thread.Sleep( 1000 );
            }         
            average = average / NumRequests;
            HttpContext.Current.Session["DS_Average"] = average.ToString();
            return resultTimes;
        }

        public string[] StartStreamTest(int NumRequests) {
            string[] resultTimes = new string[NumRequests];
            double average = 0;

            for ( int i = 0; i <= ( NumRequests - 1 ); i++ ) {
                DateTime _tmpStart = DateTime.Now;
                DateTime start = DateTime.Now;
                try {
                    //DateTime start = DateTime.Now;
                    
                    string uri = "http://qa.dlap.bfwpub.com/dlap.ashx";
                    DlapRequest request = new DlapRequest();
                    request.Parameters.Add( "cmd", "getcommandlist" );
                    request.Type = DlapRequestType.Get;
                    DlapConnection conn = new DlapConnection( uri );
                    DlapResponse responese = conn.Send_t( request );

                    DateTime end = DateTime.Now;
                    average = average + TimeDiff( start, end ).TotalMilliseconds; 
                    resultTimes[i] = "<br/>Total Milliseconds " + TimeDiff( start, end ).TotalMilliseconds.ToString();
                    //System.Threading.Thread.Sleep( 1000 );
                } catch ( Exception ex ) {
                    DateTime end = DateTime.Now;
                    average = average + TimeDiff( start, end ).TotalMilliseconds; 
                    DateTime _tmpEnd = DateTime.Now;
                    resultTimes[i] = "<br/><span style='color: red'>Failed after <b>" + TimeDiff( _tmpStart, _tmpEnd ).TotalMilliseconds.ToString() + "</b> milliseconds with Exception: " + ex.Message + "</span>";
                }
            }
            average = average / NumRequests;
            HttpContext.Current.Session["ST_Average"] = average.ToString();
            return resultTimes;
        }

        protected TimeSpan TimeDiff( DateTime start, DateTime end ) {
            TimeSpan tSpan = end.Subtract( start );
            return tSpan;
        }

        public string GetCurrentCount( int count ) {
            count++;
            return count.ToString();
        }
    }
}