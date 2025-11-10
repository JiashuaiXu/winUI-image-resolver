# Image Resolver 发布脚本
# 支持自包含发布和 MSIX 打包

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet("SelfContained", "MSIX", "SingleFile", "All")]
    [string]$PublishType = "All",
    
    [Parameter(Mandatory=$false)]
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release",
    
    [Parameter(Mandatory=$false)]
    [ValidateSet("win-x64", "win-x86", "win-arm64")]
    [string]$Runtime = "win-x64"
)

$ErrorActionPreference = "Stop"
$ProjectFile = "ImageResolver.csproj"
$PublishDir = "publish"
$Version = "1.0.0.0"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Image Resolver 发布脚本" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "发布类型: $PublishType" -ForegroundColor Yellow
Write-Host "配置: $Configuration" -ForegroundColor Yellow
Write-Host "运行时: $Runtime" -ForegroundColor Yellow
Write-Host ""

# 清理旧的发布文件
if (Test-Path $PublishDir) {
    Write-Host "清理旧的发布文件..." -ForegroundColor Yellow
    Remove-Item -Path $PublishDir -Recurse -Force
}

# 自包含发布
if ($PublishType -eq "SelfContained" -or $PublishType -eq "All") {
    Write-Host "`n[1/3] 开始自包含发布..." -ForegroundColor Green
    $SelfContainedDir = "$PublishDir\SelfContained\$Runtime"
    
    dotnet publish $ProjectFile `
        -c $Configuration `
        -r $Runtime `
        --self-contained true `
        -p:WindowsAppSDKSelfContained=true `
        -p:PublishSingleFile=false `
        -o $SelfContainedDir
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ 自包含发布完成: $SelfContainedDir" -ForegroundColor Green
        
        # 创建启动说明
        $ReadmeContent = @"
# Image Resolver - 自包含发布

## 使用方法
1. 解压整个文件夹到任意位置
2. 运行 ImageResolver.exe

## 系统要求
- Windows 10 版本 1809 (17763) 或更高版本
- Windows 11

## 文件说明
- ImageResolver.exe: 主程序
- 其他文件: 运行时依赖，请勿删除

## 版本
$Version
"@
        $ReadmeContent | Out-File -FilePath "$SelfContainedDir\README.txt" -Encoding UTF8
    } else {
        Write-Host "✗ 自包含发布失败" -ForegroundColor Red
        exit 1
    }
}

# 单文件发布
if ($PublishType -eq "SingleFile" -or $PublishType -eq "All") {
    Write-Host "`n[2/3] 开始单文件发布..." -ForegroundColor Green
    $SingleFileDir = "$PublishDir\SingleFile\$Runtime"
    
    dotnet publish $ProjectFile `
        -c $Configuration `
        -r $Runtime `
        --self-contained true `
        -p:WindowsAppSDKSelfContained=true `
        -p:PublishSingleFile=true `
        -p:IncludeNativeLibrariesForSelfExtract=true `
        -p:EnableCompressionInSingleFile=true `
        -o $SingleFileDir
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ 单文件发布完成: $SingleFileDir" -ForegroundColor Green
        
        # 创建启动说明
        $ReadmeContent = @"
# Image Resolver - 单文件发布

## 使用方法
直接运行 ImageResolver.exe 即可，无需安装。

## 系统要求
- Windows 10 版本 1809 (17763) 或更高版本
- Windows 11

## 注意
首次启动可能需要几秒钟解压文件，请耐心等待。

## 版本
$Version
"@
        $ReadmeContent | Out-File -FilePath "$SingleFileDir\README.txt" -Encoding UTF8
    } else {
        Write-Host "✗ 单文件发布失败" -ForegroundColor Red
        exit 1
    }
}

# MSIX 打包
if ($PublishType -eq "MSIX" -or $PublishType -eq "All") {
    Write-Host "`n[3/3] 开始 MSIX 打包..." -ForegroundColor Green
    
    # 检查 Package.appxmanifest 是否存在
    if (-not (Test-Path "Package.appxmanifest")) {
        Write-Host "✗ 未找到 Package.appxmanifest 文件" -ForegroundColor Red
        exit 1
    }
    
    # 检查图标文件
    $RequiredIcons = @(
        "Assets\StoreLogo.png",
        "Assets\Square44x44Logo.png",
        "Assets\Square150x150Logo.png",
        "Assets\Wide310x150Logo.png"
    )
    
    $MissingIcons = @()
    foreach ($Icon in $RequiredIcons) {
        if (-not (Test-Path $Icon)) {
            $MissingIcons += $Icon
        }
    }
    
    if ($MissingIcons.Count -gt 0) {
        Write-Host "✗ 缺少以下图标文件:" -ForegroundColor Red
        $MissingIcons | ForEach-Object { Write-Host "  - $_" -ForegroundColor Red }
        exit 1
    }
    
    # 先发布自包含版本
    $MsixPublishDir = "$PublishDir\MSIX\$Runtime"
    dotnet publish $ProjectFile `
        -c $Configuration `
        -r $Runtime `
        --self-contained true `
        -p:WindowsAppSDKSelfContained=true `
        -p:WindowsPackageType=Msix `
        -o $MsixPublishDir
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ MSIX 打包完成: $MsixPublishDir" -ForegroundColor Green
        Write-Host "`n注意: 完整的 MSIX 打包需要使用 Visual Studio 或 MakeAppx.exe" -ForegroundColor Yellow
        Write-Host "当前已生成可打包的文件，请使用以下工具完成最终打包:" -ForegroundColor Yellow
        Write-Host "  - Visual Studio: 右键项目 -> 发布 -> 创建应用包" -ForegroundColor Yellow
        Write-Host "  - MakeAppx.exe: 使用 Windows SDK 工具" -ForegroundColor Yellow
    } else {
        Write-Host "✗ MSIX 打包失败" -ForegroundColor Red
        exit 1
    }
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "发布完成！" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "发布文件位置: $PublishDir" -ForegroundColor Yellow
Write-Host ""

