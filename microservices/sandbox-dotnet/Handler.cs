using System;
using System.Collections.Generic;
using System.Net;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon;
using System.Threading.Tasks;
using System.Linq;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(
typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace AwsDotnetCsharp
{
    public class Handler
    {

        public APIGatewayProxyResponse HelloWorld(APIGatewayProxyRequest request, ILambdaContext context)
        {
            APIGatewayProxyResponse response;
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("hello", "world");
            response = CreateResponse(dict);
            return response;
        }

        public APIGatewayProxyResponse GetQuerystring(APIGatewayProxyRequest request, ILambdaContext context)
        {
            APIGatewayProxyResponse response;
            LogMessage(context, "Processing request startedd");
            if (request != null && request.QueryStringParameters.Count > 0)
            {
                try
                {
                    response = CreateResponse(request.QueryStringParameters);
                    LogMessage(context, "First Parameter Value to read is: " + request.QueryStringParameters["foo"]);
                    LogMessage(context, "Processing request succeeded.");
                }
                catch (Exception ex)
                {
                    LogMessage(context, string.Format("Processing request failed - {0}", ex.Message));
                    response = CreateResponse(null);
                }
            }
            else
            {
                LogMessage(context, "Processing request failed - Please add queryStringParameter 'foo' to your request - see sample in readme");
                response = CreateResponse(null);
            }
            return response;
        }

        public async Task<APIGatewayProxyResponse> QueryDynamoDb(APIGatewayProxyRequest request, ILambdaContext context)
        {
            APIGatewayProxyResponse response;
            LogMessage(context, "Processing request started");
            // LogMessage(request, "Request Data");
            AmazonDynamoDBConfig clientConfig = new AmazonDynamoDBConfig();
            clientConfig.RegionEndpoint = RegionEndpoint.USEast1;
            AmazonDynamoDBClient client = new AmazonDynamoDBClient(clientConfig); 
            Table table = Table.LoadTable(client, "Sandbox3Albe");
            string pk = "3albe";
            var queryConfig = new QueryOperationConfig
            {
                KeyExpression = new Expression
                {
                    ExpressionStatement = "pk = :v_pk",
                    ExpressionAttributeValues = new Dictionary<string, DynamoDBEntry>
                    {
                        { ":v_pk", pk }
                    }
                }
            };

            var search = table.Query(queryConfig);
            List<Document> documentList = new List<Document>();
            do
            {
                documentList.AddRange(await search.GetNextSetAsync());
            } while (!search.IsDone);

            response = CreateResponse(documentList.ToDictionary(doc => doc["pk"].AsString(), doc => doc.ToJson()));
            return response;
        }

        void LogMessage(ILambdaContext ctx, string msg)
        {
            ctx.Logger.LogLine(
                string.Format("{0}:{1} - {2}",
                    ctx.AwsRequestId,
                    ctx.FunctionName,
                    msg));
        }
        APIGatewayProxyResponse CreateResponse(IDictionary<string, string> result)
        {
            int statusCode = (result != null) ?
                (int)HttpStatusCode.OK :
                (int)HttpStatusCode.InternalServerError;

            string body = (result != null) ?
                JsonConvert.SerializeObject(result) : string.Empty;

            var response = new APIGatewayProxyResponse
            {
                StatusCode = statusCode,
                Body = body,
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" },
                    { "Access-Control-Allow-Origin", "*" }
                }
            };

            return response;
        }

    }



}
