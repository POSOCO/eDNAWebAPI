using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using InStep.eDNA.EzDNAApiNet;
using System.Web.Http.Cors;
using System.Globalization;
using System.Text.RegularExpressions;

namespace openHttpAPI.Controllers
{
    public class realResult
    {
        public double dval { get; set; }
        public DateTime timestamp { get; set; }
        public string status { get; set; }
        public string desc { get; set; }
        public string units { get; set; }
    }

    public class histResult
    {
        public double dval { get; set; }
        public DateTime timestamp { get; set; }
        public string status { get; set; }
    }

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ValuesController : ApiController
    {
        // GET api/values
        public object Get()
        {
            dynamic ret = new JObject();
            ret.name1 = new JArray { new[] { "venky", "sudhir" } };
            ret.name1.Add("prashanth");
            ret.name2 = "asfa";
            return new realResult { dval = 10, timestamp = DateTime.Now, status = "asfsa", units = "hrht" };
        }

        // GET api/values/history?type=snap&pnt=something&strtime=30/11/2016/00:00:00&endtime=30/11/2016/23:59:00&secs=60
        // GET api/values/real?pnt=something
        public object Get(string id, [FromUri] string pnt = "WRLDC.PHASOR.WRDC0783", [FromUri] string strtime = "30/11/2016/00:00:00", [FromUri] string endtime = "30/11/2016/23:59:00", [FromUri] int secs = 60, [FromUri] string type = "snap", [FromUri] string service = "WRDCMP.SCADA1")
        {
            //testing the function
            /*
            int nret = 1440;
            dynamic jsonObject = new JObject();
            jsonObject.data = new JArray { };
            //testing the function
            while (nret >= 0)
            {
                dynamic resultObj = new JObject();
                resultObj.dval = "val1";
                resultObj.timestamp = "val2";
                resultObj.status = "val3";
                jsonObject.data.Add(resultObj);
                
                nret = nret - 1;
            }
            return jsonObject;
            */
            //testing the function

            int nret = 0;
            string format = "dd/MM/yyyy/HH:mm:ss";
            if (id == "history")
            {
                /*
                string r = @"(\d{2})/(\d{2})/(\d{4})/(\d{2}):(\d{2}):(\d{2})";
                MatchCollection matches = Regex.Matches(strtime, r);
                foreach (Match match in matches)
                {
                    Console.WriteLine(match.Groups[1].Value);
                    Console.WriteLine(match.Groups[2].Value);
                    Console.WriteLine(match.Groups[3].Value);
                    Console.WriteLine(match.Groups[4].Value);
                    Console.WriteLine(match.Groups[5].Value);
                    Console.WriteLine(match.Groups[6].Value);
                }
                */
                //get history values
                ArrayList historyResults = new ArrayList();
                try
                {
                    uint s = 0;
                    double dval = 0;
                    DateTime timestamp = DateTime.Now;
                    string status = "";
                    TimeSpan period = TimeSpan.FromSeconds(secs);
                    //history request initiation
                    if (type == "raw")
                    { nret = History.DnaGetHistRaw(pnt, DateTime.ParseExact(strtime, format, CultureInfo.InvariantCulture), DateTime.ParseExact(endtime, format, CultureInfo.InvariantCulture), out s); }
                    else if (type == "snap")
                    { nret = History.DnaGetHistSnap(pnt, DateTime.ParseExact(strtime, format, CultureInfo.InvariantCulture), DateTime.ParseExact(endtime, format, CultureInfo.InvariantCulture), period, out s); }
                    else if (type == "average")
                    { nret = History.DnaGetHistAvg(pnt, DateTime.ParseExact(strtime, format, CultureInfo.InvariantCulture), DateTime.ParseExact(endtime, format, CultureInfo.InvariantCulture), period, out s); }
                    else if (type == "min")
                    { nret = History.DnaGetHistMin(pnt, DateTime.ParseExact(strtime, format, CultureInfo.InvariantCulture), DateTime.ParseExact(endtime, format, CultureInfo.InvariantCulture), period, out s); }
                    else if (type == "max")
                    { nret = History.DnaGetHistMax(pnt, DateTime.ParseExact(strtime, format, CultureInfo.InvariantCulture), DateTime.ParseExact(endtime, format, CultureInfo.InvariantCulture), period, out s); }

                    while (nret == 0)
                    {
                        nret = History.DnaGetNextHist(s, out dval, out timestamp, out status);
                        if (status != null)
                        {
                            historyResults.Add(new histResult { dval = dval, timestamp = timestamp, status = status });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error while fetching history results " + ex.Message);
                    historyResults = new ArrayList();
                }
                return historyResults;
            }
            else if (id == "real")
            {
                double dval = 0;
                DateTime timestamp = DateTime.Now;
                string status = "";
                string desc = "";
                string units = "";
                realResult realVal;
                try
                {
                    nret = RealTime.DNAGetRTAll(pnt, out dval, out timestamp, out status, out desc, out units);//get RT value
                    if (nret == 0)
                    {
                        realVal = new realResult { dval = dval, timestamp = timestamp, status = status, units = units };
                        return realVal;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error while fetching realtime result " + ex.Message);
                    return null;
                }
                return null;
            }
            else if (id == "longtoshort")
            {
                string shortId = "";
                try
                {
                    InStep.eDNA.EzDNAApiNet.Configuration.ShortIdFromLongId(service, pnt, out shortId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error while fetching longtoshort result " + ex.Message);
                    shortId = "";
                }
                return new { shortId = shortId };
            }
            else
            {
                return null;
            }
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
