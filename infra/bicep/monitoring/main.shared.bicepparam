/*
Shared monitoring parameters (not bound to a template).
Leaf files use: extends './main.shared.bicepparam'
Requires Bicep CLI 0.44.1+.
Do not put secrets or identifiable estate ids here.
*/

using none

param location = 'uksouth'
param productSlug = 'dertinfo'
param dailyQuotaGb = '1'
param dataRetention = 30
param enableTelemetry = false
