﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace dotnet_az
{

    public class Resource
    {
        private static Dictionary<string, string> resourceTypeApiVersions = new Dictionary<string, string>
        {
            { "Microsoft.Storage/storageAccounts", "2018-07-01" },
            { "Microsoft.Resources/deployments", "2017-05-10" },
        };


        public string Condition { get; set; }



        public string Type
        {
            get; set;
        }
        public string ApiVersion
        {
            get; set;
        }
        public string Name { get; set; }
        public string Location { get; set; } = "[resourceGroup().location]";
        public Dictionary<string, string> Tags { get; set; }
        public string Comments { get; set; }
        public Copy Copy { get; set; }
        public string[] DependsOn { get; set; }
        public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
        public Sku Sku { get; set; }
        public string Kind { get; set; }
        public Plan Plan { get; set; }
        public Resource[] Resources { get; set; }

        public void SetDefaults()
        {
            if (string.IsNullOrEmpty(ApiVersion))
            {
                ApiVersion = resourceTypeApiVersions[Type];
            }
            Include = null;
        }

        [JsonIgnore]
        public string Include { get; set; }
        [JsonIgnore]
        public Dictionary<string, object> Parameters { get; set; }
    }

}