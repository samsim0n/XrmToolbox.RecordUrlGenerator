using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace RecordUrlGeneratorTool.Helpers
{
    public static class CrmServiceHelper
    {
        public const string PluginAssemblyName = "RecordUrlGenerator.Plugins";
        public const string PluginTypeName = "RecordUrlGenerator.Plugins.GenerateRecordUrlPlugin";

        #region Component Type Constants

        public const int ComponentType_Entity = 1;
        public const int ComponentType_Attribute = 2;
        public const int ComponentType_PluginAssembly = 91;
        public const int ComponentType_PluginType = 90;
        public const int ComponentType_SdkMessageProcessingStep = 92;
        public const int ComponentType_EnvironmentVariableDefinition = 380;

        #endregion

        #region Solutions

        public static List<Entity> GetUnmanagedSolutions(IOrganizationService service)
        {
            var query = new QueryExpression("solution")
            {
                ColumnSet = new ColumnSet("friendlyname", "uniquename", "version", "publisherid"),
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression("ismanaged", ConditionOperator.Equal, false),
                        new ConditionExpression("isvisible", ConditionOperator.Equal, true),
                        new ConditionExpression("uniquename", ConditionOperator.NotEqual, "Default"),
                        new ConditionExpression("uniquename", ConditionOperator.NotEqual, "Active"),
                        new ConditionExpression("uniquename", ConditionOperator.NotEqual, "Basic")
                    }
                },
                Orders = { new OrderExpression("friendlyname", OrderType.Ascending) }
            };

            return service.RetrieveMultiple(query).Entities.ToList();
        }

        public static Entity GetPublisher(IOrganizationService service, Guid publisherId)
        {
            return service.Retrieve("publisher", publisherId,
                new ColumnSet("customizationprefix", "friendlyname", "uniquename"));
        }

        #endregion

        #region Environment Variables

        public static List<Entity> GetEnvironmentVariablesInSolution(IOrganizationService service, Guid solutionId)
        {
            var compQuery = new QueryExpression("solutioncomponent")
            {
                ColumnSet = new ColumnSet("objectid"),
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression("solutionid", ConditionOperator.Equal, solutionId),
                        new ConditionExpression("componenttype", ConditionOperator.Equal, ComponentType_EnvironmentVariableDefinition)
                    }
                }
            };

            var components = service.RetrieveMultiple(compQuery);
            if (components.Entities.Count == 0)
                return new List<Entity>();

            var ids = components.Entities
                .Select(c => c.GetAttributeValue<Guid>("objectid"))
                .Cast<object>()
                .ToArray();

            var query = new QueryExpression("environmentvariabledefinition")
            {
                ColumnSet = new ColumnSet("schemaname", "displayname", "defaultvalue", "type"),
                Criteria = new FilterExpression()
            };

            query.Criteria.Conditions.Add(
                new ConditionExpression("environmentvariabledefinitionid", ConditionOperator.In, ids));

            // Left outer join to get override value
            var valueLink = query.AddLink("environmentvariablevalue",
                "environmentvariabledefinitionid", "environmentvariabledefinitionid", JoinOperator.LeftOuter);
            valueLink.EntityAlias = "val";
            valueLink.Columns = new ColumnSet("value");

            return service.RetrieveMultiple(query).Entities.ToList();
        }

        public static Guid CreateEnvironmentVariable(IOrganizationService service, string schemaName,
            string displayName, string defaultValue, string description = null)
        {
            var envVar = new Entity("environmentvariabledefinition")
            {
                ["schemaname"] = schemaName,
                ["displayname"] = displayName,
                ["type"] = new OptionSetValue(100000000) // String
            };

            if (!string.IsNullOrEmpty(defaultValue))
                envVar["defaultvalue"] = defaultValue;

            if (!string.IsNullOrEmpty(description))
                envVar["description"] = description;

            return service.Create(envVar);
        }

        public static void SetEnvironmentVariableValue(IOrganizationService service, Guid definitionId, string value)
        {
            // Check if a value record already exists
            var query = new QueryExpression("environmentvariablevalue")
            {
                ColumnSet = new ColumnSet("environmentvariablevalueid"),
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression("environmentvariabledefinitionid", ConditionOperator.Equal, definitionId)
                    }
                }
            };

            var results = service.RetrieveMultiple(query);
            if (results.Entities.Count > 0)
            {
                var update = new Entity("environmentvariablevalue", results.Entities[0].Id)
                {
                    ["value"] = value
                };
                service.Update(update);
            }
            else
            {
                var create = new Entity("environmentvariablevalue")
                {
                    ["environmentvariabledefinitionid"] = new EntityReference("environmentvariabledefinition", definitionId),
                    ["value"] = value
                };
                service.Create(create);
            }
        }

        /// <summary>
        /// Gets the resolved value of an environment variable (override value ?? default value)
        /// </summary>
        public static string GetResolvedEnvVarValue(Entity envVarDefinition)
        {
            var aliased = envVarDefinition.GetAttributeValue<AliasedValue>("val.value");
            var overrideValue = aliased != null ? aliased.Value as string : null;
            var defaultValue = envVarDefinition.GetAttributeValue<string>("defaultvalue");
            return overrideValue ?? defaultValue;
        }

        #endregion

        #region Plugin Assembly

        public static Entity GetPluginAssembly(IOrganizationService service, string assemblyName)
        {
            var query = new QueryExpression("pluginassembly")
            {
                ColumnSet = new ColumnSet("name", "version", "pluginassemblyid"),
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression("name", ConditionOperator.Equal, assemblyName)
                    }
                }
            };

            return service.RetrieveMultiple(query).Entities.FirstOrDefault();
        }

        public static byte[] GetEmbeddedAssemblyBytes()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var resourceName = "RecordUrlGeneratorTool.Assembly.RecordUrlGenerator.Plugins.dll";

            using (var stream = executingAssembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    throw new InvalidOperationException("Embedded resource '" + resourceName + "' not found.");

                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }

        public static (string version, string publicKeyToken) GetAssemblyInfo(byte[] assemblyBytes)
        {
            var tempPath = Path.GetTempFileName();
            try
            {
                File.WriteAllBytes(tempPath, assemblyBytes);
                var asmName = AssemblyName.GetAssemblyName(tempPath);
                var version = asmName.Version.ToString();
                var tokenBytes = asmName.GetPublicKeyToken();
                var publicKeyToken = tokenBytes != null && tokenBytes.Length > 0
                    ? string.Join("", tokenBytes.Select(b => b.ToString("x2")))
                    : null;
                return (version, publicKeyToken);
            }
            finally
            {
                try { File.Delete(tempPath); } catch { }
            }
        }

        public static Guid RegisterPluginAssembly(IOrganizationService service, byte[] assemblyBytes)
        {
            var info = GetAssemblyInfo(assemblyBytes);

            var assembly = new Entity("pluginassembly")
            {
                ["name"] = PluginAssemblyName,
                ["content"] = Convert.ToBase64String(assemblyBytes),
                ["isolationmode"] = new OptionSetValue(2), // Sandbox
                ["sourcetype"] = new OptionSetValue(0),    // Database
                ["culture"] = "neutral",
                ["version"] = info.version
            };

            if (!string.IsNullOrEmpty(info.publicKeyToken))
                assembly["publickeytoken"] = info.publicKeyToken;

            return service.Create(assembly);
        }

        public static void UpdatePluginAssembly(IOrganizationService service, Guid assemblyId, byte[] assemblyBytes)
        {
            var info = GetAssemblyInfo(assemblyBytes);

            var assembly = new Entity("pluginassembly", assemblyId)
            {
                ["content"] = Convert.ToBase64String(assemblyBytes),
                ["version"] = info.version
            };

            service.Update(assembly);
        }

        #endregion

        #region Plugin Type

        public static Entity GetPluginType(IOrganizationService service, Guid assemblyId)
        {
            var query = new QueryExpression("plugintype")
            {
                ColumnSet = new ColumnSet("plugintypeid", "typename", "name"),
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression("pluginassemblyid", ConditionOperator.Equal, assemblyId),
                        new ConditionExpression("typename", ConditionOperator.Equal, PluginTypeName)
                    }
                }
            };

            return service.RetrieveMultiple(query).Entities.FirstOrDefault();
        }

        public static Guid RegisterPluginType(IOrganizationService service, Guid assemblyId)
        {
            var pluginType = new Entity("plugintype")
            {
                ["pluginassemblyid"] = new EntityReference("pluginassembly", assemblyId),
                ["typename"] = PluginTypeName,
                ["friendlyname"] = "Generate Record URL",
                ["name"] = PluginTypeName
            };

            return service.Create(pluginType);
        }

        #endregion

        #region Plugin Steps

        public static Entity GetPluginStepForEntity(IOrganizationService service, Guid pluginTypeId, string entityLogicalName)
        {
            var query = new QueryExpression("sdkmessageprocessingstep")
            {
                ColumnSet = new ColumnSet("sdkmessageprocessingstepid", "name", "configuration",
                    "stage", "mode", "rank", "statecode"),
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression("plugintypeid", ConditionOperator.Equal, pluginTypeId)
                    }
                }
            };

            // Join to sdkmessagefilter to check entity
            var filterLink = query.AddLink("sdkmessagefilter", "sdkmessagefilterid", "sdkmessagefilterid");
            filterLink.EntityAlias = "filter";
            filterLink.LinkCriteria.Conditions.Add(
                new ConditionExpression("primaryobjecttypecode", ConditionOperator.Equal, entityLogicalName));

            // Join to sdkmessage to check it's a Create message
            var messageLink = query.AddLink("sdkmessage", "sdkmessageid", "sdkmessageid");
            messageLink.EntityAlias = "msg";
            messageLink.LinkCriteria.Conditions.Add(
                new ConditionExpression("name", ConditionOperator.Equal, "Create"));

            return service.RetrieveMultiple(query).Entities.FirstOrDefault();
        }

        public static Guid CreatePluginStep(IOrganizationService service, Guid pluginTypeId,
            string entityLogicalName, string configuration)
        {
            // Get the SdkMessage for "Create"
            var messageQuery = new QueryExpression("sdkmessage")
            {
                ColumnSet = new ColumnSet("sdkmessageid"),
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression("name", ConditionOperator.Equal, "Create")
                    }
                }
            };
            var message = service.RetrieveMultiple(messageQuery).Entities.First();

            // Get the SdkMessageFilter for this entity + message
            var filterQuery = new QueryExpression("sdkmessagefilter")
            {
                ColumnSet = new ColumnSet("sdkmessagefilterid"),
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression("sdkmessageid", ConditionOperator.Equal, message.Id),
                        new ConditionExpression("primaryobjecttypecode", ConditionOperator.Equal, entityLogicalName)
                    }
                }
            };
            var filter = service.RetrieveMultiple(filterQuery).Entities.FirstOrDefault();
            if (filter == null)
                throw new InvalidOperationException("No SDK Message Filter found for entity '" + entityLogicalName + "' and message 'Create'.");

            var step = new Entity("sdkmessageprocessingstep")
            {
                ["name"] = "RecordUrlGenerator: Generate URL on " + entityLogicalName + " Create",
                ["plugintypeid"] = new EntityReference("plugintype", pluginTypeId),
                ["sdkmessageid"] = new EntityReference("sdkmessage", message.Id),
                ["sdkmessagefilterid"] = new EntityReference("sdkmessagefilter", filter.Id),
                ["stage"] = new OptionSetValue(20),        // Pre-Operation
                ["mode"] = new OptionSetValue(0),          // Synchronous
                ["rank"] = 1,
                ["configuration"] = configuration,
                ["supporteddeployment"] = new OptionSetValue(0) // Server only
            };

            return service.Create(step);
        }

        public static void UpdatePluginStepConfiguration(IOrganizationService service, Guid stepId, string configuration)
        {
            var step = new Entity("sdkmessageprocessingstep", stepId)
            {
                ["configuration"] = configuration
            };
            service.Update(step);
        }

        public static void SetPluginStepState(IOrganizationService service, Guid stepId, bool enabled)
        {
            var request = new SetStateRequest
            {
                EntityMoniker = new EntityReference("sdkmessageprocessingstep", stepId),
                State = new OptionSetValue(enabled ? 0 : 1),
                Status = new OptionSetValue(enabled ? 1 : 2)
            };
            service.Execute(request);
        }

        #endregion

        #region Tables (Entities)

        public static List<EntityMetadata> GetTablesInSolution(IOrganizationService service, Guid solutionId)
        {
            // Get entity component IDs from solution
            var compQuery = new QueryExpression("solutioncomponent")
            {
                ColumnSet = new ColumnSet("objectid"),
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression("solutionid", ConditionOperator.Equal, solutionId),
                        new ConditionExpression("componenttype", ConditionOperator.Equal, ComponentType_Entity)
                    }
                }
            };
            var components = service.RetrieveMultiple(compQuery);
            if (components.Entities.Count == 0)
                return new List<EntityMetadata>();

            var entityIds = new HashSet<Guid>(
                components.Entities.Select(c => c.GetAttributeValue<Guid>("objectid")));

            // Get all entities metadata in one call
            var request = new RetrieveAllEntitiesRequest
            {
                EntityFilters = EntityFilters.Entity
            };
            var response = (RetrieveAllEntitiesResponse)service.Execute(request);

            return response.EntityMetadata
                .Where(e => e.MetadataId.HasValue && entityIds.Contains(e.MetadataId.Value))
                .OrderBy(e => e.DisplayName?.UserLocalizedLabel?.Label ?? e.LogicalName)
                .ToList();
        }

        #endregion

        #region Fields (Attributes)

        public static List<AttributeMetadata> GetStringFieldsForEntity(IOrganizationService service, string entityLogicalName)
        {
            var request = new RetrieveEntityRequest
            {
                LogicalName = entityLogicalName,
                EntityFilters = EntityFilters.Attributes
            };

            var response = (RetrieveEntityResponse)service.Execute(request);

            return response.EntityMetadata.Attributes
                .Where(a => a is StringAttributeMetadata)
                .OrderBy(a => a.LogicalName)
                .ToList();
        }

        public static void CreateUrlField(IOrganizationService service, string entityLogicalName,
            string schemaName, string displayName, string solutionUniqueName, int maxLength = 1000)
        {
            var request = new CreateAttributeRequest
            {
                EntityName = entityLogicalName,
                Attribute = new StringAttributeMetadata
                {
                    SchemaName = schemaName,
                    DisplayName = new Label(displayName, 1033),
                    RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None),
                    MaxLength = maxLength,
                    FormatName = StringFormatName.Url,
                    Description = new Label("Auto-generated URL to this record", 1033)
                }
            };

            // Add directly to the solution
            request["SolutionUniqueName"] = solutionUniqueName;

            service.Execute(request);
        }

        public static void PublishEntity(IOrganizationService service, string entityLogicalName)
        {
            var request = new PublishXmlRequest
            {
                ParameterXml = "<importexportxml><entities><entity>" + entityLogicalName + "</entity></entities></importexportxml>"
            };
            service.Execute(request);
        }

        public static EntityMetadata GetEntityMetadata(IOrganizationService service, string entityLogicalName)
        {
            var request = new RetrieveEntityRequest
            {
                LogicalName = entityLogicalName,
                EntityFilters = EntityFilters.Entity | EntityFilters.Attributes
            };
            var response = (RetrieveEntityResponse)service.Execute(request);
            return response.EntityMetadata;
        }

        #endregion

        #region Records

        public static (List<Entity> records, int totalCount, bool moreRecords, string pagingCookie)
            GetRecords(IOrganizationService service, string entityLogicalName,
            string primaryNameField, string urlFieldName, string filter,
            int page, int pageSize, string pagingCookie)
        {
            var columns = new List<string> { urlFieldName };
            if (!string.IsNullOrEmpty(primaryNameField))
                columns.Add(primaryNameField);

            var query = new QueryExpression(entityLogicalName)
            {
                ColumnSet = new ColumnSet(columns.ToArray()),
                PageInfo = new PagingInfo
                {
                    PageNumber = page,
                    Count = pageSize,
                    ReturnTotalRecordCount = true
                }
            };

            if (!string.IsNullOrEmpty(pagingCookie))
                query.PageInfo.PagingCookie = pagingCookie;

            if (filter == "empty")
            {
                var filterExp = new FilterExpression(LogicalOperator.Or);
                filterExp.Conditions.Add(new ConditionExpression(urlFieldName, ConditionOperator.Null));
                filterExp.Conditions.Add(new ConditionExpression(urlFieldName, ConditionOperator.Equal, ""));
                query.Criteria.AddFilter(filterExp);
            }
            else if (filter == "filled")
            {
                query.Criteria.Conditions.Add(new ConditionExpression(urlFieldName, ConditionOperator.NotNull));
                query.Criteria.Conditions.Add(new ConditionExpression(urlFieldName, ConditionOperator.NotEqual, ""));
            }

            var result = service.RetrieveMultiple(query);

            return (result.Entities.ToList(), result.TotalRecordCount,
                result.MoreRecords, result.PagingCookie);
        }

        public static int BatchUpdateRecordUrls(IOrganizationService service, string entityLogicalName,
            string urlFieldName, string environmentUrl, string mdaGuid,
            Action<int> progressCallback, CancellationToken cancellationToken)
        {
            environmentUrl = environmentUrl.TrimEnd('/');
            int totalUpdated = 0;
            int page = 1;
            bool moreRecords = true;
            string pagingCookie = null;

            while (moreRecords)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var query = new QueryExpression(entityLogicalName)
                {
                    ColumnSet = new ColumnSet(false), // Only need the ID
                    PageInfo = new PagingInfo
                    {
                        PageNumber = page,
                        Count = 500
                    }
                };

                if (!string.IsNullOrEmpty(pagingCookie))
                    query.PageInfo.PagingCookie = pagingCookie;

                var filterExp = new FilterExpression(LogicalOperator.Or);
                filterExp.Conditions.Add(new ConditionExpression(urlFieldName, ConditionOperator.Null));
                filterExp.Conditions.Add(new ConditionExpression(urlFieldName, ConditionOperator.Equal, ""));
                query.Criteria.AddFilter(filterExp);

                var results = service.RetrieveMultiple(query);

                if (results.Entities.Count == 0)
                    break;

                // Build update requests
                var requests = new OrganizationRequestCollection();
                foreach (var record in results.Entities)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    string url;
                    if (!string.IsNullOrWhiteSpace(mdaGuid))
                    {
                        url = environmentUrl + "/main.aspx?appid=" + mdaGuid +
                              "&pagetype=entityrecord&etn=" + entityLogicalName +
                              "&id=" + record.Id.ToString();
                    }
                    else
                    {
                        url = environmentUrl + "/main.aspx?pagetype=entityrecord&etn=" +
                              entityLogicalName + "&id=" + record.Id.ToString();
                    }

                    var update = new Entity(entityLogicalName, record.Id);
                    update[urlFieldName] = url;
                    requests.Add(new UpdateRequest { Target = update });
                }

                // Execute in batches of 100
                for (int i = 0; i < requests.Count; i += 100)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var batch = new ExecuteMultipleRequest
                    {
                        Settings = new ExecuteMultipleSettings
                        {
                            ContinueOnError = true,
                            ReturnResponses = false
                        },
                        Requests = new OrganizationRequestCollection()
                    };

                    var batchEnd = Math.Min(i + 100, requests.Count);
                    for (int j = i; j < batchEnd; j++)
                    {
                        batch.Requests.Add(requests[j]);
                    }

                    service.Execute(batch);
                    totalUpdated += batch.Requests.Count;
                    progressCallback?.Invoke(totalUpdated);
                }

                moreRecords = results.MoreRecords;
                pagingCookie = results.PagingCookie;
                page++;
            }

            return totalUpdated;
        }

        #endregion

        #region Solution Components

        public static void AddSolutionComponent(IOrganizationService service, string solutionUniqueName,
            Guid componentId, int componentType)
        {
            var request = new AddSolutionComponentRequest
            {
                ComponentId = componentId,
                ComponentType = componentType,
                SolutionUniqueName = solutionUniqueName,
                AddRequiredComponents = false
            };

            // Retry with delay to handle eventual consistency after component creation
            const int maxRetries = 3;
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    service.Execute(request);
                    return;
                }
                catch (System.ServiceModel.FaultException) when (attempt < maxRetries)
                {
                    Thread.Sleep(2000 * attempt);
                }
            }
        }

        public static bool IsComponentInSolution(IOrganizationService service, Guid solutionId,
            Guid componentId, int componentType)
        {
            var query = new QueryExpression("solutioncomponent")
            {
                ColumnSet = new ColumnSet("solutioncomponentid"),
                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression("solutionid", ConditionOperator.Equal, solutionId),
                        new ConditionExpression("objectid", ConditionOperator.Equal, componentId),
                        new ConditionExpression("componenttype", ConditionOperator.Equal, componentType)
                    }
                },
                TopCount = 1
            };

            return service.RetrieveMultiple(query).Entities.Any();
        }

        #endregion

        #region Model-Driven Apps

        public static List<Entity> GetModelDrivenApps(IOrganizationService service)
        {
            var query = new QueryExpression("appmodule")
            {
                ColumnSet = new ColumnSet("name", "uniquename", "appmoduleid"),
                Orders = { new OrderExpression("name", OrderType.Ascending) }
            };

            return service.RetrieveMultiple(query).Entities.ToList();
        }

        #endregion

        #region URL Builder

        public static string BuildRecordUrl(string environmentUrl, string entityLogicalName, Guid recordId, string mdaGuid)
        {
            environmentUrl = environmentUrl.TrimEnd('/');
            if (!string.IsNullOrWhiteSpace(mdaGuid))
            {
                return environmentUrl + "/main.aspx?appid=" + mdaGuid +
                       "&pagetype=entityrecord&etn=" + entityLogicalName +
                       "&id=" + recordId.ToString();
            }
            return environmentUrl + "/main.aspx?pagetype=entityrecord&etn=" +
                   entityLogicalName + "&id=" + recordId.ToString();
        }

        #endregion
    }
}
