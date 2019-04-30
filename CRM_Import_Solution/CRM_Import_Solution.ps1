Import-Module "*****path****"\Cdmlet\CRM_Import_Solution.dll

Function LogWrite
				{
				   Param ([string]$logstring)
				   Add-content $Logfile -value $logstring
				}
Try{
	#Update name of solution in below line
	[xml]$XmlDocument = Get-Content -Path ""*****path****"\Importar\AuthFileImportSolution.xml"	
	$rootFolder =  "*****path****\Importar"
	$destination = "*****path****\Importar\Soluciones_Importadas"
	$crmUrl = $XmlDocument.Authentication.crmUrl
	$organization=$XmlDocument.Authentication.organization
	Get-ChildItem "$rootFolder" -Filter *.zip | Sort CreationTime | Select -First 1 |
	Foreach-Object {
			$DateStr= Get-Date
			$filePath = $_.FullName
			$fileName = $_.BaseName
			$Logfile = "*****path****\Importar\Logs\$fileName.txt"
			write-host "------------$DateStr------------"
			LogWrite "------------$DateStr------------"
			Write-host "Se encontro la solucion $fileName"
			LogWrite "Se encontro la solucion $fileName"
			Write-host "Importando Solucion..."
			LogWrite "Importando Solucion..."
			Get-CRM_Import_Solution -ManagedSolutionLocation $filePath -CrmUrl $crmUrl -Organization $organization
			Write-host "Finaliza el proceso de importacion"
			LogWrite  "Finaliza el proceso de importacion"
			Write-host "Se mueve archivo a carpeta de Soluciones Importadas"
			Move-Item -Path $filePath -Destination $destination
			}
}
Catch
{
    $ErrorMessage = $_.Exception.Message
    Write-host "Error: $ErrorMessage"
	LogWrite "---------------ERROR---------------"
	LogWrite "Error: $ErrorMessage"
	LogWrite "---------------ERROR---------------"
}
