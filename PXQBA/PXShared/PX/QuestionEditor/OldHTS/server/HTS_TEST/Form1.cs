using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Net;
using System.Windows.Forms;
using System.Diagnostics;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Security.Cryptography;
using HTS;

namespace HTS_TEST
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void doScore(string problemXML, string responseXML)
        {
            CHTSProblem hts = new CHTSProblem(@"D:\_Projects\_Levkov\_HTS_NEW\PxHTS\htsplayer\maths.js", "");
            double score = hts.doScore(problemXML, responseXML,"bacdafadf_ffabbca","agilixSign");

            txtResponse.Text += "score = " + score.ToString();

        }
        private void doProblem(string problemXML, string baseURL)
        {
            CHTSProblem hts = new CHTSProblem(@"D:\_Projects\_Levkov\_HTS_NEW\PxHTS\htsplayer\maths.js", baseURL);
            //double score = hts.doScore(problemXML, responseXML);
            hts.doProblem(problemXML,null);
            //txtResponse.Text += hts.getProblemBody();
            txtResponse.Text += hts.getProblemPageForAgilix("txtResponse", "getProblemBody");
            txtResponse.Text += "\r\n\r\n" + hts.prepareResponseXML();

        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            string s = File.ReadAllText(@"D:\_Projects\_Levkov\_HTS_NEW\TestData\score-97deb37f-1291-4177-9458-32a7d1f30353.xml");
//            string s = File.ReadAllText(@"D:\_Projects\_Levkov\HTS\hts_f3\bin\preview.xml");
//            txtResponse.Text = "";

            //string pathResponse = @"D:\_Projects\_Levkov\_HTS_NEW\response.xml";
            //string respData = File.ReadAllText(pathResponse);


            XDocument request = XDocument.Load(new System.IO.StringReader(s));
            string answer = request.Root.XPathSelectElement("submission/answer").Value;

            string problemXML = request.Root.XPathSelectElement("question/interaction/data").Value;

            doScore(problemXML, answer);
            //double score  = hts.doScore(s,respData);
            
            //hts.doProblem(s, response);
            //foreach (KeyValuePair<string, CVardef> kvp in response.vars)
            //{
            //    //Console.WriteLine(kvp.Value.Name + " = " + kvp.Value.valueToString());
            //    //txtResponse.Text += kvp.Value.Name + " = " + kvp.Value.valueToString() + "\r\n";
            //    XElement vd = kvp.Value.getVardef();
            //    txtResponse.Text += vd.ToString() + "\r\n"; ;
            //}

            //txtResponse.Text += "\r\n\r\n";
            //foreach (KeyValuePair<string, CVardef> kvp in hts.vardefs)
            //{
            //    //Console.WriteLine(kvp.Value.Name + " = " + kvp.Value.valueToString());
            //    //txtResponse.Text += kvp.Value.Name + " = " + kvp.Value.valueToString() + "\r\n";
            //    XElement vd = kvp.Value.getVardef();
            //    txtResponse.Text += vd.ToString() + "\r\n"; ;
            //}

            //double xx = CUtils.formatNumber(3.455, "#.##");
            //double yy = CUtils.formatNumber(4.355, "#.##");
            //object res = hts.doCalculate("log10(5)");

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string s = File.ReadAllText(@"D:\_Projects\_Levkov\HTS\hts_f3\bin\preview.xml");

            string path = @"D:\_Projects\_Levkov\_HTS_NEW\response.xml";
            string respData = File.ReadAllText(path);

            txtResponse.Text = "";

            doScore(s, respData);
        }
        private void button5_Click(object sender, EventArgs e)
        {
            //string s = File.ReadAllText(@"D:\_Projects\_Levkov\_New\Results_Pub\Correct\Chapter2\roget_aq_02_02_037.xml");
            string s = File.ReadAllText(@"C:\work\roget_aq_01_01_023.xml");

            //string path = @"D:\_Projects\_Levkov\_HTS_NEW\response.xml";
            //string respData = File.ReadAllText(path);

            txtResponse.Text = "";            
            //doProblem(s, "http://192.168.78.92:83/PxHTS/");
            doProblem(s, "http://coursesdev.bfwpub.com/Intellipro/");


        }


        private void button3_Click(object sender, EventArgs e)
        {
            //string cmd = "http://localhost/RxHTS/PxPlayer.ashx";
            //string cmd = "http://192.168.78.92:83/PxHTS/PxPlayer.ashx";
            string cmd = "http://localhost/RxHTS/PxScore.ashx";
            //string cmd = "http://dev.px.bfwpub.com:83/PxHTS/PxScore.ashx";
            HttpWebRequest HttpWReq =
            (HttpWebRequest)WebRequest.Create(cmd);
            HttpWReq.Method = "POST";

            Stream dataStream = HttpWReq.GetRequestStream();

            //string path = @"D:\_Projects\_Levkov\_HTS_NEW\TestData\requestA.xml";
            //request_6ce55eab-357e-4410-9ec1-6850c7006d79.xml

            string path = @"D:\_Projects\_Levkov\_HTS_NEW\TestData\score-1b6afdd6-c8b8-435a-836a-0d3ce5168691.xml";
            //string path = @"D:\_Projects\_Levkov\_HTS_NEW\TestData\request_e3a4dad1-979a-4493-98d8-58f5fa3cd9f0.xml";

            //string path = @"D:\_Projects\_Levkov\_HTS_NEW\score.xml";

            string postData = File.ReadAllText(path);
            txtResponse.Text = "";

            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            HttpWReq.ContentType = "text/xml";
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            string resp = "";
            HttpWebResponse myHttpWebResponse = null;
            try
            {
                HttpWReq.Method = "POST";
                myHttpWebResponse = (HttpWebResponse)HttpWReq.GetResponse();
                Stream receiveStream = myHttpWebResponse.GetResponseStream();
                // Insert code that uses the response object.
                Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                // Pipes the stream to a higher level stream reader with the required encoding format. 
                StreamReader readStream = new StreamReader(receiveStream, encode);
                Char[] read = new Char[256];
                // Reads 256 characters at a time.    
                int count = readStream.Read(read, 0, 256);
                //Debug.WriteLine(HttpWReq.RequestUri);
                while (count > 0)
                {
                    // Dumps the 256 characters on a string and displays the string to the console.

                    //Debug.WriteLine("(" + (int)myHttpWebResponse.StatusCode + ") " + str);
                    //Debug.WriteLine("(" + (int)myHttpWebResponse.StatusCode + ") " );
                    String str = new String(read, 0, count);
                    resp += str;
                    count = readStream.Read(read, 0, 256);
                }
                Debug.WriteLine("\r\n--------------------");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("\r\n--Exception=" + ex.Message + "\r\n");
                txtResponse.Text = "\r\n--Exception=" + ex.Message + "\r\n";
            }
            finally
            {
                if (myHttpWebResponse != null)
                    myHttpWebResponse.Close();
            }

            txtResponse.Text += "\r\n" + resp;

        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (openHTS.FileName == string.Empty)
                //folderBD.SelectedPath = @"D:\_Projects\_Levkov\_New\Results_Pub\Correct";
                openHTS.FileName = @"D:\_Projects\_Levkov\_New\Results_Pub\Correct\Chapter1\roget_aq_01_04_021.xml";
            if (openHTS.ShowDialog() == DialogResult.OK)
            {
                txtFile.Text = openHTS.FileName;

            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            double x = 0.03;
            double a = 2;
            double b=(12.0+x)/12.0;

            //double d  = (Math.Atan(x) - a*x)/(Math.Asin(x)-a*x);
            double d = Math.Log(a, b);

            txtResponse.Text += "x=" + x.ToString() + "    d=" + d.ToString() +"\r\n";

            x = x+0.01;
            b = (12.0 + x) / 12.0;

            d = d = Math.Log(a, b);
            txtResponse.Text += "x=" + x.ToString() + "    d=" + d.ToString()+"\r\n";
            x = x + 0.01;
            b = (12.0 + x) / 12.0;

            d = d = Math.Log(a, b);
            txtResponse.Text += "x=" + x.ToString() + "    d=" + d.ToString() + "\r\n";
            x = x + 0.01;
            b = (12.0 + x) / 12.0;

            d = d = Math.Log(a, b);
            txtResponse.Text += "x=" + x.ToString() + "    d=" + d.ToString() + "\r\n";
            x = x + 0.01;
            b = (12.0 + x) / 12.0;

            d = d = Math.Log(a, b);
            txtResponse.Text += "x=" + x.ToString() + "    d=" + d.ToString() + "\r\n";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string password = "bacdafadf_ffabbca";
            string salt = "agilixSign";

            string data = "Generate a Key based on a Password, Salt and HMACSHA1 pseudo-random number generator\\";

            string encr = Encrypt(data, password, salt);

            string decr = Decrypt(encr, password, salt+"0");
        }


        public string Encrypt(string dataToEncrypt, string password, string salt)
        {
            AesManaged aes = null;
            MemoryStream memoryStream = null;
            CryptoStream cryptoStream = null;

            try
            {
                //Generate a Key based on a Password, Salt and HMACSHA1 pseudo-random number generator 
                Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes(salt));

                //Create AES algorithm with 256 bit key and 128-bit block size 
                aes = new AesManaged();
                aes.Key = rfc2898.GetBytes(aes.KeySize / 8);
                aes.IV = rfc2898.GetBytes(aes.BlockSize / 8);

                //Create Memory and Crypto Streams 
                memoryStream = new MemoryStream();
                cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write);

                //Encrypt Data 
                byte[] data = Encoding.UTF8.GetBytes(dataToEncrypt);
                cryptoStream.Write(data, 0, data.Length);
                cryptoStream.FlushFinalBlock();

                //Return Base 64 String 
                return Convert.ToBase64String(memoryStream.ToArray());
            }
            finally
            {
                if (cryptoStream != null)
                    cryptoStream.Close();

                if (memoryStream != null)
                    memoryStream.Close();

                if (aes != null)
                    aes.Clear();
            }
        }

        public string Decrypt(string dataToDecrypt, string password, string salt)
        {
            AesManaged aes = null;
            MemoryStream memoryStream = null;
            CryptoStream cryptoStream = null;

            try
            {
                //Generate a Key based on a Password, Salt and HMACSHA1 pseudo-random number generator 
                Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes(salt));

                //Create AES algorithm with 256 bit key and 128-bit block size 
                aes = new AesManaged();
                aes.Key = rfc2898.GetBytes(aes.KeySize / 8);
                aes.IV = rfc2898.GetBytes(aes.BlockSize / 8);

                //Create Memory and Crypto Streams 
                memoryStream = new MemoryStream();
                cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write);

                //Decrypt Data 
                byte[] data = Convert.FromBase64String(dataToDecrypt);
                cryptoStream.Write(data, 0, data.Length);

                cryptoStream.FlushFinalBlock();

                //Return Decrypted String 
                byte[] decryptBytes = memoryStream.ToArray();
                return Encoding.UTF8.GetString(decryptBytes, 0, decryptBytes.Length);
            }
            catch { return "<usersave></usersave>"; }
            finally
            {
                try
                {
                    if (cryptoStream != null)
                        cryptoStream.Close();
                }
                catch { }
                try{
                if (memoryStream != null)
                    memoryStream.Close();
            }
                catch { }

                if (aes != null)
                    aes.Clear();
            }
        }


    }
}
