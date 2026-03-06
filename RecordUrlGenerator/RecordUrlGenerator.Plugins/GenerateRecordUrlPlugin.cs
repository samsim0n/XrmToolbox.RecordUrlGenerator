using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace RecordUrlGenerator.Plugins
{
    /// <summary>
    /// Plugin that generates a URL to a record based on its ID and type.
    /// Executes in Pre-Operation Create.
    /// Expected JSON configuration:
    /// {
    ///     "envUrlVarName": "env_url_variable_name",
    ///     "recordUrlFieldName": "url_field_logical_name", 
    ///     "mdaEnvName": "mda_guid_env_variable_name"
    /// }
    /// </summary>
    public class GenerateRecordUrlPlugin : IPlugin
    {
        private readonly string _envUrlVarName;
        private readonly string _recordUrlFieldName;
        private readonly string _mdaEnvName;

        public GenerateRecordUrlPlugin(string unsecureConfig)
        {
            if (string.IsNullOrWhiteSpace(unsecureConfig))
            {
                throw new InvalidPluginExecutionException("Plugin configuration is required.");
            }

            PluginConfig config;
            try
            {
                var serializer = new DataContractJsonSerializer(typeof(PluginConfig));
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(unsecureConfig)))
                {
                    config = (PluginConfig)serializer.ReadObject(stream);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("Invalid JSON configuration: " + ex.Message, ex);
            }

            _envUrlVarName = config.EnvUrlVarName;
            _recordUrlFieldName = config.RecordUrlFieldName;
            _mdaEnvName = config.MdaEnvName;

            if (string.IsNullOrWhiteSpace(_envUrlVarName))
            {
                throw new InvalidPluginExecutionException("The 'envUrlVarName' parameter is required in the configuration.");
            }

            if (string.IsNullOrWhiteSpace(_recordUrlFieldName))
            {
                throw new InvalidPluginExecutionException("The 'recordUrlFieldName' parameter is required in the configuration.");
            }
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            // Get services
            var tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                tracingService.Trace("Starting execution of GenerateRecordUrlPlugin");

                // Context validation
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity target)
                {
                    tracingService.Trace("Target entity: " + target.LogicalName);

                    // Build the list of environment variables to retrieve
                    var envVarNames = new List<string> { _envUrlVarName };
                    if (!string.IsNullOrWhiteSpace(_mdaEnvName))
                    {
                        envVarNames.Add(_mdaEnvName);
                    }

                    // Retrieve all environment variables in a single query
                    var envVarValues = GetEnvironmentVariableValues(service, tracingService, envVarNames);

                    // Retrieve the environment URL
                    string environmentUrl;
                    envVarValues.TryGetValue(_envUrlVarName, out environmentUrl);
                    if (string.IsNullOrWhiteSpace(environmentUrl))
                    {
                        throw new InvalidPluginExecutionException("The environment variable '" + _envUrlVarName + "' is empty or not found.");
                    }

                    // Retrieve and validate the Model-Driven App GUID (optional)
                    string mdaGuid = null;
                    if (!string.IsNullOrWhiteSpace(_mdaEnvName))
                    {
                        string mdaValue;
                        envVarValues.TryGetValue(_mdaEnvName, out mdaValue);
                        if (!string.IsNullOrWhiteSpace(mdaValue))
                        {
                            Guid parsedMdaGuid;
                            if (Guid.TryParse(mdaValue, out parsedMdaGuid))
                            {
                                mdaGuid = parsedMdaGuid.ToString();
                            }
                            else
                            {
                                throw new InvalidPluginExecutionException(
                                    "The environment variable '" + _mdaEnvName + "' does not contain a valid GUID: " + mdaValue);
                            }
                        }
                        tracingService.Trace("MDA GUID retrieved: " + (mdaGuid ?? "null"));
                    }

                    // Retrieve the record ID
                    Guid recordId = context.PrimaryEntityId;

                    tracingService.Trace("Record ID: " + recordId.ToString());

                    // Generate the URL
                    string recordUrl = GenerateRecordUrl(environmentUrl, target.LogicalName, recordId, mdaGuid);
                    tracingService.Trace("Generated URL: " + recordUrl);

                    // Update the URL field in the target entity
                    target[_recordUrlFieldName] = recordUrl;

                    tracingService.Trace("Plugin executed successfully");
                }
                else
                {
                    throw new InvalidPluginExecutionException("The 'Target' parameter is missing or is not an entity.");
                }
            }
            catch (InvalidPluginExecutionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                tracingService.Trace("Error: " + ex.Message);
                throw new InvalidPluginExecutionException("An error occurred in the GenerateRecordUrlPlugin: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Generates the URL to the record
        /// </summary>
        private string GenerateRecordUrl(string environmentUrl, string entityLogicalName, Guid recordId, string mdaGuid)
        {
            // Clean the environment URL
            environmentUrl = environmentUrl.TrimEnd('/');

            if (!string.IsNullOrWhiteSpace(mdaGuid))
            {
                // URL for Model-Driven App
                // Format: https://env.crm12.dynamics.com/main.aspx?appid=GUID&pagetype=entityrecord&etn=ENTITYNAME&id=RECORDID
                return environmentUrl + "/main.aspx?appid=" + mdaGuid + "&pagetype=entityrecord&etn=" + entityLogicalName + "&id=" + recordId.ToString();
            }
            else
            {
                // Classic URL (without Model-Driven App)
                // Format: https://env.crm12.dynamics.com/main.aspx?pagetype=entityrecord&etn=ENTITYNAME&id=RECORDID
                return environmentUrl + "/main.aspx?pagetype=entityrecord&etn=" + entityLogicalName + "&id=" + recordId.ToString();
            }
        }

        /// <summary>
        /// Retrieves the values of multiple Dataverse environment variables in a single query
        /// (outer join between definition and value)
        /// </summary>
        private Dictionary<string, string> GetEnvironmentVariableValues(
            IOrganizationService service, ITracingService tracingService, List<string> variableNames)
        {
            tracingService.Trace("Retrieving environment variables: " + string.Join(", ", variableNames));

            var result = new Dictionary<string, string>();

            var query = new QueryExpression("environmentvariabledefinition")
            {
                ColumnSet = new ColumnSet("schemaname", "defaultvalue")
            };

            // IN condition on schema names
            var condition = new ConditionExpression("schemaname", ConditionOperator.In);
            foreach (var name in variableNames)
            {
                condition.Values.Add(name);
            }
            query.Criteria.Conditions.Add(condition);

            // Outer join to environmentvariablevalue to retrieve the value in a single query
            var valueLink = query.AddLink(
                "environmentvariablevalue",
                "environmentvariabledefinitionid",
                "environmentvariabledefinitionid",
                JoinOperator.LeftOuter);
            valueLink.EntityAlias = "val";
            valueLink.Columns = new ColumnSet("value");

            var results = service.RetrieveMultiple(query);

            foreach (var entity in results.Entities)
            {
                var schemaName = entity.GetAttributeValue<string>("schemaname");
                var defaultValue = entity.GetAttributeValue<string>("defaultvalue");

                var aliasedValue = entity.GetAttributeValue<AliasedValue>("val.value");
                var overrideValue = aliasedValue != null ? aliasedValue.Value as string : null;

                var resolvedValue = overrideValue ?? defaultValue;
                tracingService.Trace("Variable '" + schemaName + "' = " + (resolvedValue ?? "null"));
                result[schemaName] = resolvedValue;
            }

            return result;
        }

        /// <summary>
        /// Plugin configuration class (deserialized from JSON)
        /// </summary>
        [DataContract]
        private class PluginConfig
        {
            [DataMember(Name = "envUrlVarName")]
            public string EnvUrlVarName { get; set; }

            [DataMember(Name = "recordUrlFieldName")]
            public string RecordUrlFieldName { get; set; }

            [DataMember(Name = "mdaEnvName")]
            public string MdaEnvName { get; set; }
        }
    }
}
