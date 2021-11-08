using System;

namespace Helpers.Models.SharedModels
{   
    /// <summary>
    ///  Simple http response class
    /// </summary>
    [Serializable]
    public class SimpleResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Content { get; set; }
    }
}
