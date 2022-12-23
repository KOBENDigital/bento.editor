using System.Collections.Generic;
using Newtonsoft.Json;

namespace Bento.Core.Models
{
	public class Grid
	{
		[JsonProperty("sections")]
		public List<Section> Sections { get; set; }
	}

	public class Section
	{
		[JsonProperty("rows")]
		public List<Row> Rows { get; set; }
	}

	public class Row
	{
		[JsonProperty("areas")]
		public List<GridArea> Areas { get; set; }
	}

	public class GridArea
	{
		[JsonProperty("controls")]
		public List<Control> Controls { get; set; }
	}

	public class Control
	{
		[JsonProperty("value")]
		public object Value { get; set; }
	}
}