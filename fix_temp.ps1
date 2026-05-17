$file = "d:\shiyan\unity\gamedevelop\cause_and_effect_shining\Assets\Scripts\Manager\LoadManager.cs"
$c = Get-Content -Path $file -Raw -Encoding UTF8

# Fix 1: Separate scene name from display text in LoadLevelCoroutine(int,int)
$old1 = 'string targetName = $"Chapter{Chapter}Floor{Level} 终点{currentEndStep}/{totalEndStep}";'
$new1 = 'string sceneName = $"Chapter{Chapter}Level{Level}";' + "`r`n        " + 'string displayText = $"Chapter{Chapter}Level{Level} 终点{currentEndStep}/{totalEndStep}";'
$c = $c.Replace($old1, $new1)

# Fix 2: Use displayText for SetLoading, sceneName for LoadSceneAsync
$old2 = 'LoadingAnimator.Instance.SetLoading(targetName);'
$new2 = 'LoadingAnimator.Instance.SetLoading(displayText);'
$c = $c.Replace($old2, $new2)

# Fix 3: Use sceneName for LoadSceneAsync (first occurrence after SetLevelCount)
$old3 = 'yield return SceneManager.LoadSceneAsync(targetName, LoadSceneMode.Additive);'
$new3 = 'yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);'
$c = $c.Replace($old3, $new3)

Set-Content -Path $file -Value $c -Encoding UTF8 -NoNewline
Write-Output 'Done'
