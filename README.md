# Record URL Generator — XrmToolBox Plugin

**Automatically generate direct URLs for your Dataverse records.**

Record URL Generator is an [XrmToolBox](https://www.xrmtoolbox.com/) plugin that registers a server-side Dataverse plugin to automatically populate a URL field every time a record is created. It also provides a bulk-update feature to backfill URLs on existing records — all configured through a step-by-step wizard, with zero code required.

---

## Features

- **Automatic URL generation** — A Dataverse plugin (Pre-Operation / Create) builds and stores a direct link to each new record.
- **Model-Driven App support** — URLs can include an `appid` parameter so the link opens directly in the correct app.
- **Environment Variables** — The base URL and optional Model-Driven App ID are stored as Dataverse Environment Variables, making the solution environment-aware and ALM-friendly.
- **Guided wizard (5 steps)** — No manual configuration needed:
  1. **Solution & Environment Variables** — Pick an unmanaged solution, create or select the required environment variables.
  2. **Assembly Deployment** — Deploy the server-side plugin assembly (embedded as a resource) and add it to the solution.
  3. **Table & Field Selection** — Choose the target table and create (or select) a URL-formatted string field.
  4. **Plugin Step Configuration** — Register the SDK Message Processing Step with auto-generated JSON configuration.
  5. **Bulk Update** — Browse existing records and batch-update those with empty URLs, with progress tracking and cancellation support.
- **Solution-aware** — Every component (environment variables, assembly, plugin step, field) is automatically added to the selected solution.
- **Bulk operations** — Update up to 500 records per page using `ExecuteMultipleRequest` (batches of 100, with `ContinueOnError`).
- **Pagination, sorting & filtering** — Browse records with paging (50/page), column sorting and filter by *All / Empty URLs / Filled URLs*.

---

## URL Format

```
# With Model-Driven App
https://<org>.crm.dynamics.com/main.aspx?appid=<GUID>&pagetype=entityrecord&etn=<table>&id=<recordid>

# Without Model-Driven App
https://<org>.crm.dynamics.com/main.aspx?pagetype=entityrecord&etn=<table>&id=<recordid>
```

---

## How It Works

1. Two **Environment Variables** hold the configuration:
   - `<publisher>_EnvironmentURL` — the base URL of the environment (e.g. `https://contoso.crm.dynamics.com`).
   - `<publisher>_ModelDrivenAppId` *(optional)* — the GUID of the Model-Driven App to open.
2. A **Dataverse plugin** (`GenerateRecordUrlPlugin`) is registered on the **Create** message (Pre-Operation, Synchronous, Sandbox).
3. On each record creation the plugin:
   - Reads the environment variables.
   - Builds the URL from the environment URL + entity logical name + record ID (+ optional app ID).
   - Sets the URL field on the target entity **before** the record is committed.
4. The **Bulk Update** feature in the XrmToolBox UI follows the same logic to backfill records created before the plugin was registered.

---

## Plugin Step Configuration (JSON)

The SDK Message Processing Step uses the following unsecure configuration:

```json
{
  "envUrlVarName": "<publisher>_EnvironmentURL",
  "recordUrlFieldName": "<prefix>_<fieldname>",
  "mdaEnvName": "<publisher>_ModelDrivenAppId"
}
```

| Property | Description |
|---|---|
| `envUrlVarName` | Schema name of the Environment Variable holding the base URL. |
| `recordUrlFieldName` | Logical name of the string field where the URL is stored. |
| `mdaEnvName` | Schema name of the Environment Variable holding the Model-Driven App GUID (optional). |

---

## Prerequisites

- [XrmToolBox](https://www.xrmtoolbox.com/) (latest version recommended).
- A connection to a **Dataverse / Dynamics 365** environment.
- An **unmanaged solution** in the target environment.
- Sufficient security privileges to register plugin assemblies and SDK steps.

---

## Installation

1. Open **XrmToolBox**.
2. Go to the **Tool Library** (Plugin Store).
3. Search for **Record URL Generator** and install it.
4. The tool appears under the plugin list — connect to your environment and launch it.

> **Manual installation**: copy the compiled `RecordUrlGeneratorTool.dll` into your XrmToolBox `Plugins` folder.

---

## Getting Started

1. **Connect** to your Dataverse environment in XrmToolBox.
2. Open **Record URL Generator**.
3. Follow the 5-step wizard:
   - **Step 1** — Select a solution and configure environment variables (the tool can create them for you).
   - **Step 2** — Deploy the plugin assembly to Dataverse.
   - **Step 3** — Choose the target table and URL field (or create a new one).
   - **Step 4** — Register the plugin step.
   - **Step 5** — Optionally bulk-update existing records.
4. From now on, every new record on the configured table will automatically get a direct URL.

---

## Project Structure

```
RecordUrlGenerator/
├── RecordUrlGenerator.sln
├── RecordUrlGenerator.Plugins/          # Server-side Dataverse plugin (.NET 4.6.2)
│   └── GenerateRecordUrlPlugin.cs       # IPlugin implementation
├── RecordUrlGeneratorTool/              # XrmToolBox plugin UI (.NET 4.8)
│   ├── MyPlugin.cs                      # Plugin metadata & entry point
│   ├── MyPluginControl.cs               # 5-step wizard (WinForms)
│   ├── Settings.cs                      # User settings
│   └── Helpers/
│       └── CrmServiceHelper.cs          # Dataverse API helper methods
└── packages/                            # NuGet dependencies
```

---

## Build

1. Open `RecordUrlGenerator.sln` in **Visual Studio 2019+**.
2. Restore NuGet packages.
3. Build the solution — the plugin assembly (`RecordUrlGenerator.Plugins.dll`) is embedded as a resource inside the XrmToolBox plugin.

| Project | Target Framework |
|---|---|
| RecordUrlGeneratorTool | .NET Framework 4.8 |
| RecordUrlGenerator.Plugins | .NET Framework 4.6.2 |

---

## Contributing

Contributions are welcome! Feel free to open an issue or submit a pull request.

---

## License

This project is provided as-is. See the repository for license details.