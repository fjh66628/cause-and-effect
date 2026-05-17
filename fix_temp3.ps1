$f = "d:\shiyan\unity\gamedevelop\cause_and_effect_shining\Assets\Scripts\Manager\LoadManager.cs"
$c = Get-Content -Path $f -Raw -Encoding UTF8

# Fix: In LoadLevelCoroutine(string sceneName, string targetName), restore SetLoading(targetName)
# Line 27 should use targetName (the parameter), not displayText
$old = 'LoadingAnimator.Instance.SetLoading(displayText);' + "`r`n        yield return new WaitForSeconds(1.2f);`r`n`r`n        // 1. 卸载原有场景 + 加载UI场景"
$new = 'LoadingAnimator.Instance.SetLoading(targetName);' + "`r`n        yield return new WaitForSeconds(1.2f);`r`n`r`n        // 1. 卸载原有场景 + 加载UI场景"
$c = $c.Replace($old, $new)

Set-Content -Path $f -Value $c -Encoding UTF8 -NoNewline
Write-Output 'Done'
