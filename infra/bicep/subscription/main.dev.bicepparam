/*
Subscription foundation — development leaf.
Admin-only: az deployment sub create --location uksouth --template-file main.bicep --parameters main.dev.bicepparam
Inherits main.shared.bicepparam; override only what differs for this environment.
Secrets / identifiable values: empty placeholders here or in shared; supply via CLI / GitHub Environment.
Requires Bicep CLI 0.44.1+.
*/

using 'main.bicep'
extends './main.shared.bicepparam'

param environmentTag = 'dev'
