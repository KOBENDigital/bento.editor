using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bento.Core.Models
{
    public class LoadStackPreviewRequest
    {
        [JsonProperty("stackItems")]
        public string StackItems { get; set; }
    }
}
