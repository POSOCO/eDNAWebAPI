using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using InStep.eDNA.EzDNAApiNet;

namespace openHttpAPI.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public object Get()
        {
            dynamic ret = new JObject();
            ret.name1 = new JArray { new[] { "venky", "sudhir" } };
            ret.name1.Add("prashanth");
            ret.name2 = "asfa";
            return ret;
        }

        // GET api/values/history?type=snap&pnt=something&strtime=11/30/2016 00:00:00.00&endtime=11/30/2008 24:59:00.00&secs=60
        // GET api/values/real?pnt=something
        public object Get(string id, [FromUri] string pnt = "WRLDC.PHASOR.WRDC0783", [FromUri] string strtime = "11/30/2016 00:00:00.00", [FromUri] string endtime = "11/30/2008 24:59:00.00", [FromUri] int secs = 60, [FromUri] string type = "snap")
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

            dynamic jsonObject = new JObject();
            jsonObject.data = new JArray { new[] { "dval", "timestamp", "status" } };
            int nret = 0;
            if (id == "history")
            {
                uint s = 0;
                double dval = 0;
                DateTime timestamp = DateTime.Now;
                string status = "";
                TimeSpan period = TimeSpan.FromSeconds(secs);
                //history request initiation
                if (type == "raw")
                { nret = History.DnaGetHistRaw(pnt, Convert.ToDateTime(strtime), Convert.ToDateTime(endtime), out s); }
                else if (type == "snap")
                { nret = History.DnaGetHistSnap(pnt, Convert.ToDateTime(strtime), Convert.ToDateTime(endtime), period, out s); }
                else if (type == "average")
                { nret = History.DnaGetHistAvg(pnt, Convert.ToDateTime(strtime), Convert.ToDateTime(endtime), period, out s); }
                else if (type == "min")
                { nret = History.DnaGetHistMin(pnt, Convert.ToDateTime(strtime), Convert.ToDateTime(endtime), period, out s); }
                else if (type == "max")
                { nret = History.DnaGetHistMax(pnt, Convert.ToDateTime(strtime), Convert.ToDateTime(endtime), period, out s); }
                //get history values
                while (nret == 0)
                {
                    nret = History.DnaGetNextHist(s, out dval, out timestamp, out status);
                    if (status != null)
                    {
                        dynamic resultObj = new JObject();
                        resultObj.dval = dval;
                        resultObj.timestamp = timestamp;
                        resultObj.status = status;
                        jsonObject.data.Add(resultObj);
                    }
                }
            }
            else if (id == "real")
            {
                jsonObject.data = new JArray { new[] { "dval", "timestamp", "status", "pointid", "description", "units" } };
                double dval = 0;
                DateTime timestamp = DateTime.Now;
                string status = "";
                string desc = "";
                string units = "";
                nret = RealTime.DNAGetRTAll(pnt, out dval, out timestamp, out status, out desc, out units);//get RT value
                if (nret == 0)
                {
                    dynamic resultObj = new JObject();
                    resultObj.dval = dval;
                    resultObj.timestamp = timestamp.ToString();
                    resultObj.status = status;
                    resultObj.pointid = pnt;
                    resultObj.description = desc;
                    resultObj.units = units;
                    jsonObject.data.Add(resultObj);
                }
            }
            return jsonObject;
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
