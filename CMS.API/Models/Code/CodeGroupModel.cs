using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace CMS.API.Models
{
    public class CodeGroupModel
    {
        public string CODE_GROUP       { get; set; }
        public string CODE_GROUP_NM    { get; set; }
        public string CODE_GROUP_DESC  { get; set; }
        public int    DISPLAY_SEQ      { get; set; }
        public string CODE_GROUP_ATTR1 { get; set; }
        public string CODE_GROUP_ATTR2 { get; set; }
        public string CODE_GROUP_ATTR3 { get; set; }
    }
}