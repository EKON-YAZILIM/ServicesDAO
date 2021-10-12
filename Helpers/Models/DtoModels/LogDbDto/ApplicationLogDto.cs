using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Helpers.Models.DtoModels.LogDbDto
{
    [Serializable]
    public partial class ApplicationLogDto
    {       
        public int ApplicationLogID { get; set; }      
        public string Application { get; set; }       
        public string Server { get; set; }       
        public DateTime Date { get; set; }       
        public string IdFieldName { get; set; }       
        public int IdField { get; set; }       
        public string Type { get; set; }     
        public string Explanation { get; set; }
    }
}
