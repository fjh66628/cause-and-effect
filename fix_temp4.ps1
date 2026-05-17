$f = "d:\shiyan\unity\gamedevelop\cause_and_effect_shining\Assets\Scripts\Manager\LoadManager.cs"
$c = Get-Content -Path $f -Raw -Encoding UTF8

# In LoadLevelCoroutine(int,int): change SetLoading(targetName) back to SetLoading(displayText)
$old = 'string displayText = $"Chapter{Chapter}Level{Level} 终点{currentEndStep}/{totalEndStep}";' + "`r`n        LoadingAnimator.Instance.SetLoading(targetName);"
$new = 'string displayText = $"Chapter{Chapter}Level{Level} 终点{currentEndStep}/{totalEndStep}";' + "`r`n        LoadingAnimator.Instance.SetLoading(displayText);"
$c = $c.Replace($old, $new)

Set-Content -Path $f -Value $c -Encoding UTF8 -NoNewline
Write-Output 'Done'
