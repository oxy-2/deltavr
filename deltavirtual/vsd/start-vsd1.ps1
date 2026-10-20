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
    if ($LASTEXITCODE -ne 0 -or -not (Test-Path $VenvPy)) {
        Fail "pip install of visual-system-designer-app failed" "check you have network, then run: .\.venv\Scripts\python.exe -m pip install -e .\visual-system-designer-app"
    }
}

# python 3.12+ needs the asyncio shim even if vsd was installed before
if (-not (Test-Path "$VenvDir\Lib\site-packages\sitecustomize.py")) {
    Copy-Item "$Root\win-stubs\sitecustomize.py" "$VenvDir\Lib\site-packages\sitecustomize.py" -Force
}

# workspace
$env:VSD_WORKSPACE = $Workspace
$EnvYml = Join-Path $Workspace "vsd-env.yml"

if (-not (Test-Path $EnvYml)) {
    Write-Host "setting up..." -ForegroundColor DarkGray
    Write-Host "clones zephyr and pulls sdk" -ForegroundColor DarkGray
    Write-Host ""
    $env:PATH = "$VenvDir\Scripts;$env:PATH"
    if (-not $env:HOME) { $env:HOME = $env:USERPROFILE }

    # bootstrap workspace directly. vsd init is linux-oriented and keeps
    # tripping over windows (bash/tar/wget/setup.cmd), so do the three
    # things it actually has to land: resources, frontend, vsd-env.yml.
    $ResDir = Join-Path $Workspace "visual-system-designer-resources"
    if (-not (Test-Path $ResDir)) {
        Write-Host "cloning visual-system-designer-resources ..." -ForegroundColor DarkGray
        & git clone --depth 1 https://github.com/antmicro/visual-system-designer-resources.git $ResDir
    }

    $FrontendDir = Join-Path $Workspace ".pipeline_manager\frontend"
    if (-not (Test-Path (Join-Path $FrontendDir "index.html"))) {
        Write-Host "building frontend (first run, takes a minute) ..." -ForegroundColor DarkGray
        & $VenvVsd init $Workspace
    }

    if (-not (Test-Path $EnvYml)) {
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
        Fail "vsd init workspace failed" "the log above has the erro but its prolly cuz no network, no bash, no npm, no money, no motion, no bitches"
    }
}

# vsd init gets linux renode protable, on win point at renode installed from usermanual
if ($EnvYml -and (Test-Path $EnvYml)) {
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
}

# run
Write-Host ""
Write-Host "starting vsd, go to http://localhost:9000" -ForegroundColor Green
Write-Host "ctrl+c to end" -ForegroundColor DarkGray
Write-Host ""

$DemoApp = Join-Path $AppDir "demo\blinky-temperature"
& $VenvVsd run --app $DemoApp