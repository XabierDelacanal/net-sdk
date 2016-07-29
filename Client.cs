﻿using Flurl;
using Flurl.Http;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using Flurl.Http.Content;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Signaturit
{
    public class Client
    {
        /**
         * Signaturit's production API URL
         */
        private const string PROD_BASE_URL = "https://api.signaturit.com";

        /**
         * Signaturit's sandbox API URL
         */
        private const string SANDBOX_BASE_URL = "https://api.sandbox.signaturit.com";

        /**
         * @var string
         */
        private string accessToken;

        /**
         * @var string
         */
        private string url;

        /**
         * @param string $accessToken
         * @param bool   $production
         */
        public Client(string accessToken, bool production = false)
        {
            this.accessToken = accessToken;
            this.url         = production ? PROD_BASE_URL : SANDBOX_BASE_URL;
        }
        
        /**
         * @param object $conditions
         *
         * @return int
         */
        public int countSignatures(object conditions = null)
        {
            dynamic json = jsonRequest("get", "signatures/count.json", conditions, null, null);

            return json.count;
        }

        /**
         * @param int     $limit
         * @param int     $offset
         * @param dynamic $conditions
         *
         * @return dynamic
         */
        public object getSignatures(int limit = 100, int offset = 0, dynamic conditions = null)
        {
            conditions = conditions == null ? new ExpandoObject() : dynamicToExpandoObject(conditions);

            conditions.limit  = limit;
            conditions.offset = offset;

            dynamic json = jsonRequest("get", "signatures.json", conditions, null, null);

            return json;
        }

        /**
         * @param string $signatureId
         *
         * @return dynamic
         */
        public object getSignature(string signatureId)
        {
            dynamic json = jsonRequest("get", $"signatures/{signatureId}.json", null, null, null);

            return json;
        }

        /**
         * @param string $signatureId
         * @param string $documentId
         *
         * @return string
         */
        public string downloadAuditTrail(string signatureId, string documentId)
        {
            string response = stringRequest("get", $"signatures/{signatureId}/documents/{documentId}/download/audit_trail", null, null, null);

            return response;
        }

        /**
         * @param string $signatureId
         * @param string $documentId
         *
         * @return string
         */
        public string downloadSignedDocument(string signatureId, string documentId)
        {
            string response = stringRequest("get", $"signatures/{signatureId}/documents/{documentId}/download/signed", null, null, null);

            return response;
        }

        /**
         * @param dynamic $files
         * @param dynamic $recipients
         * @param dynamic $parameters
         *
         * @return dynamic
         */
        public object createSignature(object files, object recipients, dynamic parameters = null)
        {
            parameters = parameters == null ? new ExpandoObject() : dynamicToExpandoObject(parameters);

            parameters.recipients = recipients;

            dynamic json = jsonRequest("post", "signatures.json", null, parameters, files);

            return json;
        }

        /**
         * @param string $signatureId
         *
         * @return dynamic
         */
        public object cancelSignature(string signatureId)
        {
            dynamic json = jsonRequest("patch", $"signatures/{signatureId}/cancel.json", null, null, null);

            return json;
        }

        /**
         * @param string $signatureId
         *
         * @return dynamic
         */
        public object sendReminder(string signatureId)
        {
            dynamic json = jsonRequest("post", $"signatures/{signatureId}/reminder.json", null, null, null);

            return json;
        }

        /**
         * @param string $brandingId
         *
         * @return dynamic
         */
        public object getBranding(string brandingId)
        {
            dynamic json = jsonRequest("get", $"brandings/{brandingId}.json", null, null, null);

            return json;
        }

        /**
         * @return dynamic
         */
        public object getBrandings()
        {
            dynamic json = jsonRequest("get", "brandings.json", null, null, null);

            return json;
        }

        /**
         * @param object $parameters
         *
         * @return dynamic
         */
        public object createBranding(object parameters = null)
        {
            dynamic json = jsonRequest("post", "brandings.json", null, parameters, null);

            return json;
        }

        /**
         * @param string $brandingId
         * @param object $parameters
         *
         * @return dynamic
         */
        public object updateBranding(string brandingId, object parameters = null)
        {
            dynamic json = jsonRequest("patch", $"brandings/{brandingId}.json", null, parameters, null);

            return json;
        }

        /**
         * @param int $limit
         * @param int $offset
         *
         * @return dynamic
         */
        public object getTemplates(int limit = 100, int offset = 0)
        {
            object conditions = new { limit = limit, offset = offset };

            dynamic json = jsonRequest("get", "templates.json", conditions, null, null);

            return json;
        }

        /**
         * @param object $conditions
         *
         * @return dynamic
         */
        public int countEmails(object conditions = null)
        {
            dynamic json = jsonRequest("get", "emails/count.json", conditions, null, null);

            return json.count;
        }

        /**
         * @param int     $limit
         * @param int     $offset
         * @param dynamic $conditions
         *
         * @return dynamic
         */
        public object getEmails(int limit = 100, int offset = 0, dynamic conditions = null)
        {
            conditions = conditions == null ? new ExpandoObject() : dynamicToExpandoObject(conditions);

            conditions.limit  = limit;
            conditions.offset = offset;

            dynamic json = jsonRequest("get", "emails.json", conditions, null, null);

            return json;
        }
        /**
         * @param string $emailId
         *
         * @return dynamic
         */
        public object getEmail(string emailId)
        {
            dynamic json = jsonRequest("get", $"emails/{emailId}.json", null, null, null);

            return json;
        }

        /**
         * @param dynamic $files
         * @param dynamic $recipients
         * @param string  $subject
         * @param string  $body
         * @param dynamic $parameters
         *
         * @return dynamic
         */
        public object createEmail(object files, dynamic recipients, string subject, string body, dynamic parameters = null)
        {
            parameters = parameters == null ? new ExpandoObject() : dynamicToExpandoObject(parameters);

            parameters.subject    = subject;
            parameters.body       = body;
            parameters.recipients = recipients;

            dynamic json = jsonRequest("post", "emails.json", null, parameters, files);

            return json;
        }

        /**
         * @param string $emailId
         * @param string $certificateId
         *
         * @return string
         */
        public string downloadEmailAuditTrail(string emailId, string certificateId)
        {
            string response = stringRequest("get", $"emails/{emailId}/certificates/{certificateId}/download/audit_trail", null, null, null);

            return response;
        }

        /**
         * @param object input
         *
         * @return ExpandoObject
         */
        private ExpandoObject dynamicToExpandoObject(object input)
        {
            string json = JsonConvert.SerializeObject(input);
            ExpandoObject output = JsonConvert.DeserializeObject<ExpandoObject>(json);

            return output;
        }

        /**
         * @param string $method
         * @param string $path
         * @param object $query
         * @param object $body
         * @param object files
         *
         * @return string
         */
        private string stringRequest(string method, string path, object query, object body, object files)
        {
            string response = Request(method, path, query, body, files).Result;

            return response;
        }

        /**
         * @param string $method
         * @param string $path
         * @param object $query
         * @param object $body
         * @param object files
         *
         * @return dynamic
         */
        private object jsonRequest(string method, string path, object query, object body, object files)
        {
            string response = stringRequest(method, path, query, body, files);
            dynamic json    = Newtonsoft.Json.JsonConvert.DeserializeObject(response);

            return json;
        }

        /**
         * @param string $method
         * @param string $path
         * @param object $query
         * @param object $body
         * @param object files
         *
         * @return Task<string>
         */
        private async Task<string> Request(string method, string path, object query, object body, object files)
        {
            Url url = new Url($"{this.url}/v3/{path}");

            FlurlClient Request = url
                .SetQueryParams(query)
                .WithOAuthBearerToken(this.accessToken)
                .WithHeader("User-Agent", "signaturit-net-sdk 1.0.0");
            
            switch (method)
            {
                case "get":
                    return await Request.GetAsync().ReceiveString();

                case "post":
                    if (files == null) {
                        body = body == null ? new {} : body;

                        return await Request.PostJsonAsync(body as object).ReceiveString();
                    }

                    var content = new CapturedMultipartContent();

                    foreach (string file in files as IList<string>)
                    {
                        string name = System.IO.Path.GetFileName(file);
                        string ext  = System.IO.Path.GetExtension(file);
                        string mime = ext == "pdf" ? "application/pdf" : "application/msword";

                        content.AddFile($"files[{name}]", file, mime);
                    }

                    captureMultipartContentInObject(content, body, "");

                    return await Request.PostAsync(content).ReceiveString();

                case "patch":
                    body = body == null ? new {} : body;

                    return await Request.PatchJsonAsync(body as object).ReceiveString();
            }

            return null;
        }

        /**
         * @param CapturedMultipartContent $content
         * @param object                   $body
         * @param string                   $key
         */
        private void captureMultipartContentInObject(CapturedMultipartContent content, object body, string key)
        {
            var data = JsonConvert.SerializeObject(body);
            var json = JObject.Parse(data);

            foreach (JToken child in json.Children()) {
                var property = child as JProperty;

                captureMultipartContentInJson(content, child, "");
            }
        }

        /**
         * @param CapturedMultipartContent $content
         * @param JToken                   $json
         * @param string                   $key
         */
        private void captureMultipartContentInJson(CapturedMultipartContent content, JToken json, string key)
        {
            var property = json as JProperty;

            if (property == null) {
                return;
            }

            if (property.Value.GetType().Name == "JArray") {
                int index = 0;

                foreach (JToken child in property.Values()) {
                    string valueKey = key == "" ? property.Name : $"{key}[{property.Name}]";
                    valueKey += $"[{index++}]";

                    foreach (JToken subChild in child) {
                        captureMultipartContentInJson(content, subChild, valueKey);
                    }
                }

                return;
            }

            if (property.Value.GetType().Name == "JObject") {
                string valueKey = key == "" ? property.Name : $"{key}[{property.Name}]";

                foreach (JToken child in property.Values()) {
                    captureMultipartContentInJson(content, child, valueKey);
                }

                return;
            }

            if (property.Value.GetType().Name == "JValue") {
                string valueKey = key == "" ? property.Name : $"{key}[{property.Name}]";

                content.AddString(valueKey, property.Value.ToString());

                return;
            }
        }
    }
}