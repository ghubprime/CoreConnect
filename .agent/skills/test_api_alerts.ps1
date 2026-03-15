$Body = @"
{
    "AlertDeviceID": "f2b0a595-5ea8-471b-975f-12e70e0f3497",
    "AlertMessage": "Mock Test Alert",
    "ApiRequestBody": null,
    "ApiRequestHeaders": null,
    "ApiRequestMethod": null,
    "ApiRequestUrl": null,
    "EmailBody": "Mock email body",
    "EmailSubject": "Mock Subject",
    "EmailTo": "admin@example.com",
    "ShouldAlert": true,
    "ShouldEmail": false,
    "ShouldSendApiRequest": false
}
"@

Write-Host "Sending mock alert to https://localhost:5001/api/Alerts/Create/"

Invoke-WebRequest -Uri "https://localhost:5001/api/Alerts/Create/" -Method Post -Headers @{
    "X-Api-Key"="3e9d8273-1dc1-4303-bd50-7a133e36b9b7:S+82XKZdvg278pSFHWtUklqHENuO5IhH"
} -Body $Body -ContentType "application/json" -SkipCertificateCheck

Write-Host "Alert sent successfully."
