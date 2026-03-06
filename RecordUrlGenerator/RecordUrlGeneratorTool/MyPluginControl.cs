using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using RecordUrlGeneratorTool.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using XrmToolBox.Extensibility;

namespace RecordUrlGeneratorTool
{
    public partial class MyPluginControl : PluginControlBase
    {
        private Settings mySettings;

        // ===== State =====
        private Entity _selectedSolution;
        private Entity _selectedPublisher;
        private string _publisherPrefix;
        private string _solutionUniqueName;

        private Entity _selectedEnvUrlVar;
        private Entity _selectedMdaVar;
        private string _envUrlValue;
        private string _mdaGuidValue;

        private Guid _pluginAssemblyId;
        private Guid _pluginTypeId;

        private EntityMetadata _selectedTableMetadata;
        private string _selectedUrlFieldName;

        private Entity _existingStep;
        private Guid _pluginStepId;

        private List<Entity> _envVarsInSolution;
        private List<Entity> _modelDrivenApps;

        // Records paging
        private int _currentPage = 1;
        private const int PageSize = 50;
        private bool _moreRecords;
        private string _pagingCookie;
        private bool _sortAscending = true;
        private string _sortColumn;

        private CancellationTokenSource _batchCts;
        private bool _isLoadingData;
        private bool _navigatingByButton;
        private bool _tablePublished;

        public MyPluginControl()
        {
            InitializeComponent();

            // Disable all tabs except the first
            for (int i = 1; i < tabWizard.TabCount; i++)
            {
                tabWizard.TabPages[i].Enabled = false;
            }

            // Proportional split: 60% top, 40% bottom
            splitStep1.SizeChanged += (s, ev) =>
            {
                if (splitStep1.Height > 50)
                    splitStep1.SplitterDistance = (int)(splitStep1.Height * 0.6);
            };
            splitStep3.SizeChanged += (s, ev) =>
            {
                if (splitStep3.Height > 50)
                    splitStep3.SplitterDistance = (int)(splitStep3.Height * 0.6);
            };

            OnCloseTool += MyPluginControl_OnCloseTool;
        }

        // =====================================================================
        // LIFECYCLE
        // =====================================================================

        private void MyPluginControl_Load(object sender, EventArgs e)
        {
            if (!SettingsManager.Instance.TryLoad(GetType(), out mySettings))
            {
                mySettings = new Settings();
            }

            ExecuteMethod(LoadSolutions);
        }

        private void MyPluginControl_OnCloseTool(object sender, EventArgs e)
        {
            _batchCts?.Cancel();
            SettingsManager.Instance.Save(GetType(), mySettings);
        }

        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail, string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);

            if (mySettings != null && detail != null)
            {
                mySettings.LastUsedOrganizationWebappUrl = detail.WebApplicationUrl;
            }

            // Reset state on connection change
            ResetWizardState();
            ExecuteMethod(LoadSolutions);
        }

        // =====================================================================
        // NAVIGATION
        // =====================================================================

        private void tsbClose_Click(object sender, EventArgs e)
        {
            CloseTool();
        }

        private void tsbBack_Click(object sender, EventArgs e)
        {
            int idx = tabWizard.SelectedIndex;
            if (idx > 0)
            {
                _navigatingByButton = true;
                tabWizard.SelectedIndex = idx - 1;
                _navigatingByButton = false;
            }
        }

        private void tsbNext_Click(object sender, EventArgs e)
        {
            int idx = tabWizard.SelectedIndex;

            switch (idx)
            {
                case 0: // Solution -> Assembly
                    if (!ValidateStep1()) return;
                    EnableTab(1);
                    _navigatingByButton = true;
                    tabWizard.SelectedIndex = 1;
                    _navigatingByButton = false;
                    break;
                case 1: // Assembly -> Table
                    EnableTab(2);
                    _navigatingByButton = true;
                    tabWizard.SelectedIndex = 2;
                    _navigatingByButton = false;
                    break;
                case 2: // Table -> Plugin Step
                    if (!ValidateStep3()) return;
                    EnableTab(3);
                    _navigatingByButton = true;
                    tabWizard.SelectedIndex = 3;
                    _navigatingByButton = false;
                    break;
                case 3: // Plugin Step -> Records
                    if (!_tablePublished)
                    {
                        MessageBox.Show("Please wait for the table to be published before proceeding.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    EnableTab(4);
                    _navigatingByButton = true;
                    tabWizard.SelectedIndex = 4;
                    _navigatingByButton = false;
                    break;
            }
        }

        private void tabWizard_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (!_navigatingByButton)
            {
                e.Cancel = true;
            }
        }

        private void tabWizard_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = tabWizard.SelectedIndex;
            tsbBack.Enabled = idx > 0;
            tsbNext.Enabled = idx < 4;

            // Show/hide data panel (visible from step 2 onwards)
            splitMain.Panel2Collapsed = idx == 0;

            // Load data for the current tab
            switch (idx)
            {
                case 1:
                    ExecuteMethod(CheckAssemblyStatus);
                    break;
                case 2:
                    ExecuteMethod(LoadTables);
                    break;
                case 3:
                    LoadPluginStepPreview();
                    break;
                case 4:
                    LoadRecords();
                    break;
            }
        }

        private void EnableTab(int index)
        {
            if (index < tabWizard.TabCount)
                tabWizard.TabPages[index].Enabled = true;
        }

        private void ResetWizardState()
        {
            _selectedSolution = null;
            _selectedPublisher = null;
            _publisherPrefix = null;
            _solutionUniqueName = null;
            _selectedEnvUrlVar = null;
            _selectedMdaVar = null;
            _envUrlValue = null;
            _mdaGuidValue = null;
            _pluginAssemblyId = Guid.Empty;
            _pluginTypeId = Guid.Empty;
            _selectedTableMetadata = null;
            _selectedUrlFieldName = null;
            _existingStep = null;
            _pluginStepId = Guid.Empty;
            _tablePublished = false;

            for (int i = 1; i < tabWizard.TabCount; i++)
                tabWizard.TabPages[i].Enabled = false;

            _navigatingByButton = true;
            tabWizard.SelectedIndex = 0;
            _navigatingByButton = false;
            splitMain.Panel2Collapsed = true;
            UpdateDataPanel();
        }

        // =====================================================================
        // STEP 1 - SOLUTION & ENVIRONMENT VARIABLES
        // =====================================================================

        private void LoadSolutions()
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading unmanaged solutions...",
                Work = (worker, args) =>
                {
                    args.Result = CrmServiceHelper.GetUnmanagedSolutions(Service);
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        ShowError(args.Error, "Error loading solutions");
                        return;
                    }

                    var solutions = (List<Entity>)args.Result;
                    var dt = new DataTable();
                    dt.Columns.Add("DisplayName", typeof(string));
                    dt.Columns.Add("UniqueName", typeof(string));
                    dt.Columns.Add("Version", typeof(string));
                    dt.Columns.Add("Entity", typeof(Entity));

                    foreach (var sol in solutions)
                    {
                        dt.Rows.Add(
                            sol.GetAttributeValue<string>("friendlyname"),
                            sol.GetAttributeValue<string>("uniquename"),
                            sol.GetAttributeValue<string>("version"),
                            sol
                        );
                    }

                    // Detach event to prevent cascading WorkAsync calls during data binding
                    _isLoadingData = true;
                    dgvSolutions.SelectionChanged -= dgvSolutions_SelectionChanged;
                    dgvSolutions.DataSource = dt;
                    dgvSolutions.Columns["Entity"].Visible = false;
                    dgvSolutions.Columns["DisplayName"].HeaderText = "Display Name";
                    dgvSolutions.Columns["UniqueName"].HeaderText = "Unique Name";
                    dgvSolutions.SelectionChanged += dgvSolutions_SelectionChanged;
                    _isLoadingData = false;
                }
            });
        }

        private void dgvSolutions_SelectionChanged(object sender, EventArgs e)
        {
            if (_isLoadingData) return;
            if (dgvSolutions.SelectedRows.Count == 0) return;

            var row = dgvSolutions.SelectedRows[0];
            _selectedSolution = row.Cells["Entity"].Value as Entity;

            if (_selectedSolution == null) return;

            _solutionUniqueName = _selectedSolution.GetAttributeValue<string>("uniquename");

            // Load publisher
            var publisherRef = _selectedSolution.GetAttributeValue<EntityReference>("publisherid");
            if (publisherRef != null)
            {
                WorkAsync(new WorkAsyncInfo
                {
                    Message = "Loading solution details...",
                    Work = (worker, args) =>
                    {
                        var publisher = CrmServiceHelper.GetPublisher(Service, publisherRef.Id);
                        var envVars = CrmServiceHelper.GetEnvironmentVariablesInSolution(Service, _selectedSolution.Id);
                        var apps = CrmServiceHelper.GetModelDrivenApps(Service);
                        args.Result = new object[] { publisher, envVars, apps };
                    },
                    PostWorkCallBack = (args) =>
                    {
                        if (args.Error != null)
                        {
                            ShowError(args.Error, "Error loading solution details");
                            return;
                        }

                        var results = (object[])args.Result;
                        _selectedPublisher = (Entity)results[0];
                        _envVarsInSolution = (List<Entity>)results[1];
                        _modelDrivenApps = (List<Entity>)results[2];

                        _publisherPrefix = _selectedPublisher.GetAttributeValue<string>("customizationprefix");

                        PopulateEnvVarDropdowns();
                        UpdateDataPanel();
                    }
                });
            }
        }

        private void PopulateEnvVarDropdowns()
        {
            // Environment URL variable
            cmbEnvUrlVar.Items.Clear();
            cmbEnvUrlVar.Items.Add("(None)");
            cmbMdaVar.Items.Clear();
            cmbMdaVar.Items.Add("(None - skip)");

            if (_envVarsInSolution != null)
            {
                foreach (var ev in _envVarsInSolution)
                {
                    var schemaName = ev.GetAttributeValue<string>("schemaname");
                    var displayName = ev.GetAttributeValue<string>("displayname") ?? schemaName;
                    var item = new EnvVarItem(displayName + " (" + schemaName + ")", ev);

                    cmbEnvUrlVar.Items.Add(item);
                    cmbMdaVar.Items.Add(item);
                }
            }

            cmbEnvUrlVar.SelectedIndex = 0;
            cmbMdaVar.SelectedIndex = 0;

            // Auto-fill: select env var ending with _EnvironmentURL
            for (int i = 1; i < cmbEnvUrlVar.Items.Count; i++)
            {
                if (cmbEnvUrlVar.Items[i] is EnvVarItem item &&
                    item.Entity.GetAttributeValue<string>("schemaname")
                        .EndsWith("_EnvironmentURL", StringComparison.OrdinalIgnoreCase))
                {
                    cmbEnvUrlVar.SelectedIndex = i;
                    break;
                }
            }

            // Auto-fill: select env var ending with _ModelDrivenAppId
            for (int i = 1; i < cmbMdaVar.Items.Count; i++)
            {
                if (cmbMdaVar.Items[i] is EnvVarItem item &&
                    item.Entity.GetAttributeValue<string>("schemaname")
                        .EndsWith("_ModelDrivenAppId", StringComparison.OrdinalIgnoreCase))
                {
                    cmbMdaVar.SelectedIndex = i;
                    break;
                }
            }
        }

        private void PopulateMdaAppsDropdown()
        {
            // MDA apps are loaded into _modelDrivenApps and shown via modal dialog
        }

        private void cmbEnvUrlVar_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbEnvUrlVar.SelectedItem is EnvVarItem item)
            {
                _selectedEnvUrlVar = item.Entity;
                _envUrlValue = CrmServiceHelper.GetResolvedEnvVarValue(item.Entity);
                txtEnvUrlValue.Text = _envUrlValue ?? "";
                btnCreateEnvUrl.Enabled = false;
            }
            else
            {
                _selectedEnvUrlVar = null;
                _envUrlValue = null;
                txtEnvUrlValue.Text = "";
                btnCreateEnvUrl.Enabled = true;
            }
            UpdateDataPanel();
        }

        private void cmbMdaVar_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbMdaVar.SelectedItem is EnvVarItem item)
            {
                _selectedMdaVar = item.Entity;
                _mdaGuidValue = CrmServiceHelper.GetResolvedEnvVarValue(item.Entity);
                txtMdaGuidValue.Text = _mdaGuidValue ?? "";
                btnCreateMda.Enabled = false;
            }
            else
            {
                _selectedMdaVar = null;
                _mdaGuidValue = null;
                txtMdaGuidValue.Text = "";
                btnCreateMda.Enabled = true;
            }
            UpdateDataPanel();
        }

        private void btnCreateEnvUrl_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_publisherPrefix))
            {
                MessageBox.Show("Please select a solution first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Pre-fill URL with connection detail
            var prefillUrl = ConnectionDetail?.WebApplicationUrl ?? "";

            var input = ShowInputDialog(
                "Enter the environment URL (e.g. https://org.crm.dynamics.com):",
                "Environment URL",
                prefillUrl);

            if (string.IsNullOrWhiteSpace(input)) return;

            var schemaName = _publisherPrefix + "_EnvironmentURL";

            ExecuteMethod(() => CreateEnvVariable(schemaName, "Environment URL", input, true));
        }

        private void btnCreateMda_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_publisherPrefix))
            {
                MessageBox.Show("Please select a solution first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_modelDrivenApps == null || _modelDrivenApps.Count == 0)
            {
                MessageBox.Show("No Model-Driven Apps found in this environment.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedApp = ShowMdaAppSelectionDialog();
            if (selectedApp == null) return;

            var schemaName = _publisherPrefix + "_ModelDrivenAppId";
            var mdaGuid = selectedApp.Id.ToString();

            ExecuteMethod(() => CreateEnvVariable(schemaName, "Model-Driven App ID", mdaGuid, false));
        }

        private Entity ShowMdaAppSelectionDialog()
        {
            using (var form = new Form())
            {
                form.Text = "Select Model-Driven App";
                form.Width = 550;
                form.Height = 180;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterParent;
                form.MaximizeBox = false;
                form.MinimizeBox = false;

                var lbl = new System.Windows.Forms.Label { Text = "Select a Model-Driven App:", Left = 15, Top = 15, Width = 500 };
                var cmb = new ComboBox
                {
                    Left = 15, Top = 45, Width = 500,
                    DropDownStyle = ComboBoxStyle.DropDownList
                };

                cmb.Items.Add("(Select an app)");
                foreach (var app in _modelDrivenApps)
                {
                    var name = app.GetAttributeValue<string>("name");
                    cmb.Items.Add(new MdaAppItem(name + " (" + app.Id + ")", app));
                }
                cmb.SelectedIndex = 0;

                var btnOk = new Button { Text = "OK", DialogResult = DialogResult.OK, Left = 340, Top = 90, Width = 80 };
                var btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Left = 430, Top = 90, Width = 80 };

                form.Controls.AddRange(new Control[] { lbl, cmb, btnOk, btnCancel });
                form.AcceptButton = btnOk;
                form.CancelButton = btnCancel;

                if (form.ShowDialog() == DialogResult.OK && cmb.SelectedItem is MdaAppItem selected)
                {
                    return selected.Entity;
                }
                return null;
            }
        }

        private void CreateEnvVariable(string schemaName, string displayName, string value, bool isEnvUrl)
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Creating environment variable '" + schemaName + "'...",
                Work = (worker, args) =>
                {
                    var id = CrmServiceHelper.CreateEnvironmentVariable(Service, schemaName, displayName, value,
                        "Created by Record URL Generator tool");

                    // Add to solution
                    CrmServiceHelper.AddSolutionComponent(Service, _solutionUniqueName, id,
                        CrmServiceHelper.ComponentType_EnvironmentVariableDefinition);

                    // Reload env vars
                    args.Result = CrmServiceHelper.GetEnvironmentVariablesInSolution(Service, _selectedSolution.Id);
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        ShowError(args.Error, "Error creating environment variable");
                        return;
                    }

                    _envVarsInSolution = (List<Entity>)args.Result;
                    PopulateEnvVarDropdowns();

                    // Auto-select the newly created variable
                    for (int i = 0; i < cmbEnvUrlVar.Items.Count; i++)
                    {
                        if (cmbEnvUrlVar.Items[i] is EnvVarItem item &&
                            item.Entity.GetAttributeValue<string>("schemaname").Contains(isEnvUrl ? "EnvironmentURL" : "ModelDrivenApp"))
                        {
                            if (isEnvUrl)
                                cmbEnvUrlVar.SelectedIndex = i;
                            else
                                cmbMdaVar.SelectedIndex = i;
                            break;
                        }
                    }

                    MessageBox.Show("Environment variable created successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            });
        }

        private bool ValidateStep1()
        {
            if (_selectedSolution == null)
            {
                MessageBox.Show("Please select a solution.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (_selectedEnvUrlVar == null)
            {
                MessageBox.Show("Please select or create the Environment URL variable.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        // =====================================================================
        // STEP 2 - ASSEMBLY DEPLOYMENT
        // =====================================================================

        private void CheckAssemblyStatus()
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Checking plugin assembly status...",
                Work = (worker, args) =>
                {
                    var assembly = CrmServiceHelper.GetPluginAssembly(Service, CrmServiceHelper.PluginAssemblyName);
                    Entity pluginType = null;

                    if (assembly != null)
                    {
                        pluginType = CrmServiceHelper.GetPluginType(Service, assembly.Id);
                    }

                    args.Result = new object[] { assembly, pluginType };
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        ShowError(args.Error, "Error checking assembly");
                        return;
                    }

                    var results = (object[])args.Result;
                    var assembly = results[0] as Entity;
                    var pluginType = results[1] as Entity;

                    if (assembly != null && pluginType != null)
                    {
                        _pluginAssemblyId = assembly.Id;
                        _pluginTypeId = pluginType.Id;

                        var version = assembly.GetAttributeValue<string>("version");
                        lblAssemblyStatus.Text = "✓ Assembly is deployed (version " + version + ").\nPlugin type '" + CrmServiceHelper.PluginTypeName + "' is registered.";
                        lblAssemblyStatus.ForeColor = Color.DarkGreen;
                        btnDeploy.Text = "Update Assembly";
                        btnDeploy.Visible = true;
                    }
                    else if (assembly != null)
                    {
                        _pluginAssemblyId = assembly.Id;
                        lblAssemblyStatus.Text = "⚠ Assembly found but plugin type is not registered.\nClick Deploy to register the plugin type.";
                        lblAssemblyStatus.ForeColor = Color.DarkOrange;
                        btnDeploy.Text = "Register Plugin Type";
                        btnDeploy.Visible = true;
                    }
                    else
                    {
                        lblAssemblyStatus.Text = "✗ The plugin assembly is not deployed to this environment.\nClick Deploy to register it.";
                        lblAssemblyStatus.ForeColor = Color.DarkRed;
                        btnDeploy.Text = "Deploy Assembly";
                        btnDeploy.Visible = true;
                    }
                }
            });
        }

        private void btnDeploy_Click(object sender, EventArgs e)
        {
            ExecuteMethod(DeployAssembly);
        }

        private void DeployAssembly()
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Deploying plugin assembly...",
                Work = (worker, args) =>
                {
                    var assemblyBytes = CrmServiceHelper.GetEmbeddedAssemblyBytes();

                    if (_pluginAssemblyId == Guid.Empty)
                    {
                        // Register new assembly
                        _pluginAssemblyId = CrmServiceHelper.RegisterPluginAssembly(Service, assemblyBytes);

                        // Verify the assembly was created
                        var verify = CrmServiceHelper.GetPluginAssembly(Service, CrmServiceHelper.PluginAssemblyName);
                        if (verify == null)
                            throw new InvalidOperationException("Assembly was created but could not be found. Please try adding it to the solution manually.");
                        _pluginAssemblyId = verify.Id;

                        // Add assembly to solution
                        CrmServiceHelper.AddSolutionComponent(Service, _solutionUniqueName, _pluginAssemblyId,
                            CrmServiceHelper.ComponentType_PluginAssembly);
                    }
                    else
                    {
                        // Update existing assembly
                        CrmServiceHelper.UpdatePluginAssembly(Service, _pluginAssemblyId, assemblyBytes);

                        // Ensure assembly is in solution
                        if (!CrmServiceHelper.IsComponentInSolution(Service, _selectedSolution.Id,
                            _pluginAssemblyId, CrmServiceHelper.ComponentType_PluginAssembly))
                        {
                            CrmServiceHelper.AddSolutionComponent(Service, _solutionUniqueName, _pluginAssemblyId,
                                CrmServiceHelper.ComponentType_PluginAssembly);
                        }
                    }

                    // Check/register plugin type
                    var pluginType = CrmServiceHelper.GetPluginType(Service, _pluginAssemblyId);
                    if (pluginType == null)
                    {
                        _pluginTypeId = CrmServiceHelper.RegisterPluginType(Service, _pluginAssemblyId);
                    }
                    else
                    {
                        _pluginTypeId = pluginType.Id;
                    }
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        ShowError(args.Error, "Error deploying assembly");
                        return;
                    }

                    lblAssemblyStatus.Text = "✓ Assembly deployed and plugin type registered successfully.";
                    lblAssemblyStatus.ForeColor = Color.DarkGreen;
                    btnDeploy.Text = "Update Assembly";
                    btnAddToSolution.Visible = false;

                    MessageBox.Show("Assembly deployed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            });
        }

        private void btnAddToSolution_Click(object sender, EventArgs e)
        {
            if (_pluginAssemblyId == Guid.Empty)
            {
                MessageBox.Show("No assembly found to add.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            WorkAsync(new WorkAsyncInfo
            {
                Message = "Adding assembly to solution...",
                Work = (worker, args) =>
                {
                    // Add assembly to solution
                    if (!CrmServiceHelper.IsComponentInSolution(Service, _selectedSolution.Id,
                        _pluginAssemblyId, CrmServiceHelper.ComponentType_PluginAssembly))
                    {
                        CrmServiceHelper.AddSolutionComponent(Service, _solutionUniqueName, _pluginAssemblyId,
                            CrmServiceHelper.ComponentType_PluginAssembly);
                    }
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        ShowError(args.Error, "Error adding to solution");
                        return;
                    }

                    btnAddToSolution.Visible = false;
                    lblAssemblyStatus.ForeColor = Color.DarkGreen;
                    MessageBox.Show("Assembly added to solution successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ExecuteMethod(CheckAssemblyStatus);
                }
            });
        }

        // =====================================================================
        // STEP 3 - TABLE & FIELD
        // =====================================================================

        private void LoadTables()
        {
            if (_selectedSolution == null) return;

            WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading tables from solution...",
                Work = (worker, args) =>
                {
                    args.Result = CrmServiceHelper.GetTablesInSolution(Service, _selectedSolution.Id);
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        ShowError(args.Error, "Error loading tables");
                        return;
                    }

                    var tables = (List<EntityMetadata>)args.Result;
                    var dt = new DataTable();
                    dt.Columns.Add("DisplayName", typeof(string));
                    dt.Columns.Add("LogicalName", typeof(string));
                    dt.Columns.Add("Metadata", typeof(EntityMetadata));

                    foreach (var table in tables)
                    {
                        dt.Rows.Add(
                            table.DisplayName?.UserLocalizedLabel?.Label ?? table.LogicalName,
                            table.LogicalName,
                            table
                        );
                    }

                    // Detach event to prevent cascading WorkAsync calls during data binding
                    _isLoadingData = true;
                    dgvTables.SelectionChanged -= dgvTables_SelectionChanged;
                    dgvTables.DataSource = dt;
                    dgvTables.Columns["Metadata"].Visible = false;
                    dgvTables.Columns["DisplayName"].HeaderText = "Display Name";
                    dgvTables.Columns["LogicalName"].HeaderText = "Logical Name";
                    dgvTables.SelectionChanged += dgvTables_SelectionChanged;
                    _isLoadingData = false;

                    // Clear selection so the user must click a row to activate field loading
                    dgvTables.ClearSelection();
                }
            });
        }

        private void dgvTables_SelectionChanged(object sender, EventArgs e)
        {
            if (_isLoadingData) return;
            if (dgvTables.SelectedRows.Count == 0) return;

            var row = dgvTables.SelectedRows[0];
            var metadata = row.Cells["Metadata"].Value as EntityMetadata;

            if (metadata == null) return;

            _selectedTableMetadata = metadata;
            UpdateDataPanel();

            // Check if a plugin step already exists for this table
            if (_pluginTypeId != Guid.Empty)
            {
                WorkAsync(new WorkAsyncInfo
                {
                    Message = "Checking existing configuration for " + metadata.LogicalName + "...",
                    Work = (worker, args) =>
                    {
                        var step = CrmServiceHelper.GetPluginStepForEntity(Service, _pluginTypeId, metadata.LogicalName);
                        var fields = CrmServiceHelper.GetStringFieldsForEntity(Service, metadata.LogicalName);
                        args.Result = new object[] { step, fields };
                    },
                    PostWorkCallBack = (args) =>
                    {
                        if (args.Error != null)
                        {
                            ShowError(args.Error, "Error checking table");
                            return;
                        }

                        var results = (object[])args.Result;
                        _existingStep = results[0] as Entity;
                        var fields = (List<AttributeMetadata>)results[1];

                        PopulateFieldDropdown(fields);

                        if (_existingStep != null)
                        {
                            // Plugin step exists - auto-select field from config if possible
                            var config = _existingStep.GetAttributeValue<string>("configuration");
                            TryAutoSelectFieldFromConfig(config);
                        }
                        else
                        {
                            // Auto-select field ending with _recordurl
                            TryAutoSelectRecordUrlField();
                        }
                    }
                });
            }
            else
            {
                // No plugin type yet, just load fields
                WorkAsync(new WorkAsyncInfo
                {
                    Message = "Loading fields...",
                    Work = (worker, args) =>
                    {
                        args.Result = CrmServiceHelper.GetStringFieldsForEntity(Service, metadata.LogicalName);
                    },
                    PostWorkCallBack = (args) =>
                    {
                        if (args.Error != null)
                        {
                            ShowError(args.Error, "Error loading fields");
                            return;
                        }
                        PopulateFieldDropdown((List<AttributeMetadata>)args.Result);
                        TryAutoSelectRecordUrlField();
                    }
                });
            }
        }

        private void PopulateFieldDropdown(List<AttributeMetadata> fields)
        {
            cmbUrlField.Items.Clear();
            cmbUrlField.Items.Add("(Select a field)");

            foreach (var field in fields)
            {
                var displayName = field.DisplayName?.UserLocalizedLabel?.Label ?? field.LogicalName;
                cmbUrlField.Items.Add(new FieldItem(displayName + " (" + field.LogicalName + ")", field));
            }

            cmbUrlField.SelectedIndex = 0;
        }

        private void TryAutoSelectFieldFromConfig(string config)
        {
            if (string.IsNullOrEmpty(config)) return;

            // Simple parse to find "recordUrlFieldName" value
            try
            {
                var key = "\"recordUrlFieldName\"";
                var idx = config.IndexOf(key);
                if (idx < 0) return;
                var colonIdx = config.IndexOf(":", idx + key.Length);
                if (colonIdx < 0) return;
                var quoteStart = config.IndexOf("\"", colonIdx + 1);
                if (quoteStart < 0) return;
                var quoteEnd = config.IndexOf("\"", quoteStart + 1);
                if (quoteEnd < 0) return;
                var fieldName = config.Substring(quoteStart + 1, quoteEnd - quoteStart - 1);

                for (int i = 0; i < cmbUrlField.Items.Count; i++)
                {
                    if (cmbUrlField.Items[i] is FieldItem item && item.Metadata.LogicalName == fieldName)
                    {
                        cmbUrlField.SelectedIndex = i;
                        break;
                    }
                }
            }
            catch { }
        }

        private void TryAutoSelectRecordUrlField()
        {
            for (int i = 1; i < cmbUrlField.Items.Count; i++)
            {
                if (cmbUrlField.Items[i] is FieldItem item &&
                    item.Metadata.LogicalName.EndsWith("_recordurl", StringComparison.OrdinalIgnoreCase))
                {
                    cmbUrlField.SelectedIndex = i;
                    return;
                }
            }
        }

        private void cmbUrlField_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbUrlField.SelectedItem is FieldItem item)
            {
                _selectedUrlFieldName = item.Metadata.LogicalName;
                btnCreateField.Enabled = false;
                // Existing field selected from server: table is already published
                if (!_tablePublished)
                {
                    _tablePublished = true;
                    UpdateDataPanel();
                }
            }
            else
            {
                _selectedUrlFieldName = null;
                btnCreateField.Enabled = true;
            }
            UpdateDataPanel();
        }

        private void btnCreateField_Click(object sender, EventArgs e)
        {
            if (_selectedTableMetadata == null)
            {
                MessageBox.Show("Please select a table first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(_publisherPrefix))
            {
                MessageBox.Show("Publisher prefix is not available.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var schemaName = _publisherPrefix + "_RecordUrl";
            var entityName = _selectedTableMetadata.LogicalName;

            WorkAsync(new WorkAsyncInfo
            {
                Message = "Creating URL field '" + schemaName + "' on " + entityName + "...",
                Work = (worker, args) =>
                {
                    CrmServiceHelper.CreateUrlField(Service, entityName, schemaName, "Record URL",
                        _solutionUniqueName, 1000);
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        ShowError(args.Error, "Error creating field");
                        return;
                    }

                    // Add the new field directly to the dropdown without re-querying metadata
                    var logicalName = (_publisherPrefix + "_recordurl").ToLowerInvariant();
                    var metadata = new StringAttributeMetadata
                    {
                        LogicalName = logicalName,
                        SchemaName = schemaName,
                        DisplayName = new Microsoft.Xrm.Sdk.Label("Record URL", 1033)
                    };
                    var fieldItem = new FieldItem("Record URL (" + logicalName + ")", metadata);
                    cmbUrlField.Items.Add(fieldItem);
                    cmbUrlField.SelectedItem = fieldItem;

                    // Publish the table so the new field is available at runtime
                    _tablePublished = false;
                    UpdateDataPanel();
                    PublishTable(entityName);

                    MessageBox.Show("URL field created successfully.\nThe table is being published in background.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            });
        }

        private bool ValidateStep3()
        {
            if (_selectedTableMetadata == null)
            {
                MessageBox.Show("Please select a table.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrEmpty(_selectedUrlFieldName))
            {
                MessageBox.Show("Please select or create a URL field.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void PublishTable(string entityLogicalName)
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Publishing table '" + entityLogicalName + "'...",
                Work = (worker, args) =>
                {
                    CrmServiceHelper.PublishEntity(Service, entityLogicalName);
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        ShowError(args.Error, "Error publishing table");
                        return;
                    }

                    _tablePublished = true;
                    UpdateDataPanel();
                }
            });
        }

        // =====================================================================
        // STEP 4 - PLUGIN STEP CONFIGURATION
        // =====================================================================

        private void LoadPluginStepPreview()
        {
            if (_selectedTableMetadata == null) return;

            var entityName = _selectedTableMetadata.LogicalName;
            lblStepEntityValue.Text = entityName;

            // Build the JSON config
            var envUrlVarSchema = _selectedEnvUrlVar?.GetAttributeValue<string>("schemaname") ?? "";
            var mdaVarSchema = _selectedMdaVar?.GetAttributeValue<string>("schemaname");

            var config = "{\r\n" +
                "  \"envUrlVarName\": \"" + envUrlVarSchema + "\",\r\n" +
                "  \"recordUrlFieldName\": \"" + (_selectedUrlFieldName ?? "") + "\"";

            if (!string.IsNullOrEmpty(mdaVarSchema))
            {
                config += ",\r\n  \"mdaEnvName\": \"" + mdaVarSchema + "\"";
            }

            config += "\r\n}";

            txtStepConfig.Text = config;

            // Check if step already exists
            if (_existingStep != null)
            {
                _pluginStepId = _existingStep.Id;
                lblStepStatusValue.Text = "✓ Step already exists. Configuration will be updated if changed.";
                lblStepStatusValue.ForeColor = Color.DarkGreen;
                btnCreateStep.Text = "Update Configuration";

                // Compare current vs generated config (normalize line endings)
                var currentConfig = (_existingStep.GetAttributeValue<string>("configuration") ?? "").Replace("\r\n", "\n").Trim();
                var newConfig = config.Replace("\r\n", "\n").Trim();
                if (string.Equals(currentConfig, newConfig, StringComparison.Ordinal))
                {
                    lblConfigStatus.Text = "✓ Configuration matches the deployed step";
                    lblConfigStatus.ForeColor = Color.DarkGreen;
                    lblConfigStatus.Visible = true;
                    txtStepConfig.Location = new Point(11, 216);
                }
                else
                {
                    lblConfigStatus.Text = "⚠ Configuration differs from the deployed step — click Update to apply";
                    lblConfigStatus.ForeColor = Color.DarkOrange;
                    lblConfigStatus.Visible = true;
                    txtStepConfig.Location = new Point(11, 216);
                }

                // Show enabled/disabled state
                var stateCode = _existingStep.GetAttributeValue<OptionSetValue>("statecode")?.Value ?? 0;
                var isEnabled = stateCode == 0;
                UpdateStepEnabledDisplay(isEnabled);
                btnToggleStep.Visible = true;
            }
            else
            {
                _pluginStepId = Guid.Empty;
                lblStepStatusValue.Text = "Step will be created when you click the button below.";
                lblStepStatusValue.ForeColor = Color.DarkOrange;
                btnCreateStep.Text = "Register Plugin Step";
                lblStepEnabledValue.Text = "-";
                lblStepEnabledValue.ForeColor = Color.Black;
                btnToggleStep.Visible = false;
                lblConfigStatus.Visible = false;
                txtStepConfig.Location = new Point(11, 200);
            }
        }

        private void btnCreateStep_Click(object sender, EventArgs e)
        {
            ExecuteMethod(RegisterOrUpdatePluginStep);
        }

        private void RegisterOrUpdatePluginStep()
        {
            var config = txtStepConfig.Text;
            var entityName = _selectedTableMetadata.LogicalName;

            WorkAsync(new WorkAsyncInfo
            {
                Message = _pluginStepId != Guid.Empty ? "Updating plugin step..." : "Registering plugin step...",
                Work = (worker, args) =>
                {
                    if (_pluginStepId != Guid.Empty)
                    {
                        // Update existing step configuration
                        CrmServiceHelper.UpdatePluginStepConfiguration(Service, _pluginStepId, config);
                    }
                    else
                    {
                        // Create new step
                        _pluginStepId = CrmServiceHelper.CreatePluginStep(Service, _pluginTypeId, entityName, config);

                        // Add step to solution
                        CrmServiceHelper.AddSolutionComponent(Service, _solutionUniqueName, _pluginStepId,
                            CrmServiceHelper.ComponentType_SdkMessageProcessingStep);
                    }
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        ShowError(args.Error, "Error with plugin step");
                        return;
                    }

                    lblStepStatusValue.Text = "✓ Plugin step registered successfully.";
                    lblStepStatusValue.ForeColor = Color.DarkGreen;
                    btnCreateStep.Text = "Update Configuration";
                    UpdateStepEnabledDisplay(true);
                    btnToggleStep.Visible = true;

                    MessageBox.Show("Plugin step configured successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            });
        }

        // =====================================================================
        // STEP 5 - RECORDS
        // =====================================================================

        private void UpdateStepEnabledDisplay(bool isEnabled)
        {
            lblStepEnabledValue.Text = isEnabled ? "✓ Enabled" : "✗ Disabled";
            lblStepEnabledValue.ForeColor = isEnabled ? Color.DarkGreen : Color.DarkRed;
            btnToggleStep.Text = isEnabled ? "Disable Step" : "Enable Step";
        }

        private void btnToggleStep_Click(object sender, EventArgs e)
        {
            if (_pluginStepId == Guid.Empty) return;

            var currentlyEnabled = lblStepEnabledValue.Text.Contains("Enabled") && !lblStepEnabledValue.Text.Contains("Disabled");
            var newState = !currentlyEnabled;

            WorkAsync(new WorkAsyncInfo
            {
                Message = newState ? "Enabling plugin step..." : "Disabling plugin step...",
                Work = (worker, args) =>
                {
                    CrmServiceHelper.SetPluginStepState(Service, _pluginStepId, newState);
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        ShowError(args.Error, "Error toggling plugin step");
                        return;
                    }

                    UpdateStepEnabledDisplay(newState);

                    // Update cached entity so LoadPluginStepPreview reads the correct state
                    if (_existingStep != null)
                    {
                        _existingStep["statecode"] = new OptionSetValue(newState ? 0 : 1);
                    }
                }
            });
        }

        private void btnLoadRecords_Click(object sender, EventArgs e)
        {
            _currentPage = 1;
            _pagingCookie = null;
            LoadRecords();
        }

        private void LoadRecords()
        {
            if (_selectedTableMetadata == null || string.IsNullOrEmpty(_selectedUrlFieldName)) return;

            var entityName = _selectedTableMetadata.LogicalName;
            var primaryNameField = _selectedTableMetadata.PrimaryNameAttribute;
            var urlField = _selectedUrlFieldName;

            string filter = "all";
            if (cmbRecordFilter.SelectedIndex == 1) filter = "empty";
            else if (cmbRecordFilter.SelectedIndex == 2) filter = "filled";

            WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading records (page " + _currentPage + ")...",
                Work = (worker, args) =>
                {
                    args.Result = CrmServiceHelper.GetRecords(Service, entityName,
                        primaryNameField, urlField, filter, _currentPage, PageSize, _pagingCookie);
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        ShowError(args.Error, "Error loading records");
                        return;
                    }

                    var result = ((List<Entity> records, int totalCount, bool moreRecords, string pagingCookie))args.Result;
                    _moreRecords = result.moreRecords;
                    _pagingCookie = result.pagingCookie;

                    var dt = new DataTable();
                    dt.Columns.Add("RecordId", typeof(string));
                    if (!string.IsNullOrEmpty(primaryNameField))
                        dt.Columns.Add("Name", typeof(string));
                    dt.Columns.Add("URL", typeof(string));

                    foreach (var record in result.records)
                    {
                        var row = dt.NewRow();
                        row["RecordId"] = record.Id.ToString();
                        if (!string.IsNullOrEmpty(primaryNameField))
                            row["Name"] = record.GetAttributeValue<string>(primaryNameField) ?? "";
                        row["URL"] = record.GetAttributeValue<string>(urlField) ?? "";
                        dt.Rows.Add(row);
                    }

                    dgvRecords.DataSource = dt;

                    // Apply sort if set
                    if (!string.IsNullOrEmpty(_sortColumn) && dt.Columns.Contains(_sortColumn))
                    {
                        var direction = _sortAscending ? "ASC" : "DESC";
                        dt.DefaultView.Sort = _sortColumn + " " + direction;
                    }

                    var totalText = result.totalCount >= 0 ? result.totalCount.ToString() : "?";
                    lblRecordCount.Text = result.records.Count + " records (total: " + totalText + ")";

                    // Paging controls
                    btnPrevPage.Enabled = _currentPage > 1;
                    btnNextPage.Enabled = _moreRecords;
                    lblPageInfo.Text = "Page " + _currentPage;
                }
            });
        }

        private void btnPrevPage_Click(object sender, EventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                _pagingCookie = null; // Reset cookie - will reload from page 1 for simplicity
                LoadRecords();
            }
        }

        private void btnNextPage_Click(object sender, EventArgs e)
        {
            if (_moreRecords)
            {
                _currentPage++;
                LoadRecords();
            }
        }

        private void dgvRecords_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dgvRecords.DataSource is DataTable dt)
            {
                var columnName = dgvRecords.Columns[e.ColumnIndex].DataPropertyName;
                if (_sortColumn == columnName)
                {
                    _sortAscending = !_sortAscending;
                }
                else
                {
                    _sortColumn = columnName;
                    _sortAscending = true;
                }

                var direction = _sortAscending ? "ASC" : "DESC";
                dt.DefaultView.Sort = columnName + " " + direction;
            }
        }

        private void btnUpdateAll_Click(object sender, EventArgs e)
        {
            ExecuteMethod(BatchUpdateRecords);
        }

        private void BatchUpdateRecords()
        {
            if (_selectedTableMetadata == null || string.IsNullOrEmpty(_selectedUrlFieldName))
            {
                MessageBox.Show("Table and field must be selected.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(_envUrlValue))
            {
                MessageBox.Show("Environment URL value is not set.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                "This will update ALL records with an empty URL field.\nAre you sure you want to continue?",
                "Confirm Batch Update",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            _batchCts = new CancellationTokenSource();
            var ct = _batchCts.Token;
            var entityName = _selectedTableMetadata.LogicalName;
            var urlField = _selectedUrlFieldName;
            var envUrl = _envUrlValue;
            var mdaGuid = _mdaGuidValue;

            progressBarRecords.Visible = true;
            progressBarRecords.Style = ProgressBarStyle.Marquee;
            btnUpdateAll.Enabled = false;

            WorkAsync(new WorkAsyncInfo
            {
                Message = "Updating records with empty URLs...",
                Work = (worker, args) =>
                {
                    args.Result = CrmServiceHelper.BatchUpdateRecordUrls(Service, entityName,
                        urlField, envUrl, mdaGuid,
                        (count) =>
                        {
                            worker.ReportProgress(0, "Updated " + count + " records...");
                        },
                        ct);
                },
                ProgressChanged = (args) =>
                {
                    SetWorkingMessage(args.UserState?.ToString() ?? "Working...");
                },
                PostWorkCallBack = (args) =>
                {
                    progressBarRecords.Visible = false;
                    btnUpdateAll.Enabled = true;

                    if (args.Error != null)
                    {
                        if (args.Error is OperationCanceledException)
                        {
                            MessageBox.Show("Batch update was cancelled.", "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            ShowError(args.Error, "Error during batch update");
                        }
                        return;
                    }

                    var totalUpdated = (int)args.Result;
                    MessageBox.Show("Successfully updated " + totalUpdated + " records.", "Batch Update Complete",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Reload records to show updated values
                    _currentPage = 1;
                    _pagingCookie = null;
                    LoadRecords();
                }
            });
        }

        // =====================================================================
        // DATA PANEL
        // =====================================================================

        private void UpdateDataPanel()
        {
            lblDataSolutionValue.Text = _solutionUniqueName ?? "-";
            lblDataPublisherValue.Text = _publisherPrefix ?? "-";
            lblDataEnvUrlValue.Text = _selectedEnvUrlVar?.GetAttributeValue<string>("schemaname") ?? "-";
            lblDataEnvUrlActualValue.Text = _envUrlValue ?? "-";
            lblDataMdaValue.Text = _selectedMdaVar?.GetAttributeValue<string>("schemaname") ?? "(none)";
            lblDataMdaActualValue.Text = _mdaGuidValue ?? "-";
            lblDataTableValue.Text = _selectedTableMetadata?.LogicalName ?? "-";

            if (_selectedUrlFieldName != null)
            {
                var publishTag = _tablePublished ? " (✓ Published)" : " (⚠ Pending)";
                lblDataFieldValue.Text = _selectedUrlFieldName + publishTag;
                lblDataFieldValue.ForeColor = _tablePublished ? Color.DarkGreen : Color.Black;
            }
            else
            {
                lblDataFieldValue.Text = "-";
                lblDataFieldValue.ForeColor = SystemColors.ControlText;
            }
        }

        // =====================================================================
        // HELPERS
        // =====================================================================

        private void ShowError(Exception ex, string title)
        {
            LogError(title + ": " + ex.Message);
            MessageBox.Show(ex.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private string ShowInputDialog(string prompt, string title, string defaultValue)
        {
            using (var form = new Form())
            {
                form.Text = title;
                form.Width = 500;
                form.Height = 180;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterParent;
                form.MaximizeBox = false;
                form.MinimizeBox = false;

                var lbl = new System.Windows.Forms.Label { Text = prompt, Left = 15, Top = 15, Width = 450 };
                var txt = new TextBox { Text = defaultValue, Left = 15, Top = 45, Width = 450 };
                var btnOk = new Button { Text = "OK", DialogResult = DialogResult.OK, Left = 300, Top = 85, Width = 80 };
                var btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Left = 390, Top = 85, Width = 80 };

                form.Controls.AddRange(new Control[] { lbl, txt, btnOk, btnCancel });
                form.AcceptButton = btnOk;
                form.CancelButton = btnCancel;

                return form.ShowDialog() == DialogResult.OK ? txt.Text : null;
            }
        }

        // =====================================================================
        // HELPER CLASSES (ComboBox items)
        // =====================================================================

        private class EnvVarItem
        {
            public string Display { get; }
            public Entity Entity { get; }

            public EnvVarItem(string display, Entity entity)
            {
                Display = display;
                Entity = entity;
            }

            public override string ToString() => Display;
        }

        private class MdaAppItem
        {
            public string Display { get; }
            public Entity Entity { get; }

            public MdaAppItem(string display, Entity entity)
            {
                Display = display;
                Entity = entity;
            }

            public override string ToString() => Display;
        }

        private class FieldItem
        {
            public string Display { get; }
            public AttributeMetadata Metadata { get; }

            public FieldItem(string display, AttributeMetadata metadata)
            {
                Display = display;
                Metadata = metadata;
            }

            public override string ToString() => Display;
        }
    }
}
