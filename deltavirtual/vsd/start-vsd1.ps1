# run start-vsd1.bat, also this is obvi just for windows, when i move to linux ill make a version for it

$ErrorActionPreference = "Continue"
$Root = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $Root

$AppDir = Join-Path $Root "visual-system-designer-app"
$VenvDir = Join-Path $Root ".venv"
$VenvPy = Join-Path $VenvDir "Scripts\python.exe"
$VenvVsd = Join-Path $VenvDir "Scripts\vsd.exe"
$Workspace = Join-Path $Root "workspace"

function Fail([string]$Msg, [string]$Fix) {
    Write-Host ""
    Write-Host "vsd can't start yet" -ForegroundColor Red
    Write-Host "  $Msg" -ForegroundColor Yellow
    if ($Fix) {
        Write-Host ""
        Write-Host "fix:" -ForegroundColor Cyan
        Write-Host "  $Fix" -ForegroundColor Cyan
    }
    Write-Host ""
    exit 1
}

function Need-Command([string]$Name, [string]$Fix) {
    $cmd = Get-Command $Name -ErrorAction SilentlyContinue
    if (-not $cmd) {
        Fail "$Name not found on PATH" $Fix
    }
    return $cmd
}

Write-Host "deltavirtual / vsd" -ForegroundColor DarkCyan
Write-Host ""


# tools for host needed

Need-Command "Python" "install python 3.10 or higher also MAKE SURE YOUR PYTHON IS IN PATH JOAO" | Out-Null
Need-Command "node" "install node.js" | Out-Null
Need-Command "npm" "npm usually comes with node so reinstall node" | Out-Null
Need-Command "git" "install git from git-scm.com i mean u need git anyways youll have it prolly" | Out-Null


# vsd init shells zephyr and zephyr sdk

$bash = Get-Command bash -ErrorAction SilentlyContinue
if (-not $bash) {
    $gitBash = "C:\Program Files\Git\bin\bash.exe"
    if (Test-Path $gitBash) {
        $env:PATH = "C:\Program Files\Git\bin;$env:PATH"
        $bash = Get-Command bash -ErrorAction SilentlyContinue
    }
}

# venv
if (-not (Test-Path $VenvPy)) {
    Write-Host "creating venv ..." -ForegroundColor DarkGray
    & python -m venv $VenvDir
    if ($LASTEXITCODE -ne 0 -or -not (Test-Path $VenvPy)) {
        Fail "couldn't create .venv" "try: python -m venv .venv   from this folder and tell google abt it"
    }
}

$env:PATH = "$VenvDir\Scripts;$env:PATH"
if (-not $env:HOME) { $env:HOME = $env:USERPROFILE }

# vsd install
if (-not (Test-Path $VenvVsd)) {
    Write-Host "installing vsd + deps (first run, needs network) ..." -ForegroundColor DarkGray
    & $VenvPy -m pip install --upgrade pip
    & $VenvPy -m pip install "$Root\win-stubs\uvloop-stub"
    & $VenvPy -m pip install "$Root\win-stubs\npm-stub"
    & $VenvPy -m pip install -e $AppDir
    Copy-Item "$Root\win-stubs\sitecustomize.py" "$VenvDir\Lib\site-packages\sitecustomize.py" -Force
    if ($LASTEXITCODE -ne 0 -or -not (Test-Path $VenvVsd)) {
        Fail "pip install of visual-system-designer-app failed" "check you have network, then run: .\.venv\Scripts\python.exe -m pip install -e .\visual-system-designer-app"
    }
}

# python 3.12+ needs the asyncio shim even if vsd was installed before
if (-not (Test-Path "$VenvDir\Lib\site-packages\sitecustomize.py")) {
    Copy-Item "$Root\win-stubs\sitecustomize.py" "$VenvDir\Lib\site-packages\sitecustomize.py" -Force
}

# npm shim must exist even if vsd was installed on a previous run. pipeline_manager
# shells out to `npm` and windows only has npm.cmd, so without .venv\Scripts\npm.exe
# the frontend build dies with WinError 2.
if (-not (Test-Path "$VenvDir\Scripts\npm.exe")) {
    Write-Host "installing npm shim ..." -ForegroundColor DarkGray
    & $VenvPy -m pip install "$Root\win-stubs\npm-stub"
}

# workspace
$env:VSD_WORKSPACE = $Workspace
$EnvYml = Join-Path $Workspace "vsd-env.yml"
$ResDir = Join-Path $Workspace "visual-system-designer-resources"
$FrontendDir = Join-Path $Workspace ".pipeline_manager\frontend"
$IndexHtml = Join-Path $FrontendDir "index.html"
$PmWorkspace = Join-Path $Workspace ".pipeline_manager\workspace"

# resources + the web ui. NOT gated on vsd-env.yml -- workspace/ is gitignored so a
# fresh clone never has the built frontend, and an earlier run can leave vsd-env.yml
# behind even when the build failed. always check index.html.
if (-not (Test-Path $ResDir)) {
    Write-Host "cloning visual-system-designer-resources ..." -ForegroundColor DarkGray
    & git clone --depth 1 https://github.com/antmicro/visual-system-designer-resources.git $ResDir
    if (-not (Test-Path $ResDir)) {
        Fail "cloning visual-system-designer-resources failed" "need network + git, then re-run this"
    }
}

if (-not (Test-Path $IndexHtml)) {
    Write-Host "building frontend (first run, takes a minute) ..." -ForegroundColor DarkGray

    # webpack hates the ESM in resources/broken_image.js unless vue.config.js
    # says it's javascript/esm. overwrite BOTH copies: the site-packages template
    # and the workspace copy (build_prepare syncs template -> workspace).
    $VueCfg = Join-Path $Root "win-stubs\pm-vue.config.js"
    $TplVue = Join-Path $VenvDir "Lib\site-packages\pipeline_manager\frontend\vue.config.js"
    $WsVue = Join-Path $PmWorkspace "frontend\vue.config.js"
    if (Test-Path $VueCfg) {
        if (Test-Path (Split-Path $TplVue -Parent)) {
            Copy-Item $VueCfg $TplVue -Force
        }
        if (Test-Path (Split-Path $WsVue -Parent)) {
            Copy-Item $VueCfg $WsVue -Force
        }
    }

    $env:PATH = "$VenvDir\Scripts;$env:PATH"
    & $VenvPy -m pipeline_manager build server-app `
        --editor-title "Visual System Designer" `
        --workspace-directory $PmWorkspace `
        --output-directory $FrontendDir `
        --assets-directory (Join-Path $ResDir "assets") `
        --favicon-path (Join-Path $ResDir "assets\visual-system-designer.svg")

    if (-not (Test-Path $IndexHtml)) {
        Fail "frontend build failed, no index.html in workspace\.pipeline_manager\frontend" "the log above is the real error. usually npm, network, or node being weird. after fixing, delete workspace\vsd-env.yml if it exists and run this again"
    }
}

# vsd-env.yml last. never write this while the frontend is missing or the next run
# will skip setup and crash on the missing directory.
if (-not (Test-Path $EnvYml)) {
    Write-Host "writing vsd-env.yml ..." -ForegroundColor DarkGray
    $renodeExe = "C:\Program Files\Renode\renode.exe"
    $rn = Get-Command renode -ErrorAction SilentlyContinue
    if ($rn) { $renodeExe = $rn.Source }
    $sdkDir = Join-Path $env:USERPROFILE "zephyr-sdk-0.16.3"
    $yml = @"
PYRENODE_BIN: '$renodeExe'
PYRENODE_RUNTIME: coreclr
ZEPHYR_SDK_INSTALL_DIR: '$sdkDir'
ZEPHYR_BASE: '$Workspace\zephyr'
"@
    New-Item -ItemType Directory -Path $Workspace -Force | Out-Null
    Set-Content -Path $EnvYml -Value $yml -Encoding UTF8
}

if (-not (Test-Path $EnvYml)) {
    Fail "couldn't write workspace\vsd-env.yml" "check the workspace folder isnt read-only"
}

# vsd init gets linux renode protable, on win point at renode installed from usermanual
$winRenode = Get-Command renode -ErrorAction SilentlyContinue
if ($winRenode) {
    $renodeExe = $winRenode.Source
    $lines = Get-Content $EnvYml
    $patched = $false
    $newLines = foreach ($line in $lines) {
        if ($line -match '^\s*PYRENODE_BIN:') {
            $patched = $true
            "PYRENODE_BIN: '$renodeExe'"
        } else {
            $line
        }
    }
    if ($patched) {
        Set-Content -Path $EnvYml -Value $newLines
    }
}

# run
Write-Host ""
Write-Host "starting vsd, go to http://localhost:9000" -ForegroundColor Green
Write-Host "ctrl+c to end" -ForegroundColor DarkGray
Write-Host ""

$DemoApp = Join-Path $AppDir "demo\blinky-temperature"
& $VenvVsd run --app $DemoApp
