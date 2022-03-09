Write-Information -MessageData "Got your features!" -InformationAction Continue

Write-Progress -Activity 'starting progress' -Status 'Starting' -PercentComplete 0
Start-Sleep -Milliseconds 600
1..10 |ForEach-Object{
    Write-Progress -Activity "updating progress" -Status 'Progressing' -PercentComplete $(5 + 6.87 * $_)
    Start-Sleep -Milliseconds 100
}
Write-Progress -Activity 'Testing progress' -Status 'Ending' -PercentComplete 99
Start-Sleep -Seconds 2
Get-ChildItem -Directory C:\Users\ #List only directories
Start-Sleep -Seconds 5
Write-Progress -Activity 'ending progress' -Status 'Done' -Completed