using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using InStep.eDNA.EzDNAApiNet;
using System.Web.Http.Cors;

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

        // GET api/values/history?type=snap&pnt=something&strtime=11/30/2016/00:00:00.00&endtime=11/30/2008/23:59:00.00&secs=60
        // GET api/values/real?pnt=something
        public object Get(string id, [FromUri] string pnt = "WRLDC.PHASOR.WRDC0783", [FromUri] string strtime = "30/11/2016/00:00:00.00", [FromUri] string endtime = "30/11/2016/23:59:00.00", [FromUri] int secs = 60, [FromUri] string type = "snap")
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
            string format = "dd/MM/yyyy/HH:mm:ss.ff";
            if (id == "history")
            {
                uint s = 0;
                double dval = 0;
                DateTime timestamp = DateTime.Now;
                string status = "";
                TimeSpan period = TimeSpan.FromSeconds(secs);
                //history request initiation
                if (type == "raw")
                { nret = History.DnaGetHistRaw(pnt, DateTime.ParseExact(strtime, format, null), DateTime.ParseExact(endtime, format, null), out s); }
                else if (type == "snap")
                { nret = History.DnaGetHistSnap(pnt, DateTime.ParseExact(strtime, format, null), DateTime.ParseExact(endtime, format, null), period, out s); }
                else if (type == "average")
                { nret = History.DnaGetHistAvg(pnt, DateTime.ParseExact(strtime, format, null), DateTime.ParseExact(endtime, format, null), period, out s); }
                else if (type == "min")
                { nret = History.DnaGetHistMin(pnt, DateTime.ParseExact(strtime, format, null), DateTime.ParseExact(endtime, format, null), period, out s); }
                else if (type == "max")
                { nret = History.DnaGetHistMax(pnt, DateTime.ParseExact(strtime, format, null), DateTime.ParseExact(endtime, format, null), period, out s); }
                //get history values
                ArrayList historyResults = new ArrayList();
                while (nret == 0)
                {
                    nret = History.DnaGetNextHist(s, out dval, out timestamp, out status);
                    if (status != null)
                    {
                        historyResults.Add(new histResult { dval = dval, timestamp = timestamp, status = status });
                    }
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
                nret = RealTime.DNAGetRTAll(pnt, out dval, out timestamp, out status, out desc, out units);//get RT value
                realResult realVal;
                if (nret == 0)
                {
                    realVal = new realResult { dval = dval, timestamp = timestamp, status = status, units = units };
                    return realVal;
                }
                return null;
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
