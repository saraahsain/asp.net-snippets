public class httpRequestMethods
{
    /*
    Return customized HttpResponseMessage
    */
    public HttpResponseMessage internalHttpResponseMessage ()
    {
        resultInternalServer = new HttpResponseMessage();
        resultInternalServer.StatusCode = HttpStatusCode.InternalServerError; //OR .BadRequest
        resultInternalServer.ReasonPhrase = "";

        resultSuccess = new HttpResponseMessage();
        resultSuccess.StatusCode = HttpStatusCode.OK; //OR .BadRequest
        resultSuccess.ReasonPhrase = "";

        return resultSuccess;
        //OR Request.CreateResponse(HttpStatusCode.OK);
    }

    /*
    Send formdata content via HttpClient Post
    */
    public string HttpClientFormData()
    {
        string responseJSON = "";
        var client = new HttpClient();
        client.BaseAddress = new Uri("My URL");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var content = new FormUrlEncodedContent(new[]
        {
            //Token formdata
            new KeyValuePair<string, string>("Username", ConfigurationManager.AppSettings["usernameToken"]),
            new KeyValuePair<string, string>("Password", ConfigurationManager.AppSettings["passwordToken"]),
            new KeyValuePair<string, string>("grant_type", "password")
        });
        var result = new HttpResponseMessage();
            try
            {
                result = client.PostAsync("Token", content).Result;
                if (!response.IsSuccessStatusCode)
                    throw // your message here
                responseJSON = result.Content.ReadAsStringAsync().Result;
                var deserialized = // new dynamicDTO()
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(responseJSON));
                DataContractJsonSerializer ser = new DataContractJsonSerializer(deserialized.GetType());
                deserialized = ser.ReadObject(ms) as dynamicDTO;
                ms.Close();
                Result = deserialized;
            }
            catch (Exception ex)
            {
                // catch error
            }
            return Result;
    }

    /*
    Send Obj via Post Http request
    */
    public string SendObjHttpRequestMsg(long ID, dynamic MyObj)
    {
        var body = GenericJsonSerializer.Serialize(MyObj);
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri("My URL"),
            Method = HttpMethod.Post,
            Content = new StringContent(body) // If byte[] new ByteArrayContent(Data)
        };
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        request.Headers.Add("Authorization", "Bearer " + token);
        HttpResponseMessage response = null;
        string responseString = null;
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            response = client.SendAsync(request).Result;
        }
        if (!response.IsSuccessStatusCode)
        {
            var errorResult = response.Content.ReadAsStringAsync();
            if (errorResult != null)
                throw new Exception(errorResult);
        }
        responseString = response.Content.ReadAsStringAsync();
        return responseString;
    }

    /*
    Send bytes via HttpClient
    */
    public string SendByteFile(long ID, byte[] fileBytes)
    {
        /*
        - This method should be send to an IHttpActionResult
        - And the file is read via the HttpContext
        - Ex:
            var files = HttpContext.Current.Request.Files;
            if (files == null || files.Count )
                throw new ArgumentException("No file detected");
            using (var binaryReader = new BinaryReader(files[0].InputStream))
                SignedBytes = binaryReader.ReadBytes(files[0].ContentLength);
        */

        // Get authorization Token
        string token = GetAccessToken();
        var client = new HttpClient();
        //Send the Authorization in the header, if any. In this example the server uses Oauth 2.0
        // Can use client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
        //Tested locally
        client.BaseAddress = new Uri("http://localhost:60345/");
        client.DefaultRequestHeaders.Accept.Clear();
        /*
        - If we need to serialize an object and send it
            string myContent = JsonConvert.SerializeObject(jsonObj);
            var buffer = Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        */
        try
        {
            var url = "url/get?ID=" + ID;
            var result = client.PostAsync(url, new ByteArrayContent(Data)).Result;
            var response = result.Content.ReadAsStringAsync().Result;
            if (!result.IsSuccessStatusCode)
            {
                var errorResult = result.Content.ReadAsStringAsync(); // if byte[] .Content.ReadAsByteArrayAsync().Result;
                if (errorResult != null)
                    throw new Exception(errorResult);
            }
            return response;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}