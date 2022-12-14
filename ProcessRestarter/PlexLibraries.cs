// <auto-generated />
//
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using ProcessRestarter;
//
//    var plexLibraries = PlexLibraries.FromJson(jsonString);

namespace ProcessRestarter
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class PlexLibraries
    {
        [JsonProperty("?xml", NullValueHandling = NullValueHandling.Ignore)]
        public Xml Xml { get; set; }

        [JsonProperty("MediaContainer", NullValueHandling = NullValueHandling.Ignore)]
        public MediaContainer MediaContainer { get; set; }

        [JsonProperty("error", NullValueHandling = NullValueHandling.Ignore)]
        public string Error { get; set; }
    }

    public partial class MediaContainer
    {
        [JsonProperty("@size")]
        public long Size { get; set; }

        [JsonProperty("@allowSync")]
        public long AllowSync { get; set; }

        [JsonProperty("@title1")]
        public string Title1 { get; set; }

        [JsonProperty("Directory")]
        public List<Directory> Directory { get; set; }
    }

    public partial class Directory
    {
        [JsonProperty("@allowSync")]
        public long AllowSync { get; set; }

        [JsonProperty("@art")]
        public string Art { get; set; }

        [JsonProperty("@composite")]
        public string Composite { get; set; }

        [JsonProperty("@filters")]
        public long Filters { get; set; }

        [JsonProperty("@refreshing")]
        public long Refreshing { get; set; }

        [JsonProperty("@thumb")]
        public string Thumb { get; set; }

        [JsonProperty("@key")]
        public long Key { get; set; }

        [JsonProperty("@type")]
        public string Type { get; set; }

        [JsonProperty("@title")]
        public string Title { get; set; }

        [JsonProperty("@agent")]
        public string Agent { get; set; }

        [JsonProperty("@scanner")]
        public string Scanner { get; set; }

        [JsonProperty("@language")]
        public string Language { get; set; }

        [JsonProperty("@uuid")]
        public Guid Uuid { get; set; }

        [JsonProperty("@updatedAt")]
        public long UpdatedAt { get; set; }

        [JsonProperty("@createdAt")]
        public long CreatedAt { get; set; }

        [JsonProperty("@scannedAt")]
        public long ScannedAt { get; set; }

        [JsonProperty("@content")]
        public long Content { get; set; }

        [JsonProperty("@directory")]
        public long DirectoryDirectory { get; set; }

        [JsonProperty("@contentChangedAt")]
        public long ContentChangedAt { get; set; }

        [JsonProperty("@hidden")]
        public long Hidden { get; set; }
    }

    public partial class LocationElement
    {
        [JsonProperty("@id")]
        public long Id { get; set; }

        [JsonProperty("@path")]
        public string Path { get; set; }
    }

    public partial class Xml
    {
        [JsonProperty("@version")]
        public string Version { get; set; }

        [JsonProperty("@encoding")]
        public string Encoding { get; set; }
    }
}
