using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using RestSharp.Authenticators;
using Google.Cloud.Dialogflow.V2;
using Google.Protobuf;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using Nancy.Json;
using Google.Apis.Auth.OAuth2;

namespace DailogFlow_UiPath_Connector.Controllers
{
    [Route("job")]
    [ApiController]
    public class UiPathApiController : Controller
    {
        private static readonly JsonParser jsonParser = new JsonParser(JsonParser.Settings.Default.WithIgnoreUnknownFields(true));

        [HttpPost]
        public async Task<JsonResult> GetWebhookResponse()
        {
            WebhookRequest request;
            
            using (var reader = new StreamReader(Request.Body))
            {
                request = jsonParser.Parse<WebhookRequest>(reader);
                
            }
            var intentname = request.QueryResult.Intent.DisplayName;
            var process = intentname;

            var parameters = request.QueryResult.Parameters.Fields;
            var inp = parameters["ss_num"].NumberValue;

            var dialogflowresponse = new WebhookResponse();
            //==========================================================
            //TO get access token from UiPath
            var clientAuthToken = new RestClient("https://account.uipath.com/oauth/token");
            clientAuthToken.Timeout = -1;
            var requestAuthToken = new RestRequest(Method.POST);
            requestAuthToken.AddHeader("Content-Type", "application/json");
            requestAuthToken.AddParameter("application/json", "{\r\n    \"grant_type\": \"refresh_token\",\r\n    \"client_id\": \"8DEv1AMNXczW3y4U15LL3jYf62jK93n5\",\r\n    \"refresh_token\": \"ooT1Vt55oCDwwyKK29Pcql8BP49lIswllMybibia8Vsyw\"\r\n}", ParameterType.RequestBody);
            IRestResponse response1 = clientAuthToken.Execute(requestAuthToken);

            JObject jObj = JObject.Parse(response1.Content);                
            String access_token = jObj["access_token"].ToString();

            //=========================================================    
            //To get release key from UiPath
            var clientReleaseKey = new RestClient("https://platform.uipath.com/infoslzyulwz/InfosysDefakjch277163/odata/Releases?$filter=ProcessKey eq '"+process+"'");
            clientReleaseKey.Timeout = -1;
            var requestReleaseKey = new RestRequest(Method.GET);
            requestReleaseKey.AddHeader("X-UIPATH-TenantName", "InfosysDefakjch277163");
            requestReleaseKey.AddHeader("X-UIPATH-OrganizationUnitId","InfosysDefault");
            requestReleaseKey.AddHeader("Authorization", "Bearer "+access_token);
            IRestResponse responseReleaseKey = clientReleaseKey.Execute(requestReleaseKey);
            //Console.WriteLine(responseReleaseKey.Content);
            JObject jObj1 = JObject.Parse(responseReleaseKey.Content);                 
            String release_key = jObj1["value"][0]["Key"].ToString();       
            //=========================================================
            //To get the robot id which will perform the process
            var clientRobotName = new RestClient("https://platform.uipath.com/infoslzyulwz/InfosysDefakjch277163/odata/Robots?$filter=Name eq 'ROBOT1'");
            clientRobotName.Timeout = -1;
            var requestRobotName = new RestRequest(Method.GET);
            requestRobotName.AddHeader("Content-Type", "application/json");
            requestRobotName.AddHeader("X-UIPATH-TenantName", "InfosysDefakjch277163");
            requestRobotName.AddHeader("X-UIPATH-OrganizationUnitId","InfosysDefault");
            requestRobotName.AddHeader("Authorization", "Bearer " + access_token);
            IRestResponse responseRobotName = clientRobotName.Execute(requestRobotName);       
            JObject jObj2 = JObject.Parse(responseRobotName.Content);                 
            String robot_id = jObj2["value"][0]["Id"].ToString();    

            //=========================================================
            // To start job with user input

            var client = new RestClient("https://platform.uipath.com/AccountLogicalName/TenantLogicalName/odata/Jobs/UiPath.Server.Configuration.OData.StartJobs");
            client.Timeout = -1;
            var requestStartJob = new RestRequest(Method.POST);
            requestStartJob.AddHeader("Content-Type", "application/json");
            requestStartJob.AddHeader("X-UIPATH-TenantName", "InfosysDefakjch277163");
            requestStartJob.AddHeader("X-UIPATH-OrganizationUnitId", "InfosysDefault");
            requestStartJob.AddHeader("Authorization", "Bearer " + access_token);
            requestStartJob.AddParameter("application/json", "{ \"startInfo\":\r\n   { \"ReleaseKey\": \""+release_key+"\",\r\n     \"Strategy\": \"Specific\",\r\n     \"RobotIds\": [ "+ robot_id + " ],\r\n     \"NoOfRobots\": 0,\r\n     \"Source\": \"Manual\",\r\n     \"InputArguments\": \"{\\\"screenshotnumber\\\":\\\"" + inp+"\\\"}\"\r\n   } \r\n}", ParameterType.RequestBody);

            IRestResponse responseStartJob = client.Execute(requestStartJob);
            
            //=========================================================
            dialogflowresponse.FulfillmentText = "Screenshot with number" + inp + "is talen successfully";
            return Json(dialogflowresponse);

        }
    }
}