using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using Newtonsoft.Json;

namespace Greenspot.Comm.SuburbMatrix
{
    public class SuburbMatrix
    {
        public float? GetDrivingDistance(string origin, string destination)
        {
            var meters = GetDrivingDistanceFromGoogleApi(origin, destination);
            if(meters != null)
            {
                return meters.Value / 1000.0f;
            }else
            {
                return null;
            }
        }

        private int? GetDrivingDistanceFromGoogleApi(string origin, string destination)
        {
            string reqUrlPattern = "https://maps.googleapis.com/maps/api/directions/json?origin={0}&destination={1}&key={2}";
            var reqUrl = string.Format(reqUrlPattern, HttpUtility.UrlEncode(origin), HttpUtility.UrlEncode(destination), "");
            using (var client = new WebClient())
            {
                var resp = JsonConvert.DeserializeObject<DirectionResponse>(client.DownloadString(reqUrl));
                if(resp.Status.Equals("OK") && resp.Routes.Count > 0)
                {
                    return resp.Routes[0].Legs[0].Distance.Value;
                }else
                {
                    return null;
                }
            }
        }
    }
}