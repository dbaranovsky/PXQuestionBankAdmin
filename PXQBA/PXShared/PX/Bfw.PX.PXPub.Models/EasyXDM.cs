using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace Bfw.PX.PXPub.Models
{
    public class EasyXDM
    {
        public EasyXDM()
        {
            
        }
        /// <summary>
        /// the full url including http of the location to the provider
        /// </summary>
        public String AbsoluteURL_Provider { get; set; }
        /// <summary>
        /// the full url including http of the location to the form action controller
        /// </summary>
        public String AbsoluteURL_Controller { get; set; }

        /// <summary>
        /// the name of the iframe the form will be posting to
        /// </summary>
        public String IFrameId { get; set; }

        /// <summary>
        /// When the IFrame finishs loading that is when the programmer can access the RPC methods
        /// </summary>
        public String onLoad_callback { get; set; }

        /// <summary>
        /// When calling a method requesting for information to be returned from the IFRAME this method gets called to handle the answer
        /// function (id, domain, location, response)
        /// id: when calling a method you need to specify an id, that id is returned back for reference
        /// domain: the domain of the iframe containing the response
        /// location: the url of the iframe containing the response
        /// response: depending on which method was called the response could be different, will generally return a part of the dom in string format
        /// </summary>
        public String onResponse_callback { get; set; }

    }
}