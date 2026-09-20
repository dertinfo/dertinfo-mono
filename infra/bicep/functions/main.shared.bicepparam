/*
Shared Functions parameters (not bound to a template).
Leaf files use: extends './main.shared.bicepparam'
Requires Bicep CLI 0.44.1+.
Do not put secrets or identifiable estate ids here.
*/

using none

param location = 'uksouth'
param productSlug = 'dertinfo'
param enableTelemetry = false
// Default false. Set true only after images storage and Application Insights exist (see comment on this param in main.bicep).
param prerequisitesExist = false
// PLACEHOLDER — GitHub Environment variable AZURE_MONITOR_FUNCTIONS_EXCESSIVEUSE_EMAIL via Functions infra CD
param excessiveUseNotifyEmail = ''
