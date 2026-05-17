$lf = "d:\shiyan\unity\gamedevelop\cause_and_effect_shining\Assets\Scripts\Manager\LoadManager.cs"
$lc = Get-Content -Path $lf -Raw -Encoding UTF8
$lc = $lc.Replace('"Chapter{GameManager.Instance.getChapterNumber}Floor{GameManager.Instance.getLevelNumber}"', '"Chapter{GameManager.Instance.getChapterNumber}Level{GameManager.Instance.getLevelNumber}"')
Set-Content -Path $lf -Value $lc -Encoding UTF8 -NoNewline

$gf = "d:\shiyan\unity\gamedevelop\cause_and_effect_shining\Assets\Scripts\Manager\GameManager.cs"
$gc = Get-Content -Path $gf -Raw -Encoding UTF8
$gc = $gc.Replace('{ChapterNumber}Floor{levelNumber}', '{ChapterNumber}Level{levelNumber}')
Set-Content -Path $gf -Value $gc -Encoding UTF8 -NoNewline
Write-Output 'Done'
