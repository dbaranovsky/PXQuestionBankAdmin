using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using System.Xml;
using System.Xml.Serialization;
using Bfw.Agilix.Dlap;
using System.IO;
using System.Net;

namespace WebApplicationTest
{
    public partial class _default : System.Web.UI.Page
    {
 
        protected void Page_Load(object sender, EventArgs e) {
            if ( !IsPostBack ) {
                ViewState["DS_Count"] = "0";
                ViewState["ST_Count"] = "0";
            }
        }

        protected void btnTestDS_Click( object sender, EventArgs e ) {
            Process p = Process.GetCurrentProcess();
            try {
                XMLDataFactory t_factory = new XMLDataFactory();
                int _numReq;
                bool IsInt = int.TryParse( txtNumDSRequests.Text, out _numReq );
                string[] result = t_factory.StartDSTest( _numReq );
                foreach ( string s in result ) {
                    lblDS.Text += s;
                }
                lblDS.Text += "<hr />Average Time (MS): " + HttpContext.Current.Session["DS_Average"].ToString() + "<hr />";
            } catch ( Exception ex ) {
                lblStream.Text += "<br /><span style='color: red'>" + ex.Message + "</span>";
            } finally {
                int _ds_count;
                bool IsInt = int.TryParse( ViewState["DS_Count"].ToString(), out _ds_count );
                if ( IsInt ) {
                    XMLDataFactory df = new XMLDataFactory();
                    ViewState["DS_Count"] = df.GetCurrentCount( _ds_count );
                    lblAttemptsDS.Text = ViewState["DS_Count"].ToString();
                }
            }
        }

        protected void btnTestStream_Click( object sender, EventArgs e ) {
            Process p = Process.GetCurrentProcess();
            try {
                XMLDataFactory t_factory = new XMLDataFactory();
                int _numReq;
                bool IsInt = int.TryParse( txtNumStreamRequests.Text, out _numReq );
                string[] result = t_factory.StartStreamTest( _numReq );
                foreach ( string s in result ) {
                    lblStream.Text += s;
                }
                lblStream.Text += "<hr />Average Time (MS): " + HttpContext.Current.Session["ST_Average"].ToString() + "<hr />";
            } catch ( Exception ex ) {
                lblStream.Text += "<br /><span style='color: red'>" + ex.Message + "</span>";
            } finally {
                int _st_count;
                bool IsInt = int.TryParse( ViewState["ST_Count"].ToString(), out _st_count );
                if ( IsInt ) {
                    XMLDataFactory df = new XMLDataFactory();
                    ViewState["ST_Count"] = df.GetCurrentCount( _st_count );
                    lblAttemptsStream.Text = ViewState["ST_Count"].ToString();
                }
            }
        }
    }
}