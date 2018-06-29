---
external help file: Microsoft.VisualStudio.Setup.PowerShell.dll-Help.xml
Module Name: VSSetup
online version: https://github.com/Microsoft/vssetup.powershell/raw/master/docs/VSSetup/Select-VSSetupInstance.md
schema: 2.0.0
---

# Select-VSSetupInstance

## SYNOPSIS
Selects instances of Visual Studio and related products based on criteria.

## SYNTAX

```
Select-VSSetupInstance [-Instance] <Instance[]> [-Product <String[]>] [-Require <String[]>] [-RequireAny]
 [-Version <String>] [-Latest] [<CommonParameters>]
```

## DESCRIPTION
You can specify zero or more products (by default, Visual Studio Community, Professional, and Enterprise are selected) to find, along with zero or more workloads that all are required by any instances enumerated. Additionally, you can specify a version range to limit the results or request the latest instance. All criteria are combined to return the best instance or instances for your needs.

## EXAMPLES

### Example 1
```
PS C:\> Get-VSSetupInstance | Select-VSSetupInstance -Latest
```

Select the most-recently installed instance of the highest version of Visual Studio Community, Professional, or Enterprise installed.

### Example 2
```
PS C:\> Get-VSSetupInstance | Select-VSSetupInstance -Product * -Require 'Microsoft.VisualStudio.Component.VC.Tools.x86.x64'
```

Selects any product with the native Visual C++ compilers installed.

## PARAMETERS

### -Instance
One or more instances from which to select.

```yaml
Type: Instance[]
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Latest
Select the most recently installed instance with the highest version (within the optional `-Version` range).

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Product
One or more products to select. Wildcards are supported.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: Microsoft.VisualStudio.Product.Community, Microsoft.VisualStudio.Product.Professional, Microsoft.VisualStudio.Product.Enterprise
Accept pipeline input: False
Accept wildcard characters: False
```

### -Require
One or more workloads or components to select. All requirements specified must be met. Wildcards are not supported.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -RequireAny
Change the behavior of -Require such that any one or more requirements specified must be met.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Version
A version range to limit results. A single version like '15.0' is equivalent to '[15.0,)', which means versions 15.0 and newer are in range. You can also specify versions like '[15.0,16.0)' to limit results to Visual Studio 2017 only (15.x).

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see about_CommonParameters (http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### Microsoft.VisualStudio.Setup.Instance[]
One or more instances from which to select.

## OUTPUTS

### Microsoft.VisualStudio.Setup.Instance
Zero or more instances that met specified criteria.

## NOTES

## RELATED LINKS
