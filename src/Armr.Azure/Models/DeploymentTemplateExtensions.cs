﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Armr.Models
{
    public static class DeploymentTemplateExtensions
    {
    }


    public interface ITemplateBuilder : IBuilder<ArmDeploymentTemplate>
    {
        ITemplateBuilder Schema(string schemaUrl);
        ITemplateBuilder ContentVersion(string version);
        ITemplateBuilder ApiVersion(string version);
        ITemplateBuilder Parameters(Func<IParametersBuilder, IParametersBuilder> parameterBuilder);
        ITemplateBuilder Variables(Func<IVariablesBuilder, IVariablesBuilder> variablesBuilder);
        ITemplateBuilder Functions(Func<IFunctionsBuilder, IFunctionsBuilder> functionsBuilder);
    }
    public interface IParametersBuilder : IBuilder<Dictionary<string, Parameter>>
    {
        IParametersBuilder Add<T>(string name, object defaultValue = null, int? minLength = null, int? maxLength = null, int? minValue = null, int? maxValue = null, params object[] allowedValues) where T : Parameter;
        IParametersBuilder String(string name, string defaultValue = null, int? minLength = null, int? maxLength = null, params string[] allowedValues);
        IParametersBuilder SecureString(string name, string defaultValue = null, int? minLength = null, int? maxLength = null, params string[] allowedValues);
        IParametersBuilder Integer(string name, int? defaultValue = null, int? minValue = null, int? maxValue = null, params int[] allowedValues);
        IParametersBuilder Boolean(string name, bool defaultValue = false);
        IParametersBuilder Object(string name, object defaultValue = null, params object[] allowedValues);
        IParametersBuilder SecureObject(string name, object defaultValue = null, params object[] allowedValues);
        IParametersBuilder Array(string name, object[] defaultValue = null, params object[][] allowedValues);
    }

    public interface IVariablesBuilder : IBuilder<Dictionary<string, object>>
    {
        IVariablesBuilder Define<T>(string name, T value);
        IVariablesBuilder Define(object variable);
    }

    public interface IResourcesBuilder : IBuilder<IEnumerable<Resource>>
    {
     //   IResourcesBuilder Add<T>(string name) where T : Resource;
        IResourcesBuilder Add<T>(T instance) where T : Resource;
        IResourcesBuilder Add<T>(string apiVersion = null, string type = null, string name = null, string location = null, Dictionary<string, string> tags = null, Dictionary<string, object> properties = null, Sku sku = null, string kind = null, Plan Plan = null, Action<IResourcesBuilder> resources = null) where T : Resource;
        IResourcesBuilder Add(string apiVersion, string type, string name, string location = null, Dictionary<string, string> tags = null, Dictionary<string, object> properties = null, Sku sku = null, string kind = null, Plan Plan = null, Action<IResourcesBuilder> resources = null);
    }


    public class ResourcesBuilder : IResourcesBuilder
    {
        private List<Resource> resources = new List<Resource>();

        public IResourcesBuilder Add<T>(T instance) where T : Resource
        {
            //instance = Activator.CreateInstance<T>();
            resources.Add(instance);
            return this;
        }
       
        public IResourcesBuilder Add<T>(string apiVersion = null, string type = null, string name = null, string location = null, Dictionary<string, string> tags = null, Dictionary<string, object> properties = null, Sku sku = null, string kind = null, Plan plan = null, Action<IResourcesBuilder> resources = null) where T : Resource
        {
            var resource = Activator.CreateInstance<T>();
            resource.Name = name;
            resource.Type = type;
            resource.ApiVersion = apiVersion;
            resource.Location = location;
            resource.Tags = tags;
            resource.Properties = properties;
            resource.Sku = sku;
            resource.Plan = plan;
            resource.Kind = kind;

            if (resources != null)
            {
                var children = new ResourcesBuilder();
                resources(children);
                resource.Resources = children.Build().ToArray();
            }

            this.resources.Add(resource);
            return this;
        }

        public IResourcesBuilder Add(string apiVersion = null, string type = null, string name = null, string location = null, Dictionary<string, string> tags = null, Dictionary<string, object> properties = null, Sku sku = null, string kind = null, Plan plan = null, Action<IResourcesBuilder> resources = null)
        {
            return Add<Resource>(apiVersion, type, name, location, tags, properties, sku, kind, plan, resources);
        }

        public IEnumerable<Resource> Build()
        {
            return this.resources;
        }

    }

    public interface IFunctionsBuilder : IBuilder<object[]>
    {
        IFunctionsBuilder Define<T>(string name, T value);
        IFunctionsBuilder Define(object function);
    }

    public class FunctionsBuilder : IFunctionsBuilder
    {
        private List<object> functions = new List<object>();
        public object[] Build()
        {
            return functions.ToArray();
        }

        public IFunctionsBuilder Define<T>(string name, T value)
        {
            functions.Add(new KeyValuePair<string, object>(name, value));
            return this;
        }

        public IFunctionsBuilder Define(object function)
        {
            functions.Add(function);
            return this;
        }
    }

    public class VariablesBuilder : IVariablesBuilder
    {
        private Dictionary<string, object> variables = new Dictionary<string, object>();

        public Dictionary<string, object> Build()
        {
            return variables;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => variables.GetEnumerator();

        public IVariablesBuilder Define<T>(string name, T value)
        {
            variables.Add(name, value);
            return this;
        }

        public IVariablesBuilder Define(object variable)
        {
            var properties = variable.GetType().GetProperties();
            foreach (var property in properties)
            {
                var key = property.Name;
                var value = property.GetValue(variable);
                Define(key, value);
            }

            return this;
        }
    }

    public class ParametersBuilder : IParametersBuilder
    {
        private Dictionary<string, Parameter> parameters = new Dictionary<string, Parameter>();


        public IParametersBuilder Array(string name, object[] defaultValue = null, params object[][] allowedValues)
        {
            throw new NotImplementedException();
        }

        public IParametersBuilder Boolean(string name, bool defaultValue = false)
        {
            throw new NotImplementedException();
        }

        public IParametersBuilder Integer(string name, int? defaultValue = null, int? minValue = null, int? maxValue = null, params int[] allowedValues)
        {
            parameters.Add(name, new IntParameter { DefaultValue = defaultValue, MinValue = minValue, MaxValue = maxValue });
            return this;
        }

        public IParametersBuilder Object(string name, object defaultValue = null, params object[] allowedValues)
        {
            throw new NotImplementedException();
        }

        public IParametersBuilder SecureObject(string name, object defaultValue = null, params object[] allowedValues)
        {
            throw new NotImplementedException();
        }

        public IParametersBuilder SecureString(string name, string defaultValue = null, int? minLength = null, int? maxLength = null, params string[] allowedValues)
        {
            throw new NotImplementedException();
        }

        public IParametersBuilder String(string name, string defaultValue = null, int? minLength = null, int? maxLength = null, params string[] allowedValues)
        {
            parameters.Add(name, new StringParameter { DefaultValue = defaultValue, MinLength = minLength, MaxLength = maxLength });
            return this;
        }



        public IEnumerator<KeyValuePair<string, Parameter>> GetEnumerator()
        {
            return parameters.GetEnumerator();
        }

        public Dictionary<string, Parameter> Build()
        {
            return parameters;
        }



        public IParametersBuilder Add<T>(string name, object defaultValue = null, int? minLength = null, int? maxLength = null, int? minValue = null, int? maxValue = null, params object[] allowedValues) where T : Parameter
        {
            var p = Activator.CreateInstance<T>();
            // parameters.Add(name, p defaultValue, minLength, maxLength, minValue, maxValue, allowedValues);
            return this;
        }

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return parameters.GetEnumerator();
        //}
    }

    public class Armr
    {
        public static ITemplateBuilder Create()
        {
            return new TemplateBuilder();
        }
    }

    public interface IBuilder<T>
    {
        T Build();
    }

    public interface IBuilder : IBuilder<object> { }

    public class TemplateBuilder : ITemplateBuilder
    {
        private ArmDeploymentTemplate template;
        public TemplateBuilder()
        {
            template = new ArmDeploymentTemplate();
        }
        public ITemplateBuilder Schema(string schemaUrl)
        {
            template.Schema = schemaUrl;
            return this;
        }
        public ITemplateBuilder ContentVersion(string version)
        {
            template.ContentVersion = version;
            return this;
        }
        public ITemplateBuilder ApiVersion(string version)
        {
            template.ApiProfile = version;
            return this;
        }



        Func<IParametersBuilder, IParametersBuilder> parameters;
        Func<IVariablesBuilder, IVariablesBuilder> variables;
        Func<IFunctionsBuilder, IFunctionsBuilder> functions;

        public ITemplateBuilder Parameters(Func<IParametersBuilder, IParametersBuilder> parameterBuilder)
        {
            parameters = parameterBuilder;


            return this;
        }

        public ArmDeploymentTemplate Build()
        {


            template.Parameters = parameters(new ParametersBuilder()).Build();
            template.Variables = variables(new VariablesBuilder()).Build();
            template.Functions = functions(new FunctionsBuilder()).Build();



            return template;
        }

        public ITemplateBuilder Variables(Func<IVariablesBuilder, IVariablesBuilder> variablesBuilder)
        {
            variables = variablesBuilder;

            return this;
        }

        public ITemplateBuilder Functions(Func<IFunctionsBuilder, IFunctionsBuilder> functionsBuilder)
        {
            functions = functionsBuilder;
            return this;
        }
    }
}