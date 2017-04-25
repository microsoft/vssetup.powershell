---
external help file: Microsoft.VisualStudio.Setup.PowerShell.dll-Help.xml
online version: https://github.com/Microsoft/vssetup.powershell/raw/master/docs/VSSetup/Get-VSSetupInstance.md
schema: 2.0.0
---

# Get-VSSetupInstance

## SYNOPSIS
Enumerates instances of Visual Studio and related products.

## SYNTAX

### All (Default)
```
Get-VSSetupInstance [-All]
```

### Path
```
Get-VSSetupInstance [-Path] <String[]>
```

### LiteralPath
```
Get-VSSetupInstance -LiteralPath <String[]>
```

## DESCRIPTION
Enumerates instances of Visual Studio and related products. By default, instances with fatal errors are not returned by you can pass `-All` to enumerate them as well.

## EXAMPLES

### Example 1
```
PS C:\> Get-VSSetupInstance -All
```

Enumerates all instances of Visual Studio and related products even if a fatal error was raised during the last operation.

### Example 2
```
PS C:\> Get-VSSetupInstance 'C:\Program Files (x86)\Microsoft Visual Studio\2017\Community'
```

Gets the instance for the product installed to the given directory.

## PARAMETERS

### -All
Enumerate all instances of Visual Studio - even those with fatal errors.

```yaml
Type: SwitchParameter
Parameter Sets: All
Aliases: 

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -LiteralPath
The path to the product installation directory. Wildcards are not supported.

```yaml
Type: String[]
Parameter Sets: LiteralPath
Aliases: PSPath

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Path
The path to the product installation directory. Wildcards are supported.

```yaml
Type: String[]
Parameter Sets: Path
Aliases: 

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: True
```

## INPUTS

### System.String[]
One or more paths to product installation directories.

## OUTPUTS

### Microsoft.VisualStudio.Setup.Instance
Information about each instance enumerated.
