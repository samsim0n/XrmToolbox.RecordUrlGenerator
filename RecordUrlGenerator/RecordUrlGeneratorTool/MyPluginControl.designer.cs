namespace RecordUrlGeneratorTool
{
    partial class MyPluginControl
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Detach event handlers to prevent COM context issues during disposal
                dgvSolutions.SelectionChanged -= dgvSolutions_SelectionChanged;
                dgvTables.SelectionChanged -= dgvTables_SelectionChanged;
                dgvRecords.ColumnHeaderMouseClick -= dgvRecords_ColumnHeaderMouseClick;

                _batchCts?.Dispose();

                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.toolStripMenu = new System.Windows.Forms.ToolStrip();
            this.tsbClose = new System.Windows.Forms.ToolStripButton();
            this.tssSep1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbBack = new System.Windows.Forms.ToolStripButton();
            this.tsbNext = new System.Windows.Forms.ToolStripButton();
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.tabWizard = new System.Windows.Forms.TabControl();

            // ===== Tab 1 - Solution & Env Vars =====
            this.tabSolution = new System.Windows.Forms.TabPage();
            this.lblStep1Desc = new System.Windows.Forms.Label();
            this.splitStep1 = new System.Windows.Forms.SplitContainer();
            this.lblSolutions = new System.Windows.Forms.Label();
            this.dgvSolutions = new System.Windows.Forms.DataGridView();
            this.pnlEnvVars = new System.Windows.Forms.Panel();
            this.grpMda = new System.Windows.Forms.GroupBox();
            this.txtMdaGuidValue = new System.Windows.Forms.TextBox();
            this.lblMdaApp = new System.Windows.Forms.Label();
            this.btnCreateMda = new System.Windows.Forms.Button();
            this.cmbMdaVar = new System.Windows.Forms.ComboBox();
            this.lblMdaVar = new System.Windows.Forms.Label();
            this.grpEnvUrl = new System.Windows.Forms.GroupBox();
            this.txtEnvUrlValue = new System.Windows.Forms.TextBox();
            this.lblEnvUrlValue = new System.Windows.Forms.Label();
            this.btnCreateEnvUrl = new System.Windows.Forms.Button();
            this.cmbEnvUrlVar = new System.Windows.Forms.ComboBox();
            this.lblEnvUrlVar = new System.Windows.Forms.Label();

            // ===== Tab 2 - Assembly Deployment =====
            this.tabAssembly = new System.Windows.Forms.TabPage();
            this.lblStep2Desc = new System.Windows.Forms.Label();
            this.pnlAssemblyContent = new System.Windows.Forms.Panel();
            this.lblAssemblyStatus = new System.Windows.Forms.Label();
            this.btnDeploy = new System.Windows.Forms.Button();
            this.btnAddToSolution = new System.Windows.Forms.Button();

            // ===== Tab 3 - Table & Field =====
            this.tabTable = new System.Windows.Forms.TabPage();
            this.lblStep3Desc = new System.Windows.Forms.Label();
            this.splitStep3 = new System.Windows.Forms.SplitContainer();
            this.lblTables = new System.Windows.Forms.Label();
            this.dgvTables = new System.Windows.Forms.DataGridView();
            this.grpUrlField = new System.Windows.Forms.GroupBox();
            this.btnCreateField = new System.Windows.Forms.Button();
            this.cmbUrlField = new System.Windows.Forms.ComboBox();
            this.lblUrlField = new System.Windows.Forms.Label();

            // ===== Tab 4 - Plugin Step =====
            this.tabPluginStep = new System.Windows.Forms.TabPage();
            this.lblStep4Desc = new System.Windows.Forms.Label();
            this.grpStepPreview = new System.Windows.Forms.GroupBox();
            this.txtStepConfig = new System.Windows.Forms.TextBox();
            this.lblStepConfigLabel = new System.Windows.Forms.Label();
            this.lblStepStatusValue = new System.Windows.Forms.Label();
            this.lblStepStatusLabel = new System.Windows.Forms.Label();
            this.lblStepModeValue = new System.Windows.Forms.Label();
            this.lblStepModeLabel = new System.Windows.Forms.Label();
            this.lblStepEntityValue = new System.Windows.Forms.Label();
            this.lblStepEntityLabel = new System.Windows.Forms.Label();
            this.lblStepStageValue = new System.Windows.Forms.Label();
            this.lblStepStageLabel = new System.Windows.Forms.Label();
            this.lblStepMessageValue = new System.Windows.Forms.Label();
            this.lblStepMessageLabel = new System.Windows.Forms.Label();
            this.lblStepEnabledLabel = new System.Windows.Forms.Label();
            this.lblStepEnabledValue = new System.Windows.Forms.Label();
            this.btnToggleStep = new System.Windows.Forms.Button();
            this.lblConfigStatus = new System.Windows.Forms.Label();
            this.btnCreateStep = new System.Windows.Forms.Button();

            // ===== Tab 5 - Records =====
            this.tabRecords = new System.Windows.Forms.TabPage();
            this.lblStep5Desc = new System.Windows.Forms.Label();
            this.pnlRecordsToolbar = new System.Windows.Forms.Panel();
            this.cmbRecordFilter = new System.Windows.Forms.ComboBox();
            this.lblRecordFilter = new System.Windows.Forms.Label();
            this.btnLoadRecords = new System.Windows.Forms.Button();
            this.btnUpdateAll = new System.Windows.Forms.Button();
            this.lblRecordCount = new System.Windows.Forms.Label();
            this.progressBarRecords = new System.Windows.Forms.ProgressBar();
            this.dgvRecords = new System.Windows.Forms.DataGridView();
            this.pnlPaging = new System.Windows.Forms.Panel();
            this.btnPrevPage = new System.Windows.Forms.Button();
            this.lblPageInfo = new System.Windows.Forms.Label();
            this.btnNextPage = new System.Windows.Forms.Button();

            // ===== Data Panel =====
            this.grpData = new System.Windows.Forms.GroupBox();
            this.tblData = new System.Windows.Forms.TableLayoutPanel();
            this.lblDataSolutionLabel = new System.Windows.Forms.Label();
            this.lblDataSolutionValue = new System.Windows.Forms.Label();
            this.lblDataPublisherLabel = new System.Windows.Forms.Label();
            this.lblDataPublisherValue = new System.Windows.Forms.Label();
            this.lblDataEnvUrlLabel = new System.Windows.Forms.Label();
            this.lblDataEnvUrlValue = new System.Windows.Forms.Label();
            this.lblDataMdaLabel = new System.Windows.Forms.Label();
            this.lblDataMdaValue = new System.Windows.Forms.Label();
            this.lblDataTableLabel = new System.Windows.Forms.Label();
            this.lblDataTableValue = new System.Windows.Forms.Label();
            this.lblDataEnvUrlActualLabel = new System.Windows.Forms.Label();
            this.lblDataEnvUrlActualValue = new System.Windows.Forms.Label();
            this.lblDataMdaActualLabel = new System.Windows.Forms.Label();
            this.lblDataMdaActualValue = new System.Windows.Forms.Label();
            this.lblDataFieldLabel = new System.Windows.Forms.Label();
            this.lblDataFieldValue = new System.Windows.Forms.Label();

            // Suspend layout
            this.toolStripMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.tabWizard.SuspendLayout();
            this.tabSolution.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitStep1)).BeginInit();
            this.splitStep1.Panel1.SuspendLayout();
            this.splitStep1.Panel2.SuspendLayout();
            this.splitStep1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSolutions)).BeginInit();
            this.pnlEnvVars.SuspendLayout();
            this.grpMda.SuspendLayout();
            this.grpEnvUrl.SuspendLayout();
            this.tabAssembly.SuspendLayout();
            this.pnlAssemblyContent.SuspendLayout();
            this.tabTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitStep3)).BeginInit();
            this.splitStep3.Panel1.SuspendLayout();
            this.splitStep3.Panel2.SuspendLayout();
            this.splitStep3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTables)).BeginInit();
            this.grpUrlField.SuspendLayout();
            this.tabPluginStep.SuspendLayout();
            this.grpStepPreview.SuspendLayout();
            this.tabRecords.SuspendLayout();
            this.pnlRecordsToolbar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecords)).BeginInit();
            this.pnlPaging.SuspendLayout();
            this.grpData.SuspendLayout();
            this.tblData.SuspendLayout();
            this.SuspendLayout();

            // ======================================================================
            // toolStripMenu
            // ======================================================================
            this.toolStripMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStripMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.tsbClose, this.tssSep1, this.tsbBack, this.tsbNext });
            this.toolStripMenu.Location = new System.Drawing.Point(0, 0);
            this.toolStripMenu.Name = "toolStripMenu";
            this.toolStripMenu.Size = new System.Drawing.Size(1100, 31);
            this.toolStripMenu.TabIndex = 0;

            // tsbClose
            this.tsbClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbClose.Name = "tsbClose";
            this.tsbClose.Size = new System.Drawing.Size(86, 28);
            this.tsbClose.Text = "Close";
            this.tsbClose.Click += new System.EventHandler(this.tsbClose_Click);

            // tssSep1
            this.tssSep1.Name = "tssSep1";
            this.tssSep1.Size = new System.Drawing.Size(6, 31);

            // tsbBack
            this.tsbBack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbBack.Enabled = false;
            this.tsbBack.Name = "tsbBack";
            this.tsbBack.Size = new System.Drawing.Size(60, 28);
            this.tsbBack.Text = "\u25C0 Back";
            this.tsbBack.Click += new System.EventHandler(this.tsbBack_Click);

            // tsbNext
            this.tsbNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbNext.Name = "tsbNext";
            this.tsbNext.Size = new System.Drawing.Size(60, 28);
            this.tsbNext.Text = "Next \u25B6";
            this.tsbNext.Click += new System.EventHandler(this.tsbNext_Click);

            // ======================================================================
            // splitMain
            // ======================================================================
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitMain.Location = new System.Drawing.Point(0, 31);
            this.splitMain.Name = "splitMain";
            this.splitMain.Panel2Collapsed = true;
            this.splitMain.SplitterDistance = 820;
            this.splitMain.Size = new System.Drawing.Size(1100, 569);
            this.splitMain.TabIndex = 1;

            // splitMain.Panel1 -> tabWizard
            this.splitMain.Panel1.Controls.Add(this.tabWizard);

            // splitMain.Panel2 -> grpData
            this.splitMain.Panel2.Controls.Add(this.grpData);
            this.splitMain.Panel2MinSize = 260;

            // ======================================================================
            // tabWizard
            // ======================================================================
            this.tabWizard.Controls.Add(this.tabSolution);
            this.tabWizard.Controls.Add(this.tabAssembly);
            this.tabWizard.Controls.Add(this.tabTable);
            this.tabWizard.Controls.Add(this.tabPluginStep);
            this.tabWizard.Controls.Add(this.tabRecords);
            this.tabWizard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabWizard.Name = "tabWizard";
            this.tabWizard.SelectedIndex = 0;
            this.tabWizard.TabIndex = 0;
            this.tabWizard.SelectedIndexChanged += new System.EventHandler(this.tabWizard_SelectedIndexChanged);
            this.tabWizard.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabWizard_Selecting);

            // ======================================================================
            // TAB 1 - SOLUTION & ENVIRONMENT VARIABLES
            // ======================================================================
            this.tabSolution.Controls.Add(this.splitStep1);
            this.tabSolution.Controls.Add(this.lblStep1Desc);
            this.tabSolution.Location = new System.Drawing.Point(4, 29);
            this.tabSolution.Name = "tabSolution";
            this.tabSolution.Padding = new System.Windows.Forms.Padding(6);
            this.tabSolution.Text = "1. Solution & Variables";

            // lblStep1Desc
            this.lblStep1Desc.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblStep1Desc.Name = "lblStep1Desc";
            this.lblStep1Desc.Size = new System.Drawing.Size(800, 50);
            this.lblStep1Desc.Text = "Select the unmanaged solution where all components will be created. Then select or create the environment variables that will store the environment URL and (optionally) the Model-Driven App GUID.";
            this.lblStep1Desc.Padding = new System.Windows.Forms.Padding(0, 0, 0, 6);

            // splitStep1
            this.splitStep1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitStep1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitStep1.Name = "splitStep1";
            this.splitStep1.SplitterDistance = 250;

            // splitStep1.Panel1 -> Solutions
            this.splitStep1.Panel1.Controls.Add(this.dgvSolutions);
            this.splitStep1.Panel1.Controls.Add(this.lblSolutions);

            // splitStep1.Panel2 -> Env vars
            this.splitStep1.Panel2.Controls.Add(this.pnlEnvVars);

            // lblSolutions
            this.lblSolutions.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSolutions.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblSolutions.Name = "lblSolutions";
            this.lblSolutions.Size = new System.Drawing.Size(800, 22);
            this.lblSolutions.Text = "Unmanaged Solutions:";

            // dgvSolutions
            this.dgvSolutions.AllowUserToAddRows = false;
            this.dgvSolutions.AllowUserToDeleteRows = false;
            this.dgvSolutions.AllowUserToResizeRows = false;
            this.dgvSolutions.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSolutions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSolutions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSolutions.MultiSelect = false;
            this.dgvSolutions.Name = "dgvSolutions";
            this.dgvSolutions.ReadOnly = true;
            this.dgvSolutions.RowHeadersVisible = false;
            this.dgvSolutions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSolutions.TabIndex = 1;
            this.dgvSolutions.SelectionChanged += new System.EventHandler(this.dgvSolutions_SelectionChanged);

            // pnlEnvVars
            this.pnlEnvVars.AutoScroll = true;
            this.pnlEnvVars.Controls.Add(this.grpMda);
            this.pnlEnvVars.Controls.Add(this.grpEnvUrl);
            this.pnlEnvVars.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlEnvVars.Name = "pnlEnvVars";

            // grpEnvUrl
            this.grpEnvUrl.Controls.Add(this.txtEnvUrlValue);
            this.grpEnvUrl.Controls.Add(this.lblEnvUrlValue);
            this.grpEnvUrl.Controls.Add(this.btnCreateEnvUrl);
            this.grpEnvUrl.Controls.Add(this.cmbEnvUrlVar);
            this.grpEnvUrl.Controls.Add(this.lblEnvUrlVar);
            this.grpEnvUrl.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpEnvUrl.Name = "grpEnvUrl";
            this.grpEnvUrl.Size = new System.Drawing.Size(800, 100);
            this.grpEnvUrl.Text = "Environment URL Variable (required)";
            this.grpEnvUrl.Padding = new System.Windows.Forms.Padding(8);

            // lblEnvUrlVar
            this.lblEnvUrlVar.Location = new System.Drawing.Point(11, 24);
            this.lblEnvUrlVar.Name = "lblEnvUrlVar";
            this.lblEnvUrlVar.Size = new System.Drawing.Size(120, 20);
            this.lblEnvUrlVar.Text = "Variable:";

            // cmbEnvUrlVar
            this.cmbEnvUrlVar.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEnvUrlVar.Location = new System.Drawing.Point(137, 21);
            this.cmbEnvUrlVar.Name = "cmbEnvUrlVar";
            this.cmbEnvUrlVar.Size = new System.Drawing.Size(350, 28);
            this.cmbEnvUrlVar.TabIndex = 1;
            this.cmbEnvUrlVar.SelectedIndexChanged += new System.EventHandler(this.cmbEnvUrlVar_SelectedIndexChanged);

            // btnCreateEnvUrl
            this.btnCreateEnvUrl.Location = new System.Drawing.Point(500, 19);
            this.btnCreateEnvUrl.Name = "btnCreateEnvUrl";
            this.btnCreateEnvUrl.Size = new System.Drawing.Size(140, 30);
            this.btnCreateEnvUrl.Text = "Create Variable";
            this.btnCreateEnvUrl.Click += new System.EventHandler(this.btnCreateEnvUrl_Click);

            // lblEnvUrlValue
            this.lblEnvUrlValue.Location = new System.Drawing.Point(11, 60);
            this.lblEnvUrlValue.Name = "lblEnvUrlValue";
            this.lblEnvUrlValue.Size = new System.Drawing.Size(120, 20);
            this.lblEnvUrlValue.Text = "URL value:";

            // txtEnvUrlValue
            this.txtEnvUrlValue.Location = new System.Drawing.Point(137, 57);
            this.txtEnvUrlValue.Name = "txtEnvUrlValue";
            this.txtEnvUrlValue.Size = new System.Drawing.Size(350, 26);
            this.txtEnvUrlValue.ReadOnly = true;
            this.txtEnvUrlValue.TabIndex = 4;

            // grpMda
            this.grpMda.Controls.Add(this.txtMdaGuidValue);
            this.grpMda.Controls.Add(this.lblMdaApp);
            this.grpMda.Controls.Add(this.btnCreateMda);
            this.grpMda.Controls.Add(this.cmbMdaVar);
            this.grpMda.Controls.Add(this.lblMdaVar);
            this.grpMda.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpMda.Location = new System.Drawing.Point(0, 100);
            this.grpMda.Name = "grpMda";
            this.grpMda.Size = new System.Drawing.Size(800, 100);
            this.grpMda.Text = "Model-Driven App Variable (optional)";
            this.grpMda.Padding = new System.Windows.Forms.Padding(8);

            // lblMdaVar
            this.lblMdaVar.Location = new System.Drawing.Point(11, 24);
            this.lblMdaVar.Name = "lblMdaVar";
            this.lblMdaVar.Size = new System.Drawing.Size(120, 20);
            this.lblMdaVar.Text = "Variable:";

            // cmbMdaVar
            this.cmbMdaVar.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMdaVar.Location = new System.Drawing.Point(137, 21);
            this.cmbMdaVar.Name = "cmbMdaVar";
            this.cmbMdaVar.Size = new System.Drawing.Size(350, 28);
            this.cmbMdaVar.TabIndex = 1;
            this.cmbMdaVar.SelectedIndexChanged += new System.EventHandler(this.cmbMdaVar_SelectedIndexChanged);

            // btnCreateMda
            this.btnCreateMda.Location = new System.Drawing.Point(500, 19);
            this.btnCreateMda.Name = "btnCreateMda";
            this.btnCreateMda.Size = new System.Drawing.Size(140, 30);
            this.btnCreateMda.Text = "Create Variable";
            this.btnCreateMda.Click += new System.EventHandler(this.btnCreateMda_Click);

            // lblMdaApp
            this.lblMdaApp.Location = new System.Drawing.Point(11, 60);
            this.lblMdaApp.Name = "lblMdaApp";
            this.lblMdaApp.Size = new System.Drawing.Size(120, 20);
            this.lblMdaApp.Text = "GUID value:";

            // txtMdaGuidValue
            this.txtMdaGuidValue.Location = new System.Drawing.Point(137, 57);
            this.txtMdaGuidValue.Name = "txtMdaGuidValue";
            this.txtMdaGuidValue.Size = new System.Drawing.Size(350, 26);
            this.txtMdaGuidValue.ReadOnly = true;
            this.txtMdaGuidValue.TabIndex = 3;

            // ======================================================================
            // TAB 2 - ASSEMBLY DEPLOYMENT
            // ======================================================================
            this.tabAssembly.Controls.Add(this.pnlAssemblyContent);
            this.tabAssembly.Controls.Add(this.lblStep2Desc);
            this.tabAssembly.Location = new System.Drawing.Point(4, 29);
            this.tabAssembly.Name = "tabAssembly";
            this.tabAssembly.Padding = new System.Windows.Forms.Padding(6);
            this.tabAssembly.Text = "2. Assembly";

            // lblStep2Desc
            this.lblStep2Desc.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblStep2Desc.Name = "lblStep2Desc";
            this.lblStep2Desc.Size = new System.Drawing.Size(800, 50);
            this.lblStep2Desc.Text = "The Record URL Generator plugin assembly must be deployed to your Dataverse environment. It will be registered in sandbox isolation mode and added to your selected solution.";
            this.lblStep2Desc.Padding = new System.Windows.Forms.Padding(0, 0, 0, 6);

            // pnlAssemblyContent
            this.pnlAssemblyContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAssemblyContent.Name = "pnlAssemblyContent";
            this.pnlAssemblyContent.Controls.Add(this.btnAddToSolution);
            this.pnlAssemblyContent.Controls.Add(this.btnDeploy);
            this.pnlAssemblyContent.Controls.Add(this.lblAssemblyStatus);

            // lblAssemblyStatus
            this.lblAssemblyStatus.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblAssemblyStatus.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblAssemblyStatus.Name = "lblAssemblyStatus";
            this.lblAssemblyStatus.Size = new System.Drawing.Size(800, 80);
            this.lblAssemblyStatus.Text = "Checking assembly status...";
            this.lblAssemblyStatus.Padding = new System.Windows.Forms.Padding(0, 20, 0, 0);

            // btnDeploy
            this.btnDeploy.Location = new System.Drawing.Point(6, 100);
            this.btnDeploy.Name = "btnDeploy";
            this.btnDeploy.Size = new System.Drawing.Size(200, 36);
            this.btnDeploy.Text = "Deploy Assembly";
            this.btnDeploy.Visible = false;
            this.btnDeploy.Click += new System.EventHandler(this.btnDeploy_Click);

            // btnAddToSolution
            this.btnAddToSolution.Location = new System.Drawing.Point(220, 100);
            this.btnAddToSolution.Name = "btnAddToSolution";
            this.btnAddToSolution.Size = new System.Drawing.Size(200, 36);
            this.btnAddToSolution.Text = "Add to Solution";
            this.btnAddToSolution.Visible = false;
            this.btnAddToSolution.Click += new System.EventHandler(this.btnAddToSolution_Click);

            // ======================================================================
            // TAB 3 - TABLE & FIELD
            // ======================================================================
            this.tabTable.Controls.Add(this.splitStep3);
            this.tabTable.Controls.Add(this.lblStep3Desc);
            this.tabTable.Location = new System.Drawing.Point(4, 29);
            this.tabTable.Name = "tabTable";
            this.tabTable.Padding = new System.Windows.Forms.Padding(6);
            this.tabTable.Text = "3. Table & Field";

            // lblStep3Desc
            this.lblStep3Desc.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblStep3Desc.Name = "lblStep3Desc";
            this.lblStep3Desc.Size = new System.Drawing.Size(800, 50);
            this.lblStep3Desc.Text = "Select the table on which you want to generate record URLs. Then select an existing text field to store the URL, or create a new URL field automatically.";
            this.lblStep3Desc.Padding = new System.Windows.Forms.Padding(0, 0, 0, 6);

            // splitStep3
            this.splitStep3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitStep3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitStep3.Name = "splitStep3";
            this.splitStep3.SplitterDistance = 280;

            // splitStep3.Panel1 -> Tables
            this.splitStep3.Panel1.Controls.Add(this.dgvTables);
            this.splitStep3.Panel1.Controls.Add(this.lblTables);

            // splitStep3.Panel2 -> Field
            this.splitStep3.Panel2.Controls.Add(this.grpUrlField);

            // lblTables
            this.lblTables.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTables.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblTables.Name = "lblTables";
            this.lblTables.Size = new System.Drawing.Size(800, 22);
            this.lblTables.Text = "Tables in Solution:";

            // dgvTables
            this.dgvTables.AllowUserToAddRows = false;
            this.dgvTables.AllowUserToDeleteRows = false;
            this.dgvTables.AllowUserToResizeRows = false;
            this.dgvTables.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTables.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTables.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTables.MultiSelect = false;
            this.dgvTables.Name = "dgvTables";
            this.dgvTables.ReadOnly = true;
            this.dgvTables.RowHeadersVisible = false;
            this.dgvTables.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTables.TabIndex = 1;
            this.dgvTables.SelectionChanged += new System.EventHandler(this.dgvTables_SelectionChanged);

            // grpUrlField
            this.grpUrlField.Controls.Add(this.btnCreateField);
            this.grpUrlField.Controls.Add(this.cmbUrlField);
            this.grpUrlField.Controls.Add(this.lblUrlField);
            this.grpUrlField.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpUrlField.Name = "grpUrlField";
            this.grpUrlField.Padding = new System.Windows.Forms.Padding(8);
            this.grpUrlField.Text = "URL Field";

            // lblUrlField
            this.lblUrlField.Location = new System.Drawing.Point(11, 28);
            this.lblUrlField.Name = "lblUrlField";
            this.lblUrlField.Size = new System.Drawing.Size(120, 20);
            this.lblUrlField.Text = "Field:";

            // cmbUrlField
            this.cmbUrlField.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbUrlField.Location = new System.Drawing.Point(137, 25);
            this.cmbUrlField.Name = "cmbUrlField";
            this.cmbUrlField.Size = new System.Drawing.Size(350, 28);
            this.cmbUrlField.TabIndex = 1;
            this.cmbUrlField.SelectedIndexChanged += new System.EventHandler(this.cmbUrlField_SelectedIndexChanged);

            // btnCreateField
            this.btnCreateField.Location = new System.Drawing.Point(500, 24);
            this.btnCreateField.Name = "btnCreateField";
            this.btnCreateField.Size = new System.Drawing.Size(140, 30);
            this.btnCreateField.Text = "Create Field";
            this.btnCreateField.Click += new System.EventHandler(this.btnCreateField_Click);

            // ======================================================================
            // TAB 4 - PLUGIN STEP
            // ======================================================================
            this.tabPluginStep.Controls.Add(this.grpStepPreview);
            this.tabPluginStep.Controls.Add(this.btnCreateStep);
            this.tabPluginStep.Controls.Add(this.lblStep4Desc);
            this.tabPluginStep.Location = new System.Drawing.Point(4, 29);
            this.tabPluginStep.Name = "tabPluginStep";
            this.tabPluginStep.Padding = new System.Windows.Forms.Padding(6);
            this.tabPluginStep.Text = "4. Plugin Step";

            // lblStep4Desc
            this.lblStep4Desc.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblStep4Desc.Name = "lblStep4Desc";
            this.lblStep4Desc.Size = new System.Drawing.Size(800, 50);
            this.lblStep4Desc.Text = "Review the plugin step configuration below. The step will trigger on record creation and automatically populate the URL field. You may edit the JSON configuration if needed (not recommended).";
            this.lblStep4Desc.Padding = new System.Windows.Forms.Padding(0, 0, 0, 6);

            // grpStepPreview
            this.grpStepPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpStepPreview.Name = "grpStepPreview";
            this.grpStepPreview.Padding = new System.Windows.Forms.Padding(8);
            this.grpStepPreview.Text = "Step Configuration Preview";
            this.grpStepPreview.Controls.Add(this.txtStepConfig);
            this.grpStepPreview.Controls.Add(this.lblConfigStatus);
            this.grpStepPreview.Controls.Add(this.lblStepConfigLabel);
            this.grpStepPreview.Controls.Add(this.btnToggleStep);
            this.grpStepPreview.Controls.Add(this.lblStepEnabledValue);
            this.grpStepPreview.Controls.Add(this.lblStepEnabledLabel);
            this.grpStepPreview.Controls.Add(this.lblStepStatusValue);
            this.grpStepPreview.Controls.Add(this.lblStepStatusLabel);
            this.grpStepPreview.Controls.Add(this.lblStepModeValue);
            this.grpStepPreview.Controls.Add(this.lblStepModeLabel);
            this.grpStepPreview.Controls.Add(this.lblStepEntityValue);
            this.grpStepPreview.Controls.Add(this.lblStepEntityLabel);
            this.grpStepPreview.Controls.Add(this.lblStepStageValue);
            this.grpStepPreview.Controls.Add(this.lblStepStageLabel);
            this.grpStepPreview.Controls.Add(this.lblStepMessageValue);
            this.grpStepPreview.Controls.Add(this.lblStepMessageLabel);

            // lblStepMessageLabel
            this.lblStepMessageLabel.Location = new System.Drawing.Point(11, 28);
            this.lblStepMessageLabel.Name = "lblStepMessageLabel";
            this.lblStepMessageLabel.Size = new System.Drawing.Size(110, 20);
            this.lblStepMessageLabel.Text = "Message:";
            this.lblStepMessageLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);

            // lblStepMessageValue
            this.lblStepMessageValue.Location = new System.Drawing.Point(130, 28);
            this.lblStepMessageValue.Name = "lblStepMessageValue";
            this.lblStepMessageValue.Size = new System.Drawing.Size(300, 20);
            this.lblStepMessageValue.Text = "Create";

            // lblStepStageLabel
            this.lblStepStageLabel.Location = new System.Drawing.Point(11, 52);
            this.lblStepStageLabel.Name = "lblStepStageLabel";
            this.lblStepStageLabel.Size = new System.Drawing.Size(110, 20);
            this.lblStepStageLabel.Text = "Stage:";
            this.lblStepStageLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);

            // lblStepStageValue
            this.lblStepStageValue.Location = new System.Drawing.Point(130, 52);
            this.lblStepStageValue.Name = "lblStepStageValue";
            this.lblStepStageValue.Size = new System.Drawing.Size(300, 20);
            this.lblStepStageValue.Text = "Pre-Operation (20)";

            // lblStepEntityLabel
            this.lblStepEntityLabel.Location = new System.Drawing.Point(11, 76);
            this.lblStepEntityLabel.Name = "lblStepEntityLabel";
            this.lblStepEntityLabel.Size = new System.Drawing.Size(110, 20);
            this.lblStepEntityLabel.Text = "Entity:";
            this.lblStepEntityLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);

            // lblStepEntityValue
            this.lblStepEntityValue.Location = new System.Drawing.Point(130, 76);
            this.lblStepEntityValue.Name = "lblStepEntityValue";
            this.lblStepEntityValue.Size = new System.Drawing.Size(300, 20);
            this.lblStepEntityValue.Text = "-";

            // lblStepModeLabel
            this.lblStepModeLabel.Location = new System.Drawing.Point(11, 100);
            this.lblStepModeLabel.Name = "lblStepModeLabel";
            this.lblStepModeLabel.Size = new System.Drawing.Size(110, 20);
            this.lblStepModeLabel.Text = "Mode:";
            this.lblStepModeLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);

            // lblStepModeValue
            this.lblStepModeValue.Location = new System.Drawing.Point(130, 100);
            this.lblStepModeValue.Name = "lblStepModeValue";
            this.lblStepModeValue.Size = new System.Drawing.Size(300, 20);
            this.lblStepModeValue.Text = "Synchronous";

            // lblStepStatusLabel
            this.lblStepStatusLabel.Location = new System.Drawing.Point(11, 124);
            this.lblStepStatusLabel.Name = "lblStepStatusLabel";
            this.lblStepStatusLabel.Size = new System.Drawing.Size(110, 20);
            this.lblStepStatusLabel.Text = "Status:";
            this.lblStepStatusLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);

            // lblStepStatusValue
            this.lblStepStatusValue.Location = new System.Drawing.Point(130, 124);
            this.lblStepStatusValue.Name = "lblStepStatusValue";
            this.lblStepStatusValue.Size = new System.Drawing.Size(600, 20);
            this.lblStepStatusValue.Text = "-";
            this.lblStepStatusValue.AutoSize = true;
            this.lblStepStatusValue.ForeColor = System.Drawing.Color.DarkOrange;

            // lblStepEnabledLabel
            this.lblStepEnabledLabel.Location = new System.Drawing.Point(11, 148);
            this.lblStepEnabledLabel.Name = "lblStepEnabledLabel";
            this.lblStepEnabledLabel.Size = new System.Drawing.Size(110, 20);
            this.lblStepEnabledLabel.Text = "Enabled:";
            this.lblStepEnabledLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);

            // lblStepEnabledValue
            this.lblStepEnabledValue.Location = new System.Drawing.Point(130, 148);
            this.lblStepEnabledValue.Name = "lblStepEnabledValue";
            this.lblStepEnabledValue.Size = new System.Drawing.Size(150, 20);
            this.lblStepEnabledValue.Text = "-";

            // btnToggleStep
            this.btnToggleStep.Location = new System.Drawing.Point(290, 144);
            this.btnToggleStep.Name = "btnToggleStep";
            this.btnToggleStep.Size = new System.Drawing.Size(130, 30);
            this.btnToggleStep.Text = "Disable Step";
            this.btnToggleStep.Visible = false;
            this.btnToggleStep.Click += new System.EventHandler(this.btnToggleStep_Click);

            // lblStepConfigLabel
            this.lblStepConfigLabel.Location = new System.Drawing.Point(11, 176);
            this.lblStepConfigLabel.Name = "lblStepConfigLabel";
            this.lblStepConfigLabel.Size = new System.Drawing.Size(200, 20);
            this.lblStepConfigLabel.Text = "Unsecure Configuration (JSON):";
            this.lblStepConfigLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);

            // lblConfigStatus
            this.lblConfigStatus.Location = new System.Drawing.Point(11, 196);
            this.lblConfigStatus.Name = "lblConfigStatus";
            this.lblConfigStatus.Size = new System.Drawing.Size(600, 20);
            this.lblConfigStatus.AutoSize = true;
            this.lblConfigStatus.Text = "";
            this.lblConfigStatus.Visible = false;

            // txtStepConfig
            this.txtStepConfig.Location = new System.Drawing.Point(11, 200);
            this.txtStepConfig.Multiline = true;
            this.txtStepConfig.Name = "txtStepConfig";
            this.txtStepConfig.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtStepConfig.Size = new System.Drawing.Size(600, 120);
            this.txtStepConfig.Font = new System.Drawing.Font("Consolas", 10F);
            this.txtStepConfig.TabIndex = 10;
            this.txtStepConfig.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;

            // btnCreateStep
            this.btnCreateStep.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnCreateStep.Name = "btnCreateStep";
            this.btnCreateStep.Size = new System.Drawing.Size(800, 36);
            this.btnCreateStep.Text = "Register Plugin Step";
            this.btnCreateStep.Click += new System.EventHandler(this.btnCreateStep_Click);

            // ======================================================================
            // TAB 5 - RECORDS
            // ======================================================================
            this.tabRecords.Controls.Add(this.dgvRecords);
            this.tabRecords.Controls.Add(this.pnlPaging);
            this.tabRecords.Controls.Add(this.pnlRecordsToolbar);
            this.tabRecords.Controls.Add(this.lblStep5Desc);
            this.tabRecords.Location = new System.Drawing.Point(4, 29);
            this.tabRecords.Name = "tabRecords";
            this.tabRecords.Padding = new System.Windows.Forms.Padding(6);
            this.tabRecords.Text = "5. Records";

            // lblStep5Desc
            this.lblStep5Desc.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblStep5Desc.Name = "lblStep5Desc";
            this.lblStep5Desc.Size = new System.Drawing.Size(800, 50);
            this.lblStep5Desc.Text = "Preview records and their current URL values below. You can filter, sort, and batch-update all records that have an empty URL field. The URL is generated using the same logic as the plugin.";
            this.lblStep5Desc.Padding = new System.Windows.Forms.Padding(0, 0, 0, 6);

            // pnlRecordsToolbar
            this.pnlRecordsToolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlRecordsToolbar.Name = "pnlRecordsToolbar";
            this.pnlRecordsToolbar.Size = new System.Drawing.Size(800, 44);
            this.pnlRecordsToolbar.Controls.Add(this.progressBarRecords);
            this.pnlRecordsToolbar.Controls.Add(this.lblRecordCount);
            this.pnlRecordsToolbar.Controls.Add(this.btnUpdateAll);
            this.pnlRecordsToolbar.Controls.Add(this.btnLoadRecords);
            this.pnlRecordsToolbar.Controls.Add(this.cmbRecordFilter);
            this.pnlRecordsToolbar.Controls.Add(this.lblRecordFilter);

            // lblRecordFilter
            this.lblRecordFilter.Location = new System.Drawing.Point(3, 10);
            this.lblRecordFilter.Name = "lblRecordFilter";
            this.lblRecordFilter.Size = new System.Drawing.Size(50, 20);
            this.lblRecordFilter.Text = "Filter:";

            // cmbRecordFilter
            this.cmbRecordFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRecordFilter.Items.AddRange(new object[] { "All Records", "Empty URL", "With URL" });
            this.cmbRecordFilter.Location = new System.Drawing.Point(55, 7);
            this.cmbRecordFilter.Name = "cmbRecordFilter";
            this.cmbRecordFilter.Size = new System.Drawing.Size(150, 28);
            this.cmbRecordFilter.SelectedIndex = 0;
            this.cmbRecordFilter.TabIndex = 1;

            // btnLoadRecords
            this.btnLoadRecords.Location = new System.Drawing.Point(215, 6);
            this.btnLoadRecords.Name = "btnLoadRecords";
            this.btnLoadRecords.Size = new System.Drawing.Size(100, 30);
            this.btnLoadRecords.Text = "Load";
            this.btnLoadRecords.Click += new System.EventHandler(this.btnLoadRecords_Click);

            // btnUpdateAll
            this.btnUpdateAll.Location = new System.Drawing.Point(325, 4);
            this.btnUpdateAll.Name = "btnUpdateAll";
            this.btnUpdateAll.Size = new System.Drawing.Size(160, 34);
            this.btnUpdateAll.Text = "Update Empty URLs";
            this.btnUpdateAll.Click += new System.EventHandler(this.btnUpdateAll_Click);

            // lblRecordCount
            this.lblRecordCount.Location = new System.Drawing.Point(500, 10);
            this.lblRecordCount.Name = "lblRecordCount";
            this.lblRecordCount.Size = new System.Drawing.Size(200, 20);
            this.lblRecordCount.Text = "";

            // progressBarRecords
            this.progressBarRecords.Location = new System.Drawing.Point(710, 8);
            this.progressBarRecords.Name = "progressBarRecords";
            this.progressBarRecords.Size = new System.Drawing.Size(180, 24);
            this.progressBarRecords.Visible = false;
            this.progressBarRecords.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;

            // dgvRecords
            this.dgvRecords.AllowUserToAddRows = false;
            this.dgvRecords.AllowUserToDeleteRows = false;
            this.dgvRecords.AllowUserToResizeRows = false;
            this.dgvRecords.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvRecords.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRecords.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvRecords.MultiSelect = false;
            this.dgvRecords.Name = "dgvRecords";
            this.dgvRecords.ReadOnly = true;
            this.dgvRecords.RowHeadersVisible = false;
            this.dgvRecords.TabIndex = 5;
            this.dgvRecords.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvRecords_ColumnHeaderMouseClick);

            // pnlPaging
            this.pnlPaging.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlPaging.Name = "pnlPaging";
            this.pnlPaging.Size = new System.Drawing.Size(800, 34);
            this.pnlPaging.Controls.Add(this.btnNextPage);
            this.pnlPaging.Controls.Add(this.lblPageInfo);
            this.pnlPaging.Controls.Add(this.btnPrevPage);

            // btnPrevPage
            this.btnPrevPage.Location = new System.Drawing.Point(3, 3);
            this.btnPrevPage.Name = "btnPrevPage";
            this.btnPrevPage.Size = new System.Drawing.Size(80, 28);
            this.btnPrevPage.Text = "\u25C0 Prev";
            this.btnPrevPage.Enabled = false;
            this.btnPrevPage.Click += new System.EventHandler(this.btnPrevPage_Click);

            // lblPageInfo
            this.lblPageInfo.Location = new System.Drawing.Point(90, 7);
            this.lblPageInfo.Name = "lblPageInfo";
            this.lblPageInfo.Size = new System.Drawing.Size(200, 20);
            this.lblPageInfo.Text = "Page 1";
            this.lblPageInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // btnNextPage
            this.btnNextPage.Location = new System.Drawing.Point(300, 3);
            this.btnNextPage.Name = "btnNextPage";
            this.btnNextPage.Size = new System.Drawing.Size(80, 28);
            this.btnNextPage.Text = "Next \u25B6";
            this.btnNextPage.Enabled = false;
            this.btnNextPage.Click += new System.EventHandler(this.btnNextPage_Click);

            // ======================================================================
            // DATA PANEL (right side)
            // ======================================================================
            this.grpData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpData.Name = "grpData";
            this.grpData.Text = "Configuration Data";
            this.grpData.Padding = new System.Windows.Forms.Padding(8);
            this.grpData.Controls.Add(this.tblData);

            // tblData
            this.tblData.Dock = System.Windows.Forms.DockStyle.Top;
            this.tblData.ColumnCount = 2;
            this.tblData.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tblData.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblData.RowCount = 8;
            this.tblData.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tblData.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tblData.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tblData.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tblData.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tblData.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tblData.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tblData.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tblData.Name = "tblData";
            this.tblData.Size = new System.Drawing.Size(240, 212);

            // Row 0: Solution
            this.lblDataSolutionLabel.Text = "Solution:";
            this.lblDataSolutionLabel.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblDataSolutionLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDataSolutionValue.Text = "-";
            this.lblDataSolutionValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDataSolutionValue.AutoEllipsis = true;
            this.tblData.Controls.Add(this.lblDataSolutionLabel, 0, 0);
            this.tblData.Controls.Add(this.lblDataSolutionValue, 1, 0);

            // Row 1: Publisher
            this.lblDataPublisherLabel.Text = "Prefix:";
            this.lblDataPublisherLabel.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblDataPublisherLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDataPublisherValue.Text = "-";
            this.lblDataPublisherValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDataPublisherValue.AutoEllipsis = true;
            this.tblData.Controls.Add(this.lblDataPublisherLabel, 0, 1);
            this.tblData.Controls.Add(this.lblDataPublisherValue, 1, 1);

            // Row 2: Env URL
            this.lblDataEnvUrlLabel.Text = "Env URL:";
            this.lblDataEnvUrlLabel.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblDataEnvUrlLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDataEnvUrlValue.Text = "-";
            this.lblDataEnvUrlValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDataEnvUrlValue.AutoEllipsis = true;
            this.tblData.Controls.Add(this.lblDataEnvUrlLabel, 0, 2);
            this.tblData.Controls.Add(this.lblDataEnvUrlValue, 1, 2);

            // Row 3: Env URL Value
            this.lblDataEnvUrlActualLabel.Text = "URL Val:";
            this.lblDataEnvUrlActualLabel.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblDataEnvUrlActualLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDataEnvUrlActualValue.Text = "-";
            this.lblDataEnvUrlActualValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDataEnvUrlActualValue.AutoEllipsis = true;
            this.tblData.Controls.Add(this.lblDataEnvUrlActualLabel, 0, 3);
            this.tblData.Controls.Add(this.lblDataEnvUrlActualValue, 1, 3);

            // Row 4: MDA
            this.lblDataMdaLabel.Text = "MDA:";
            this.lblDataMdaLabel.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblDataMdaLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDataMdaValue.Text = "-";
            this.lblDataMdaValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDataMdaValue.AutoEllipsis = true;
            this.tblData.Controls.Add(this.lblDataMdaLabel, 0, 4);
            this.tblData.Controls.Add(this.lblDataMdaValue, 1, 4);

            // Row 5: MDA GUID Value
            this.lblDataMdaActualLabel.Text = "GUID:";
            this.lblDataMdaActualLabel.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblDataMdaActualLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDataMdaActualValue.Text = "-";
            this.lblDataMdaActualValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDataMdaActualValue.AutoEllipsis = true;
            this.tblData.Controls.Add(this.lblDataMdaActualLabel, 0, 5);
            this.tblData.Controls.Add(this.lblDataMdaActualValue, 1, 5);

            // Row 6: Table
            this.lblDataTableLabel.Text = "Table:";
            this.lblDataTableLabel.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblDataTableLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDataTableValue.Text = "-";
            this.lblDataTableValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDataTableValue.AutoEllipsis = true;
            this.tblData.Controls.Add(this.lblDataTableLabel, 0, 6);
            this.tblData.Controls.Add(this.lblDataTableValue, 1, 6);

            // Row 7: Field
            this.lblDataFieldLabel.Text = "Field:";
            this.lblDataFieldLabel.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.lblDataFieldLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDataFieldValue.Text = "-";
            this.lblDataFieldValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDataFieldValue.AutoEllipsis = true;
            this.tblData.Controls.Add(this.lblDataFieldLabel, 0, 7);
            this.tblData.Controls.Add(this.lblDataFieldValue, 1, 7);

            // ======================================================================
            // MyPluginControl
            // ======================================================================
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.toolStripMenu);
            this.Name = "MyPluginControl";
            this.Size = new System.Drawing.Size(1100, 600);
            this.Load += new System.EventHandler(this.MyPluginControl_Load);

            // Resume layout
            this.toolStripMenu.ResumeLayout(false);
            this.toolStripMenu.PerformLayout();
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.tabWizard.ResumeLayout(false);
            this.tabSolution.ResumeLayout(false);
            this.splitStep1.Panel1.ResumeLayout(false);
            this.splitStep1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitStep1)).EndInit();
            this.splitStep1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSolutions)).EndInit();
            this.pnlEnvVars.ResumeLayout(false);
            this.grpMda.ResumeLayout(false);
            this.grpEnvUrl.ResumeLayout(false);
            this.grpEnvUrl.PerformLayout();
            this.tabAssembly.ResumeLayout(false);
            this.pnlAssemblyContent.ResumeLayout(false);
            this.tabTable.ResumeLayout(false);
            this.splitStep3.Panel1.ResumeLayout(false);
            this.splitStep3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitStep3)).EndInit();
            this.splitStep3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTables)).EndInit();
            this.grpUrlField.ResumeLayout(false);
            this.tabPluginStep.ResumeLayout(false);
            this.grpStepPreview.ResumeLayout(false);
            this.grpStepPreview.PerformLayout();
            this.tabRecords.ResumeLayout(false);
            this.pnlRecordsToolbar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecords)).EndInit();
            this.pnlPaging.ResumeLayout(false);
            this.grpData.ResumeLayout(false);
            this.tblData.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        // ToolStrip
        private System.Windows.Forms.ToolStrip toolStripMenu;
        private System.Windows.Forms.ToolStripButton tsbClose;
        private System.Windows.Forms.ToolStripSeparator tssSep1;
        private System.Windows.Forms.ToolStripButton tsbBack;
        private System.Windows.Forms.ToolStripButton tsbNext;

        // Main layout
        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.TabControl tabWizard;

        // Tab 1 - Solution & Env Vars
        private System.Windows.Forms.TabPage tabSolution;
        private System.Windows.Forms.Label lblStep1Desc;
        private System.Windows.Forms.SplitContainer splitStep1;
        private System.Windows.Forms.Label lblSolutions;
        private System.Windows.Forms.DataGridView dgvSolutions;
        private System.Windows.Forms.Panel pnlEnvVars;
        private System.Windows.Forms.GroupBox grpEnvUrl;
        private System.Windows.Forms.Label lblEnvUrlVar;
        private System.Windows.Forms.ComboBox cmbEnvUrlVar;
        private System.Windows.Forms.Button btnCreateEnvUrl;
        private System.Windows.Forms.Label lblEnvUrlValue;
        private System.Windows.Forms.TextBox txtEnvUrlValue;
        private System.Windows.Forms.GroupBox grpMda;
        private System.Windows.Forms.Label lblMdaVar;
        private System.Windows.Forms.ComboBox cmbMdaVar;
        private System.Windows.Forms.Button btnCreateMda;
        private System.Windows.Forms.Label lblMdaApp;
        private System.Windows.Forms.TextBox txtMdaGuidValue;

        // Tab 2 - Assembly
        private System.Windows.Forms.TabPage tabAssembly;
        private System.Windows.Forms.Label lblStep2Desc;
        private System.Windows.Forms.Panel pnlAssemblyContent;
        private System.Windows.Forms.Label lblAssemblyStatus;
        private System.Windows.Forms.Button btnDeploy;
        private System.Windows.Forms.Button btnAddToSolution;

        // Tab 3 - Table & Field
        private System.Windows.Forms.TabPage tabTable;
        private System.Windows.Forms.Label lblStep3Desc;
        private System.Windows.Forms.SplitContainer splitStep3;
        private System.Windows.Forms.Label lblTables;
        private System.Windows.Forms.DataGridView dgvTables;
        private System.Windows.Forms.GroupBox grpUrlField;
        private System.Windows.Forms.Label lblUrlField;
        private System.Windows.Forms.ComboBox cmbUrlField;
        private System.Windows.Forms.Button btnCreateField;

        // Tab 4 - Plugin Step
        private System.Windows.Forms.TabPage tabPluginStep;
        private System.Windows.Forms.Label lblStep4Desc;
        private System.Windows.Forms.GroupBox grpStepPreview;
        private System.Windows.Forms.Label lblStepMessageLabel;
        private System.Windows.Forms.Label lblStepMessageValue;
        private System.Windows.Forms.Label lblStepStageLabel;
        private System.Windows.Forms.Label lblStepStageValue;
        private System.Windows.Forms.Label lblStepEntityLabel;
        private System.Windows.Forms.Label lblStepEntityValue;
        private System.Windows.Forms.Label lblStepModeLabel;
        private System.Windows.Forms.Label lblStepModeValue;
        private System.Windows.Forms.Label lblStepStatusLabel;
        private System.Windows.Forms.Label lblStepStatusValue;
        private System.Windows.Forms.Label lblStepConfigLabel;
        private System.Windows.Forms.TextBox txtStepConfig;
        private System.Windows.Forms.Label lblConfigStatus;
        private System.Windows.Forms.Label lblStepEnabledLabel;
        private System.Windows.Forms.Label lblStepEnabledValue;
        private System.Windows.Forms.Button btnToggleStep;
        private System.Windows.Forms.Button btnCreateStep;

        // Tab 5 - Records
        private System.Windows.Forms.TabPage tabRecords;
        private System.Windows.Forms.Label lblStep5Desc;
        private System.Windows.Forms.Panel pnlRecordsToolbar;
        private System.Windows.Forms.Label lblRecordFilter;
        private System.Windows.Forms.ComboBox cmbRecordFilter;
        private System.Windows.Forms.Button btnLoadRecords;
        private System.Windows.Forms.Button btnUpdateAll;
        private System.Windows.Forms.Label lblRecordCount;
        private System.Windows.Forms.ProgressBar progressBarRecords;
        private System.Windows.Forms.DataGridView dgvRecords;
        private System.Windows.Forms.Panel pnlPaging;
        private System.Windows.Forms.Button btnPrevPage;
        private System.Windows.Forms.Label lblPageInfo;
        private System.Windows.Forms.Button btnNextPage;

        // Data Panel
        private System.Windows.Forms.GroupBox grpData;
        private System.Windows.Forms.TableLayoutPanel tblData;
        private System.Windows.Forms.Label lblDataSolutionLabel;
        private System.Windows.Forms.Label lblDataSolutionValue;
        private System.Windows.Forms.Label lblDataPublisherLabel;
        private System.Windows.Forms.Label lblDataPublisherValue;
        private System.Windows.Forms.Label lblDataEnvUrlLabel;
        private System.Windows.Forms.Label lblDataEnvUrlValue;
        private System.Windows.Forms.Label lblDataMdaLabel;
        private System.Windows.Forms.Label lblDataMdaValue;
        private System.Windows.Forms.Label lblDataTableLabel;
        private System.Windows.Forms.Label lblDataTableValue;
        private System.Windows.Forms.Label lblDataEnvUrlActualLabel;
        private System.Windows.Forms.Label lblDataEnvUrlActualValue;
        private System.Windows.Forms.Label lblDataMdaActualLabel;
        private System.Windows.Forms.Label lblDataMdaActualValue;
        private System.Windows.Forms.Label lblDataFieldLabel;
        private System.Windows.Forms.Label lblDataFieldValue;
    }
}
