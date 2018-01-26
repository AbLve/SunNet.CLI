using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ModelBinding;
using Newtonsoft.Json;
using Sunnet.Framework.Core.Tool;

namespace Sunnet.Cli.UIBase.Models
{
    /// <summary>
    /// 提交表单的Response信息类
    /// </summary>
    public class PostFormResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostFormResponse"/> class.
        /// Success is false for default.
        /// </summary>
        public PostFormResponse()
            : this(false)
        {
        }

        public PostFormResponse(bool success)
        {
            Success = success;
        }

        /// <summary>
        /// Updates the response properties from specified result.
        /// </summary>
        /// <param name="result">The result.</param>
        public void Update(OperationResult result)
        {
            this.Success = result.ResultType == OperationResultType.Success;
            this.Message = result.Message;
            this.Data = result.AppendData;
        }

        public static PostFormResponse GetSuccessResponse(object data)
        {
            PostFormResponse response = new PostFormResponse();
            response.Success = true;
            response.Data = data;
            return response;
        }
        public static PostFormResponse GetFailResponse(string message)
        {
            PostFormResponse response = new PostFormResponse();
            response.Success = true;
            response.Message = message;
            return response;
        }

        [JsonProperty("success")]
        public bool Success { get; set; }

        public string OtherMsg { get; set; }

        [JsonProperty("data")]
        public object Data { get; set; }
        [JsonProperty("msg")]
        public string Message { get; set; }

        [JsonProperty("modelState")]
        public object ModelState { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance in JSON format.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        /// Author : JackZhang
        /// Date   : 6/22/2015 09:59:58
        public override string ToString()
        {
            return JsonHelper.SerializeObject(this);
        }
    }
}