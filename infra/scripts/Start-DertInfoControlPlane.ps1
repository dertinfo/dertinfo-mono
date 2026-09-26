<#
.SYNOPSIS
  Menu for the DertInfo estate control-plane scripts.

.DESCRIPTION
  Lists each script in this folder whose comment-based help has a .SYNOPSIS.
  Shows that help, collects parameters from the script param() block, and runs
  the script only after you confirm. Optional parameters left blank are omitted.

.EXAMPLE
  .\Start-DertInfoControlPlane.ps1
#>

$script:ControlPlaneScriptPath = $MyInvocation.MyCommand.Path
$script:ControlPlaneScriptDir = if ($PSScriptRoot) { $PSScriptRoot } else { Split-Path -Parent $script:ControlPlaneScriptPath }

function Get-DertInfoControlPlaneHelpText {
  param(
    $Help,
    [string] $Name
  )

  if ($null -eq $Help -or $null -eq $Help.Parameters) {
    return ''
  }

  foreach ($key in @($Help.Parameters.Keys)) {
    if ([string]::Equals([string]$key, $Name, [System.StringComparison]::OrdinalIgnoreCase)) {
      return ([string]$Help.Parameters[$key]).Trim()
    }
  }

  return ''
}

function Get-DertInfoControlPlaneParameters {
  param(
    $Ast,
    $Help
  )

  if ($null -eq $Ast.ParamBlock -or $null -eq $Ast.ParamBlock.Parameters) {
    return
  }

  foreach ($paramAst in $Ast.ParamBlock.Parameters) {
    $name = $paramAst.Name.VariablePath.UserPath
    $mandatory = $false
    $validateSet = New-Object System.Collections.Generic.List[string]
    $isSwitch = $paramAst.StaticType.Name -eq 'SwitchParameter'
    $defaultText = $null

    foreach ($attr in $paramAst.Attributes) {
      $typeName = $attr.TypeName.Name
      if ($typeName -eq 'switch') {
        $isSwitch = $true
      }
      if ($typeName -eq 'ValidateSet') {
        foreach ($arg in $attr.PositionalArguments) {
          $validateSet.Add([string]$arg.SafeGetValue())
        }
      }
      if ($typeName -eq 'Parameter') {
        foreach ($named in $attr.NamedArguments) {
          if ($named.ArgumentName -eq 'Mandatory') {
            $mandatory = [bool]$named.Argument.SafeGetValue()
          }
        }
      }
    }

    if ($null -ne $paramAst.DefaultValue) {
      $rawDefault = $paramAst.DefaultValue.Extent.Text.Trim()
      if ($rawDefault -ne "''" -and $rawDefault -ne '""') {
        $defaultText = $rawDefault
      }
    }

    [pscustomobject]@{
      Name         = $name
      Help         = (Get-DertInfoControlPlaneHelpText -Help $Help -Name $name)
      Mandatory    = [bool]$mandatory
      ValidateSet  = @($validateSet)
      IsSwitch     = [bool]$isSwitch
      DefaultText  = $defaultText
    }
  }
}

function Get-DertInfoControlPlaneMenuItems {
  $items = New-Object System.Collections.Generic.List[object]
  $files = @(Get-ChildItem -LiteralPath $script:ControlPlaneScriptDir -Filter '*.ps1' -File | Sort-Object Name)

  foreach ($file in $files) {
    if ([string]::Equals($file.FullName, $script:ControlPlaneScriptPath, [System.StringComparison]::OrdinalIgnoreCase)) {
      continue
    }

    $tokens = $null
    $errors = $null
    $ast = [System.Management.Automation.Language.Parser]::ParseFile($file.FullName, [ref]$tokens, [ref]$errors)
    $help = $ast.GetHelpContent()
    if ($null -eq $help -or [string]::IsNullOrWhiteSpace($help.Synopsis)) {
      continue
    }

    $examples = @()
    if ($null -ne $help.Examples) {
      foreach ($example in @($help.Examples)) {
        $text = ([string]$example).Trim()
        if (-not [string]::IsNullOrWhiteSpace($text)) {
          $examples += $text
        }
      }
    }

    $description = ''
    if ($help.Description) {
      $description = ([string]$help.Description).Trim()
    }

    $parameters = @(Get-DertInfoControlPlaneParameters -Ast $ast -Help $help)

    $items.Add([pscustomobject]@{
      Name        = $file.Name
      Path        = $file.FullName
      Synopsis    = ([string]$help.Synopsis).Trim()
      Description = $description
      Examples    = $examples
      Parameters  = $parameters
    })
  }

  foreach ($item in $items) {
    Write-Output $item
  }
}

function ConvertTo-DertInfoControlPlaneSplat {
  param(
    [hashtable] $Supplied
  )

  $splat = @{}
  if ($null -eq $Supplied) {
    return $splat
  }

  foreach ($key in @($Supplied.Keys)) {
    $splat[$key] = $Supplied[$key]
  }

  return $splat
}

function Read-DertInfoControlPlaneChoice {
  param(
    [string] $Prompt,
    [int] $Max
  )

  while ($true) {
    $raw = Read-Host $Prompt
    $number = 0
    if ([int]::TryParse($raw, [ref]$number) -and $number -ge 1 -and $number -le $Max) {
      return $number
    }
    Write-Host "Enter a number from 1 to $Max."
  }
}

function Test-DertInfoControlPlaneYesNo {
  param(
    [string] $Raw
  )

  $token = if ($null -eq $Raw) { '' } else { $Raw.Trim() }
  if ($token -eq '1' -or [string]::Equals($token, 'y', [System.StringComparison]::OrdinalIgnoreCase)) {
    return 'yes'
  }
  if ($token -eq '2' -or [string]::Equals($token, 'n', [System.StringComparison]::OrdinalIgnoreCase)) {
    return 'no'
  }
  return ''
}

function Read-DertInfoControlPlaneYesNo {
  param(
    [string] $Prompt = 'Select'
  )

  while ($true) {
    $answer = Test-DertInfoControlPlaneYesNo -Raw (Read-Host $Prompt)
    if ($answer -eq 'yes') {
      return $true
    }
    if ($answer -eq 'no') {
      return $false
    }
    Write-Host 'Enter 1, 2, Y, or N.'
  }
}

function Read-DertInfoControlPlaneParameterValue {
  param(
    $Parameter
  )

  $kind = if ($Parameter.Mandatory) { 'mandatory' } else { 'optional' }
  Write-Host ''
  Write-Host ("{0} ({1})" -f $Parameter.Name, $kind)
  if (-not [string]::IsNullOrWhiteSpace($Parameter.Help)) {
    Write-Host $Parameter.Help
  }

  if ($Parameter.IsSwitch) {
    Write-Host '1. Yes'
    Write-Host '2. No'
    if (-not $Parameter.Mandatory) {
      Write-Host 'Enter = not supplied'
    }
    while ($true) {
      $raw = Read-Host 'Select'
      if ([string]::IsNullOrWhiteSpace($raw)) {
        if ($Parameter.Mandatory) {
          Write-Host 'A value is required.'
          continue
        }
        return $null
      }
      $answer = Test-DertInfoControlPlaneYesNo -Raw $raw
      if ($answer -eq 'yes') {
        return $true
      }
      if ($answer -eq 'no') {
        if ($Parameter.Mandatory) {
          Write-Host 'A value is required.'
          continue
        }
        return $null
      }
      Write-Host 'Enter 1, 2, Y, or N.'
    }
  }

  if (@($Parameter.ValidateSet).Count -gt 0) {
    $values = @($Parameter.ValidateSet)
    for ($i = 0; $i -lt $values.Count; $i++) {
      Write-Host ("{0}. {1}" -f ($i + 1), $values[$i])
    }
    if (-not $Parameter.Mandatory) {
      Write-Host 'Enter = not supplied'
    }
    while ($true) {
      $raw = Read-Host 'Select'
      if ([string]::IsNullOrWhiteSpace($raw)) {
        if ($Parameter.Mandatory) {
          Write-Host 'A value is required.'
          continue
        }
        return $null
      }
      $number = 0
      if ([int]::TryParse($raw, [ref]$number) -and $number -ge 1 -and $number -le $values.Count) {
        return [string]$values[$number - 1]
      }
      Write-Host "Enter a number from 1 to $($values.Count)."
    }
  }

  while ($true) {
    $raw = Read-Host 'Value'
    if ([string]::IsNullOrWhiteSpace($raw)) {
      if ($Parameter.Mandatory) {
        Write-Host 'A value is required.'
        continue
      }
      return $null
    }
    return $raw
  }
}

function Read-DertInfoControlPlaneParameters {
  param(
    $Parameters
  )

  $supplied = @{}
  foreach ($parameter in @($Parameters)) {
    $value = Read-DertInfoControlPlaneParameterValue -Parameter $parameter
    if ($null -ne $value) {
      $supplied[$parameter.Name] = $value
    }
  }
  return $supplied
}

function Format-DertInfoControlPlaneParameterValue {
  param(
    $Parameter,
    [hashtable] $Supplied
  )

  if (-not $Supplied.ContainsKey($Parameter.Name)) {
    return 'not supplied'
  }
  if ($Parameter.IsSwitch) {
    return 'Yes'
  }
  return [string]$Supplied[$Parameter.Name]
}

function Edit-DertInfoControlPlaneParameter {
  param(
    $Item,
    [hashtable] $Supplied
  )

  Write-Host ''
  Write-Host 'Which parameter?'
  $parameters = @($Item.Parameters)
  for ($i = 0; $i -lt $parameters.Count; $i++) {
    $shown = Format-DertInfoControlPlaneParameterValue -Parameter $parameters[$i] -Supplied $Supplied
    Write-Host ("{0}. {1} = {2}" -f ($i + 1), $parameters[$i].Name, $shown)
  }

  $choice = Read-DertInfoControlPlaneChoice -Prompt 'Select' -Max $parameters.Count
  $chosen = $parameters[$choice - 1]
  $value = Read-DertInfoControlPlaneParameterValue -Parameter $chosen
  if ($null -eq $value) {
    [void]$Supplied.Remove($chosen.Name)
  }
  else {
    $Supplied[$chosen.Name] = $value
  }
}

function Show-DertInfoControlPlaneHome {
  param(
    $Items
  )

  $items = @($Items)
  Write-Host ''
  Write-Host 'DertInfo control plane'
  Write-Host ''
  for ($i = 0; $i -lt $items.Count; $i++) {
    Write-Host ("{0}. {1}" -f ($i + 1), $items[$i].Name)
    Write-Host ("   {0}" -f $items[$i].Synopsis)
  }

  $quit = $items.Count + 1
  Write-Host "$quit. Quit"
  $choice = Read-DertInfoControlPlaneChoice -Prompt 'Select' -Max $quit
  if ($choice -eq $quit) {
    return $null
  }
  return $items[$choice - 1]
}

function Show-DertInfoControlPlaneDetail {
  param(
    $Item
  )

  Write-Host ''
  Write-Host $Item.Name
  Write-Host ''
  Write-Host $Item.Synopsis
  Write-Host ''
  if (-not [string]::IsNullOrWhiteSpace($Item.Description)) {
    Write-Host $Item.Description
    Write-Host ''
  }

  $parameters = @($Item.Parameters)
  if ($parameters.Count -gt 0) {
    Write-Host 'Parameters'
    foreach ($parameter in $parameters) {
      $kind = if ($parameter.Mandatory) { 'mandatory' } else { 'optional' }
      Write-Host ''
      Write-Host ("{0} ({1})" -f $parameter.Name, $kind)
      if (-not [string]::IsNullOrWhiteSpace($parameter.Help)) {
        Write-Host $parameter.Help
      }
      if (@($parameter.ValidateSet).Count -gt 0) {
        Write-Host ("Allowed values: {0}" -f (@($parameter.ValidateSet) -join ', '))
      }
      if (-not [string]::IsNullOrWhiteSpace($parameter.DefaultText)) {
        Write-Host ("Default: {0}" -f $parameter.DefaultText)
      }
    }
    Write-Host ''
  }

  $examples = @($Item.Examples)
  if ($examples.Count -gt 0) {
    Write-Host 'Examples'
    Write-Host ''
    foreach ($example in $examples) {
      Write-Host $example
      Write-Host ''
    }
  }

  Write-Host 'Run this script?'
  Write-Host '1. Yes'
  Write-Host '2. No'
  return Read-DertInfoControlPlaneYesNo
}

function Show-DertInfoControlPlaneConfirm {
  param(
    $Item,
    [hashtable] $Supplied
  )

  $parameters = @($Item.Parameters)
  while ($true) {
    Write-Host ''
    Write-Host $Item.Name
    Write-Host ''
    Write-Host $Item.Synopsis
    Write-Host ''

    if ($parameters.Count -gt 0) {
      Write-Host 'Parameters'
      foreach ($parameter in $parameters) {
        $shown = Format-DertInfoControlPlaneParameterValue -Parameter $parameter -Supplied $Supplied
        Write-Host ("  {0}: {1}" -f $parameter.Name, $shown)
      }
      Write-Host ''
      Write-Host '1. Continue'
      Write-Host '2. Edit'
      Write-Host '3. Abandon'
      $choice = Read-DertInfoControlPlaneChoice -Prompt 'Select' -Max 3
      if ($choice -eq 1) {
        return 'continue'
      }
      if ($choice -eq 3) {
        return 'abandon'
      }
      Edit-DertInfoControlPlaneParameter -Item $Item -Supplied $Supplied
      continue
    }

    Write-Host '1. Continue'
    Write-Host '2. Abandon'
    $choice = Read-DertInfoControlPlaneChoice -Prompt 'Select' -Max 2
    if ($choice -eq 1) {
      return 'continue'
    }
    return 'abandon'
  }
}

function Invoke-DertInfoControlPlaneScript {
  param(
    $Item,
    [hashtable] $Supplied
  )

  $splat = ConvertTo-DertInfoControlPlaneSplat -Supplied $Supplied
  try {
    & $Item.Path @splat
  }
  catch {
    Write-Error -ErrorRecord $_ -ErrorAction Continue
  }
}

function Read-DertInfoControlPlaneReturnHome {
  Write-Host ''
  Write-Host 'Return to home?'
  Write-Host '1. Yes'
  Write-Host '2. No'
  return Read-DertInfoControlPlaneYesNo
}

function Invoke-DertInfoControlPlaneMenu {
  while ($true) {
    $items = @(Get-DertInfoControlPlaneMenuItems)
    $item = Show-DertInfoControlPlaneHome -Items $items
    if ($null -eq $item) {
      return
    }

    $run = Show-DertInfoControlPlaneDetail -Item $item
    if (-not $run) {
      continue
    }

    $supplied = @{}
    if (@($item.Parameters).Count -gt 0) {
      $supplied = Read-DertInfoControlPlaneParameters -Parameters $item.Parameters
    }

    $decision = Show-DertInfoControlPlaneConfirm -Item $item -Supplied $supplied
    if ($decision -ne 'continue') {
      continue
    }

    Invoke-DertInfoControlPlaneScript -Item $item -Supplied $supplied
    if (-not (Read-DertInfoControlPlaneReturnHome)) {
      return
    }
  }
}

if ($MyInvocation.InvocationName -ne '.') {
  Invoke-DertInfoControlPlaneMenu
}
