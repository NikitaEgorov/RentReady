# Rent Ready Technical Assessment

Don't forget to add the connection string to the `local.settings.json`

```json
"ConnectionStrings": {
    "CrmConnectionString": "AuthType=OAuth;Username=???;Password=???;Url=https://*.dynamics.com/;"
}
```

## Issues
* Untypical behaviour with timezone
* Validate JSONScheme (date format) in code. DateOnly unsupported by swagger
* `EndOn` is include or not