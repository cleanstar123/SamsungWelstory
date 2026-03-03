using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace CMS.API.Models
{
    public class CodeModel
    {
        public string CODE_GROUP      { get; set; }
        public string CODE            { get; set; }
        public string CODE_NAME       { get; set; }
        public string DISPLAY_SEQ     { get; set; }
        public string CODE_DESC       { get; set; }
        public string CODE_ATTR1      { get; set; }
        public string CODE_ATTR2      { get; set; }
        public string CODE_ATTR3      { get; set; }
    }
}