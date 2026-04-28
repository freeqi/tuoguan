@echo off

REM 清理所有项目的obj和bin文件夹
echo 开始清理项目文件夹...

REM CDGService.Application
if exist "CDGService.Application\bin" rmdir /s /q "CDGService.Application\bin"
if exist "CDGService.Application\obj" rmdir /s /q "CDGService.Application\obj"
echo 清理 CDGService.Application 完成

REM CDGService.Data
if exist "CDGService.Data\bin" rmdir /s /q "CDGService.Data\bin"
if exist "CDGService.Data\obj" rmdir /s /q "CDGService.Data\obj"
echo 清理 CDGService.Data 完成

REM CDGService.Store
if exist "CDGService.Store\bin" rmdir /s /q "CDGService.Store\bin"
if exist "CDGService.Store\obj" rmdir /s /q "CDGService.Store\obj"
echo 清理 CDGService.Store 完成

REM CDGService.ThirdPartyLib
if exist "CDGService.ThirdPartyLib\bin" rmdir /s /q "CDGService.ThirdPartyLib\bin"
if exist "CDGService.ThirdPartyLib\obj" rmdir /s /q "CDGService.ThirdPartyLib\obj"
echo 清理 CDGService.ThirdPartyLib 完成

REM CDGService.Utils
if exist "CDGService.Utils\bin" rmdir /s /q "CDGService.Utils\bin"
if exist "CDGService.Utils\obj" rmdir /s /q "CDGService.Utils\obj"
echo 清理 CDGService.Utils 完成

REM SimpleLogger
if exist "SimpleLogger\bin" rmdir /s /q "SimpleLogger\bin"
if exist "SimpleLogger\obj" rmdir /s /q "SimpleLogger\obj"
echo 清理 SimpleLogger 完成

REM CDGService.WebAPI
if exist "src\CDGService.WebAPI\bin" rmdir /s /q "src\CDGService.WebAPI\bin"
if exist "src\CDGService.WebAPI\obj" rmdir /s /q "src\CDGService.WebAPI\obj"
echo 清理 CDGService.WebAPI 完成

REM Autofac.Extras.DynamicProxy.Core
if exist "src\Autofac.Extras.DynamicProxy.Core\bin" rmdir /s /q "src\Autofac.Extras.DynamicProxy.Core\bin"
if exist "src\Autofac.Extras.DynamicProxy.Core\obj" rmdir /s /q "src\Autofac.Extras.DynamicProxy.Core\obj"
echo 清理 Autofac.Extras.DynamicProxy.Core 完成

echo 清理完成！
pause